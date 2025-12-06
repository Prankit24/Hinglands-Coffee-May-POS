using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PhanMemBanHang.Model;

namespace PhanMemBanHang.ViewModel
{
    public class QLDonHangVM : BaseViewModel, IDisposable
    {
        private readonly HighlandsCoffeeDBEntities db = new HighlandsCoffeeDBEntities();


        private ObservableCollection<DonHangDTO> danhSachDonHang;
        public ObservableCollection<DonHangDTO> DanhSachDonHang
        {
            get => danhSachDonHang;
            set => SetProperty(ref danhSachDonHang, value);
        }

        private ObservableCollection<ChiTietDonDTO> danhSachChiTiet;
        public ObservableCollection<ChiTietDonDTO> DanhSachChiTiet
        {
            get => danhSachChiTiet;
            set => SetProperty(ref danhSachChiTiet, value);
        }

        public ObservableCollection<NhanVien> DanhSachNhanVien { get; set; }

        public ObservableCollection<string> DanhSachTrangThai { get; set; }
        public ObservableCollection<string> DanhSachTrangThaiThanhToan { get; set; }
        public ObservableCollection<string> DanhSachPhuongThucTT { get; set; }
        public ObservableCollection<string> DanhSachLoaiDon { get; set; }
        public ObservableCollection<string> DanhSachSapXep { get; set; }

   

        private DonHangDTO donHangDangChon;
        public DonHangDTO DonHangDangChon
        {
            get => donHangDangChon;
            set
            {
                if (SetProperty(ref donHangDangChon, value))
                {
                    if (value != null)
                    {
                        LoadChiTiet(value.MaHD);
                        CoTheHoanTra = value.TrangThai == "Hoàn Thành";
                    }
                    else
                    {
                        DanhSachChiTiet = new ObservableCollection<ChiTietDonDTO>();
                        CoTheHoanTra = false;
                    }
                }
            }
        }

        private bool coTheHoanTra;
        public bool CoTheHoanTra
        {
            get => coTheHoanTra;
            set => SetProperty(ref coTheHoanTra, value);
        }

       

        private string tuKhoaTimKiem;
        public string TuKhoaTimKiem
        {
            get => tuKhoaTimKiem;
            set
            {
                if (SetProperty(ref tuKhoaTimKiem, value))
                {
                    LocDonHang();
                }
            }
        }

        private DateTime? tuNgay;
        public DateTime? TuNgay
        {
            get => tuNgay;
            set
            {
                if (SetProperty(ref tuNgay, value))
                {
                    LocDonHang();
                }
            }
        }

        private DateTime? denNgay;
        public DateTime? DenNgay
        {
            get => denNgay;
            set
            {
                if (SetProperty(ref denNgay, value))
                {
                    LocDonHang();
                }
            }
        }

        private string trangThaiLoc = "Tất cả";
        public string TrangThaiLoc
        {
            get => trangThaiLoc;
            set
            {
                if (SetProperty(ref trangThaiLoc, value))
                {
                    LocDonHang();
                }
            }
        }

        private string trangThaiThanhToanLoc = "Tất cả";
        public string TrangThaiThanhToanLoc
        {
            get => trangThaiThanhToanLoc;
            set
            {
                if (SetProperty(ref trangThaiThanhToanLoc, value))
                {
                    LocDonHang();
                }
            }
        }

        private string phuongThucTTLoc = "Tất cả";
        public string PhuongThucTTLoc
        {
            get => phuongThucTTLoc;
            set
            {
                if (SetProperty(ref phuongThucTTLoc, value))
                {
                    LocDonHang();
                }
            }
        }

        private string loaiDonLoc = "Tất cả";
        public string LoaiDonLoc
        {
            get => loaiDonLoc;
            set
            {
                if (SetProperty(ref loaiDonLoc, value))
                {
                    LocDonHang();
                }
            }
        }

        private int? nhanVienLoc;
        public int? NhanVienLoc
        {
            get => nhanVienLoc;
            set
            {
                if (SetProperty(ref nhanVienLoc, value))
                {
                    LocDonHang();
                }
            }
        }

        private string sapXepTheo = "Mới nhất";
        public string SapXepTheo
        {
            get => sapXepTheo;
            set
            {
                if (SetProperty(ref sapXepTheo, value))
                {
                    LocDonHang();
                }
            }
        }

      

        private decimal doanhThuHomNay;
        public decimal DoanhThuHomNay
        {
            get => doanhThuHomNay;
            set => SetProperty(ref doanhThuHomNay, value);
        }

        private int donHoanThanh;
        public int DonHoanThanh
        {
            get => donHoanThanh;
            set => SetProperty(ref donHoanThanh, value);
        }

        private int donDaHuy;
        public int DonDaHuy
        {
            get => donDaHuy;
            set => SetProperty(ref donDaHuy, value);
        }

        private decimal doanhThuTienMat;
        public decimal DoanhThuTienMat
        {
            get => doanhThuTienMat;
            set => SetProperty(ref doanhThuTienMat, value);
        }

        private decimal doanhThuChuyenKhoan;
        public decimal DoanhThuChuyenKhoan
        {
            get => doanhThuChuyenKhoan;
            set => SetProperty(ref doanhThuChuyenKhoan, value);
        }

        private decimal doanhThuThe;
        public decimal DoanhThuThe
        {
            get => doanhThuThe;
            set => SetProperty(ref doanhThuThe, value);
        }

        private int tongSoDonHang;
        public int TongSoDonHang
        {
            get => tongSoDonHang;
            set => SetProperty(ref tongSoDonHang, value);
        }

        private decimal tongGiamGiaHienThi;
        public decimal TongGiamGiaHienThi
        {
            get => tongGiamGiaHienThi;
            set => SetProperty(ref tongGiamGiaHienThi, value);
        }

        private decimal tongThucThuHienThi;
        public decimal TongThucThuHienThi
        {
            get => tongThucThuHienThi;
            set => SetProperty(ref tongThucThuHienThi, value);
        }


        public QLDonHangVM()
        {
  
            DanhSachTrangThai = new ObservableCollection<string>
            {
                "Tất cả", "Hoàn Thành", "Đã Huỷ", "Đang xử lý"
            };

            DanhSachTrangThaiThanhToan = new ObservableCollection<string>
            {
                "Tất cả", "Đã thanh toán", "Chưa thanh toán"
            };

            DanhSachPhuongThucTT = new ObservableCollection<string>
            {
                "Tất cả", "Tiền mặt", "Chuyển khoản"
            };

            DanhSachLoaiDon = new ObservableCollection<string>
            {
                "Tất cả", "Tại quán", "Mang về"
            };

            DanhSachSapXep = new ObservableCollection<string>
            {
                "Mới nhất", "Cũ nhất", "Tổng tiền cao", "Tổng tiền thấp"
            };

            LoadNhanVien();
            LoadDonHang();
            TinhThongKe();
        }
        

        private void LoadNhanVien()
        {
            var list = db.NhanVien.Where(x => x.TrangThai == true).ToList();
            DanhSachNhanVien = new ObservableCollection<NhanVien>(list);
        }

        public void LoadDonHang()
        {
            var query =
                from hd in db.HoaDon
                join nv in db.NhanVien on hd.MaNV equals nv.MaNV
                join kh in db.KhachHang on hd.MaKH equals kh.MaKH into temp
                from kh in temp.DefaultIfEmpty()
                orderby hd.NgayLap descending
                select new DonHangDTO
                {
                    MaHD = hd.MaHD,
                    NgayLap = hd.NgayLap,
                    TenKhachHang = kh != null ? kh.TenKH : "Khách lẻ",
                    SDTKhachHang = kh != null ? kh.SDT : "",
                    NhanVien = nv.HoTen,
                    TongTien = hd.TongTien,
                    ThanhTien = hd.ThanhTien,
                    TrangThai = hd.TrangThai,
                    PhuongThucTT = hd.PhuongThucTT,
                    LoaiDon = hd.LoaiDon,
                    TongSoLuong = db.ChiTietHoaDon
                                    .Where(ct => ct.MaHD == hd.MaHD)
                                    .Sum(ct => (int?)ct.SoLuong) ?? 0,
         
                    GhiChu = string.Empty,
                    DiaChiGiaoHang = string.Empty,
                    NguoiTao = nv.HoTen,
                    NgayCapNhatCuoi = hd.NgayLap
                };

            var listDon = query.ToList();
            DanhSachDonHang = new ObservableCollection<DonHangDTO>(listDon);

            TongGiamGiaHienThi = 0;
            TongThucThuHienThi = listDon.Sum(x => x.ThanhTien);
        }

        public void LocDonHang()
        {
            var query =
                from hd in db.HoaDon
                join nv in db.NhanVien on hd.MaNV equals nv.MaNV
                join kh in db.KhachHang on hd.MaKH equals kh.MaKH into temp
                from kh in temp.DefaultIfEmpty()
                select new DonHangDTO
                {
                    MaHD = hd.MaHD,
                    NgayLap = hd.NgayLap,
                    TenKhachHang = kh != null ? kh.TenKH : "Khách lẻ",
                    SDTKhachHang = kh != null ? kh.SDT : "",
                    NhanVien = nv.HoTen,
                    TongTien = hd.TongTien,
                    ThanhTien = hd.ThanhTien,
                    TrangThai = hd.TrangThai,
                    PhuongThucTT = hd.PhuongThucTT,
                    LoaiDon = hd.LoaiDon,
                    TongSoLuong = db.ChiTietHoaDon
                                    .Where(ct => ct.MaHD == hd.MaHD)
                                    .Sum(ct => (int?)ct.SoLuong) ?? 0,
                    GhiChu = string.Empty,
                    DiaChiGiaoHang = string.Empty,
                    NguoiTao = nv.HoTen,
                    NgayCapNhatCuoi = hd.NgayLap
                };

            if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
            {
                string key = TuKhoaTimKiem.ToLower();
                query = query.Where(x =>
                    x.MaHD.ToString().Contains(key) ||
                    x.TenKhachHang.ToLower().Contains(key) ||
                    x.SDTKhachHang.Contains(key));
            }

            if (TuNgay.HasValue)
                query = query.Where(x => x.NgayLap >= TuNgay.Value);

            if (DenNgay.HasValue)
                query = query.Where(x => x.NgayLap <= DenNgay.Value.AddDays(1));
            
            if (TrangThaiLoc != "Tất cả")
                query = query.Where(x => x.TrangThai == TrangThaiLoc);

        
            if (PhuongThucTTLoc != "Tất cả")
                query = query.Where(x => x.PhuongThucTT == PhuongThucTTLoc);
        
            
            if (LoaiDonLoc != "Tất cả")
                query = query.Where(x => x.LoaiDon == LoaiDonLoc);

            if (NhanVienLoc.HasValue)
            {
                var nv = DanhSachNhanVien?.FirstOrDefault(n => n.MaNV == NhanVienLoc.Value);
                if (nv != null)
                    query = query.Where(x => x.NhanVien == nv.HoTen);
            }

            var list = query.ToList();

   
            switch (SapXepTheo)
            {
                case "Cũ nhất":
                    list = list.OrderBy(x => x.NgayLap).ToList();
                    break;
                case "Tổng tiền cao":
                    list = list.OrderByDescending(x => x.ThanhTien).ToList();
                    break;
                case "Tổng tiền thấp":
                    list = list.OrderBy(x => x.ThanhTien).ToList();
                    break;
                default: 
                    list = list.OrderByDescending(x => x.NgayLap).ToList();
                    break;
            }

            DanhSachDonHang = new ObservableCollection<DonHangDTO>(list);

            TongGiamGiaHienThi = 0; 
            TongThucThuHienThi = list.Sum(x => x.ThanhTien);
        }

        private void LoadChiTiet(int maHD)
        {
            var query =
                from ct in db.ChiTietHoaDon
                join sp in db.SanPham on ct.MaSP equals sp.MaSP
                where ct.MaHD == maHD
                select new ChiTietDonDTO
                {
                    MaCT = ct.MaCT,
                    MaHD = ct.MaHD,
                    MaSP = ct.MaSP,
                    TenSP = sp.TenSP,
                    Size = ct.Size,
                    SoLuong = ct.SoLuong,
                    DonGia = ct.DonGia,
                    ThanhTien = ct.ThanhTien,
                    GiamGia = 0,
                    GhiChu = string.Empty
                };

            DanhSachChiTiet = new ObservableCollection<ChiTietDonDTO>(query.ToList());
        }

        public void TinhThongKe()
        {
            var today = DateTime.Today;

            DoanhThuHomNay = db.HoaDon
                .Where(x => x.NgayLap >= today && x.TrangThai == "Hoàn Thành")
                .Sum(x => (decimal?)x.ThanhTien) ?? 0;

            DonHoanThanh = db.HoaDon.Count(x => x.NgayLap >= today && x.TrangThai == "Hoàn Thành");
            DonDaHuy = db.HoaDon.Count(x => x.NgayLap >= today && x.TrangThai == "Đã Huỷ");

            DoanhThuTienMat = db.HoaDon
                .Where(x => x.NgayLap >= today && x.PhuongThucTT == "Tiền mặt")
                .Sum(x => (decimal?)x.ThanhTien) ?? 0;

            DoanhThuChuyenKhoan = db.HoaDon
                .Where(x => x.NgayLap >= today && x.PhuongThucTT == "Chuyển khoản")
                .Sum(x => (decimal?)x.ThanhTien) ?? 0;

            TongSoDonHang = db.HoaDon.Count(x => x.NgayLap >= today);
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }


    public class DonHangDTO
    {
        public int MaHD { get; set; }
        public DateTime NgayLap { get; set; }
        public string TenKhachHang { get; set; }
        public string SDTKhachHang { get; set; }
        public string NhanVien { get; set; }
        public decimal TongTien { get; set; }
        public decimal ThanhTien { get; set; }
        public string TrangThai { get; set; }
        public string PhuongThucTT { get; set; }
        public string LoaiDon { get; set; }
        public int TongSoLuong { get; set; }
        public string GhiChu { get; set; }
        public string DiaChiGiaoHang { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayCapNhatCuoi { get; set; }
    }

    public class ChiTietDonDTO
    {
        public int MaCT { get; set; }
        public int MaHD { get; set; }
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public string Size { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal GiamGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string GhiChu { get; set; }
    }
}
