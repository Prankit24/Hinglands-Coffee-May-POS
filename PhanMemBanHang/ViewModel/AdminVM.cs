using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PhanMemBanHang.Model;

namespace PhanMemBanHang.ViewModel
{
    public class AdminVM : BaseViewModel, IDisposable
    {
        private readonly HighlandsCoffeeDBEntities db = new HighlandsCoffeeDBEntities();

        // ================== PROPERTIES ==================

        private int _soLuongNhanVien;
        public int SoLuongNhanVien
        {
            get => _soLuongNhanVien;
            set => SetProperty(ref _soLuongNhanVien, value);
        }

        private string _taiKhoan;
        public string TaiKhoan
        {
            get => _taiKhoan;
            set => SetProperty(ref _taiKhoan, value);
        }

        private int _soLuongDonHang;
        public int SoLuongDonHang
        {
            get => _soLuongDonHang;
            set => SetProperty(ref _soLuongDonHang, value);
        }

        private decimal _doanhThu;
        public decimal DoanhThu
        {
            get => _doanhThu;
            set => SetProperty(ref _doanhThu, value);
        }

        // ⚠️ Dùng cho binding SẢN PHẨM trong XAML: SoLuongSanPham
        private int _soLuongSanPham;
        public int SoLuongSanPham
        {
            get => _soLuongSanPham;
            set => SetProperty(ref _soLuongSanPham, value);
        }

        private int _sanPhamSapHet;
        public int SanPhamSapHet
        {
            get => _sanPhamSapHet;
            set => SetProperty(ref _sanPhamSapHet, value);
        }

        private int _tongNhanVien;
        public int TongNhanVien
        {
            get => _tongNhanVien;
            set => SetProperty(ref _tongNhanVien, value);
        }

        private ObservableCollection<HoaDon> _danhSachDonHang;
        public ObservableCollection<HoaDon> DanhSachDonHang
        {
            get => _danhSachDonHang;
            set => SetProperty(ref _danhSachDonHang, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // ================== COMMANDS ==================

        public ICommand TaiLaiCommand { get; }
        public ICommand LocTheoNgayCommand { get; }
        public ICommand LocTheoTrangThaiCommand { get; }

        // ================== CTOR ==================

        public AdminVM()
        {
            // Khởi tạo command (dù XAML hiện tại chưa dùng cũng không sao)
            TaiLaiCommand = new RelayCommand(_ => TaiLai());
            LocTheoNgayCommand = new RelayCommand(param => LocTheoNgay(param));
            LocTheoTrangThaiCommand = new RelayCommand(param => LocTheoTrangThai(param as string));

            // Load dữ liệu ban đầu
            TaiLai();
        }

        // ================== LOAD THỐNG KÊ ==================

        private void LoadThongTin()
        {
            try
            {
                IsLoading = true;

                // Lấy 1 quản lý bất kỳ
                var nhanVien = db.NhanVien.FirstOrDefault(x => x.VaiTro == "QuanLy" && x.TrangThai == true);

                if (nhanVien != null)
                {
                    TaiKhoan = nhanVien.HoTen;
                }
                else
                {
                    TaiKhoan = "Quản lý";
                }

                // Đếm nhân viên
                SoLuongNhanVien = db.NhanVien.Count(x => x.VaiTro == "NhanVien");
                TongNhanVien = db.NhanVien.Count();

                // Đếm sản phẩm
                SoLuongSanPham = db.SanPham.Count();

                // TODO: nếu có cột tồn kho thì tính SanPhamSapHet ở đây
                SanPhamSapHet = 0;

                // Đơn hàng & doanh thu hôm nay
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                var hoaDonHomNay = db.HoaDon
                    .Where(x => x.NgayLap >= today && x.NgayLap < tomorrow)
                    .ToList();

                SoLuongDonHang = hoaDonHomNay.Count;
                DoanhThu = hoaDonHomNay.Sum(x => (decimal?)x.TongTien) ?? 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi load thông tin: {ex.Message}");
                MessageBox.Show($"Không thể tải thông tin: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ================== LOAD ĐƠN HÀNG ==================

        private void LoadDanhSachDonHang()
        {
            try
            {
                IsLoading = true;

                var danhSach = db.HoaDon
                    .OrderByDescending(x => x.NgayLap)
                    .Take(50)
                    .ToList();

                DanhSachDonHang = new ObservableCollection<HoaDon>(danhSach);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi load danh sách đơn hàng: {ex.Message}");
                MessageBox.Show($"Không thể tải danh sách đơn hàng: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                DanhSachDonHang = new ObservableCollection<HoaDon>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ================== LỌC ĐƠN HÀNG ==================

        public void LocDonHangTheoNgay(DateTime tuNgay, DateTime denNgay)
        {
            try
            {
                IsLoading = true;

                var danhSach = db.HoaDon
                    .Where(x => x.NgayLap >= tuNgay && x.NgayLap <= denNgay)
                    .OrderByDescending(x => x.NgayLap)
                    .ToList();

                DanhSachDonHang = new ObservableCollection<HoaDon>(danhSach);

                SoLuongDonHang = danhSach.Count;
                DoanhThu = danhSach.Sum(x => (decimal?)x.TongTien) ?? 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi lọc đơn hàng: {ex.Message}");
                MessageBox.Show($"Không thể lọc đơn hàng: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void LocDonHangTheoTrangThai(string trangThai)
        {
            try
            {
                IsLoading = true;

                var danhSach = db.HoaDon
                    .Where(x => x.TrangThai == trangThai)
                    .OrderByDescending(x => x.NgayLap)
                    .ToList();

                DanhSachDonHang = new ObservableCollection<HoaDon>(danhSach);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi lọc theo trạng thái: {ex.Message}");
                MessageBox.Show($"Không thể lọc theo trạng thái: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ================== COMMAND HANDLERS ==================

        public void TaiLai(object parameter = null)
        {
            LoadThongTin();
            LoadDanhSachDonHang();
        }

        private void LocTheoNgay(object parameter)
        {
            if (parameter is Tuple<DateTime, DateTime> range)
            {
                LocDonHangTheoNgay(range.Item1, range.Item2);
            }
        }

        private void LocTheoTrangThai(object parameter)
        {
            var trangThai = parameter as string;
            if (!string.IsNullOrEmpty(trangThai))
            {
                LocDonHangTheoTrangThai(trangThai);
            }
        }

        // ================== DISPOSE ==================

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}
