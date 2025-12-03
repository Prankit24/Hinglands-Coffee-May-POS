using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PhanMemBanHang.Model;

namespace PhanMemBanHang.ViewModel
{
    public class POSVM : BaseViewModel, IDisposable
    {
        private readonly HighlandsCoffeeDBEntities _db = new HighlandsCoffeeDBEntities();

        // Thông tin nhân viên
        private string _tenNhanVien;
        public string TenNhanVien
        {
            get => _tenNhanVien;
            set => SetProperty(ref _tenNhanVien, value);
        }

        private int _maNhanVien;
        public int MaNhanVien
        {
            get => _maNhanVien;
            set => SetProperty(ref _maNhanVien, value);
        }

        // Thông tin đơn hàng
        private string _thongTinDonHang;
        public string ThongTinDonHang
        {
            get => _thongTinDonHang;
            set => SetProperty(ref _thongTinDonHang, value);
        }

        private string _gioHienTai;
        public string GioHienTai
        {
            get => _gioHienTai;
            set => SetProperty(ref _gioHienTai, value);
        }

        // Tìm kiếm và lọc
        private string _tuKhoaTimKiem;
        public string TuKhoaTimKiem
        {
            get => _tuKhoaTimKiem;
            set
            {
                SetProperty(ref _tuKhoaTimKiem, value);
                LocSanPham();
            }
        }

        private int? _loaiDangChon;
        private ObservableCollection<SanPhamViewModel> _danhSachSanPhamGoc;

        private ObservableCollection<SanPhamViewModel> _danhSachSanPham;
        public ObservableCollection<SanPhamViewModel> DanhSachSanPham
        {
            get => _danhSachSanPham;
            set => SetProperty(ref _danhSachSanPham, value);
        }

        // Giỏ hàng
        private ObservableCollection<MonTrongGio> _gioHang;
        public ObservableCollection<MonTrongGio> GioHang
        {
            get => _gioHang;
            set => SetProperty(ref _gioHang, value);
        }

        // Tổng tiền
        private decimal _tamTinh;
        public decimal TamTinh
        {
            get => _tamTinh;
            set => SetProperty(ref _tamTinh, value);
        }

        private decimal _giamGia;
        public decimal GiamGia
        {
            get => _giamGia;
            set => SetProperty(ref _giamGia, value);
        }

        private decimal _tongThanhToan;
        public decimal TongThanhToan
        {
            get => _tongThanhToan;
            set => SetProperty(ref _tongThanhToan, value);
        }

        // Commands
        public ICommand LocTheoLoaiCommand { get; set; }
        public ICommand ChonSanPhamCommand { get; set; }
        public ICommand TangSoLuongCommand { get; set; }
        public ICommand GiamSoLuongCommand { get; set; }
        public ICommand XoaKhoiGioCommand { get; set; }
        public ICommand ThanhToanCommand { get; set; }
        public ICommand HuyDonCommand { get; set; }
        public ICommand LuuTamCommand { get; set; }

        // Constructor
        public POSVM()
        {
            KhoiTaoCommands();
            TaiDanhSachSanPham();
            GioHang = new ObservableCollection<MonTrongGio>();

            ThongTinDonHang = $"Đơn hàng mới - {DateTime.Now:dd/MM/yyyy}";
            GioHienTai = DateTime.Now.ToString("HH:mm");

            // Update thời gian mỗi phút
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);
            timer.Tick += (s, e) => GioHienTai = DateTime.Now.ToString("HH:mm");
            timer.Start();
        }

        public POSVM(string tenNV, int maNV) : this()
        {
            TenNhanVien = tenNV;
            MaNhanVien = maNV;
        }

        private void KhoiTaoCommands()
        {
            LocTheoLoaiCommand = new RelayCommand(p => LocTheoLoai(p));
            ChonSanPhamCommand = new RelayCommand(p => ChonSanPham(p as SanPhamViewModel));
            TangSoLuongCommand = new RelayCommand(p => TangSoLuong(p as MonTrongGio));
            GiamSoLuongCommand = new RelayCommand(p => GiamSoLuong(p as MonTrongGio));
            XoaKhoiGioCommand = new RelayCommand(p => XoaKhoiGio(p as MonTrongGio));
            ThanhToanCommand = new RelayCommand(p => ThanhToan());
            HuyDonCommand = new RelayCommand(p => HuyDon());
            LuuTamCommand = new RelayCommand(p => LuuTam());
        }

        private void TaiDanhSachSanPham()
        {
            var dsSanPham = _db.SanPham
                .Where(sp => sp.TrangThai == true)
                .ToList()
                .Select(sp => new SanPhamViewModel(sp))
                .ToList();

            _danhSachSanPhamGoc = new ObservableCollection<SanPhamViewModel>(dsSanPham);
            DanhSachSanPham = new ObservableCollection<SanPhamViewModel>(dsSanPham);
        }

        private void LocTheoLoai(object maLoai)
        {
            if (maLoai == null)
            {
                _loaiDangChon = null;
            }
            else
            {
                _loaiDangChon = Convert.ToInt32(maLoai);
            }
            LocSanPham();
        }

        private void LocSanPham()
        {
            var ketQua = _danhSachSanPhamGoc.AsEnumerable();

            // Lọc theo loại
            if (_loaiDangChon.HasValue)
            {
                ketQua = ketQua.Where(sp => sp.MaLoai == _loaiDangChon.Value);
            }

            // Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
            {
                var tuKhoa = TuKhoaTimKiem.ToLower().Trim();
                ketQua = ketQua.Where(sp => sp.TenSP.ToLower().Contains(tuKhoa));
            }

            DanhSachSanPham = new ObservableCollection<SanPhamViewModel>(ketQua);
        }

        private void ChonSanPham(SanPhamViewModel sp)
        {
            if (sp == null) return;

            // Tìm món trong giỏ với cùng mã SP và size
            var monTrongGio = GioHang.FirstOrDefault(m =>
                m.MaSP == sp.MaSP && m.Size == sp.SizeHienTai);

            if (monTrongGio != null)
            {
                // Đã có -> tăng số lượng
                monTrongGio.SoLuong++;
            }
            else
            {
                // Chưa có -> thêm mới
                GioHang.Add(new MonTrongGio
                {
                    MaSP = sp.MaSP,
                    TenSP = sp.TenSP,
                    Size = sp.SizeHienTai,
                    DonGia = sp.GiaTheoSize,
                    SoLuong = 1
                });
            }

            TinhTongTien();
        }

        private void TangSoLuong(MonTrongGio mon)
        {
            if (mon != null)
            {
                mon.SoLuong++;
                TinhTongTien();
            }
        }

        private void GiamSoLuong(MonTrongGio mon)
        {
            if (mon != null)
            {
                if (mon.SoLuong > 1)
                {
                    mon.SoLuong--;
                    TinhTongTien();
                }
                else
                {
                    XoaKhoiGio(mon);
                }
            }
        }

        private void XoaKhoiGio(MonTrongGio mon)
        {
            if (mon != null)
            {
                GioHang.Remove(mon);
                TinhTongTien();
            }
        }

        private void TinhTongTien()
        {
            TamTinh = GioHang.Sum(m => m.ThanhTien);
            GiamGia = 0;
            TongThanhToan = TamTinh - GiamGia;
        }

        private void ThanhToan()
        {
            if (!GioHang.Any())
            {
                MessageBox.Show("Chưa có sản phẩm trong giỏ hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Tổng thanh toán: {TongThanhToan:N0} ₫\n\nXác nhận thanh toán?",
                "Thanh toán",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                // TODO: sau này lấy MaPager & MaKH thật từ UI
                int maPagerMacDinh = 1;   // ví dụ
                int maKhachVangLai = 1;   // ví dụ, phải tồn tại trong bảng KhachHang

                var hoaDon = new HoaDon
                {
                    NgayLap = DateTime.Now,
                    ThoiGianGoi = DateTime.Now,
                    MaNV = MaNhanVien,
                    MaPager = maPagerMacDinh,
                    MaKH = maKhachVangLai,
                    TongTien = TamTinh,
                    ThanhTien = TongThanhToan,
                    PhuongThucTT = "Tiền mặt",     // phải đúng 1 trong 2: "Tiền mặt" / "Chuyển khoản"
                    LoaiDon = "TaiCho",       // đúng với CHECK constraint
                    TrangThai = "HoanThanh"     // hoặc "ChoXuLy" tùy flow của bạn
                };

                _db.HoaDon.Add(hoaDon);
                _db.SaveChanges();

                foreach (var mon in GioHang)
                {
                    _db.ChiTietHoaDon.Add(new ChiTietHoaDon
                    {
                        MaHD = hoaDon.MaHD,
                        MaSP = mon.MaSP,
                        SoLuong = mon.SoLuong,
                        DonGia = mon.DonGia,
                        ThanhTien = mon.ThanhTien,
                        Size = mon.Size
                    });
                }
                _db.SaveChanges();

                MessageBox.Show($"Thanh toán thành công!\nMã hóa đơn: {hoaDon.MaHD}",
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                GioHang.Clear();
                TinhTongTien();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thanh toán: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void HuyDon()
        {
            if (GioHang.Any())
            {
                var result = MessageBox.Show("Bạn có chắc muốn hủy đơn hàng này?",
                    "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    GioHang.Clear();
                    TinhTongTien();
                }
            }
        }

        private void LuuTam()
        {
            MessageBox.Show("Tính năng đang phát triển!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }

    // ViewModel cho sản phẩm
    public class SanPhamViewModel : INotifyPropertyChanged
    {
        private SanPham _sanPham;

        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public int MaLoai { get; set; }
        public decimal GiaSize_S { get; set; }
        public decimal GiaSize_M { get; set; }
        public decimal GiaSize_L { get; set; }

        private string _sizeHienTai = "M";
        public string SizeHienTai
        {
            get => _sizeHienTai;
            set
            {
                _sizeHienTai = value;
                OnPropertyChanged(nameof(SizeHienTai));
                OnPropertyChanged(nameof(GiaTheoSize));
            }
        }

        public decimal GiaTheoSize
        {
            get
            {
                switch (SizeHienTai)
                {
                    case "S":
                        return GiaSize_S;
                    case "L":
                        return GiaSize_L;
                    default:
                        return GiaSize_M;
                }
            }
        }

        public SanPhamViewModel(SanPham sp)
        {
            _sanPham = sp;
            MaSP = sp.MaSP;
            TenSP = sp.TenSP;
            MaLoai = sp.MaLoai;
            GiaSize_S = sp.GiaSizeS ?? 0M;   // dùng 0M cho decimal
            GiaSize_M = sp.GiaSizeM ?? 0M;
            GiaSize_L = sp.GiaSizeL ?? 0M;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    // Model cho món trong giỏ
    public class MonTrongGio : INotifyPropertyChanged
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public string Size { get; set; }
        public decimal DonGia { get; set; }

        private int _soLuong;
        public int SoLuong
        {
            get => _soLuong;
            set
            {
                _soLuong = value;
                OnPropertyChanged(nameof(SoLuong));
                OnPropertyChanged(nameof(ThanhTien));
            }
        }

        public decimal ThanhTien => DonGia * SoLuong;

        public string TenHienThi => $"{TenSP} ({Size})";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}