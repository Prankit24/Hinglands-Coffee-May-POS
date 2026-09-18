using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;

namespace PhanMemBanHang.Model
{
    internal static class PortableDatabase
    {
        private static readonly object Sync = new object();

        public static string DatabasePath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "MayPOS.db");

        private static string ConnectionString =>
            $"Data Source={DatabasePath};Version=3;Pooling=True;Journal Mode=WAL;";

        public static void EnsureCreated()
        {
            lock (Sync)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath));
                bool isNew = !File.Exists(DatabasePath);
                if (isNew)
                    SQLiteConnection.CreateFile(DatabasePath);

                using (var con = Open())
                {
                    CreateSchema(con);
                    if (ScalarInt(con, "SELECT COUNT(*) FROM NhanVien") == 0)
                        Seed(con);
                }
            }
        }

        private static SQLiteConnection Open()
        {
            var con = new SQLiteConnection(ConnectionString);
            con.Open();
            using (var cmd = new SQLiteCommand("PRAGMA foreign_keys = OFF;", con))
                cmd.ExecuteNonQuery();
            return con;
        }

        private static void CreateSchema(SQLiteConnection con)
        {
            var sql = @"
CREATE TABLE IF NOT EXISTS NhanVien (
    MaNV INTEGER PRIMARY KEY,
    TaiKhoan TEXT NOT NULL,
    MatKhau TEXT NOT NULL,
    VaiTro TEXT,
    TrangThai INTEGER NOT NULL DEFAULT 1,
    HoTen TEXT,
    GioiTinh TEXT,
    NgaySinh TEXT,
    SoDienThoai TEXT,
    Email TEXT,
    DiaChi TEXT,
    ChucVu TEXT,
    NgayVaoLam TEXT,
    Luong NUMERIC,
    HinhAnh TEXT
);
CREATE TABLE IF NOT EXISTS KhachHang (
    MaKH INTEGER PRIMARY KEY,
    TenKH TEXT,
    SDT TEXT,
    Email TEXT,
    DiaChi TEXT,
    DiemTichLuy INTEGER NOT NULL DEFAULT 0
);
CREATE TABLE IF NOT EXISTS LoaiSanPham (
    MaLoai INTEGER PRIMARY KEY,
    TenLoai TEXT,
    ThuTu INTEGER NOT NULL DEFAULT 0,
    HienThi INTEGER NOT NULL DEFAULT 1
);
CREATE TABLE IF NOT EXISTS SanPham (
    MaSP INTEGER PRIMARY KEY,
    TenSP TEXT,
    MaLoai INTEGER NOT NULL,
    GiaSizeS NUMERIC,
    GiaSizeM NUMERIC,
    GiaSizeL NUMERIC,
    TrangThai INTEGER NOT NULL DEFAULT 1,
    HienThi INTEGER NOT NULL DEFAULT 1,
    HinhAnh TEXT
);
CREATE TABLE IF NOT EXISTS Pager (
    MaPager INTEGER PRIMARY KEY,
    TenPager TEXT,
    GhiChu TEXT,
    TrangThai TEXT,
    SoLanSuDung INTEGER NOT NULL DEFAULT 0
);
CREATE TABLE IF NOT EXISTS HoaDon (
    MaHD INTEGER PRIMARY KEY,
    NgayLap TEXT NOT NULL,
    MaNV INTEGER NOT NULL,
    MaPager INTEGER NOT NULL,
    MaKH INTEGER NOT NULL,
    TongTien NUMERIC NOT NULL,
    ThanhTien NUMERIC NOT NULL,
    TrangThai TEXT,
    PhuongThucTT TEXT,
    ThoiGianGoi TEXT NOT NULL,
    LoaiDon TEXT
);
CREATE TABLE IF NOT EXISTS ChiTietHoaDon (
    MaCT INTEGER PRIMARY KEY,
    MaHD INTEGER NOT NULL,
    MaSP INTEGER NOT NULL,
    Size TEXT,
    SoLuong INTEGER NOT NULL,
    DonGia NUMERIC NOT NULL,
    ThanhTien NUMERIC NOT NULL
);";

            using (var cmd = new SQLiteCommand(sql, con))
                cmd.ExecuteNonQuery();
        }

        private static int ScalarInt(SQLiteConnection con, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, con))
                return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private static void Seed(SQLiteConnection con)
        {
            using (var tx = con.BeginTransaction())
            {
                InsertNhanVien(con, tx, new NhanVien
                {
                    MaNV = 1,
                    TaiKhoan = "admin",
                    MatKhau = "123456",
                    VaiTro = "Quản lý kho",
                    TrangThai = true,
                    HoTen = "Quản lý Demo",
                    GioiTinh = "Nữ",
                    NgaySinh = new DateTime(2003, 1, 1),
                    SoDienThoai = "0900000001",
                    Email = "admin@maypos.demo",
                    DiaChi = "Đà Nẵng",
                    ChucVu = "Quản lý",
                    NgayVaoLam = DateTime.Today.AddYears(-1),
                    Luong = 12000000m,
                    HinhAnh = string.Empty
                });

                InsertNhanVien(con, tx, new NhanVien
                {
                    MaNV = 2,
                    TaiKhoan = "nhanvien",
                    MatKhau = "123456",
                    VaiTro = "Thu ngân",
                    TrangThai = true,
                    HoTen = "Nhân viên Demo",
                    GioiTinh = "Nam",
                    NgaySinh = new DateTime(2003, 6, 1),
                    SoDienThoai = "0900000002",
                    Email = "staff@maypos.demo",
                    DiaChi = "Đà Nẵng",
                    ChucVu = "Nhân viên",
                    NgayVaoLam = DateTime.Today.AddMonths(-8),
                    Luong = 7000000m,
                    HinhAnh = string.Empty
                });

                InsertKhachHang(con, tx, new KhachHang { MaKH = 1, TenKH = "Khách vãng lai", SDT = "0000000000", Email = "", DiaChi = "", DiemTichLuy = 0 });
                InsertKhachHang(con, tx, new KhachHang { MaKH = 2, TenKH = "Nguyễn Minh Anh", SDT = "0905123456", Email = "minhanh@example.com", DiaChi = "Đà Nẵng", DiemTichLuy = 120 });

                InsertLoaiSanPham(con, tx, new LoaiSanPham { MaLoai = 1, TenLoai = "Trà", ThuTu = 1, HienThi = true });
                InsertLoaiSanPham(con, tx, new LoaiSanPham { MaLoai = 2, TenLoai = "Freeze", ThuTu = 2, HienThi = true });
                InsertLoaiSanPham(con, tx, new LoaiSanPham { MaLoai = 3, TenLoai = "Cà phê", ThuTu = 3, HienThi = true });
                InsertLoaiSanPham(con, tx, new LoaiSanPham { MaLoai = 4, TenLoai = "Bánh", ThuTu = 4, HienThi = true });

                int sp = 1;
                // Trà (MaLoai = 1)
                InsertSanPham(con, tx, P(sp++, "Trà Sen Vàng", 1, 39000, 45000, 51000, @"Image\tea\Tra_Sen_Vang.png"));
                InsertSanPham(con, tx, P(sp++, "Trà Sen Vàng Củ Năng", 1, 42000, 48000, 54000, @"Image\tea\Tra_Sen_Vang_Cu_Nang.png"));
                InsertSanPham(con, tx, P(sp++, "Trà Thanh Đào", 1, 39000, 45000, 51000, @"Image\tea\Tra_Thanh_Dao.png"));
                InsertSanPham(con, tx, P(sp++, "Trà Thạch Đào", 1, 39000, 45000, 51000, @"Image\tea\Tra_Thach_Dao.png"));
                InsertSanPham(con, tx, P(sp++, "Trà Thạch Vải", 1, 39000, 45000, 51000, @"Image\tea\Tra_Thach_Vai.png"));
                InsertSanPham(con, tx, P(sp++, "Trà Xanh Đậu Đỏ", 1, 42000, 48000, 54000, @"Image\tea\Tra_Xanh_Dau_Do.png"));

                // Freeze (MaLoai = 2)
                InsertSanPham(con, tx, P(sp++, "Cookies & Cream", 2, 49000, 55000, 61000, @"Image\frezee\Cookies_Cream.png"));
                InsertSanPham(con, tx, P(sp++, "Freeze Trà Xanh", 2, 49000, 55000, 61000, @"Image\frezee\Freeze_Tra_Xanh.png"));
                InsertSanPham(con, tx, P(sp++, "Freeze Sô-cô-la", 2, 49000, 55000, 61000, @"Image\frezee\Freeze_Socola.png"));
                InsertSanPham(con, tx, P(sp++, "Classic Phin Freeze", 2, 49000, 55000, 61000, @"Image\frezee\Classic_Phin_Freeze.png"));
                InsertSanPham(con, tx, P(sp++, "Caramel Phin Sô-cô-la", 2, 52000, 58000, 64000, @"Image\frezee\Caramel_Phin_Socola.png"));

                // Cà phê (MaLoai = 3)
                InsertSanPham(con, tx, P(sp++, "Phin Sữa Đá", 3, 29000, 35000, 41000, @"Image\coffee\phinsuada.png"));
                InsertSanPham(con, tx, P(sp++, "Phin Đen Đá", 3, 29000, 35000, 41000, @"Image\coffee\phindenda.png"));
                InsertSanPham(con, tx, P(sp++, "Bạc Xỉu Đá", 3, 29000, 35000, 41000, @"Image\coffee\bacxiuda.png"));
                InsertSanPham(con, tx, P(sp++, "Americano", 3, 35000, 41000, 47000, @"Image\coffee\americano.png"));
                InsertSanPham(con, tx, P(sp++, "Cappuccino", 3, 45000, 51000, 57000, @"Image\coffee\capuchino.png"));
                InsertSanPham(con, tx, P(sp++, "Latte", 3, 45000, 51000, 57000, @"Image\coffee\late.png"));
                InsertSanPham(con, tx, P(sp++, "Caramel Macchiato", 3, 49000, 55000, 61000, @"Image\coffee\caramelmachiato.png"));
                InsertSanPham(con, tx, P(sp++, "Mocha Macchiato", 3, 49000, 55000, 61000, @"Image\coffee\mochamachiato.png"));

                // Bánh (MaLoai = 4) - cùng giá cho ba size để phù hợp UI hiện tại.
                InsertSanPham(con, tx, P(sp++, "Bánh Tiramisu", 4, 39000, 39000, 39000, @"Image\cake\Banh_Tiramisu.png"));
                InsertSanPham(con, tx, P(sp++, "Bánh Croissant", 4, 29000, 29000, 29000, @"Image\cake\Banh_Croissant.png"));
                InsertSanPham(con, tx, P(sp++, "Bánh Chuối", 4, 29000, 29000, 29000, @"Image\cake\Banh_chuoi.png"));
                InsertSanPham(con, tx, P(sp++, "Bánh Phô Mai Cà Phê", 4, 39000, 39000, 39000, @"Image\cake\Banh_Pho_Mai_Ca_Phe.png"));
                InsertSanPham(con, tx, P(sp++, "Bánh Phô Mai Trà Xanh", 4, 39000, 39000, 39000, @"Image\cake\Banh_Pho_Mai_Tra_Xanh.png"));

                InsertPager(con, tx, new Pager { MaPager = 1, TenPager = "Pager 01", GhiChu = "Pager mặc định", TrangThai = "Sẵn sàng", SoLanSuDung = 0 });
                InsertPager(con, tx, new Pager { MaPager = 2, TenPager = "Pager 02", GhiChu = "Pager dự phòng", TrangThai = "Sẵn sàng", SoLanSuDung = 0 });

                var now = DateTime.Now;
                InsertHoaDon(con, tx, new HoaDon
                {
                    MaHD = 1, NgayLap = now.AddHours(-3), MaNV = 2, MaPager = 1, MaKH = 1,
                    TongTien = 64000m, ThanhTien = 64000m, TrangThai = "Hoàn Thành",
                    PhuongThucTT = "Tiền mặt", ThoiGianGoi = now.AddHours(-3), LoaiDon = "Mang về"
                });
                InsertChiTietHoaDon(con, tx, new ChiTietHoaDon { MaCT = 1, MaHD = 1, MaSP = 12, Size = "S", SoLuong = 1, DonGia = 29000m, ThanhTien = 29000m });
                InsertChiTietHoaDon(con, tx, new ChiTietHoaDon { MaCT = 2, MaHD = 1, MaSP = 14, Size = "M", SoLuong = 1, DonGia = 35000m, ThanhTien = 35000m });

                InsertHoaDon(con, tx, new HoaDon
                {
                    MaHD = 2, NgayLap = now.AddHours(-1), MaNV = 2, MaPager = 1, MaKH = 2,
                    TongTien = 100000m, ThanhTien = 100000m, TrangThai = "Hoàn Thành",
                    PhuongThucTT = "Chuyển khoản", ThoiGianGoi = now.AddHours(-1), LoaiDon = "Tại quán"
                });
                InsertChiTietHoaDon(con, tx, new ChiTietHoaDon { MaCT = 3, MaHD = 2, MaSP = 1, Size = "M", SoLuong = 1, DonGia = 45000m, ThanhTien = 45000m });
                InsertChiTietHoaDon(con, tx, new ChiTietHoaDon { MaCT = 4, MaHD = 2, MaSP = 7, Size = "M", SoLuong = 1, DonGia = 55000m, ThanhTien = 55000m });

                tx.Commit();
            }
        }

        private static SanPham P(int id, string ten, int loai, decimal s, decimal m, decimal l, string hinh)
        {
            return new SanPham
            {
                MaSP = id, TenSP = ten, MaLoai = loai,
                GiaSizeS = s, GiaSizeM = m, GiaSizeL = l,
                TrangThai = true, HienThi = true, HinhAnh = hinh
            };
        }

        public static List<NhanVien> LoadNhanVien()
        {
            var list = new List<NhanVien>();
            using (var con = Open())
            using (var cmd = new SQLiteCommand("SELECT * FROM NhanVien ORDER BY MaNV", con))
            using (var r = cmd.ExecuteReader())
                while (r.Read()) list.Add(new NhanVien
                {
                    MaNV = I(r, "MaNV"), TaiKhoan = S(r, "TaiKhoan"), MatKhau = S(r, "MatKhau"),
                    VaiTro = S(r, "VaiTro"), TrangThai = B(r, "TrangThai"), HoTen = S(r, "HoTen"),
                    GioiTinh = S(r, "GioiTinh"), NgaySinh = DN(r, "NgaySinh"), SoDienThoai = S(r, "SoDienThoai"),
                    Email = S(r, "Email"), DiaChi = S(r, "DiaChi"), ChucVu = S(r, "ChucVu"),
                    NgayVaoLam = DN(r, "NgayVaoLam"), Luong = MN(r, "Luong"), HinhAnh = S(r, "HinhAnh")
                });
            return list;
        }

        public static List<KhachHang> LoadKhachHang()
        {
            var list = new List<KhachHang>();
            using (var con = Open())
            using (var cmd = new SQLiteCommand("SELECT * FROM KhachHang ORDER BY MaKH", con))
            using (var r = cmd.ExecuteReader())
                while (r.Read()) list.Add(new KhachHang
                {
                    MaKH = I(r, "MaKH"), TenKH = S(r, "TenKH"), SDT = S(r, "SDT"),
                    Email = S(r, "Email"), DiaChi = S(r, "DiaChi"), DiemTichLuy = I(r, "DiemTichLuy")
                });
            return list;
        }

        public static List<LoaiSanPham> LoadLoaiSanPham()
        {
            var list = new List<LoaiSanPham>();
            using (var con = Open())
            using (var cmd = new SQLiteCommand("SELECT * FROM LoaiSanPham ORDER BY ThuTu, MaLoai", con))
            using (var r = cmd.ExecuteReader())
                while (r.Read()) list.Add(new LoaiSanPham
                {
                    MaLoai = I(r, "MaLoai"), TenLoai = S(r, "TenLoai"), ThuTu = I(r, "ThuTu"), HienThi = B(r, "HienThi")
                });
            return list;
        }

        public static List<SanPham> LoadSanPham()
        {
            var list = new List<SanPham>();
            using (var con = Open())
            using (var cmd = new SQLiteCommand("SELECT * FROM SanPham ORDER BY MaSP", con))
            using (var r = cmd.ExecuteReader())
                while (r.Read()) list.Add(new SanPham
                {
                    MaSP = I(r, "MaSP"), TenSP = S(r, "TenSP"), MaLoai = I(r, "MaLoai"),
                    GiaSizeS = MN(r, "GiaSizeS"), GiaSizeM = MN(r, "GiaSizeM"), GiaSizeL = MN(r, "GiaSizeL"),
                    TrangThai = B(r, "TrangThai"), HienThi = B(r, "HienThi"), HinhAnh = S(r, "HinhAnh")
                });
            return list;
        }

        public static List<Pager> LoadPager()
        {
            var list = new List<Pager>();
            using (var con = Open())
            using (var cmd = new SQLiteCommand("SELECT * FROM Pager ORDER BY MaPager", con))
            using (var r = cmd.ExecuteReader())
                while (r.Read()) list.Add(new Pager
                {
                    MaPager = I(r, "MaPager"), TenPager = S(r, "TenPager"), GhiChu = S(r, "GhiChu"),
                    TrangThai = S(r, "TrangThai"), SoLanSuDung = I(r, "SoLanSuDung")
                });
            return list;
        }

        public static List<HoaDon> LoadHoaDon()
        {
            var list = new List<HoaDon>();
            using (var con = Open())
            using (var cmd = new SQLiteCommand("SELECT * FROM HoaDon ORDER BY MaHD", con))
            using (var r = cmd.ExecuteReader())
                while (r.Read()) list.Add(new HoaDon
                {
                    MaHD = I(r, "MaHD"), NgayLap = D(r, "NgayLap"), MaNV = I(r, "MaNV"),
                    MaPager = I(r, "MaPager"), MaKH = I(r, "MaKH"), TongTien = M(r, "TongTien"),
                    ThanhTien = M(r, "ThanhTien"), TrangThai = S(r, "TrangThai"), PhuongThucTT = S(r, "PhuongThucTT"),
                    ThoiGianGoi = D(r, "ThoiGianGoi"), LoaiDon = S(r, "LoaiDon")
                });
            return list;
        }

        public static List<ChiTietHoaDon> LoadChiTietHoaDon()
        {
            var list = new List<ChiTietHoaDon>();
            using (var con = Open())
            using (var cmd = new SQLiteCommand("SELECT * FROM ChiTietHoaDon ORDER BY MaCT", con))
            using (var r = cmd.ExecuteReader())
                while (r.Read()) list.Add(new ChiTietHoaDon
                {
                    MaCT = I(r, "MaCT"), MaHD = I(r, "MaHD"), MaSP = I(r, "MaSP"), Size = S(r, "Size"),
                    SoLuong = I(r, "SoLuong"), DonGia = M(r, "DonGia"), ThanhTien = M(r, "ThanhTien")
                });
            return list;
        }

        public static void SaveAll(HighlandsCoffeeDBEntities db)
        {
            lock (Sync)
            {
                using (var con = Open())
                using (var tx = con.BeginTransaction())
                {
                    Execute(con, tx, "DELETE FROM ChiTietHoaDon");
                    Execute(con, tx, "DELETE FROM HoaDon");
                    Execute(con, tx, "DELETE FROM SanPham");
                    Execute(con, tx, "DELETE FROM LoaiSanPham");
                    Execute(con, tx, "DELETE FROM Pager");
                    Execute(con, tx, "DELETE FROM KhachHang");
                    Execute(con, tx, "DELETE FROM NhanVien");

                    foreach (var x in db.NhanVien) InsertNhanVien(con, tx, x);
                    foreach (var x in db.KhachHang) InsertKhachHang(con, tx, x);
                    foreach (var x in db.LoaiSanPham) InsertLoaiSanPham(con, tx, x);
                    foreach (var x in db.SanPham) InsertSanPham(con, tx, x);
                    foreach (var x in db.Pager) InsertPager(con, tx, x);
                    foreach (var x in db.HoaDon) InsertHoaDon(con, tx, x);
                    foreach (var x in db.ChiTietHoaDon) InsertChiTietHoaDon(con, tx, x);

                    tx.Commit();
                }
            }
        }

        private static void Execute(SQLiteConnection con, SQLiteTransaction tx, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, con, tx)) cmd.ExecuteNonQuery();
        }

        private static SQLiteCommand Cmd(SQLiteConnection con, SQLiteTransaction tx, string sql)
        {
            return new SQLiteCommand(sql, con, tx);
        }

        private static void InsertNhanVien(SQLiteConnection con, SQLiteTransaction tx, NhanVien x)
        {
            using (var c = Cmd(con, tx, @"INSERT INTO NhanVien
(MaNV,TaiKhoan,MatKhau,VaiTro,TrangThai,HoTen,GioiTinh,NgaySinh,SoDienThoai,Email,DiaChi,ChucVu,NgayVaoLam,Luong,HinhAnh)
VALUES(@MaNV,@TaiKhoan,@MatKhau,@VaiTro,@TrangThai,@HoTen,@GioiTinh,@NgaySinh,@SoDienThoai,@Email,@DiaChi,@ChucVu,@NgayVaoLam,@Luong,@HinhAnh)"))
            {
                Pm(c, "@MaNV", x.MaNV); Pm(c, "@TaiKhoan", x.TaiKhoan); Pm(c, "@MatKhau", x.MatKhau); Pm(c, "@VaiTro", x.VaiTro);
                Pm(c, "@TrangThai", x.TrangThai ? 1 : 0); Pm(c, "@HoTen", x.HoTen); Pm(c, "@GioiTinh", x.GioiTinh); Pm(c, "@NgaySinh", DT(x.NgaySinh));
                Pm(c, "@SoDienThoai", x.SoDienThoai); Pm(c, "@Email", x.Email); Pm(c, "@DiaChi", x.DiaChi); Pm(c, "@ChucVu", x.ChucVu);
                Pm(c, "@NgayVaoLam", DT(x.NgayVaoLam)); Pm(c, "@Luong", x.Luong); Pm(c, "@HinhAnh", x.HinhAnh); c.ExecuteNonQuery();
            }
        }

        private static void InsertKhachHang(SQLiteConnection con, SQLiteTransaction tx, KhachHang x)
        {
            using (var c = Cmd(con, tx, "INSERT INTO KhachHang(MaKH,TenKH,SDT,Email,DiaChi,DiemTichLuy) VALUES(@1,@2,@3,@4,@5,@6)"))
            { Pm(c,"@1",x.MaKH); Pm(c,"@2",x.TenKH); Pm(c,"@3",x.SDT); Pm(c,"@4",x.Email); Pm(c,"@5",x.DiaChi); Pm(c,"@6",x.DiemTichLuy); c.ExecuteNonQuery(); }
        }

        private static void InsertLoaiSanPham(SQLiteConnection con, SQLiteTransaction tx, LoaiSanPham x)
        {
            using (var c = Cmd(con, tx, "INSERT INTO LoaiSanPham(MaLoai,TenLoai,ThuTu,HienThi) VALUES(@1,@2,@3,@4)"))
            { Pm(c,"@1",x.MaLoai); Pm(c,"@2",x.TenLoai); Pm(c,"@3",x.ThuTu); Pm(c,"@4",x.HienThi?1:0); c.ExecuteNonQuery(); }
        }

        private static void InsertSanPham(SQLiteConnection con, SQLiteTransaction tx, SanPham x)
        {
            using (var c = Cmd(con, tx, "INSERT INTO SanPham(MaSP,TenSP,MaLoai,GiaSizeS,GiaSizeM,GiaSizeL,TrangThai,HienThi,HinhAnh) VALUES(@1,@2,@3,@4,@5,@6,@7,@8,@9)"))
            { Pm(c,"@1",x.MaSP); Pm(c,"@2",x.TenSP); Pm(c,"@3",x.MaLoai); Pm(c,"@4",x.GiaSizeS); Pm(c,"@5",x.GiaSizeM); Pm(c,"@6",x.GiaSizeL); Pm(c,"@7",x.TrangThai?1:0); Pm(c,"@8",x.HienThi?1:0); Pm(c,"@9",x.HinhAnh); c.ExecuteNonQuery(); }
        }

        private static void InsertPager(SQLiteConnection con, SQLiteTransaction tx, Pager x)
        {
            using (var c = Cmd(con, tx, "INSERT INTO Pager(MaPager,TenPager,GhiChu,TrangThai,SoLanSuDung) VALUES(@1,@2,@3,@4,@5)"))
            { Pm(c,"@1",x.MaPager); Pm(c,"@2",x.TenPager); Pm(c,"@3",x.GhiChu); Pm(c,"@4",x.TrangThai); Pm(c,"@5",x.SoLanSuDung); c.ExecuteNonQuery(); }
        }

        private static void InsertHoaDon(SQLiteConnection con, SQLiteTransaction tx, HoaDon x)
        {
            using (var c = Cmd(con, tx, "INSERT INTO HoaDon(MaHD,NgayLap,MaNV,MaPager,MaKH,TongTien,ThanhTien,TrangThai,PhuongThucTT,ThoiGianGoi,LoaiDon) VALUES(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11)"))
            { Pm(c,"@1",x.MaHD); Pm(c,"@2",DT(x.NgayLap)); Pm(c,"@3",x.MaNV); Pm(c,"@4",x.MaPager); Pm(c,"@5",x.MaKH); Pm(c,"@6",x.TongTien); Pm(c,"@7",x.ThanhTien); Pm(c,"@8",x.TrangThai); Pm(c,"@9",x.PhuongThucTT); Pm(c,"@10",DT(x.ThoiGianGoi)); Pm(c,"@11",x.LoaiDon); c.ExecuteNonQuery(); }
        }

        private static void InsertChiTietHoaDon(SQLiteConnection con, SQLiteTransaction tx, ChiTietHoaDon x)
        {
            using (var c = Cmd(con, tx, "INSERT INTO ChiTietHoaDon(MaCT,MaHD,MaSP,Size,SoLuong,DonGia,ThanhTien) VALUES(@1,@2,@3,@4,@5,@6,@7)"))
            { Pm(c,"@1",x.MaCT); Pm(c,"@2",x.MaHD); Pm(c,"@3",x.MaSP); Pm(c,"@4",x.Size); Pm(c,"@5",x.SoLuong); Pm(c,"@6",x.DonGia); Pm(c,"@7",x.ThanhTien); c.ExecuteNonQuery(); }
        }

        private static void Pm(SQLiteCommand cmd, string name, object value)
        {
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        private static string DT(DateTime value) => value.ToString("o", CultureInfo.InvariantCulture);
        private static object DT(DateTime? value) => value.HasValue ? (object)DT(value.Value) : DBNull.Value;

        private static string S(SQLiteDataReader r, string n) => r[n] == DBNull.Value ? string.Empty : Convert.ToString(r[n], CultureInfo.InvariantCulture);
        private static int I(SQLiteDataReader r, string n) => r[n] == DBNull.Value ? 0 : Convert.ToInt32(r[n], CultureInfo.InvariantCulture);
        private static bool B(SQLiteDataReader r, string n) => I(r, n) != 0;
        private static decimal M(SQLiteDataReader r, string n) => r[n] == DBNull.Value ? 0m : Convert.ToDecimal(r[n], CultureInfo.InvariantCulture);
        private static decimal? MN(SQLiteDataReader r, string n) => r[n] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r[n], CultureInfo.InvariantCulture);
        private static DateTime D(SQLiteDataReader r, string n) => DateTime.Parse(S(r, n), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        private static DateTime? DN(SQLiteDataReader r, string n)
        {
            var s = S(r, n);
            return string.IsNullOrWhiteSpace(s) ? (DateTime?)null : DateTime.Parse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }
    }
}
