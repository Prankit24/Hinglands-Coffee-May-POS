using PhanMemBanHang.View;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using PhanMemBanHang.Model;

namespace PhanMemBanHang.ViewModel
{
    public class DangNhapVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private readonly HighlandsCoffeeDBEntities db = new HighlandsCoffeeDBEntities();

        // ===== THUỘC TÍNH =====
        public string TaiKhoan { get => taiKhoan; set { taiKhoan = value; OnPropertyChanged(); } }
        private string taiKhoan;

        public string MatKhau { get => matKhau; set { matKhau = value; OnPropertyChanged(); } }
        private string matKhau;

        public string MaXacNhan { get => _maXacNhan; set { _maXacNhan = value; OnPropertyChanged(); } }
        private string _maXacNhan;

        public string CurrentCaptcha { get => currentCaptcha; set { currentCaptcha = value; OnPropertyChanged(); } }
        private string currentCaptcha;

        public string ChucVu { get => chucVu; set { chucVu = value; OnPropertyChanged(); } }
        private string chucVu;

        public DangNhapVM()
        {
            TaoCaptcha();
        }

        // ===== TẠO CAPTCHA =====
        public void TaoCaptcha()
        {
            Random rd = new Random();
            int length = rd.Next(4, 5);
            string captcha = "";
            int count = 0;

            do
            {
                int chr = rd.Next(48, 123);
                bool isDigit = (chr >= 48 && chr <= 57);
                bool isUpper = (chr >= 65 && chr <= 90);
                bool isLower = (chr >= 97 && chr <= 122);

                if (isDigit || isUpper || isLower)
                {
                    captcha += (char)chr;
                    count++;
                    if (count == length)
                        break;
                }
            } while (true);

            CurrentCaptcha = captcha;
            MaXacNhan = string.Empty;
        }

        // ===== ĐĂNG NHẬP =====
        public void DangNhap()
        {
            if (string.IsNullOrWhiteSpace(TaiKhoan))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!");
                TaoCaptcha();
                return;
            }

            if (string.IsNullOrWhiteSpace(MatKhau))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!");
                TaoCaptcha();
                return;
            }

            if (string.IsNullOrWhiteSpace(MaXacNhan))
            {
                MessageBox.Show("Vui lòng nhập mã xác nhận!");
                TaoCaptcha();
                return;
            }

            if (!string.Equals(MaXacNhan, CurrentCaptcha, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Mã xác nhận không đúng!");
                TaoCaptcha();
                return;
            }

            try
            {
                NhanVien nv = db.NhanVien.FirstOrDefault(t => t.TaiKhoan == TaiKhoan && t.TrangThai == true);

                if (nv == null)
                {
                    MessageBox.Show("Tài khoản không tồn tại!");
                    return;
                }

                if (nv.MatKhau != MatKhau)
                {
                    MessageBox.Show("Sai mật khẩu!");
                    return;
                }

                ChucVu = nv.ChucVu;
                MessageBox.Show($"Đăng nhập thành công! Chức vụ: {ChucVu}");

                if (ChucVu == "Nhân viên")
                {
                    POSWindow winNV = new POSWindow(nv.MaNV, nv.HoTen, nv.ChucVu);
                    winNV.Show();
                    Application.Current.MainWindow = winNV;
                }
                else
                {
                    var winQL = new AdminWindow();
                    winQL.Show();
                    Application.Current.MainWindow = winQL;
                }


                Application.Current.Windows.OfType<DangNhap>().FirstOrDefault()?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi CSDL: " + ex.Message);
            }
        }
    }
}
