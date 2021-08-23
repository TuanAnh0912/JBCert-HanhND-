using System;
using System.IO;
using jbcert.DATA.IdentityModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class JBC_CoreContext : IdentityDbContext<ApplicationUser>
    {
        string connectionString = "";
        public JBC_CoreContext()
        {
            string basePath = Directory.GetCurrentDirectory();
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("ConnStr");
        }

        public JBC_CoreContext(DbContextOptions<JBC_CoreContext> options)
            : base(options)
        {
            string basePath = Directory.GetCurrentDirectory();
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("ConnStr");
        }

        public virtual DbSet<AnhLoaiBang> AnhLoaiBangs { get; set; }
        public virtual DbSet<AnhPhoi> AnhPhois { get; set; }
        //public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        //public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        //public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        //public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        //public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        //public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        //public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual DbSet<Bang> Bangs { get; set; }
        public virtual DbSet<CapDonVi> CapDonVis { get; set; }
        public virtual DbSet<ChucNang> ChucNangs { get; set; }
        public virtual DbSet<DanToc> DanTocs { get; set; }
        public virtual DbSet<DonVi> DonVis { get; set; }
        public virtual DbSet<FileDinhKemYeuCau> FileDinhKemYeuCaus { get; set; }
        public virtual DbSet<GioiTinh> GioiTinhs { get; set; }
        public virtual DbSet<HinhThucCap> HinhThucCaps { get; set; }
        public virtual DbSet<HocSinh> HocSinhs { get; set; }
        public virtual DbSet<HocSinhFileDinhKem> HocSinhFileDinhKems { get; set; }
        public virtual DbSet<Huyen> Huyens { get; set; }
        public virtual DbSet<LienKetCapDonViChucNang> LienKetCapDonViChucNangs { get; set; }
        public virtual DbSet<LienKetHocSinhYeuCau> LienKetHocSinhYeuCaus { get; set; }
        public virtual DbSet<LienKetLoaiYeuCauCapDonVi> LienKetLoaiYeuCauCapDonVis { get; set; }
        public virtual DbSet<LienKetNhomNguoiDungChucNang> LienKetNhomNguoiDungChucNangs { get; set; }
        public virtual DbSet<LoaiBang> LoaiBangs { get; set; }
        public virtual DbSet<LoaiYeuCau> LoaiYeuCaus { get; set; }
        public virtual DbSet<LogHocSinh> LogHocSinhs { get; set; }
        public virtual DbSet<LogNguoiDung> LogNguoiDungs { get; set; }
        public virtual DbSet<LogVanBang> LogVanBangs { get; set; }
        public virtual DbSet<LogYeuCau> LogYeuCaus { get; set; }
        public virtual DbSet<NguoiDung> NguoiDungs { get; set; }
        public virtual DbSet<NhomNguoiDung> NhomNguoiDungs { get; set; }
        public virtual DbSet<PhoiVanBang> Phois { get; set; }
        public virtual DbSet<PhongBan> PhongBans { get; set; }
        public virtual DbSet<QuyTacFormat> QuyTacFormats { get; set; }
        public virtual DbSet<ThongTinVanBang> ThongTinVanBangs { get; set; }
        public virtual DbSet<Tinh> Tinhs { get; set; }
        public virtual DbSet<TrangThaiBang> TrangThaiBangs { get; set; }
        public virtual DbSet<TrangThaiPhoi> TrangThaiPhois { get; set; }
        public virtual DbSet<TrangThaiYeuCau> TrangThaiYeuCaus { get; set; }
        public virtual DbSet<TruongDuLieu> TruongDuLieus { get; set; }
        public virtual DbSet<TruongDuLieuLoaiBang> TruongDuLieuLoaiBangs { get; set; }
        public virtual DbSet<Xa> Xas { get; set; }
        public virtual DbSet<YeuCau> YeuCaus { get; set; }
        public virtual DbSet<MonHoc> MonHocs { get; set; }
        public virtual DbSet<DiemMonHoc> DiemMonHocs { get; set; }
        public virtual DbSet<LopHoc> LopHocs { get; set; }
        public virtual DbSet<NhapPhoi> NhapPhois { get; set; }
        public virtual DbSet<XepLoaiTotNghiep> XepLoaiTotNghieps { get; set; }
        public virtual DbSet<MauCongVan> MauCongVans { get; set; }
        public virtual DbSet<FileDinhKemPhoi> FileDinhKemPhois { get; set; }
        public virtual DbSet<LogPhoi> LogPhois { get; set; }
        public virtual DbSet<FileDinhKemLoaiBang> FileDinhKemLoaiBangs { get; set; }
        public virtual DbSet<AnhVanBang> AnhVanBangs { get; set; }
        public virtual DbSet<LogCaiChinh> LogCaiChinhs { get; set; }
        public virtual DbSet<NhomTaoVanBang> NhomTaoVanBangs { get; set; }
        public virtual DbSet<HocSinhTrongNhomTaoVanBang> HocSinhTrongNhomTaoVanBangs { get; set; }
        public virtual DbSet<BangCu> BangCus { get; set; }
        public virtual DbSet<CaiChinh> CaiChinhs { get; set; }
        public virtual DbSet<ThongTinCaiChinh> ThongTinCaiChinhs { get; set; }
        public virtual DbSet<ThongTinVanBangCu> ThongTinVanBangCus { get; set; }
        public virtual DbSet<LoaiNhomTaoVanBang> LoaiNhomTaoVanBangs { get; set; }
        public virtual DbSet<FileDinhKemCaiChinh> FileDinhKemCaiChinhs { get; set; }
        public virtual DbSet<ThongBao> ThongBaos { get; set; }
        public virtual DbSet<RoomUserConnection> RoomUserConnections { get; set; }
        public virtual DbSet<UserConnection> UserConnections { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<ThongBaoType> ThongBaoTypes { get; set; }
        public virtual DbSet<ThongBaoTypeCapDonVi> ThongBaoTypeCapDonVis { get; set; }
        public virtual DbSet<LogCapPhoi> LogCapPhois { get; set; }
        public virtual DbSet<NhapPhoi_Phoi> NhapPhoi_Phois { get; set; }
        public virtual DbSet<YeuCauPhatBang> YeuCauPhatBangs { get; set; }
        public virtual DbSet<AnhYeuCauPhatBang> AnhYeuCauPhatBangs { get; set; }
        public virtual DbSet<TrangThaiYeuCauPhatBang> TrangThaiYeuCauPhatBangs { get; set; }
        public virtual DbSet<FileHocSinhYeuCau> FileHocSinhYeuCaus  { get; set; }
        public virtual DbSet<SoHoa> SoHoas  { get; set; }
        public virtual DbSet<SoGocFileDinhKem> SoGocFileDinhKem { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnhLoaiBang>(entity =>
            {
                entity.ToTable("AnhLoaiBang");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.Property(e => e.Url).IsUnicode(false);
            });

            modelBuilder.Entity<AnhPhoi>(entity =>
            {
                entity.ToTable("AnhPhoi");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.Property(e => e.Url).IsUnicode(false);
            });

            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Bang>(entity =>
            {
                entity.ToTable("Bang");

                entity.HasIndex(e => e.HocSinhId, "IX_Bang_HocSinhId");

                entity.HasIndex(e => e.PhoiId, "IX_Bang_PhoiId");

                entity.HasIndex(e => e.TrangThaiBangId, "IX_Bang_TrangThaiBangId");

                entity.Property(e => e.CmtnguoiLayBang)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CMTNguoiLayBang");

                entity.Property(e => e.DuongDanFileAnh).IsUnicode(false);

                entity.Property(e => e.DuongDanFileDeIn).IsUnicode(false);

                entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.Property(e => e.SoDienThoaiNguoiLayBang)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Phoi)
                    .WithMany(p => p.Bangs)
                    .HasForeignKey(d => d.PhoiId)
                    .HasConstraintName("FK_Bang_Phoi");

                entity.HasOne(d => d.TrangThaiBang)
                    .WithMany(p => p.Bangs)
                    .HasForeignKey(d => d.TrangThaiBangId)
                    .HasConstraintName("FK_Bang_TrangThaiBang");
            });

            modelBuilder.Entity<CapDonVi>(entity =>
            {
                entity.ToTable("CapDonVi");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.TenCapDonVi).HasMaxLength(150);
            });

            modelBuilder.Entity<ChucNang>(entity =>
            {
                entity.ToTable("ChucNang");

                entity.Property(e => e.AuthCode).HasMaxLength(50);

                entity.Property(e => e.Icon).HasMaxLength(50);

                entity.Property(e => e.TenChucNang).HasMaxLength(150);
            });

            modelBuilder.Entity<DanToc>(entity =>
            {
                entity.ToTable("DanToc");

                entity.Property(e => e.Ten).HasMaxLength(50);
            });

            modelBuilder.Entity<DonVi>(entity =>
            {
                entity.ToTable("DonVi");

                entity.HasIndex(e => e.CapDonViId, "IX_DonVi_CapDonViId");

                entity.Property(e => e.DiaGioiHanhChinh).HasMaxLength(10);

                entity.Property(e => e.MaDonVi).HasMaxLength(10);

                entity.Property(e => e.TenDonVi).HasMaxLength(250);

                entity.HasOne(d => d.CapDonVi)
                    .WithMany(p => p.DonVis)
                    .HasForeignKey(d => d.CapDonViId)
                    .HasConstraintName("FK_DonVi_CapDonVi");
            });

            modelBuilder.Entity<FileDinhKemYeuCau>(entity =>
            {
                entity.HasKey(e => e.FileId)
                    .HasName("PK_FileDinhKemYeuCau_1");

                entity.ToTable("FileDinhKemYeuCau");

                entity.HasIndex(e => e.YeuCauId, "IX_FileDinhKemYeuCau_YeuCauId");

                entity.Property(e => e.FileId).HasMaxLength(30);

                entity.Property(e => e.Ext).HasMaxLength(10);

                entity.Property(e => e.IconFile).HasMaxLength(250);

                entity.Property(e => e.MimeTypes).HasMaxLength(550);

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.Property(e => e.TenFile).HasMaxLength(250);

                entity.Property(e => e.Url).HasMaxLength(550);

                entity.HasOne(d => d.YeuCau)
                    .WithMany(p => p.FileDinhKemYeuCaus)
                    .HasForeignKey(d => d.YeuCauId)
                    .HasConstraintName("FK_FileDinhKemYeuCau_YeuCau");
            });

            modelBuilder.Entity<GioiTinh>(entity =>
            {
                entity.ToTable("GioiTinh");

                entity.Property(e => e.Ten).HasMaxLength(10);
            });

            modelBuilder.Entity<HinhThucCap>(entity =>
            {
                entity.ToTable("HinhThucCap");
            });

            modelBuilder.Entity<HocSinh>(entity =>
            {
                entity.ToTable("HocSinh");

                entity.HasIndex(e => e.DanTocId, "IX_HocSinh_DanTocId");

                entity.HasIndex(e => e.GioiTinhId, "IX_HocSinh_GioiTinhId");

                entity.HasIndex(e => e.TruongHocId, "IX_HocSinh_TruongHocId");

                entity.Property(e => e.DanToc).HasMaxLength(20);

                entity.Property(e => e.GioiTinh).HasMaxLength(40);

                entity.Property(e => e.HinhThucDaoTao).HasMaxLength(50);

                entity.Property(e => e.HoKhauThuongTru).HasMaxLength(250);

                entity.Property(e => e.HoVaTen).HasMaxLength(100);

                entity.Property(e => e.LopHoc).HasMaxLength(100);

                entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");

                entity.Property(e => e.NgaySinh).HasColumnType("date");

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.Property(e => e.NoiSinh).HasMaxLength(250);

                entity.Property(e => e.SoVaoSo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.XepLoaiTotNghiep).HasMaxLength(50);

                entity.HasOne(d => d.DanTocNavigation)
                    .WithMany(p => p.HocSinhs)
                    .HasForeignKey(d => d.DanTocId)
                    .HasConstraintName("FK_HocSinh_DanToc");

                entity.HasOne(d => d.GioiTinhNavigation)
                    .WithMany(p => p.HocSinhs)
                    .HasForeignKey(d => d.GioiTinhId)
                    .HasConstraintName("FK_HocSinh_GioiTinh");

                entity.HasOne(d => d.TruongHocNavigation)
                    .WithMany(p => p.HocSinhs)
                    .HasForeignKey(d => d.TruongHocId)
                    .HasConstraintName("FK_HocSinh_DonVi");
            });

            modelBuilder.Entity<HocSinhFileDinhKem>(entity =>
            {
                entity.HasKey(e => e.FileId)
                    .HasName("PK_FileDinhKem");

                entity.ToTable("HocSinhFileDinhKem");

                entity.HasIndex(e => e.HocSinhId, "IX_HocSinhFileDinhKem_HocSinhId");

                entity.Property(e => e.FileId).HasMaxLength(20);

                entity.Property(e => e.Ext).HasMaxLength(10);

                entity.Property(e => e.IconFile).HasMaxLength(550);

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.Property(e => e.TenFile).HasMaxLength(250);

                entity.Property(e => e.Url).HasColumnType("text");

                entity.HasOne(d => d.HocSinh)
                    .WithMany(p => p.HocSinhFileDinhKems)
                    .HasForeignKey(d => d.HocSinhId)
                    .HasConstraintName("FK_HocSinhFileDinhKem_HocSinh");
            });

            modelBuilder.Entity<Huyen>(entity =>
            {
                entity.ToTable("Huyen");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Ten).HasMaxLength(50);
            });

            modelBuilder.Entity<LienKetCapDonViChucNang>(entity =>
            {
                entity.HasKey(e => e.LienKetId);

                entity.ToTable("LienKetCapDonViChucNang");

                entity.HasIndex(e => e.CapDonViId, "IX_LienKetCapDonViChucNang_CapDonViId");

                entity.HasIndex(e => e.ChucNangId, "IX_LienKetCapDonViChucNang_ChucNangId");

                entity.HasOne(d => d.CapDonVi)
                    .WithMany(p => p.LienKetCapDonViChucNangs)
                    .HasForeignKey(d => d.CapDonViId)
                    .HasConstraintName("FK_LienKetCapDonViChucNang_CapDonVi");

                entity.HasOne(d => d.ChucNang)
                    .WithMany(p => p.LienKetCapDonViChucNangs)
                    .HasForeignKey(d => d.ChucNangId)
                    .HasConstraintName("FK_LienKetCapDonViChucNang_ChucNang");
            });

            modelBuilder.Entity<LienKetHocSinhYeuCau>(entity =>
            {
                entity.HasKey(e => e.LienKetId);

                entity.ToTable("LienKetHocSinhYeuCau");

                entity.HasIndex(e => e.HocSinhId, "IX_LienKetHocSinhYeuCau_HocSinhId");

                entity.HasIndex(e => e.YeuCauId, "IX_LienKetHocSinhYeuCau_YeuCauId");

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.HasOne(d => d.HocSinh)
                    .WithMany(p => p.LienKetHocSinhYeuCaus)
                    .HasForeignKey(d => d.HocSinhId)
                    .HasConstraintName("FK_LienKetHocSinhYeuCau_HocSinh");

                entity.HasOne(d => d.YeuCau)
                    .WithMany(p => p.LienKetHocSinhYeuCaus)
                    .HasForeignKey(d => d.YeuCauId)
                    .HasConstraintName("FK_LienKetHocSinhYeuCau_YeuCau");
            });

            modelBuilder.Entity<LienKetLoaiYeuCauCapDonVi>(entity =>
            {
                entity.HasKey(e => e.LienKetId);

                entity.ToTable("LienKetLoaiYeuCauCapDonVi");

                entity.HasIndex(e => e.CapDonViId, "IX_LienKetLoaiYeuCauCapDonVi_CapDonViId");

                entity.HasIndex(e => e.LoaiYeuCauId, "IX_LienKetLoaiYeuCauCapDonVi_LoaiYeuCauId");

                entity.HasOne(d => d.CapDonVi)
                    .WithMany(p => p.LienKetLoaiYeuCauCapDonVis)
                    .HasForeignKey(d => d.CapDonViId)
                    .HasConstraintName("FK_LienKetLoaiYeuCauCapDonVi_CapDonVi");

                entity.HasOne(d => d.LoaiYeuCau)
                    .WithMany(p => p.LienKetLoaiYeuCauCapDonVis)
                    .HasForeignKey(d => d.LoaiYeuCauId)
                    .HasConstraintName("FK_LienKetLoaiYeuCauCapDonVi_LoaiYeuCau");
            });

            modelBuilder.Entity<LienKetNhomNguoiDungChucNang>(entity =>
            {
                entity.HasKey(e => e.LienKetId);

                entity.ToTable("LienKetNhomNguoiDungChucNang");

                entity.HasIndex(e => e.ChucNangid, "IX_LienKetNhomNguoiDungChucNang_ChucNangid");

                entity.HasIndex(e => e.NhomNguoiDungId, "IX_LienKetNhomNguoiDungChucNang_NhomNguoiDungId");

                entity.HasOne(d => d.ChucNang)
                    .WithMany(p => p.LienKetNhomNguoiDungChucNangs)
                    .HasForeignKey(d => d.ChucNangid)
                    .HasConstraintName("FK_LienKetNhomNguoiDungChucNang_ChucNang");

                entity.HasOne(d => d.NhomNguoiDung)
                    .WithMany(p => p.LienKetNhomNguoiDungChucNangs)
                    .HasForeignKey(d => d.NhomNguoiDungId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LienKetNhomNguoiDungChucNang_NhomNguoiDung");
            });

            modelBuilder.Entity<LoaiBang>(entity =>
            {
                entity.ToTable("LoaiBang");

                entity.HasIndex(e => e.HinhThucCapId, "IX_LoaiBang_HinhThucCapId");

                entity.Property(e => e.MaLoaiBang)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaNoiIn)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.Property(e => e.Ten).HasColumnType("ntext");

                entity.HasOne(d => d.HinhThucCap)
                    .WithMany(p => p.LoaiBangs)
                    .HasForeignKey(d => d.HinhThucCapId)
                    .HasConstraintName("FK_LoaiBang_HinhThucCap");
            });

            modelBuilder.Entity<LoaiYeuCau>(entity =>
            {
                entity.ToTable("LoaiYeuCau");
            });

            modelBuilder.Entity<LogHocSinh>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.ToTable("LogHocSinh");

                entity.HasIndex(e => e.HocSinhId, "IX_LogHocSinh_HocSinhId");

                entity.Property(e => e.HanhDong).HasMaxLength(250);

                entity.Property(e => e.HoTen).HasMaxLength(100);

                entity.Property(e => e.ThoiGian).HasColumnType("datetime");

                entity.HasOne(d => d.HocSinh)
                    .WithMany(p => p.LogHocSinhs)
                    .HasForeignKey(d => d.HocSinhId)
                    .HasConstraintName("FK_LogHocSinh_HocSinh");
            });

            modelBuilder.Entity<LogNguoiDung>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.ToTable("LogNguoiDung");

                entity.HasIndex(e => e.NguoiDungId, "IX_LogNguoiDung_NguoiDungId");

                entity.Property(e => e.HanhDong).HasMaxLength(550);

                entity.Property(e => e.Ip)
                    .HasMaxLength(20)
                    .HasColumnName("IP");

                entity.Property(e => e.ThoiGian).HasColumnType("datetime");

                entity.HasOne(d => d.NguoiDung)
                    .WithMany(p => p.LogNguoiDungs)
                    .HasForeignKey(d => d.NguoiDungId)
                    .HasConstraintName("FK_LogNguoiDung_NguoiDung");
            });

            modelBuilder.Entity<LogVanBang>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.ToTable("LogVanBang");

                entity.HasIndex(e => e.VanBangId, "IX_LogVanBang_VanBangId");

                entity.Property(e => e.HanhDong).HasMaxLength(450);

                entity.Property(e => e.HoTen).HasMaxLength(150);

                entity.Property(e => e.ThoiGian).HasColumnType("datetime");

                entity.HasOne(d => d.VanBang)
                    .WithMany(p => p.LogVanBangs)
                    .HasForeignKey(d => d.VanBangId)
                    .HasConstraintName("FK_LogVanBang_Bang");
            });

            modelBuilder.Entity<LogYeuCau>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.ToTable("LogYeuCau");

                entity.HasIndex(e => e.YeuCauId, "IX_LogYeuCau_YeuCauId");

                entity.Property(e => e.HanhDong).HasMaxLength(250);

                entity.Property(e => e.HoTen).HasMaxLength(100);

                entity.Property(e => e.ThoiGian).HasColumnType("datetime");

                entity.HasOne(d => d.YeuCau)
                    .WithMany(p => p.LogYeuCaus)
                    .HasForeignKey(d => d.YeuCauId)
                    .HasConstraintName("FK_LogYeuCau_YeuCau");
            });

            modelBuilder.Entity<NguoiDung>(entity =>
            {
                entity.ToTable("NguoiDung");

                entity.HasIndex(e => e.DonViId, "IX_NguoiDung_DonViId");

                entity.HasIndex(e => e.NhomNguoiDungId, "IX_NguoiDung_NhomNguoiDungId");

                entity.HasIndex(e => e.PhongBanId, "IX_NguoiDung_PhongBanId");

                entity.Property(e => e.NguoiDungId).ValueGeneratedNever();

                entity.Property(e => e.DiaChi).HasMaxLength(250);

                entity.Property(e => e.DienThoai).HasMaxLength(20);

                entity.Property(e => e.Email).HasMaxLength(150);

                entity.Property(e => e.HoTen).HasMaxLength(150);

                entity.Property(e => e.MatKhau).HasMaxLength(250);

                entity.Property(e => e.NgaySinh).HasColumnType("date");

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.Property(e => e.SoCanCuoc).HasMaxLength(20);

                entity.Property(e => e.TenDangNhap).HasMaxLength(50);

                entity.HasOne(d => d.DonVi)
                    .WithMany(p => p.NguoiDungs)
                    .HasForeignKey(d => d.DonViId)
                    .HasConstraintName("FK_NguoiDung_DonVi");

                entity.HasOne(d => d.NhomNguoiDung)
                    .WithMany(p => p.NguoiDungs)
                    .HasForeignKey(d => d.NhomNguoiDungId)
                    .HasConstraintName("FK_NguoiDung_NhomNguoiDung");

                entity.HasOne(d => d.PhongBan)
                    .WithMany(p => p.NguoiDungs)
                    .HasForeignKey(d => d.PhongBanId)
                    .HasConstraintName("FK_NguoiDung_PhongBan");
            });

            modelBuilder.Entity<NhomNguoiDung>(entity =>
            {
                entity.ToTable("NhomNguoiDung");

                entity.HasIndex(e => e.PhongBanId, "IX_NhomNguoiDung_PhongBanId");

                entity.Property(e => e.TenNhomNguoiDung).HasMaxLength(150);

                entity.HasOne(d => d.PhongBan)
                    .WithMany(p => p.NhomNguoiDungs)
                    .HasForeignKey(d => d.PhongBanId)
                    .HasConstraintName("FK_NhomNguoiDung_PhongBan");
            });

            modelBuilder.Entity<PhoiVanBang>(entity =>
            {
                entity.ToTable("Phoi");

                entity.HasIndex(e => e.TrangThaiPhoiId, "IX_Phoi_TrangThaiPhoiId");

                entity.Property(e => e.MoTaTrangThai).HasColumnType("ntext");

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.Property(e => e.SoHieu).IsUnicode(false);

                entity.HasOne(d => d.TrangThaiPhoi)
                    .WithMany(p => p.Phois)
                    .HasForeignKey(d => d.TrangThaiPhoiId)
                    .HasConstraintName("FK_Phoi_TrangThaiPhoi");
            });

            modelBuilder.Entity<PhongBan>(entity =>
            {
                entity.ToTable("PhongBan");

                entity.HasIndex(e => e.DonViId, "IX_PhongBan_DonViId");

                entity.Property(e => e.TenPhongBan).HasMaxLength(250);

                entity.HasOne(d => d.DonVi)
                    .WithMany(p => p.PhongBans)
                    .HasForeignKey(d => d.DonViId)
                    .HasConstraintName("FK_PhongBan_DonVi");
            });

            modelBuilder.Entity<QuyTacFormat>(entity =>
            {
                entity.ToTable("QuyTacFormat");

                entity.Property(e => e.Format)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ThongTinVanBang>(entity =>
            {
                entity.HasKey(e => new { e.TruongDuLieuCode, e.BangId });

                entity.ToTable("ThongTinVanBang");

                entity.HasIndex(e => e.BangId, "IX_ThongTinVanBang_BangId");

                entity.Property(e => e.TruongDuLieuCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.GiaTri)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.HasOne(d => d.Bang)
                    .WithMany(p => p.ThongTinVanBangs)
                    .HasForeignKey(d => d.BangId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ThongTinVanBang_Bang");

                entity.HasOne(d => d.TruongDuLieuCodeNavigation)
                    .WithMany(p => p.ThongTinVanBangs)
                    .HasForeignKey(d => d.TruongDuLieuCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ThongTinVanBang_TruongDuLieu");
            });

            modelBuilder.Entity<Tinh>(entity =>
            {
                entity.ToTable("Tinh");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Ten).HasMaxLength(50);
            });

            modelBuilder.Entity<TrangThaiBang>(entity =>
            {
                entity.ToTable("TrangThaiBang");

                entity.Property(e => e.MaMauTrangThai)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Ten).HasMaxLength(50);
            });

            modelBuilder.Entity<TrangThaiPhoi>(entity =>
            {
                entity.ToTable("TrangThaiPhoi");

                entity.Property(e => e.Ten).HasMaxLength(50);
            });

            modelBuilder.Entity<TrangThaiYeuCau>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK_TrangThaiYeuCau_1");

                entity.ToTable("TrangThaiYeuCau");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.Border).HasMaxLength(50);

                entity.Property(e => e.MaMau).HasMaxLength(50);

                entity.Property(e => e.MauChu).HasMaxLength(50);

                entity.Property(e => e.TenTrangThai).HasMaxLength(50);
            });

            modelBuilder.Entity<TruongDuLieu>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("TruongDuLieu");

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.Property(e => e.Ten).HasColumnType("ntext");
            });

            modelBuilder.Entity<TruongDuLieuLoaiBang>(entity =>
            {
                entity.HasKey(e => new { e.LoaiBangId, e.TruongDuLieuCode });

                entity.ToTable("TruongDuLieuLoaiBang");

                entity.HasIndex(e => e.TruongDuLieuCode, "IX_TruongDuLieuLoaiBang_TruongDuLieuCode");

                entity.Property(e => e.TruongDuLieuCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Format).HasColumnType("text");

                entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.Property(e => e.TenTruongDuLieu).HasMaxLength(50);

                entity.HasOne(d => d.LoaiBang)
                    .WithMany(p => p.TruongDuLieuLoaiBangs)
                    .HasForeignKey(d => d.LoaiBangId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TruongDuLieuLoaiBang_LoaiBang");

                entity.HasOne(d => d.TruongDuLieuCodeNavigation)
                    .WithMany(p => p.TruongDuLieuLoaiBangs)
                    .HasForeignKey(d => d.TruongDuLieuCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TruongDuLieuLoaiBang_TruongDuLieu");
            });

            modelBuilder.Entity<Xa>(entity =>
            {
                entity.ToTable("Xa");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Ten).HasMaxLength(50);
            });

            modelBuilder.Entity<YeuCau>(entity =>
            {
                entity.ToTable("YeuCau");

                entity.HasIndex(e => e.LoaiYeuCauId, "IX_YeuCau_LoaiYeuCauId");

                entity.HasIndex(e => e.MaTrangThaiYeuCau, "IX_YeuCau_MaTrangThaiYeuCau");

                entity.Property(e => e.MaTrangThaiYeuCau).HasMaxLength(20);

                entity.Property(e => e.MaYeuCau).HasMaxLength(10);

                entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");

                entity.Property(e => e.NgayGuiYeuCau).HasColumnType("datetime");

                entity.Property(e => e.NgayTao).HasColumnType("datetime");

                entity.Property(e => e.NguoiTaoYeuCau).HasMaxLength(250);

                entity.Property(e => e.TenYeuCau).HasMaxLength(550);

                entity.HasOne(d => d.LoaiYeuCau)
                    .WithMany(p => p.YeuCaus)
                    .HasForeignKey(d => d.LoaiYeuCauId)
                    .HasConstraintName("FK_YeuCau_LoaiYeuCau");

                entity.HasOne(d => d.MaTrangThaiYeuCauNavigation)
                    .WithMany(p => p.YeuCaus)
                    .HasForeignKey(d => d.MaTrangThaiYeuCau)
                    .HasConstraintName("FK_YeuCau_TrangThaiYeuCau");
            });
            base.OnModelCreating(modelBuilder);
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
