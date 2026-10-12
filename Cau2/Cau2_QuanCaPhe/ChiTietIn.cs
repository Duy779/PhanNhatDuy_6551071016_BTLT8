namespace QuanCaPhe;

/// <summary>DTO cho lưới chi tiết và cho report (đăng ký với tên "ChiTiet").</summary>
public class ChiTietIn
{
    public int MaCT { get; set; }     // phục vụ sửa/xóa, report không dùng
    public int MaMon { get; set; }    // phục vụ chọn lại món trên form
    public int STT { get; set; }
    public string TenMon { get; set; }
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
}

/// <summary>Một dòng của lưới danh sách hóa đơn (tổng tiền do truy vấn tính).</summary>
public class HoaDonRow
{
    public int MaHD { get; set; }
    public string MaHDText => $"HD{MaHD:D6}";
    public DateTime NgayLap { get; set; }
    public int SoBan { get; set; }
    public string HinhThucTT { get; set; }
    public decimal TongTien { get; set; }
}
