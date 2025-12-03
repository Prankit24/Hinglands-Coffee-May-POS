
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PhanMemBanHang.Model;

namespace PhanMemBanHang.ViewModel
{
    public class QLDonHangVM : BaseViewModel, IDisposable
    {
        private readonly HighlandsCoffeeDBEntities _db = new HighlandsCoffeeDBEntities();

        // ===== DANH SÁCH =====
        private ObservableCollection<DonHangDTO> _danhSachDonHang;
        public ObservableCollection<DonHangDTO> DanhSachDonHang
        {
            get => _danhSachDonHang;
            set => SetProperty(ref _danhSachDonHang, value);
        }

        private ObservableCollection<ChiTietDonDTO> _danhSachChiTiet;
        public ObservableCollection<ChiTietDonDTO> DanhSachChiTiet
        {
            get => _danhSachChiTiet;
            set => SetProperty(ref _danhSachChiTiet, value);
        }

        private ObservableCollection<NhanVien> _danhSachNhanVien;
        public ObservableCollection<NhanVien> DanhSachNhanVien
        {
            get => _danhSachNhanVien;
            set => SetProperty(ref _danhSachNhanVien, value);
        }

        // ===== BỘ LỌC =====
        private string _tuKhoaTimKiem;
        public string TuKhoaTimKiem
        {
            get => _tuKhoaTimKiem;
            set
            {
                SetProperty(ref _tuKhoaTimKiem, value);
                LocDonHang();
            }
        }

        private DateTime? _tuNgay;
        public DateTime? TuNgay
        {
            get => _tuNgay;
            set => SetProperty(ref _tuNgay, value);
        }

        private DateTime? _denNgay;
        public DateTime? DenNgay
        {
            get => _denNgay;
            set => SetProperty(ref _denNgay, value);
        }

        private string _trangThaiLoc = "Tất cả";
        public string TrangThaiLoc
        {
            get => _trangThaiLoc;
            set => SetProperty(ref _trangThaiLoc, value);
        }

        private string _phuongThucTTLoc = "Tất cả";
        public string PhuongThucTTLoc
        {
            get => _phuongThucTTLoc;
            set => SetProperty(ref _phuongThucTTLoc, value);
        }

        private string _loaiDonLoc = "Tất cả";
        public string LoaiDonLoc
        {
            get => _loaiDonLoc;
            set => SetProperty(ref _loaiDonLoc, value);
        }

        private int? _nhanVienLoc;
        public int? NhanVienLoc
        {
            get => _nhanVienLoc;
            set => SetProperty(ref _nhanVienLoc, value);
        }

        private string _sapXepTheo = "Mới nhất";
        public string SapXepTheo
        {
            get => _sapXepTheo;
            set => SetProperty(ref _sapXepTheo, value);
        }

        // ===== SELECTION =====
        private DonHangDTO _donHangDangChon;
        public DonHangDTO DonHangDangChon
        {
            get => _donHangDangChon;
            set
            {
                SetProperty(ref _donHangDangChon, value);
                if (value != null)
                {
                    LoadChiTiet(value.MaHD);
                }
                else
                {
                    DanhSachChiTiet?.Clear();
                }
            }
        }

        // ===== THỐNG KÊ =====
        private decimal _doanhThuHomNay;
        public decimal DoanhThuHomNay
        {
            get => _doanhThuHomNay;
            set => SetProperty(ref _doanhThuHomNay, value);
        }

        private int _soDonHomNay;
        public int SoDonHomNay
        {
            get => _soDonHomNay;
            set => SetProperty(ref _soDonHomNay, value);
        }

        private int _tongSoDonHang;
        public int TongSoDonHang
        {
            get => _tongSoDonHang;
            set => SetProperty(ref _tongSoDonHang, value);
        }

        private decimal _tongGiaTriHienThi;
        public decimal TongGiaTriHienThi
        {
            get => _tongGiaTriHienThi;
            set => SetProperty(ref _tongGiaTriHienThi, value);
        }

        private int _donChoXuLy;
        public int DonChoXuLy
        {
            get => _donChoXuLy;
            set => SetProperty(ref _donChoXuLy, value);
        }

        private int _donDangLam;
        public int DonDangLam
        {
            get => _donDangLam;
            set => SetProperty(ref _donDangLam, value);
        }

        // ===== DANH SÁCH COMBO =====
        public ObservableCollection<string> DanhSachTrangThai { get; set; }
        public ObservableCollection<string> DanhSachPhuongThucTT { get; set; }
        public ObservableCollection<string> DanhSachLoaiDon { get; set; }
        public ObservableCollection<string> DanhSachSapXep { get; set; }

        // ===== COMMANDS =====
        public ICommand TimKiemCommand { get; set; }
        public ICommand LamMoiCommand { get; set; }
        public ICommand XuatExcelCommand { get; set; }
        public ICommand CapNhatTrangThaiCommand { get; set; }
        public ICommand InPhieuCommand { get; set; }

        public QLDonHangVM()
        {
            InitializeData();
            LoadNhanVien();
            LoadDonHang();
            TinhThongKe();

            // Commands
            TimKiemCommand = new RelayCommand(p => LocDonHang());
            LamMoiCommand = new RelayCommand(p => LamMoi());
            XuatExcelCommand = new RelayCommand(p => XuatExcel());
            CapNhatTrangThaiCommand = new RelayCommand(p => CapNhatTrangThai(), p => DonHangDangChon != null);
            InPhieuCommand = new RelayCommand(p => InPhieu(), p => DonHangDangChon != null);
        }

        private void InitializeData()
        {
            DanhSachTrangThai = new ObservableCollection<string>
            {
                "Tất cả", "ChoXuLy", "DangLam", "SanSang", "DaGiao", "HoanThanh"
            };

            DanhSachPhuongThucTT = new ObservableCollection<string>
            {
                "Tất cả", "Tiền mặt", "Chuyển khoản"
            };

            DanhSachLoaiDon = new ObservableCollection<string>
            {
                "Tất cả", "TaiCho", "MangDi"
            };

            DanhSachSapXep = new ObservableCollection<string>
            {
                "Mới nhất", "Cũ nhất", "Giá trị cao", "Giá trị thấp"
            };
        }

        private void LoadNhanVien()
        {
            var list = _db.NhanVien.Where(nv => nv.TrangThai == true).ToList();
            var all = new NhanVien { MaNV = 0, HoTen = "Tất cả nhân viên"};
            list.Insert(0, all);
            DanhSachNhanVien = new ObservableCollection<NhanVien>(list);
        }

        private void LoadDonHang()
        {
            var query = from hd in _db.HoaDon
                        join nv in _db.NhanVien on hd.MaNV equals nv.MaNV
                        join kh in _db.KhachHang on hd.MaKH equals kh.MaKH
                        join pg in _db.Pager on hd.MaPager equals pg.MaPager
                        orderby hd.NgayLap descending
                        select new DonHangDTO
                        {
                            MaHD = hd.MaHD,
                            NgayLap = hd.NgayLap,
                            ThoiGianGoi = hd.ThoiGianGoi,
                            MaPager = hd.MaPager,
                            TenPager = pg.TenPager,
                            KhachHang = kh.TenKH,
                            NhanVien = nv.HoTen,
                            TongTien = hd.TongTien,
                            ThanhTien = hd.ThanhTien,
                            TrangThai = hd.TrangThai,
                            PhuongThucTT = hd.PhuongThucTT,
                            LoaiDon = hd.LoaiDon,
                            MaNV = hd.MaNV,
                            TongSoLuong = _db.ChiTietHoaDon.Where(ct => ct.MaHD == hd.MaHD).Sum(ct => (int?)ct.SoLuong) ?? 0
                        };

            DanhSachDonHang = new ObservableCollection<DonHangDTO>(query.ToList());
            TongSoDonHang = DanhSachDonHang.Count;
            TinhTongGiaTri();
        }

        public void LocDonHang()
        {
            var query = from hd in _db.HoaDon
                        join nv in _db.NhanVien on hd.MaNV equals nv.MaNV
                        join kh in _db.KhachHang on hd.MaKH equals kh.MaKH
                        join pg in _db.Pager on hd.MaPager equals pg.MaPager
                        select new
                        {
                            hd.MaHD,
                            hd.NgayLap,
                            hd.ThoiGianGoi,
                            hd.MaPager,
                            TenPager = pg.TenPager,
                            KhachHang = kh.TenKH,
                            NhanVien = nv.HoTen,
                            hd.TongTien,
                            hd.ThanhTien,
                            hd.TrangThai,
                            hd.PhuongThucTT,
                            hd.LoaiDon,
                            hd.MaNV,
                            TongSoLuong = _db.ChiTietHoaDon.Where(ct => ct.MaHD == hd.MaHD).Sum(ct => (int?)ct.SoLuong) ?? 0
                        };

            // Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
            {
                string keyword = TuKhoaTimKiem.ToLower();
                query = query.Where(h => h.MaHD.ToString().Contains(keyword) ||
                                        h.KhachHang.ToLower().Contains(keyword) ||
                                        h.TenPager.ToLower().Contains(keyword));
            }

            // Lọc theo ngày
            if (TuNgay.HasValue)
            {
                query = query.Where(h => h.NgayLap >= TuNgay.Value);
            }
            if (DenNgay.HasValue)
            {
                var denNgayEnd = DenNgay.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(h => h.NgayLap <= denNgayEnd);
            }

            // Lọc theo trạng thái
            if (TrangThaiLoc != "Tất cả")
            {
                query = query.Where(h => h.TrangThai == TrangThaiLoc);
            }

            // Lọc theo phương thức thanh toán
            if (PhuongThucTTLoc != "Tất cả")
            {
                query = query.Where(h => h.PhuongThucTT == PhuongThucTTLoc);
            }

            // Lọc theo loại đơn
            if (LoaiDonLoc != "Tất cả")
            {
                query = query.Where(h => h.LoaiDon == LoaiDonLoc);
            }

            // Lọc theo nhân viên
            if (NhanVienLoc.HasValue && NhanVienLoc.Value > 0)
            {
                query = query.Where(h => h.MaNV == NhanVienLoc.Value);
            }

            var list = query.ToList().Select(h => new DonHangDTO
            {
                MaHD = h.MaHD,
                NgayLap = h.NgayLap,
                ThoiGianGoi = h.ThoiGianGoi,
                MaPager = h.MaPager,
                TenPager = h.TenPager,
                KhachHang = h.KhachHang,
                NhanVien = h.NhanVien,
                TongTien = h.TongTien,
                ThanhTien = h.ThanhTien,
                TrangThai = h.TrangThai,
                PhuongThucTT = h.PhuongThucTT,
                LoaiDon = h.LoaiDon,
                MaNV = h.MaNV,
                TongSoLuong = h.TongSoLuong
            }).ToList();

            DanhSachDonHang = new ObservableCollection<DonHangDTO>(list);
            ApDungSapXep();
            TinhTongGiaTri();
        }

        public void ApDungSapXep()
        {
            if (DanhSachDonHang == null || !DanhSachDonHang.Any()) return;

            List<DonHangDTO> sorted = null;

            switch (SapXepTheo)
            {
                case "Mới nhất":
                    sorted = DanhSachDonHang.OrderByDescending(h => h.NgayLap).ToList();
                    break;
                case "Cũ nhất":
                    sorted = DanhSachDonHang.OrderBy(h => h.NgayLap).ToList();
                    break;
                case "Giá trị cao":
                    sorted = DanhSachDonHang.OrderByDescending(h => h.TongTien).ToList();
                    break;
                case "Giá trị thấp":
                    sorted = DanhSachDonHang.OrderBy(h => h.TongTien).ToList();
                    break;
                default:
                    sorted = DanhSachDonHang.OrderByDescending(h => h.NgayLap).ToList();
                    break;
            }

            DanhSachDonHang = new ObservableCollection<DonHangDTO>(sorted);
        }

        private void LoadChiTiet(int maHD)
        {
            var query = from ct in _db.ChiTietHoaDon
                        join sp in _db.SanPham on ct.MaSP equals sp.MaSP
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
                            ThanhTien = ct.ThanhTien
                        };

            DanhSachChiTiet = new ObservableCollection<ChiTietDonDTO>(query.ToList());
        }

        private void TinhThongKe()
        {
            var today = DateTime.Today;

            // Doanh thu hôm nay
            DoanhThuHomNay = _db.HoaDon
                .Where(h => h.NgayLap >= today && h.TrangThai == "HoanThanh")
                .Sum(h => (decimal?)h.ThanhTien) ?? 0;

            // Số đơn hôm nay
            SoDonHomNay = _db.HoaDon.Count(h => h.NgayLap >= today);

            // Đơn chờ xử lý
            DonChoXuLy = _db.HoaDon.Count(h => h.TrangThai == "ChoXuLy");

            // Đơn đang làm
            DonDangLam = _db.HoaDon.Count(h => h.TrangThai == "DangLam");
        }

        private void TinhTongGiaTri()
        {
            TongGiaTriHienThi = DanhSachDonHang?.Sum(h => h.TongTien) ?? 0;
        }

        public void ChonDonHang(HoaDon hd)
        {
            if (hd == null) return;
            LoadChiTiet(hd.MaHD);
        }

        private void LamMoi()
        {
            TuKhoaTimKiem = string.Empty;
            TuNgay = null;
            DenNgay = null;
            TrangThaiLoc = "Tất cả";
            PhuongThucTTLoc = "Tất cả";
            LoaiDonLoc = "Tất cả";
            NhanVienLoc = null;
            SapXepTheo = "Mới nhất";

            LoadDonHang();
            TinhThongKe();
            DonHangDangChon = null;
        }

        private void XuatExcel()
        {
            MessageBox.Show("Chức năng xuất Excel đang được phát triển!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CapNhatTrangThai()
        {
            if (DonHangDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: Implement cập nhật trạng thái
            MessageBox.Show($"Cập nhật trạng thái đơn #{DonHangDangChon.MaHD}", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void InPhieu()
        {
            if (DonHangDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"In phiếu đơn #{DonHangDangChon.MaHD}", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }

    // ===== DTO CLASSES =====
    public class DonHangDTO
    {
        public int MaHD { get; set; }
        public DateTime NgayLap { get; set; }
        public DateTime ThoiGianGoi { get; set; }
        public int MaPager { get; set; }
        public string TenPager { get; set; }
        public string KhachHang { get; set; }
        public string NhanVien { get; set; }
        public decimal TongTien { get; set; }
        public decimal ThanhTien { get; set; }
        public string TrangThai { get; set; }
        public string PhuongThucTT { get; set; }
        public string LoaiDon { get; set; }
        public int MaNV { get; set; }
        public int TongSoLuong { get; set; }
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
        public decimal ThanhTien { get; set; }
    }
}