using System.Drawing.Printing;
using System.Text.RegularExpressions;
using FastReport.Export.Image;
using FRReport = FastReport.Report;

namespace QuanCaPhe;

/// <summary>Tiện ích: biến report đã Prepare() thành ảnh từng trang; in các ảnh đó bằng PrintDocument.</summary>
public static class ReportPages
{
    public static List<Bitmap> Render(FRReport preparedReport, int dpi)
    {
        var pages = new List<Bitmap>();
        string dir = Path.Combine(Path.GetTempPath(), "HoaDon_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            using var exp = new ImageExport
            {
                ImageFormat = ImageExportFormat.Png,
                SeparateFiles = true,
                ResolutionX = dpi,
                ResolutionY = dpi
            };
            preparedReport.Export(exp, Path.Combine(dir, "page.png"));

            foreach (var f in Directory.GetFiles(dir, "*.png").OrderBy(SoTrang))
            {
                using var ms = new MemoryStream(File.ReadAllBytes(f));
                using var img = Image.FromStream(ms);
                pages.Add(new Bitmap(img));
            }
        }
        finally
        {
            try { Directory.Delete(dir, true); } catch { }
        }
        if (pages.Count == 0) throw new InvalidOperationException("Report không có trang nào.");
        return pages;
    }

    private static int SoTrang(string file)
    {
        var m = Regex.Matches(Path.GetFileNameWithoutExtension(file), @"\d+");
        return m.Count > 0 ? int.Parse(m[^1].Value) : 0;
    }

    /// <summary>Mở hộp thoại in của Windows rồi in các trang (ném InvalidPrinterException nếu chưa có máy in).</summary>
    public static void In(IWin32Window owner, IReadOnlyList<Bitmap> pages, string tenTaiLieu)
    {
        using var pd = new PrintDocument { DocumentName = tenTaiLieu };
        int i = 0;
        pd.BeginPrint += (_, _) => i = 0;
        pd.PrintPage += (_, e) =>
        {
            var bmp = pages[i];
            var area = e.PageBounds;
            float scale = Math.Min((float)area.Width / bmp.Width, (float)area.Height / bmp.Height);
            int w = (int)(bmp.Width * scale), h = (int)(bmp.Height * scale);
            e.Graphics.DrawImage(bmp, area.Left + (area.Width - w) / 2, area.Top, w, h);
            i++;
            e.HasMorePages = i < pages.Count;
        };

        using var dlg = new PrintDialog { Document = pd, UseEXDialog = true };
        if (dlg.ShowDialog(owner) == DialogResult.OK)
            pd.Print();
    }
}
