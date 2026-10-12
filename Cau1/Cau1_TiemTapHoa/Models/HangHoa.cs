namespace TiemTapHoa.Models;

// Lớp này giống kết quả scaffold; nếu bạn scaffold lại thì file này sẽ được sinh đè.
public partial class HangHoa
{
    public int MaHang { get; set; }
    public string TenHang { get; set; }
    public string LoaiHang { get; set; }
    public string DonVi { get; set; }
    public decimal GiaBan { get; set; }
    public int SoLuongTon { get; set; }
    public DateTime NgayNhap { get; set; }
    public bool ConKinhDoanh { get; set; }
}
