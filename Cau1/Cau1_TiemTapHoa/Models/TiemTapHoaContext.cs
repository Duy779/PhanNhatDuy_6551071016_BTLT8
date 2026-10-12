using Microsoft.EntityFrameworkCore;

namespace TiemTapHoa.Models;

public partial class TiemTapHoaContext : DbContext
{
    public TiemTapHoaContext() { }

    public TiemTapHoaContext(DbContextOptions<TiemTapHoaContext> options) : base(options) { }

    public virtual DbSet<HangHoa> HangHoas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer(AppConfig.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HangHoa>(entity =>
        {
            entity.HasKey(e => e.MaHang).HasName("PK_HangHoa");
            entity.ToTable("HangHoa");

            entity.Property(e => e.DonVi).HasMaxLength(20);
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.LoaiHang).HasMaxLength(50);
            entity.Property(e => e.NgayNhap).HasColumnType("datetime");
            entity.Property(e => e.TenHang).IsRequired().HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
