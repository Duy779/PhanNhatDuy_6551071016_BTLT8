using System.Text.RegularExpressions;
using FastReport.Export.Image;
using FRReport = FastReport.Report;

namespace TiemTapHoa;

/// <summary>
/// Cửa sổ xem trước report (FastReport OpenSource không có sẵn viewer cho WinForms).
/// Xuất từng trang của report đã Prepare() thành ảnh PNG rồi hiển thị.
/// </summary>
public class PreviewForm : Form
{
    private readonly List<Bitmap> _pages = new();
    private int _index;
    private readonly Panel _host = new() { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.Gray };
    private readonly PictureBox _pic = new() { SizeMode = PictureBoxSizeMode.AutoSize, BackColor = Color.White };
    private readonly Label _lbl = new() { AutoSize = true, Margin = new Padding(12, 8, 0, 0) };
    private readonly Button _btnPrev = new() { Text = "◀ Trước", AutoSize = true };
    private readonly Button _btnNext = new() { Text = "Sau ▶", AutoSize = true };

    public PreviewForm(FRReport preparedReport)
    {
        Text = "FastReport Preview";
        Font = new Font("Segoe UI", 9F);
        ClientSize = new Size(900, 700);
        StartPosition = FormStartPosition.CenterParent;

        var bar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(6) };
        bar.Controls.AddRange(new Control[] { _btnPrev, _btnNext, _lbl });
        _host.Controls.Add(_pic);
        Controls.Add(_host);   // thêm Fill trước, Top sau để dock đúng thứ tự
        Controls.Add(bar);

        _btnPrev.Click += (_, _) => ShowPage(_index - 1);
        _btnNext.Click += (_, _) => ShowPage(_index + 1);
        _host.Resize += (_, _) => CanGiua();

        NapTrang(preparedReport);
        ShowPage(0);
    }

    private void NapTrang(FRReport report)
    {
        string dir = Path.Combine(Path.GetTempPath(), "BangGia_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            using var exp = new ImageExport
            {
                ImageFormat = ImageExportFormat.Png,
                SeparateFiles = true,          // mỗi trang một file
                ResolutionX = 96,
                ResolutionY = 96
            };
            report.Export(exp, Path.Combine(dir, "page.png"));

            var files = Directory.GetFiles(dir, "*.png").OrderBy(SoTrang).ToList();
            foreach (var f in files)
            {
                using var ms = new MemoryStream(File.ReadAllBytes(f));
                using var img = Image.FromStream(ms);
                _pages.Add(new Bitmap(img));   // sao chép để không phụ thuộc stream/file
            }
        }
        finally
        {
            try { Directory.Delete(dir, true); } catch { /* bỏ qua */ }
        }
        if (_pages.Count == 0) throw new InvalidOperationException("Report không có trang nào để xem trước.");
    }

    private static int SoTrang(string file)
    {
        var m = Regex.Matches(Path.GetFileNameWithoutExtension(file), @"\d+");
        return m.Count > 0 ? int.Parse(m[^1].Value) : 0;
    }

    private void ShowPage(int i)
    {
        if (i < 0 || i >= _pages.Count) return;
        _index = i;
        _pic.Image = _pages[i];
        _lbl.Text = $"Trang {i + 1}/{_pages.Count}";
        _btnPrev.Enabled = i > 0;
        _btnNext.Enabled = i < _pages.Count - 1;
        CanGiua();
        _host.AutoScrollPosition = new Point(0, 0);
    }

    private void CanGiua()
    {
        _pic.Left = Math.Max(10, (_host.ClientSize.Width - _pic.Width) / 2);
        _pic.Top = 10;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _pic.Image = null;
        foreach (var b in _pages) b.Dispose();
        base.OnFormClosed(e);
    }
}
