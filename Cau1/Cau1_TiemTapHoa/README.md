# Câu 1 – Tiệm tạp hóa Nhà trường UTC2

## Các bước chạy
1. Chạy `Sql/TiemTapHoa.sql` trên `(localdb)\MSSQLLocalDB` (SSMS / Azure Data Studio / SQL Server Object Explorer).
2. Mở thư mục bằng Visual Studio 2022+ (hoặc `dotnet restore`).
3. (Tuỳ chọn) scaffold lại để lấy Model do EF sinh:

   dotnet tool install --global dotnet-ef
   dotnet ef dbcontext scaffold "Server=(localdb)\MSSQLLocalDB;Database=TiemTapHoa;Trusted_Connection=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -c TiemTapHoaContext --force

   (Model/Context đã viết sẵn giống kết quả scaffold nên có thể bỏ qua bước này.)
4. F5 để chạy. File `Reports/BangGia.frx` tự được copy sang thư mục output.

## Gói NuGet
Microsoft.EntityFrameworkCore.SqlServer, Microsoft.EntityFrameworkCore.Design, ClosedXML,
FastReport.OpenSource, FastReport.OpenSource.Export.PdfSimple.
(Bản OpenSource không có viewer cho WinForms nên dùng `PreviewForm.cs` tự viết.)
Nếu dùng FastReport .NET bản thương mại/trial của lớp: thay các gói FastReport.OpenSource.* bằng gói `FastReport.Net`
và đổi `using FastReport.Export.PdfSimple` + `PDFSimpleExport` thành `FastReport.Export.Pdf` + `PDFExport`.
