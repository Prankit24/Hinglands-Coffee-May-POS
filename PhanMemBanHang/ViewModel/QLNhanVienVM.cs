using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using PhanMemBanHang.Model;

namespace PhanMemBanHang.ViewModel
{
    public class QLNhanVienVM : BaseViewModel, IDisposable
    {
        public readonly HighlandsCoffeeDBEntities db;
        public ObservableCollection<NhanVien> danhSachNhanVien;
        public ObservableCollection<NhanVien> danhSachNhanVienGoc;
        public NhanVien nv;
        public string tuKhoaTimKiem;
        public string vaiTroLoc;
        public string chucVuLoc;

        public QLNhanVienVM()
        {
            db = new HighlandsCoffeeDBEntities();
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

            LoadDanhSachNhanVien();
            LamMoi();
            vaiTroLoc = "Tất cả vai trò";
            chucVuLoc = "Tất cả chức vụ";
        }
        public ObservableCollection<NhanVien> DanhSachNhanVien
        {
            get => danhSachNhanVien;
            set
            {
                danhSachNhanVien = value;
                OnPropertyChanged();
                CapNhatThongKe();
            }
        }
        public NhanVien NhanVienDangChon
        {
            get => nv;
            set
            {
                nv = value;
                OnPropertyChanged();

                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string TuKhoaTimKiem
        {
            get => tuKhoaTimKiem;
            set
            {
                tuKhoaTimKiem = value;
                OnPropertyChanged();
                TimKiem();
            }
        }

        public string VaiTroLoc
        {
            get => vaiTroLoc;
            set
            {
                vaiTroLoc = value;
                OnPropertyChanged();
            }
        }

        public string ChucVuLoc
        {
            get => chucVuLoc;
            set
            {
                chucVuLoc = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> DanhSachGioiTinh { get; set; }
        public ObservableCollection<string> DanhSachVaiTro { get; set; }
        public ObservableCollection<string> DanhSachChucVu { get; set; }
        public ObservableCollection<string> DanhSachLocVaiTro { get; set; }
        public ObservableCollection<string> DanhSachLocChucVu { get; set; }

        public int tongSoNhanVien;
        public int TongSoNhanVien
        {
            get => tongSoNhanVien;
            set { tongSoNhanVien = value; OnPropertyChanged(); }
        }

        public int soNVDangHoatDong;
        public int SoNVDangHoatDong
        {
            get => soNVDangHoatDong;
            set { soNVDangHoatDong = value; OnPropertyChanged(); }
        }

        public int soNVNgungHoatDong;
        public int SoNVNgungHoatDong
        {
            get => soNVNgungHoatDong;
            set { soNVNgungHoatDong = value; OnPropertyChanged(); }
        }


        public void TimKiem()
        {
            if (danhSachNhanVienGoc == null) return;

            var ketQua = danhSachNhanVienGoc.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
            {
                var keyword = TuKhoaTimKiem.ToLower();
                ketQua = ketQua.Where(nv =>
                    (!string.IsNullOrEmpty(nv.HoTen) && nv.HoTen.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(nv.SoDienThoai) && nv.SoDienThoai.Contains(keyword)) ||
                    (!string.IsNullOrEmpty(nv.Email) && nv.Email.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(nv.TaiKhoan) && nv.TaiKhoan.ToLower().Contains(keyword))
                );
            }

            if (!string.IsNullOrEmpty(VaiTroLoc) && VaiTroLoc != "Tất cả vai trò")
            {
                ketQua = ketQua.Where(nv => nv.VaiTro == VaiTroLoc);
            }

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

        public void LoadDanhSachNhanVien()
        {
            try
            {
                var list = db.NhanVien.ToList();
                danhSachNhanVienGoc = new ObservableCollection<NhanVien>(list);
                DanhSachNhanVien = new ObservableCollection<NhanVien>(list);
                CapNhatThongKe();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách nhân viên: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CapNhatThongKe()
        {
            if (danhSachNhanVienGoc == null) return;

            TongSoNhanVien = danhSachNhanVienGoc.Count;
            SoNVDangHoatDong = danhSachNhanVienGoc.Count(nv => nv.TrangThai);
            SoNVNgungHoatDong = danhSachNhanVienGoc.Count(nv => !nv.TrangThai);
        }

        public void ThemMoiVM()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NhanVienDangChon?.HoTen))
                {
                    MessageBox.Show("Vui lòng nhập tên nhân viên, không thể để trống",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NhanVienDangChon?.TaiKhoan))
                {
                    MessageBox.Show("Vui lòng tạo tài khoản cho nhân viên, không thể để trống",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NhanVienDangChon?.MatKhau))
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
                if (!string.IsNullOrWhiteSpace(NhanVienDangChon.SoDienThoai))
                {
                    if (!Regex.IsMatch(NhanVienDangChon.SoDienThoai ?? "", @"^0[0-9]{9}$"))
                    {
                        MessageBox.Show("Không đúng định dạng số điện thoại", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                if (NhanVienDangChon.Luong.HasValue && NhanVienDangChon.Luong < 0)
                {
                    MessageBox.Show("Lương phải lớn hơn hoặc bằng 0!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var taiKhoanTonTai = db.NhanVien.Any(nv => nv.TaiKhoan == NhanVienDangChon.TaiKhoan);
                if (taiKhoanTonTai)
                {
                    MessageBox.Show("Tài khoản đã tồn tại!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                db.NhanVien.Add(NhanVienDangChon);
                if (db.SaveChanges() > 0)
                {
                    MessageBox.Show("Thêm nhân viên thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadDanhSachNhanVien();
                    LamMoi();
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

        public void CapNhatVM()
        {
            try
            {
                if (NhanVienDangChon == null || NhanVienDangChon.MaNV == 0)
                {
                    MessageBox.Show("Vui lòng chọn nhân viên cần cập nhật!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

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

                if (!string.IsNullOrWhiteSpace(NhanVienDangChon.SoDienThoai))
                {
                    if (!Regex.IsMatch(NhanVienDangChon.SoDienThoai ?? "", @"^0[0-9]{9}$"))
                    {
                        MessageBox.Show("Số điện thoại phải có 10 chữ số, bắt đầu bằng 0!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                if (NhanVienDangChon.Luong.HasValue && NhanVienDangChon.Luong < 0)
                {
                    MessageBox.Show("Lương phải lớn hơn hoặc bằng 0!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show("Bạn có chắc chắn muốn cập nhật thông tin nhân viên này?",
                    "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    int saveResult = db.SaveChanges();

                    if (saveResult > 0)
                    {
                        MessageBox.Show("Cập nhật nhân viên thành công!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDanhSachNhanVien();
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

        public void XoaVM()
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
                    var nhanVien = db.NhanVien.Find(NhanVienDangChon.MaNV);
                    if (nhanVien != null)
                    {
                        db.NhanVien.Remove(nhanVien);
                        int saveResult = db.SaveChanges();

                        if (saveResult > 0)
                        {
                            MessageBox.Show("Xóa nhân viên thành công!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadDanhSachNhanVien();
                            LamMoi();
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

        public void LamMoi()
        {
            NhanVienDangChon = new NhanVien
            {
                GioiTinh = "Nữ",
                TrangThai = true,
                NgayVaoLam = DateTime.Now,
                VaiTro = "Phục vụ",
                ChucVu = "Nhân viên",
                MatKhau = string.Empty
            };
            TuKhoaTimKiem = string.Empty;
            VaiTroLoc = "Tất cả vai trò";
            ChucVuLoc = "Tất cả chức vụ";
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}