using System.Globalization;

namespace TiemTapHoa;

static class AppConfig
{
    public const string ConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=TiemTapHoa;Trusted_Connection=True;TrustServerCertificate=True";
}

static class Program
{
    [STAThread]
    static void Main()
    {
        // Định dạng số kiểu Việt Nam: 6.000 (dấu chấm hàng nghìn)
        var vi = new CultureInfo("vi-VN");
        CultureInfo.DefaultThreadCurrentCulture = vi;
        CultureInfo.DefaultThreadCurrentUICulture = vi;
        CultureInfo.CurrentCulture = vi;

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
