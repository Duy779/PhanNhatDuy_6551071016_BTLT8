using FRReport = FastReport.Report;

namespace QuanCaPhe;

/// <summary>Cửa sổ xem trước report (FastReport OpenSource không có sẵn viewer cho WinForms).</summary>
public class PreviewForm : Form
{
    private readonly List<Bitmap> _pages;
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
        Controls.Add(_host);
        Controls.Add(bar);

        _btnPrev.Click += (_, _) => ShowPage(_index - 1);
        _btnNext.Click += (_, _) => ShowPage(_index + 1);
        _host.Resize += (_, _) => CanGiua();

        _pages = ReportPages.Render(preparedReport, 96);
        ShowPage(0);
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
