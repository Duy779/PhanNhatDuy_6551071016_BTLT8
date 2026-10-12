namespace QuanCaPhe.Models;

public partial class ChiTietHoaDon
{
    public int MaCT { get; set; }
    public int MaHD { get; set; }
    public int MaMon { get; set; }
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }

    public virtual HoaDon MaHdNavigation { get; set; }
    public virtual Mon MaMonNavigation { get; set; }
}
