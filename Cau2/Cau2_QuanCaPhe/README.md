# Câu 2 – Quán cà phê Góc Phố: hóa đơn gọi món

1. Chạy `Sql/QuanCaPhe.sql` trên `(localdb)\MSSQLLocalDB`:
   sqlcmd -S "(localdb)\MSSQLLocalDB" -E -f 65001 -i Sql\QuanCaPhe.sql
2. Mở `QuanCaPhe.csproj` bằng Visual Studio, Restore NuGet, Build (Ctrl+Shift+B), chạy (F5).
3. Model/DbContext viết sẵn giống kết quả scaffold (tên thuộc tính điều hướng: MaHdNavigation, MaMonNavigation).
   Nếu scaffold lại, tên thuộc tính có thể khác và phải sửa code cho khớp — khuyên dùng bản viết sẵn.
4. Report: Reports/HoaDon.frx (nguồn ChiTiet = List<ChiTietIn>, tham số TenQuan/SoHD/NgayLap/SoBan/HinhThuc).
   Nút In dùng PrintDialog của Windows (bản FastReport OpenSource không có hộp thoại in).
