using System.Data;
using System.Diagnostics;
using ClosedXML.Excel;
using FastReport.Export.PdfSimple;
using Microsoft.EntityFrameworkCore;
using TiemTapHoa.Models;
using FRReport = FastReport.Report;

namespace TiemTapHoa;

public partial class MainForm : Form
{
    private bool _binding;          // đang nạp lưới -> bỏ qua SelectionChanged
    private bool _suppressFilter;   // đang đặt lại bộ lọc bằng code -> không tải lại nhiều lần
    private int _loadSeq;           // chống kết quả cũ ghi đè kết quả mới khi gõ nhanh

    public MainForm()
    {
        InitializeComponent();
    }

    // ===================== KHỞI TẠO & TẢI DỮ LIỆU =====================

    private async void MainForm_Load(object sender, EventArgs e)
    {
        try
        {
            _suppressFilter = true;
            cboLocLoai.SelectedIndex = 0;
            _suppressFilter = false;
            ClearInputs();
            await LoadDataAsync(keepSelection: false);
        }
        catch (Exception ex) { BaoLoi("tải dữ liệu", ex); }
    }

    /// <summary>Truy vấn (async) theo bộ lọc hiện tại rồi đổ lên lưới.</summary>
    private async Task LoadDataAsync(bool keepSelection = true, int? selectId = null)
    {
        int seq = ++_loadSeq;
        string keyword = txtTim.Text.Trim();
        string loai = cboLocLoai.SelectedIndex > 0 ? cboLocLoai.Text : null;
        bool chiCon = chkChiCon.Checked;
        int? keepId = selectId ?? (keepSelection ? GetSelectedId() : null);

        List<HangHoa> list;
        await using (var db = new TiemTapHoaContext())
        {
            IQueryable<HangHoa> q = db.HangHoas.AsNoTracking();
            if (keyword.Length > 0) q = q.Where(h => h.TenHang.Contains(keyword));
            if (loai != null) q = q.Where(h => h.LoaiHang == loai);
            if (chiCon) q = q.Where(h => h.ConKinhDoanh);
            list = await q.OrderBy(h => h.MaHang).ToListAsync();
        }
        if (seq != _loadSeq) return;   // đã có lần tải mới hơn

        _binding = true;
        dgv.DataSource = list;
        _binding = false;

        bool found = false;
        if (keepId != null)
        {
            foreach (DataGridViewRow r in dgv.Rows)
            {
                if ((r.DataBoundItem as HangHoa)?.MaHang == keepId)
                {
                    dgv.CurrentCell = r.Cells[1];
                    r.Selected = true;
                    found = true;
                    break;
                }
            }
        }
        if (!found)
        {
            dgv.ClearSelection();
            dgv.CurrentCell = null;
        }
        lblTrangThai.Text = $"Tìm thấy {list.Count} mặt hàng";
    }

    private int? GetSelectedId() => (dgv.CurrentRow?.DataBoundItem as HangHoa)?.MaHang;

    // ===================== LƯỚI <-> CONTROL NHẬP LIỆU =====================

    private void dgv_SelectionChanged(object sender, EventArgs e)
    {
        if (_binding) return;
        if (dgv.CurrentRow?.DataBoundItem is HangHoa h) HienThi(h);
    }

    private void HienThi(HangHoa h)
    {
        txtTenHang.Text = h.TenHang;
        ChonItem(cboLoai, h.LoaiHang);
        ChonItem(cboDonVi, h.DonVi);
        nudGia.Value = Math.Min(Math.Max(h.GiaBan, nudGia.Minimum), nudGia.Maximum);
        nudTon.Value = Math.Min(Math.Max(h.SoLuongTon, nudTon.Minimum), nudTon.Maximum);
        dtpNgay.Value = h.NgayNhap < dtpNgay.MinDate ? dtpNgay.MinDate : h.NgayNhap;
        chkConKD.Checked = h.ConKinhDoanh;
    }

    private static void ChonItem(ComboBox cbo, string value)
    {
        if (string.IsNullOrEmpty(value)) { cbo.SelectedIndex = -1; return; }
        int idx = cbo.FindStringExact(value);
        if (idx < 0) { cbo.Items.Add(value); idx = cbo.Items.Count - 1; }
        cbo.SelectedIndex = idx;
    }

    private void DocForm(HangHoa h)
    {
        h.TenHang = txtTenHang.Text.Trim();
        h.LoaiHang = cboLoai.SelectedItem as string;
        h.DonVi = cboDonVi.SelectedItem as string;
        h.GiaBan = nudGia.Value;
        h.SoLuongTon = (int)nudTon.Value;
        h.NgayNhap = dtpNgay.Value;
        h.ConKinhDoanh = chkConKD.Checked;
    }

    private void ClearInputs()
    {
        txtTenHang.Clear();
        cboLoai.SelectedIndex = -1;
        cboDonVi.SelectedIndex = -1;
        nudGia.Value = nudGia.Minimum;
        nudTon.Value = 0;
        dtpNgay.Value = DateTime.Now;
        chkConKD.Checked = true;
    }

    private bool KiemTraDuLieu()
    {
        if (string.IsNullOrWhiteSpace(txtTenHang.Text))
        {
            MessageBox.Show("Tên hàng không được để trống.", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTenHang.Focus();
            return false;
        }
        if (nudGia.Value <= 0)
        {
            MessageBox.Show("Giá bán phải lớn hơn 0.", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            nudGia.Focus();
            nudGia.Select(0, nudGia.Text.Length);
            return false;
        }
        return true;
    }

    // ===================== THÊM – SỬA – XÓA – LÀM MỚI =====================

    private async void btnThem_Click(object sender, EventArgs e)
    {
        if (!KiemTraDuLieu()) return;
        try
        {
            var h = new HangHoa();
            DocForm(h);
            await using (var db = new TiemTapHoaContext())
            {
                db.HangHoas.Add(h);
                await db.SaveChangesAsync();
            }
            await LoadDataAsync(selectId: h.MaHang);
            MessageBox.Show("Đã thêm hàng hóa mới.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { BaoLoi("thêm hàng hóa", ex); }
    }

    private async void btnSua_Click(object sender, EventArgs e)
    {
        int? id = GetSelectedId();
        if (id == null)
        {
            MessageBox.Show("Hãy chọn một mặt hàng trên lưới để sửa.", "Chưa chọn dòng", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!KiemTraDuLieu()) return;
        try
        {
            await using (var db = new TiemTapHoaContext())
            {
                var h = await db.HangHoas.FindAsync(id.Value);
                if (h == null)
                {
                    MessageBox.Show("Mặt hàng này không còn tồn tại (có thể đã bị xóa).", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    await LoadDataAsync(keepSelection: false);
                    return;
                }
                DocForm(h);
                await db.SaveChangesAsync();
            }
            await LoadDataAsync(selectId: id);
            MessageBox.Show("Đã cập nhật hàng hóa.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { BaoLoi("sửa hàng hóa", ex); }
    }

    private async void btnXoa_Click(object sender, EventArgs e)
    {
        if (dgv.CurrentRow?.DataBoundItem is not HangHoa sel)
        {
            MessageBox.Show("Hãy chọn một mặt hàng trên lưới để xóa.", "Chưa chọn dòng", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var kq = MessageBox.Show($"Bạn có chắc muốn xóa \"{sel.TenHang}\" không?", "Xác nhận xóa",
                                 MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (kq != DialogResult.Yes) return;
        try
        {
            await using (var db = new TiemTapHoaContext())
            {
                await db.HangHoas.Where(h => h.MaHang == sel.MaHang).ExecuteDeleteAsync();
            }
            ClearInputs();
            await LoadDataAsync(keepSelection: false);
        }
        catch (Exception ex) { BaoLoi("xóa hàng hóa", ex); }
    }

    private async void btnLamMoi_Click(object sender, EventArgs e)
    {
        try
        {
            tmrTim.Stop();
            _suppressFilter = true;
            txtTim.Clear();
            cboLocLoai.SelectedIndex = 0;
            chkChiCon.Checked = false;
            _suppressFilter = false;

            await LoadDataAsync(keepSelection: false);
            ClearInputs();
            txtTenHang.Focus();
        }
        catch (Exception ex) { BaoLoi("làm mới dữ liệu", ex); }
    }

    // ===================== TÌM KIẾM (Timer 350 ms) =====================

    private void txtTim_TextChanged(object sender, EventArgs e)
    {
        if (_suppressFilter) return;
        tmrTim.Stop();     // gõ tiếp thì đếm lại từ đầu
        tmrTim.Start();
    }

    private async void tmrTim_Tick(object sender, EventArgs e)
    {
        tmrTim.Stop();
        try { await LoadDataAsync(); }
        catch (Exception ex) { BaoLoi("tìm kiếm", ex); }
    }

    private async void LocThayDoi(object sender, EventArgs e)
    {
        if (_suppressFilter) return;
        try { await LoadDataAsync(); }
        catch (Exception ex) { BaoLoi("lọc dữ liệu", ex); }
    }

    // ===================== REPORT (FastReport, nguồn DataTable) =====================

    /// <summary>Truy vấn hàng còn kinh doanh bằng EF Core rồi chuyển sang DataTable.</summary>
    private static async Task<DataTable> LayBangGiaAsync()
    {
        List<HangHoa> ds;
        await using (var db = new TiemTapHoaContext())
        {
            ds = await db.HangHoas.AsNoTracking()
                         .Where(h => h.ConKinhDoanh)
                         .OrderBy(h => h.MaHang)
                         .ToListAsync();
        }
        var dt = new DataTable("HangHoa");
        dt.Columns.Add("TenHang", typeof(string));
        dt.Columns.Add("DonVi", typeof(string));
        dt.Columns.Add("GiaBan", typeof(decimal));
        foreach (var h in ds) dt.Rows.Add(h.TenHang, h.DonVi, h.GiaBan);
        return dt;
    }

    private static FRReport TaoReportBangGia(DataTable dt)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Reports", "BangGia.frx");
        if (!File.Exists(path))
            throw new FileNotFoundException("Không tìm thấy mẫu report: " + path);

        var report = new FRReport();
        report.Load(path);
        report.RegisterData(dt, "HangHoa");
        report.GetDataSource("HangHoa").Enabled = true;                       // bắt buộc
        report.SetParameterValue("NgayIn", DateTime.Now.ToString("dd/MM/yyyy")); // tham số từ code
        return report;
    }

    private async void btnXemBangGia_Click(object sender, EventArgs e)
    {
        try
        {
            var dt = await LayBangGiaAsync();
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Không có mặt hàng nào đang kinh doanh để in bảng giá.", "Không có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using var report = TaoReportBangGia(dt);
            report.Prepare();
            using var f = new PreviewForm(report);   // form xem trước tự viết
            f.ShowDialog(this);
        }
        catch (Exception ex) { BaoLoi("xem bảng giá", ex); }
    }

    private async void btnXuatPdf_Click(object sender, EventArgs e)
    {
        try
        {
            var dt = await LayBangGiaAsync();
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Không có mặt hàng nào đang kinh doanh để xuất PDF.", "Không có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string path = ChonFileLuu("PDF (*.pdf)|*.pdf", "BangGia.pdf");
            if (path == null) return;

            using var report = TaoReportBangGia(dt);
            report.Prepare();
            using var pdf = new PDFSimpleExport();
            report.Export(pdf, path);

            HoiMoFile(path);
        }
        catch (Exception ex) { BaoLoi("xuất PDF", ex); }
    }

    // ===================== XUẤT EXCEL (ClosedXML) =====================

    /// <summary>Cách 1: xuất đúng các dòng đang hiển thị trên DataGridView.</summary>
    private async void btnXuatExcelLuoi_Click(object sender, EventArgs e)
    {
        try
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("Lưới đang không có dòng nào để xuất.", "Không có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string path = ChonFileLuu("Excel (*.xlsx)|*.xlsx", "DanhSachHangHoa.xlsx");
            if (path == null) return;

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Hàng hóa");
            var cols = dgv.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).OrderBy(c => c.DisplayIndex).ToList();

            for (int c = 0; c < cols.Count; c++)
                ws.Cell(1, c + 1).Value = cols[c].HeaderText;

            int r = 2;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                for (int c = 0; c < cols.Count; c++)
                    GanGiaTri(ws.Cell(r, c + 1), row.Cells[cols[c].Index].Value);
                r++;
            }
            DinhDangBang(ws, cols.Count, r - 1);

            await Task.Run(() => wb.SaveAs(path));
            HoiMoFile(path);
        }
        catch (Exception ex) { BaoLoi("xuất Excel", ex); }
    }

    /// <summary>Gán giá trị đúng kiểu để Excel hiểu là số / ngày chứ không phải chuỗi.</summary>
    private static void GanGiaTri(IXLCell cell, object v)
    {
        switch (v)
        {
            case null: break;
            case decimal d: cell.Value = d; cell.Style.NumberFormat.Format = "#,##0"; break;
            case int i: cell.Value = i; break;
            case DateTime dt: cell.Value = dt; cell.Style.NumberFormat.Format = "dd/MM/yyyy"; break;
            case bool b: cell.Value = b ? "Còn bán" : "Ngừng bán"; break;
            default: cell.Value = v.ToString(); break;
        }
    }

    private static void DinhDangBang(IXLWorksheet ws, int soCot, int dongCuoi)
    {
        var header = ws.Range(1, 1, 1, soCot);
        header.Style.Font.Bold = true;
        header.Style.Fill.BackgroundColor = XLColor.LightGray;
        header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Range(1, 1, dongCuoi, soCot).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        ws.Range(1, 1, dongCuoi, soCot).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Columns().AdjustToContents();
    }

    /// <summary>Cách 2: truy vấn lại bằng EF Core, có công thức và dòng Tổng.</summary>
    private async void btnXuatExcelTonKho_Click(object sender, EventArgs e)
    {
        try
        {
            List<HangHoa> ds;
            await using (var db = new TiemTapHoaContext())
            {
                ds = await db.HangHoas.AsNoTracking()
                             .Where(h => h.ConKinhDoanh)
                             .OrderBy(h => h.MaHang)
                             .ToListAsync();
            }
            if (ds.Count == 0)
            {
                MessageBox.Show("Không có mặt hàng nào đang kinh doanh để xuất tồn kho.", "Không có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string path = ChonFileLuu("Excel (*.xlsx)|*.xlsx", "TonKho.xlsx");
            if (path == null) return;

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Tồn kho");
            string[] tieuDe = { "STT", "Tên hàng", "Loại hàng", "Đơn vị", "Giá bán", "Số lượng tồn", "Ngày nhập", "Giá trị tồn" };
            for (int c = 0; c < tieuDe.Length; c++) ws.Cell(1, c + 1).Value = tieuDe[c];

            int r = 2;
            foreach (var h in ds)
            {
                ws.Cell(r, 1).Value = r - 1;
                ws.Cell(r, 2).Value = h.TenHang;
                ws.Cell(r, 3).Value = h.LoaiHang;
                ws.Cell(r, 4).Value = h.DonVi;
                ws.Cell(r, 5).Value = h.GiaBan;
                ws.Cell(r, 6).Value = h.SoLuongTon;
                ws.Cell(r, 7).Value = h.NgayNhap;
                ws.Cell(r, 8).FormulaA1 = $"E{r}*F{r}";           // Giá trị tồn = giá bán × SL tồn
                r++;
            }
            int cuoi = r - 1;                                       // dòng dữ liệu cuối

            ws.Range(2, 5, cuoi, 6).Style.NumberFormat.Format = "#,##0";
            ws.Range(2, 7, cuoi, 7).Style.NumberFormat.Format = "dd/MM/yyyy";
            ws.Range(2, 8, cuoi + 1, 8).Style.NumberFormat.Format = "#,##0";

            // Dòng Tổng
            ws.Cell(r, 1).Value = "Tổng";
            ws.Cell(r, 8).FormulaA1 = $"SUM(H2:H{cuoi})";
            ws.Range(r, 1, r, 8).Style.Font.Bold = true;

            DinhDangBang(ws, tieuDe.Length, r);
            ws.Range(1, 1, cuoi, tieuDe.Length).SetAutoFilter();   // AutoFilter (không gồm dòng Tổng)
            ws.SheetView.FreezeRows(1);                             // cố định dòng tiêu đề

            await Task.Run(() => wb.SaveAs(path));
            HoiMoFile(path);
        }
        catch (Exception ex) { BaoLoi("xuất Excel tồn kho", ex); }
    }

    // ===================== TIỆN ÍCH =====================

    private static string ChonFileLuu(string filter, string tenMacDinh)
    {
        using var dlg = new SaveFileDialog { Filter = filter, FileName = tenMacDinh, OverwritePrompt = true };
        return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;
    }

    private static void HoiMoFile(string path)
    {
        var kq = MessageBox.Show($"Đã lưu file:\n{path}\n\nBạn có muốn mở file không?", "Hoàn tất",
                                 MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (kq == DialogResult.Yes)
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }

    private static void BaoLoi(string viec, Exception ex)
    {
        string noiDung = ex switch
        {
            IOException => "Không ghi được file. Có thể file đang được mở bằng Excel/trình đọc PDF.\n" +
                           "Hãy đóng file rồi thử lại.",
            UnauthorizedAccessException => "Không có quyền ghi vào vị trí đã chọn. Hãy chọn thư mục khác.",
            DbUpdateException d => "Lỗi khi lưu vào CSDL: " + (d.InnerException?.Message ?? d.Message),
            _ => ex.Message
        };
        MessageBox.Show($"Lỗi khi {viec}:\n{noiDung}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
