using System.Globalization;

namespace QuanCaPhe;

static class AppConfig
{
    public const string ConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=QuanCaPhe;Trusted_Connection=True;TrustServerCertificate=True";
}

static class Program
{
    [STAThread]
    static void Main()
    {
        var vi = new CultureInfo("vi-VN");
        CultureInfo.DefaultThreadCurrentCulture = vi;
        CultureInfo.DefaultThreadCurrentUICulture = vi;
        CultureInfo.CurrentCulture = vi;

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
