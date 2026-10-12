using System.Diagnostics;
using System.Drawing.Printing;
using FastReport.Export.PdfSimple;
using Microsoft.EntityFrameworkCore;
using QuanCaPhe.Models;
using FRReport = FastReport.Report;

namespace QuanCaPhe;

public partial class MainForm : Form
{
    private const string TenQuan = "QUÁN CÀ PHÊ GÓC PHỐ";

    private bool _binding;      // đang nạp lưới bằng code -> bỏ qua SelectionChanged
    private int _seqDS, _seqCT; // chống kết quả cũ ghi đè kết quả mới
    private int? _maHD;         // hóa đơn đang chọn

    public MainForm() { InitializeComponent(); }

    private static string MaHDText(int id) => $"HD{id:D6}";

    // ===================== KHỞI TẠO =====================

    private async void MainForm_Load(object sender, EventArgs e)
    {
        try
        {
            cboLocBan.SelectedIndex = 0;
            cboLocHT.SelectedIndex = 0;
            cboBan.SelectedIndex = 0;

            await using (var db = new QuanCaPheContext())
            {
                var mons = (await db.Mons.AsNoTracking().OrderBy(m => m.MaMon)
                                    .Select(m => new { m.MaMon, m.TenMon, m.DonGia }).ToListAsync())
                           .Select(m => new MonItem { MaMon = m.MaMon, Hien = $"{m.TenMon} - {m.DonGia:N0}" }).ToList();
                cboMon.DataSource = mons;
                cboMon.DisplayMember = "Hien";
                cboMon.ValueMember = "MaMon";
                cboMon.SelectedIndex = -1;

                // Khoảng ngày mặc định phủ toàn bộ dữ liệu hiện có
                if (await db.HoaDons.AnyAsync())
                {
                    var min = await db.HoaDons.MinAsync(h => h.NgayLap);
                    var max = await db.HoaDons.MaxAsync(h => h.NgayLap);
                    dtpTu.Value = min.Date;
                    dtpDen.Value = max.Date > DateTime.Today ? max.Date : DateTime.Today;
                }
            }
            await LoadDanhSachAsync(keepSel: false);
        }
        catch (Exception ex) { BaoLoi("tải dữ liệu", ex); }
    }

    // ===================== DANH SÁCH HÓA ĐƠN (lưới trái) =====================

    private async Task LoadDanhSachAsync(int? selectId = null, bool keepSel = true)
    {
        int seq = ++_seqDS;
        DateTime tu = dtpTu.Value.Date, den = dtpDen.Value.Date.AddDays(1);
        int? ban = cboLocBan.SelectedIndex > 0 ? cboLocBan.SelectedIndex : null;
        string ht = cboLocHT.SelectedIndex > 0 ? cboLocHT.Text : null;
        string mon = txtLocMon.Text.Trim();
        int? keepId = selectId ?? (keepSel ? _maHD : null);

        List<HoaDonRow> rows;
        await using (var db = new QuanCaPheContext())
        {
            IQueryable<HoaDon> q = db.HoaDons.AsNoTracking().Where(h => h.NgayLap >= tu && h.NgayLap < den);
            if (ban != null) { int b = ban.Value; q = q.Where(h => h.SoBan == b); }
            if (ht != null) q = q.Where(h => h.HinhThucTT == ht);
            if (mon.Length > 0)   // lọc qua bảng chi tiết và bảng món
                q = q.Where(h => h.ChiTietHoaDons.Any(c => c.MaMonNavigation.TenMon.Contains(mon)));

            rows = await q.OrderBy(h => h.MaHD)
                          .Select(h => new HoaDonRow
                          {
                              MaHD = h.MaHD,
                              NgayLap = h.NgayLap,
                              SoBan = h.SoBan,
                              HinhThucTT = h.HinhThucTT,
                              TongTien = h.ChiTietHoaDons.Sum(c => c.SoLuong * c.DonGia)
                          }).ToListAsync();
        }
        if (seq != _seqDS) return;

        _binding = true;
        dgvHD.DataSource = rows;
        bool found = false;
        if (keepId != null)
        {
            foreach (DataGridViewRow r in dgvHD.Rows)
            {
                if ((r.DataBoundItem as HoaDonRow)?.MaHD == keepId)
                {
                    dgvHD.CurrentCell = r.Cells[0];
                    r.Selected = true;
                    found = true;
                    break;
                }
            }
        }
        if (!found) { dgvHD.ClearSelection(); dgvHD.CurrentCell = null; }
        _binding = false;

        lblTongDT.Text = $"Tổng doanh thu đang hiển thị: {rows.Sum(r => r.TongTien):N0} đ";

        if (found) await LoadHoaDonAsync(keepId.Value);
        else { _maHD = null; XoaChiTiet(); }
    }

    private async void btnTim_Click(object sender, EventArgs e)
    {
        if (dtpTu.Value.Date > dtpDen.Value.Date)
        {
            MessageBox.Show("\"Từ ngày\" không được lớn hơn \"Đến ngày\".", "Khoảng ngày không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        try { await LoadDanhSachAsync(); }
        catch (Exception ex) { BaoLoi("tìm kiếm hóa đơn", ex); }
    }

    private async void dgvHD_SelectionChanged(object sender, EventArgs e)
    {
        if (_binding) return;
        try
        {
            if (dgvHD.CurrentRow?.DataBoundItem is HoaDonRow r) await LoadHoaDonAsync(r.MaHD);
            else { _maHD = null; XoaChiTiet(); }
        }
        catch (Exception ex) { BaoLoi("tải chi tiết hóa đơn", ex); }
    }

    // ===================== CHI TIẾT HÓA ĐƠN (lưới phải) =====================

    /// <summary>Đọc một hóa đơn kèm các dòng chi tiết (đã có STT, thành tiền).</summary>
    private static async Task<(HoaDon, List<ChiTietIn>)?> LayHoaDonAsync(int maHD)
    {
        await using var db = new QuanCaPheContext();
        var h = await db.HoaDons.AsNoTracking().FirstOrDefaultAsync(x => x.MaHD == maHD);
        if (h == null) return null;
        var ds = await db.ChiTietHoaDons.AsNoTracking()
                         .Where(c => c.MaHD == maHD)
                         .OrderBy(c => c.MaCT)
                         .Select(c => new ChiTietIn
                         {
                             MaCT = c.MaCT,
                             MaMon = c.MaMon,
                             TenMon = c.MaMonNavigation.TenMon,
                             SoLuong = c.SoLuong,
                             DonGia = c.DonGia,
                             ThanhTien = c.SoLuong * c.DonGia
                         }).ToListAsync();
        for (int i = 0; i < ds.Count; i++) ds[i].STT = i + 1;
        return (h, ds);
    }

    private async Task LoadHoaDonAsync(int maHD)
    {
        int seq = ++_seqCT;
        var data = await LayHoaDonAsync(maHD);
        if (seq != _seqCT) return;
        if (data == null) { _maHD = null; XoaChiTiet(); return; }
        var (h, ds) = data.Value;

        _maHD = maHD;
        _binding = true;
        dtpNgay.Value = h.NgayLap;
        cboBan.SelectedIndex = Math.Clamp(h.SoBan, 1, 10) - 1;
        radTienMat.Checked = h.HinhThucTT == "Tiền mặt";
        radChuyenKhoan.Checked = !radTienMat.Checked;
        txtGhiChu.Text = h.GhiChu ?? "";
        grpCT.Text = "Chi tiết hóa đơn " + MaHDText(h.MaHD);
        dgvCT.DataSource = ds;
        _binding = false;
        lblTongCong.Text = $"Tổng cộng: {ds.Sum(c => c.ThanhTien):N0} đ";
    }

    private void XoaChiTiet()
    {
        _binding = true;
        dgvCT.DataSource = null;
        _binding = false;
        grpCT.Text = "Chi tiết hóa đơn";
        lblTongCong.Text = "Tổng cộng: 0 đ";
    }

    private void dgvCT_SelectionChanged(object sender, EventArgs e)
    {
        if (_binding) return;
        if (dgvCT.CurrentRow?.DataBoundItem is ChiTietIn c)
        {
            nudSL.Value = Math.Clamp(c.SoLuong, (int)nudSL.Minimum, (int)nudSL.Maximum);
            cboMon.SelectedValue = c.MaMon;
        }
    }

    // ===================== HÓA ĐƠN: THÊM – SỬA – XÓA =====================

    /// <summary>Đọc các control vào đối tượng hóa đơn; trả về false (và báo lỗi) nếu thiếu dữ liệu.</summary>
    private bool DocHoaDon(HoaDon h)
    {
        if (cboBan.SelectedIndex < 0)
        {
            MessageBox.Show("Hãy chọn số bàn.", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cboBan.Focus();
            return false;
        }
        var d = dtpNgay.Value;
        h.NgayLap = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0);
        h.SoBan = cboBan.SelectedIndex + 1;
        h.HinhThucTT = radTienMat.Checked ? "Tiền mặt" : "Chuyển khoản";
        h.GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim();
        return true;
    }

    private bool CoChonHoaDon()
    {
        if (_maHD != null) return true;
        MessageBox.Show("Hãy chọn một hóa đơn ở danh sách bên trái.", "Chưa chọn hóa đơn", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return false;
    }

    private async void btnHDMoi_Click(object sender, EventArgs e)
    {
        try
        {
            var h = new HoaDon();
            if (!DocHoaDon(h)) return;
            await using (var db = new QuanCaPheContext())
            {
                db.HoaDons.Add(h);
                await db.SaveChangesAsync();
            }
            await LoadDanhSachAsync(selectId: h.MaHD);
            if (_maHD != h.MaHD)
                MessageBox.Show($"Đã tạo {MaHDText(h.MaHD)} nhưng hóa đơn không nằm trong bộ lọc hiện tại nên chưa hiện ở danh sách.",
                                "Đã tạo hóa đơn", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { BaoLoi("tạo hóa đơn", ex); }
    }

    private async void btnSuaHD_Click(object sender, EventArgs e)
    {
        if (!CoChonHoaDon()) return;
        int id = _maHD.Value;
        try
        {
            await using (var db = new QuanCaPheContext())
            {
                var h = await db.HoaDons.FindAsync(id);
                if (h == null) { await LoadDanhSachAsync(keepSel: false); return; }
                if (!DocHoaDon(h)) return;
                await db.SaveChangesAsync();
            }
            await LoadDanhSachAsync(selectId: id);
        }
        catch (Exception ex) { BaoLoi("sửa hóa đơn", ex); }
    }

    private async void btnXoaHD_Click(object sender, EventArgs e)
    {
        if (!CoChonHoaDon()) return;
        int id = _maHD.Value;
        var kq = MessageBox.Show($"Xóa hóa đơn {MaHDText(id)} và toàn bộ các món trong hóa đơn?", "Xác nhận xóa",
                                 MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (kq != DialogResult.Yes) return;
        try
        {
            await using (var db = new QuanCaPheContext())
            await using (var tx = await db.Database.BeginTransactionAsync())
            {
                await db.ChiTietHoaDons.Where(c => c.MaHD == id).ExecuteDeleteAsync();  // xóa chi tiết trước
                await db.HoaDons.Where(h => h.MaHD == id).ExecuteDeleteAsync();         // rồi mới xóa hóa đơn
                await tx.CommitAsync();
            }
            _maHD = null;
            await LoadDanhSachAsync(keepSel: false);
        }
        catch (Exception ex) { BaoLoi("xóa hóa đơn", ex); }
    }

    // ===================== MÓN TRONG HÓA ĐƠN =====================

    private async void btnThemMon_Click(object sender, EventArgs e)
    {
        if (!CoChonHoaDon()) return;
        if (cboMon.SelectedValue is not int maMon)
        {
            MessageBox.Show("Hãy chọn món cần thêm.", "Chưa chọn món", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cboMon.Focus();
            return;
        }
        int id = _maHD.Value, sl = (int)nudSL.Value;
        try
        {
            await using (var db = new QuanCaPheContext())
            {
                var mon = await db.Mons.FindAsync(maMon);
                if (mon == null) { MessageBox.Show("Món này không còn trong thực đơn.", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                db.ChiTietHoaDons.Add(new ChiTietHoaDon { MaHD = id, MaMon = maMon, SoLuong = sl, DonGia = mon.DonGia }); // giá tại thời điểm thêm
                await db.SaveChangesAsync();
            }
            await LoadDanhSachAsync(selectId: id);
        }
        catch (Exception ex) { BaoLoi("thêm món", ex); }
    }

    private ChiTietIn DongChiTietDangChon()
    {
        if (dgvCT.CurrentRow?.DataBoundItem is ChiTietIn c) return c;
        MessageBox.Show("Hãy chọn một món ở lưới chi tiết.", "Chưa chọn món", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return null;
    }

    private async void btnSuaSL_Click(object sender, EventArgs e)
    {
        if (!CoChonHoaDon()) return;
        var c = DongChiTietDangChon();
        if (c == null) return;
        int id = _maHD.Value, sl = (int)nudSL.Value, maCT = c.MaCT;
        try
        {
            await using (var db = new QuanCaPheContext())
            {
                await db.ChiTietHoaDons.Where(x => x.MaCT == maCT)
                        .ExecuteUpdateAsync(s => s.SetProperty(x => x.SoLuong, sl));
            }
            await LoadDanhSachAsync(selectId: id);
        }
        catch (Exception ex) { BaoLoi("sửa số lượng", ex); }
    }

    private async void btnXoaMon_Click(object sender, EventArgs e)
    {
        if (!CoChonHoaDon()) return;
        var c = DongChiTietDangChon();
        if (c == null) return;
        var kq = MessageBox.Show($"Xóa món \"{c.TenMon}\" khỏi hóa đơn?", "Xác nhận xóa món",
                                 MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (kq != DialogResult.Yes) return;
        int id = _maHD.Value, maCT = c.MaCT;
        try
        {
            await using (var db = new QuanCaPheContext())
            {
                await db.ChiTietHoaDons.Where(x => x.MaCT == maCT).ExecuteDeleteAsync();
            }
            await LoadDanhSachAsync(selectId: id);
        }
        catch (Exception ex) { BaoLoi("xóa món", ex); }
    }

    // ===================== REPORT (FastReport, nguồn List<T> + tham số) =====================

    /// <summary>Lấy dữ liệu in; báo MessageBox và trả null nếu chưa chọn hóa đơn hoặc hóa đơn chưa có món.</summary>
    private async Task<(HoaDon, List<ChiTietIn>)?> LayDuLieuInAsync()
    {
        if (!CoChonHoaDon()) return null;
        var data = await LayHoaDonAsync(_maHD.Value);
        if (data == null)
        {
            MessageBox.Show("Hóa đơn này không còn tồn tại.", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }
        if (data.Value.Item2.Count == 0)
        {
            MessageBox.Show("Hóa đơn chưa có món nào nên không thể in.", "Hóa đơn trống", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return null;
        }
        return data;
    }

    private static FRReport TaoReport(HoaDon h, List<ChiTietIn> ds)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Reports", "HoaDon.frx");
        if (!File.Exists(path)) throw new FileNotFoundException("Không tìm thấy mẫu report: " + path);

        var report = new FRReport();
        report.Load(path);
        report.RegisterData(ds, "ChiTiet");                       // List<ChiTietIn>
        report.GetDataSource("ChiTiet").Enabled = true;
        report.SetParameterValue("TenQuan", TenQuan);             // tham số từ code
        report.SetParameterValue("SoHD", MaHDText(h.MaHD));
        report.SetParameterValue("NgayLap", h.NgayLap.ToString("dd/MM/yyyy HH:mm"));
        report.SetParameterValue("SoBan", h.SoBan.ToString());
        report.SetParameterValue("HinhThuc", h.HinhThucTT);
        return report;
    }

    private async void btnXemHD_Click(object sender, EventArgs e)
    {
        try
        {
            var data = await LayDuLieuInAsync();
            if (data == null) return;
            var (h, ds) = data.Value;
            using var report = TaoReport(h, ds);
            report.Prepare();
            using var f = new PreviewForm(report);
            f.ShowDialog(this);
        }
        catch (Exception ex) { BaoLoi("xem hóa đơn", ex); }
    }

    private async void btnXuatPdf_Click(object sender, EventArgs e)
    {
        try
        {
            var data = await LayDuLieuInAsync();
            if (data == null) return;
            var (h, ds) = data.Value;

            using var dlg = new SaveFileDialog
            {
                Filter = "PDF (*.pdf)|*.pdf",
                FileName = $"HoaDon_{MaHDText(h.MaHD)}.pdf",
                OverwritePrompt = true
            };
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            using var report = TaoReport(h, ds);
            report.Prepare();
            using var pdf = new PDFSimpleExport();
            report.Export(pdf, dlg.FileName);

            var kq = MessageBox.Show($"Đã lưu file:\n{dlg.FileName}\n\nBạn có muốn mở file không?", "Hoàn tất",
                                     MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (kq == DialogResult.Yes)
                Process.Start(new ProcessStartInfo(dlg.FileName) { UseShellExecute = true });
        }
        catch (Exception ex) { BaoLoi("xuất PDF", ex); }
    }

    private async void btnIn_Click(object sender, EventArgs e)
    {
        List<Bitmap> pages = null;
        try
        {
            var data = await LayDuLieuInAsync();
            if (data == null) return;
            var (h, ds) = data.Value;
            using var report = TaoReport(h, ds);
            report.Prepare();
            pages = ReportPages.Render(report, 200);
            ReportPages.In(this, pages, "Hóa đơn " + MaHDText(h.MaHD));
        }
        catch (Exception ex) { BaoLoi("in hóa đơn", ex); }
        finally { if (pages != null) foreach (var p in pages) p.Dispose(); }
    }

    // ===================== TIỆN ÍCH =====================

    private static void BaoLoi(string viec, Exception ex)
    {
        string noiDung = ex switch
        {
            InvalidPrinterException => "Không tìm thấy máy in. Hãy cài đặt máy in (ví dụ \"Microsoft Print to PDF\") rồi thử lại.",
            IOException => "Không ghi được file. Có thể file đang được mở bằng chương trình khác.\nHãy đóng file rồi thử lại.",
            UnauthorizedAccessException => "Không có quyền ghi vào vị trí đã chọn. Hãy chọn thư mục khác.",
            DbUpdateException d => "Lỗi khi lưu vào CSDL: " + (d.InnerException?.Message ?? d.Message),
            _ => ex.Message
        };
        MessageBox.Show($"Lỗi khi {viec}:\n{noiDung}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private sealed class MonItem
    {
        public int MaMon { get; set; }
        public string Hien { get; set; }
    }
}
