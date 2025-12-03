using PhanMemBanHang.View;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
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
        private string taiKhoan;
        public string TaiKhoan
        {
            get => taiKhoan;
            set
            {
                if (taiKhoan != value)
                {
                    taiKhoan = value;
                    OnPropertyChanged();
                }
            }
        }

        private string matKhau;
        public string MatKhau
        {
            get => matKhau;
            set
            {
                if (matKhau != value)
                {
                    matKhau = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _maXacNhan;
        public string MaXacNhan
        {
            get => _maXacNhan;
            set
            {
                if (_maXacNhan != value)
                {
                    _maXacNhan = value;
                    OnPropertyChanged();
                }
            }
        }

        private string currentCaptcha;
        public string CurrentCaptcha
        {
            get => currentCaptcha;
            set
            {
                if (currentCaptcha != value)
                {
                    currentCaptcha = value;
                    OnPropertyChanged();
                }
            }
        }

        private string vaiTro;
        public string VaiTro
        {
            get => vaiTro;
            set
            {
                if (vaiTro != value)
                {
                    vaiTro = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand DangNhapCommand { get; set; }
        public ICommand RefreshCaptchaCommand { get; set; }

        public DangNhapVM()
        {
            TaoCaptcha();

            RefreshCaptchaCommand = new RelayCommand(_ =>
            {
                TaoCaptcha();
            });

            DangNhapCommand = new RelayCommand(_ =>
            {
                DangNhap();
            });
        }
        
        private void TaoCaptcha()
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
        private void DangNhap()
        {
            if (string.IsNullOrWhiteSpace(TaiKhoan))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                TaoCaptcha();
                return;
            }

            if (string.IsNullOrWhiteSpace(MatKhau))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                TaoCaptcha();
                return;
            }

            if (string.IsNullOrWhiteSpace(MaXacNhan))
            {
                MessageBox.Show("Vui lòng nhập mã xác nhận!",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                TaoCaptcha();
                return;
            }
            if (!string.Equals(MaXacNhan, CurrentCaptcha, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Mã xác nhận không đúng, vui lòng thử lại!",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                TaoCaptcha();
                return;
            }
            try
            {
                NhanVien nv = db.NhanVien.FirstOrDefault(t => t.TaiKhoan == TaiKhoan && t.TrangThai == true);
                if (nv == null)
                {
                    MessageBox.Show("Tài khoản không tồn tại",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (nv.MatKhau != MatKhau)
                {
                    MessageBox.Show("Mật khẩu không đúng.",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                VaiTro = nv.VaiTro;
                MessageBox.Show("Đăng nhập thành công! với vai trò: "+VaiTro,
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                if (VaiTro == "NhanVien")
                {
                    var winNV = new POSWindow();
                    winNV.Show();
                    Application.Current.MainWindow = winNV;
                }
                else
                {
                    var winQL = new AdminWindow();
                    winQL.Show();
                    Application.Current.MainWindow = winQL;
                }
                var loginWin = Application.Current.Windows.OfType<DangNhap>().FirstOrDefault();
                loginWin.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message,
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
