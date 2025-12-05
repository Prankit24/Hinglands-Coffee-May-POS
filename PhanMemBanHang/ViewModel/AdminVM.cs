using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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

        public AdminVM()
        {
            LoadThongTin();
            LoadDanhSachDonHang();
        }

        private void LoadThongTin()
        {
            try
            {
                IsLoading = true;

                var nhanVien = db.NhanVien.FirstOrDefault(x => x.ChucVu == "QuanLy" && x.TrangThai == true);

                TaiKhoan = nhanVien != null ? nhanVien.HoTen : "Quản lý";

                SoLuongNhanVien = db.NhanVien.Count(x => x.ChucVu == "NhanVien");
                TongNhanVien = db.NhanVien.Count();

                SoLuongSanPham = db.SanPham.Count();
                SanPhamSapHet = 0; // TODO

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
        public void Dispose()
        {
            db?.Dispose();
        }
    }
}
