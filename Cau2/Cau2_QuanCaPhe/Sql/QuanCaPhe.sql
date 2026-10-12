-- Script tạo CSDL QuanCaPhe (SQL Server LocalDB)
USE master;
GO
IF DB_ID(N'QuanCaPhe') IS NOT NULL
BEGIN
    ALTER DATABASE QuanCaPhe SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanCaPhe;
END
GO
CREATE DATABASE QuanCaPhe;
GO
USE QuanCaPhe;
GO
CREATE TABLE dbo.Mon
(
    MaMon    INT           NOT NULL CONSTRAINT PK_Mon PRIMARY KEY,
    TenMon   NVARCHAR(100) NOT NULL,
    NhomMon  NVARCHAR(30)  NULL,       -- Cà phê, Trà, Bánh, Nước đóng chai
    DonGia   DECIMAL(18,0) NOT NULL CONSTRAINT CK_Mon_DonGia CHECK (DonGia > 0)
);
GO
CREATE TABLE dbo.HoaDon
(
    MaHD        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_HoaDon PRIMARY KEY,
    NgayLap     DATETIME      NOT NULL,
    SoBan       INT           NOT NULL CONSTRAINT CK_HoaDon_SoBan CHECK (SoBan BETWEEN 1 AND 10),
    HinhThucTT  NVARCHAR(20)  NOT NULL CONSTRAINT CK_HoaDon_HinhThucTT CHECK (HinhThucTT IN (N'Tiền mặt', N'Chuyển khoản')),
    GhiChu      NVARCHAR(200) NULL
);
GO
CREATE TABLE dbo.ChiTietHoaDon
(
    MaCT     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY,
    MaHD     INT           NOT NULL CONSTRAINT FK_CT_HoaDon REFERENCES dbo.HoaDon(MaHD),
    MaMon    INT           NOT NULL CONSTRAINT FK_CT_Mon REFERENCES dbo.Mon(MaMon),
    SoLuong  INT           NOT NULL CONSTRAINT CK_CT_SoLuong CHECK (SoLuong > 0),
    DonGia   DECIMAL(18,0) NOT NULL   -- giá tại thời điểm bán
);
GO
INSERT INTO dbo.Mon (MaMon, TenMon, NhomMon, DonGia) VALUES
(1, N'Cà phê sữa đá', N'Cà phê',          29000),
(2, N'Bạc xỉu',       N'Cà phê',          32000),
(3, N'Trà đào cam sả',N'Trà',             45000),
(4, N'Bánh tiramisu', N'Bánh',            39000),
(5, N'Nước suối',     N'Nước đóng chai',  12000);
GO
INSERT INTO dbo.HoaDon (NgayLap, SoBan, HinhThucTT) VALUES
('20261009 08:15:00', 3, N'Tiền mặt'),       -- HD000001
('20261009 09:40:00', 5, N'Chuyển khoản');   -- HD000002
GO
INSERT INTO dbo.ChiTietHoaDon (MaHD, MaMon, SoLuong, DonGia) VALUES
(1, 1, 2, 29000),
(1, 3, 1, 45000),
(1, 4, 1, 39000),
(2, 2, 1, 32000),
(2, 5, 2, 12000);
GO
SELECT h.MaHD, SUM(c.SoLuong * c.DonGia) AS TongTien
FROM dbo.HoaDon h JOIN dbo.ChiTietHoaDon c ON c.MaHD = h.MaHD GROUP BY h.MaHD;  -- 142000, 56000
