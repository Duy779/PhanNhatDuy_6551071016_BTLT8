namespace QuanCaPhe.Models;

public partial class Mon
{
    public int MaMon { get; set; }
    public string TenMon { get; set; }
    public string NhomMon { get; set; }
    public decimal DonGia { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
}
