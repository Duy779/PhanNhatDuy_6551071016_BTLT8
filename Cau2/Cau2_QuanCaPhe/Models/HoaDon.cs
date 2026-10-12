namespace QuanCaPhe.Models;

public partial class HoaDon
{
    public int MaHD { get; set; }
    public DateTime NgayLap { get; set; }
    public int SoBan { get; set; }
    public string HinhThucTT { get; set; }
    public string GhiChu { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
}
