-- Script tạo CSDL TiemTapHoa (SQL Server LocalDB)
-- Chạy trong SSMS / Azure Data Studio / VS (kết nối (localdb)\MSSQLLocalDB)
USE master;
GO
IF DB_ID(N'TiemTapHoa') IS NOT NULL
BEGIN
    ALTER DATABASE TiemTapHoa SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TiemTapHoa;
END
GO
CREATE DATABASE TiemTapHoa;
GO
USE TiemTapHoa;
GO
CREATE TABLE dbo.HangHoa
(
    MaHang        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_HangHoa PRIMARY KEY,
    TenHang       NVARCHAR(100)     NOT NULL,
    LoaiHang      NVARCHAR(50)      NULL,   -- Đồ uống, Thực phẩm khô, Gia vị, Đồ gia dụng
    DonVi         NVARCHAR(20)      NULL,   -- Chai, Gói, Hộp, Túi
    GiaBan        DECIMAL(18,0)     NOT NULL CONSTRAINT CK_HangHoa_GiaBan CHECK (GiaBan > 0),
    SoLuongTon    INT               NOT NULL CONSTRAINT CK_HangHoa_SoLuongTon CHECK (SoLuongTon >= 0),
    NgayNhap      DATETIME          NOT NULL,
    ConKinhDoanh  BIT               NOT NULL   -- 1 = còn bán, 0 = ngừng bán
);
GO
INSERT INTO dbo.HangHoa (TenHang, LoaiHang, DonVi, GiaBan, SoLuongTon, NgayNhap, ConKinhDoanh) VALUES
(N'Nước suối Aquafina 500ml',  N'Đồ uống',       N'Chai',  6000, 120, '20261002', 1),
(N'Mì Hảo Hảo tôm chua cay',   N'Thực phẩm khô', N'Gói',   4500, 200, '20261002', 1),
(N'Dầu ăn Tường An 1L',        N'Gia vị',        N'Chai', 48000,  35, '20261005', 1),
(N'Bột giặt OMO 800g',         N'Đồ gia dụng',   N'Túi',  52000,  40, '20261005', 1),
(N'Sữa tươi Vinamilk 1L',      N'Đồ uống',       N'Hộp',  32000,  60, '20261008', 1),
(N'Nước mắm Nam Ngư 500ml',    N'Gia vị',        N'Chai', 38000,  25, '20261001', 0);
GO
SELECT * FROM dbo.HangHoa;
