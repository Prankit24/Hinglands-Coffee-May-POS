using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PhanMemBanHang.Model;

namespace PhanMemBanHang.ViewModel
{
    public class QLNhanVienVM : BaseViewModel, IDisposable
    {
        private readonly HighlandsCoffeeDBEntities _context;
        private ObservableCollection<NhanVien> _danhSachNhanVien;
        private ObservableCollection<NhanVien> _danhSachNhanVienGoc;
        private NhanVien _nhanVienDangChon;
        private string _tuKhoaTimKiem;
        private string _vaiTroLoc;
        private string _chucVuLoc;
        private string _matKhauMoi;

        public QLNhanVienVM()
        {
            _context = new HighlandsCoffeeDBEntities();

            // Khởi tạo danh sách
            DanhSachGioiTinh = new ObservableCollection<string> { "Nam", "Nữ", "Khác" };
            DanhSachVaiTro = new ObservableCollection<string>
            {
                "Thu ngân",
                "Pha chế",
                "Phục vụ",
                "Bảo vệ",
                "Quản lý kho"
            };
            DanhSachChucVu = new ObservableCollection<string> { "Nhân viên", "Quản lý" };

            // Danh sách lọc
            DanhSachLocVaiTro = new ObservableCollection<string> { "Tất cả vai trò" };
            foreach (var vaiTro in DanhSachVaiTro)
            {
                DanhSachLocVaiTro.Add(vaiTro);
            }

            DanhSachLocChucVu = new ObservableCollection<string> { "Tất cả chức vụ" };
            foreach (var chucVu in DanhSachChucVu)
            {
                DanhSachLocChucVu.Add(chucVu);
            }

            // Khởi tạo Commands
            ThemMoiCommand = new RelayCommand(p => ThemMoi(p), p => CanThemMoi(p));
            CapNhatCommand = new RelayCommand(p => CapNhat(p), p => CanCapNhat(p));
            XoaCommand = new RelayCommand(p => Xoa(p), p => CanXoa(p));
            LamMoiCommand = new RelayCommand(p => LamMoi(p));
            
            // Load dữ liệu
            LoadDanhSachNhanVien();
            LamMoi(null);
            // Mặc định lọc
            _vaiTroLoc = "Tất cả vai trò";
            _chucVuLoc = "Tất cả chức vụ";
        }

        #region Properties

        public ObservableCollection<NhanVien> DanhSachNhanVien
        {
            get => _danhSachNhanVien;
            set
            {
                _danhSachNhanVien = value;
                OnPropertyChanged();
                CapNhatThongKe();
            }
        }

        public NhanVien NhanVienDangChon
        {
            get => _nhanVienDangChon;
            set
            {
                _nhanVienDangChon = value;
                OnPropertyChanged();

                // Cập nhật trạng thái các nút
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string TuKhoaTimKiem
        {
            get => _tuKhoaTimKiem;
            set
            {
                _tuKhoaTimKiem = value;
                OnPropertyChanged();
                TimKiem();
            }
        }

        public string VaiTroLoc
        {
            get => _vaiTroLoc;
            set
            {
                _vaiTroLoc = value;
                OnPropertyChanged();
            }
        }

        public string ChucVuLoc
        {
            get => _chucVuLoc;
            set
            {
                _chucVuLoc = value;
                OnPropertyChanged();
            }
        }

        public string MatKhauMoi
        {
            get => _matKhauMoi;
            set
            {
                _matKhauMoi = value;
                OnPropertyChanged();
            }
        }

        // Danh sách cho ComboBox
        public ObservableCollection<string> DanhSachGioiTinh { get; set; }
        public ObservableCollection<string> DanhSachVaiTro { get; set; }
        public ObservableCollection<string> DanhSachChucVu { get; set; }
        public ObservableCollection<string> DanhSachLocVaiTro { get; set; }
        public ObservableCollection<string> DanhSachLocChucVu { get; set; }

        // Thống kê
        private int _tongSoNhanVien;
        public int TongSoNhanVien
        {
            get => _tongSoNhanVien;
            set { _tongSoNhanVien = value; OnPropertyChanged(); }
        }

        private int _soNVDangHoatDong;
        public int SoNVDangHoatDong
        {
            get => _soNVDangHoatDong;
            set { _soNVDangHoatDong = value; OnPropertyChanged(); }
        }

        private int _soNVNgungHoatDong;
        public int SoNVNgungHoatDong
        {
            get => _soNVNgungHoatDong;
            set { _soNVNgungHoatDong = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands

        public ICommand ThemMoiCommand { get; }
        public ICommand CapNhatCommand { get; }
        public ICommand XoaCommand { get; }
        public ICommand LamMoiCommand { get; }

        #endregion

        #region Methods

        private void LoadDanhSachNhanVien()
        {
            try
            {
                var list = _context.NhanVien.ToList();
                _danhSachNhanVienGoc = new ObservableCollection<NhanVien>(list);
                DanhSachNhanVien = new ObservableCollection<NhanVien>(list);
                CapNhatThongKe();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách nhân viên: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TimKiem()
        {
            if (_danhSachNhanVienGoc == null) return;

            var ketQua = _danhSachNhanVienGoc.AsEnumerable();

            // Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
            {
                var keyword = TuKhoaTimKiem.ToLower();
                ketQua = ketQua.Where(nv =>
                    (!string.IsNullOrEmpty(nv.HoTen) && nv.HoTen.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(nv.HoTen) && nv.HoTen.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(nv.SoDienThoai) && nv.SoDienThoai.Contains(keyword)) ||
                    (!string.IsNullOrEmpty(nv.Email) && nv.Email.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(nv.TaiKhoan) && nv.TaiKhoan.ToLower().Contains(keyword))
                );
            }

            // Lọc theo vai trò
            if (!string.IsNullOrEmpty(VaiTroLoc) && VaiTroLoc != "Tất cả vai trò")
            {
                ketQua = ketQua.Where(nv => nv.VaiTro == VaiTroLoc);
            }

            // Lọc theo chức vụ
            if (!string.IsNullOrEmpty(ChucVuLoc) && ChucVuLoc != "Tất cả chức vụ")
            {
                ketQua = ketQua.Where(nv => nv.ChucVu == ChucVuLoc);
            }

            DanhSachNhanVien = new ObservableCollection<NhanVien>(ketQua);
        }

        public void LocTheoVaiTro()
        {
            TimKiem();
        }

        public void LocTheoChucVu()
        {
            TimKiem();
        }

        private void CapNhatThongKe()
        {
            if (_danhSachNhanVienGoc == null) return;

            TongSoNhanVien = _danhSachNhanVienGoc.Count;
            SoNVDangHoatDong = _danhSachNhanVienGoc.Count(nv => nv.TrangThai);
            SoNVNgungHoatDong = _danhSachNhanVienGoc.Count(nv => !nv.TrangThai);
        }

        private void ThemMoi(object parameter)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(NhanVienDangChon?.HoTen))
                {
                    MessageBox.Show("Vui lòng nhập tên nhân viên!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NhanVienDangChon?.TaiKhoan))
                {
                    MessageBox.Show("Vui lòng nhập tài khoản!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(MatKhauMoi))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NhanVienDangChon?.VaiTro))
                {
                    MessageBox.Show("Vui lòng chọn vai trò!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate số điện thoại nếu có
                if (!string.IsNullOrWhiteSpace(NhanVienDangChon.SoDienThoai))
                {
                    if (NhanVienDangChon.SoDienThoai.Length < 10 ||
                        !NhanVienDangChon.SoDienThoai.All(char.IsDigit))
                    {
                        MessageBox.Show("Số điện thoại phải có ít nhất 10 chữ số!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // Validate lương
                if (NhanVienDangChon.Luong.HasValue && NhanVienDangChon.Luong < 0)
                {
                    MessageBox.Show("Lương phải lớn hơn hoặc bằng 0!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra tài khoản đã tồn tại
                var taiKhoanTonTai = _context.NhanVien.Any(nv => nv.TaiKhoan == NhanVienDangChon.TaiKhoan);
                if (taiKhoanTonTai)
                {
                    MessageBox.Show("Tài khoản đã tồn tại!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Mã hóa mật khẩu
                NhanVienDangChon.MatKhau = MaHoaMatKhau(MatKhauMoi);

                // Thêm vào database
                _context.NhanVien.Add(NhanVienDangChon);
                int result = _context.SaveChanges();

                if (result > 0)
                {
                    MessageBox.Show("Thêm nhân viên thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadDanhSachNhanVien();
                    LamMoi(null);
                }
                else
                {
                    MessageBox.Show("Thêm nhân viên thất bại!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanThemMoi(object parameter)
        {
            return NhanVienDangChon != null;
        }

        private void CapNhat(object parameter)
        {
            try
            {
                if (NhanVienDangChon == null || NhanVienDangChon.MaNV == 0)
                {
                    MessageBox.Show("Vui lòng chọn nhân viên cần cập nhật!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate tương tự như ThemMoi
                if (string.IsNullOrWhiteSpace(NhanVienDangChon?.HoTen))
                {
                    MessageBox.Show("Vui lòng nhập tên nhân viên!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NhanVienDangChon?.TaiKhoan))
                {
                    MessageBox.Show("Vui lòng nhập tài khoản!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NhanVienDangChon?.VaiTro))
                {
                    MessageBox.Show("Vui lòng chọn vai trò!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate số điện thoại
                if (!string.IsNullOrWhiteSpace(NhanVienDangChon.SoDienThoai))
                {
                    if (NhanVienDangChon.SoDienThoai.Length < 10 ||
                        !NhanVienDangChon.SoDienThoai.All(char.IsDigit))
                    {
                        MessageBox.Show("Số điện thoại phải có ít nhất 10 chữ số!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // Validate lương
                if (NhanVienDangChon.Luong.HasValue && NhanVienDangChon.Luong < 0)
                {
                    MessageBox.Show("Lương phải lớn hơn hoặc bằng 0!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Nếu có mật khẩu mới thì cập nhật
                if (!string.IsNullOrWhiteSpace(MatKhauMoi))
                {
                    NhanVienDangChon.MatKhau = MaHoaMatKhau(MatKhauMoi);
                }

                var result = MessageBox.Show("Bạn có chắc chắn muốn cập nhật thông tin nhân viên này?",
                    "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Đánh dấu entity đã thay đổi
                    _context.Entry(NhanVienDangChon).State = EntityState.Modified;
                    int saveResult = _context.SaveChanges();

                    if (saveResult > 0)
                    {
                        MessageBox.Show("Cập nhật nhân viên thành công!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDanhSachNhanVien();
                        MatKhauMoi = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật nhân viên thất bại!", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanCapNhat(object parameter)
        {
            return NhanVienDangChon != null && NhanVienDangChon.MaNV > 0;
        }

        private void Xoa(object parameter)
        {
            try
            {
                if (NhanVienDangChon == null || NhanVienDangChon.MaNV == 0)
                {
                    MessageBox.Show("Vui lòng chọn nhân viên cần xóa!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa nhân viên '{NhanVienDangChon.HoTen}'?\n\n" +
                    "Lưu ý: Thao tác này không thể hoàn tác!",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var nhanVien = _context.NhanVien.Find(NhanVienDangChon.MaNV);
                    if (nhanVien != null)
                    {
                        _context.NhanVien.Remove(nhanVien);
                        int saveResult = _context.SaveChanges();

                        if (saveResult > 0)
                        {
                            MessageBox.Show("Xóa nhân viên thành công!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadDanhSachNhanVien();
                            LamMoi(null);
                        }
                        else
                        {
                            MessageBox.Show("Xóa nhân viên thất bại!", "Lỗi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa nhân viên: {ex.Message}\n\n" +
                    "Có thể nhân viên này đã có dữ liệu liên quan trong hệ thống.",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanXoa(object parameter)
        {
            return NhanVienDangChon != null && NhanVienDangChon.MaNV > 0;
        }

        private void LamMoi(object parameter)
        {
            NhanVienDangChon = new NhanVien
            {
                GioiTinh = "Nữ",
                TrangThai = true, // Mặc định là đang hoạt động
                NgayVaoLam = DateTime.Now,
                VaiTro = "Phục vụ",
                ChucVu = "Nhân viên"
            };
            MatKhauMoi = string.Empty;
            TuKhoaTimKiem = string.Empty;
            VaiTroLoc = "Tất cả vai trò";
            ChucVuLoc = "Tất cả chức vụ";
        }

        // Hàm mã hóa mật khẩu đơn giản (nên dùng BCrypt.Net trong thực tế)
        private string MaHoaMatKhau(string matKhau)
        {
            // Trong thực tế, nên dùng BCrypt.Net-Next
            // return BCrypt.Net.BCrypt.HashPassword(matKhau);

            // Tạm thời dùng cách đơn giản (KHÔNG AN TOÀN cho production)
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(matKhau));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _context?.Dispose();
        }

        #endregion
    }
}