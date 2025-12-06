using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PhanMemBanHang.Model;

namespace PhanMemBanHang.ViewModel
{
    public class AdminVM : BaseViewModel, IDisposable
    {
        private readonly HighlandsCoffeeDBEntities db;
        private int soLuongNhanVien;
        public int SoLuongNhanVien
        {
            get => soLuongNhanVien;
            set => SetProperty(ref soLuongNhanVien, value);
        }

        private string hoTen;
        public string HoTen
        {
            get => hoTen;
            set => SetProperty(ref hoTen, value);
        }

        private string chucVu;
        public string ChucVu
        {
            get => chucVu;
            set => SetProperty(ref chucVu, value);
        }


        private int soLuongDonHang;
        public int SoLuongDonHang
        {
            get => soLuongDonHang;
            set => SetProperty(ref soLuongDonHang, value);
        }

        private decimal doanhThu;
        public decimal DoanhThu
        {
            get => doanhThu;
            set => SetProperty(ref doanhThu, value);
        }

        private int soLuongSanPham;
        public int SoLuongSanPham
        {
            get => soLuongSanPham;
            set => SetProperty(ref soLuongSanPham, value);
        }

        private int sanPhamSapHet;
        public int SanPhamSapHet
        {
            get => sanPhamSapHet;
            set => SetProperty(ref sanPhamSapHet, value);
        }

        private int tongNhanVien;
        public int TongNhanVien
        {
            get => tongNhanVien;
            set => SetProperty(ref tongNhanVien, value);
        }

        private ObservableCollection<HoaDon> danhSachDonHang;
        public ObservableCollection<HoaDon> DanhSachDonHang
        {
            get => danhSachDonHang;
            set => SetProperty(ref danhSachDonHang, value);
        }

        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        public AdminVM()
        {
            db = new HighlandsCoffeeDBEntities();
            LoadThongTin();
            LoadDanhSachDonHang();
        }

        private void LoadThongTin()
        {
            try
            {
                IsLoading = true;

                var nhanVien = db.NhanVien.FirstOrDefault(x => x.ChucVu == "Quản lý" && x.TrangThai == true);
                if (nhanVien != null)
                {
                    HoTen = nhanVien.HoTen;
                    ChucVu = nhanVien.ChucVu;  
                }
                else
                {
                    HoTen = "Quản lý";
                    ChucVu = string.Empty;
                }

                SoLuongNhanVien = db.NhanVien.Count(x => x.TrangThai == true);
                TongNhanVien = db.NhanVien.Count();

                SoLuongSanPham = db.SanPham.Count();
                SanPhamSapHet = 0; 

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
