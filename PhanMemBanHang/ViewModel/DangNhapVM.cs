using PhanMemBanHang.View;
using System;
using System.Linq;
using System.Windows;
using PhanMemBanHang.Model;

namespace PhanMemBanHang.ViewModel
{
    public class DangNhapVM : BaseViewModel
    {
        private readonly HighlandsCoffeeDBEntities db;


        private string taiKhoan;
        public string TaiKhoan
        {
            get => taiKhoan;
            set { taiKhoan = value; OnPropertyChanged(); }
        }

        private string matKhau;
        public string MatKhau
        {
            get => matKhau;
            set { matKhau = value; OnPropertyChanged(); }
        }

        private string _maXacNhan;
        public string MaXacNhan
        {
            get => _maXacNhan;
            set { _maXacNhan = value; OnPropertyChanged(); }
        }

        private string currentCaptcha;
        public string CurrentCaptcha
        {
            get => currentCaptcha;
            set { currentCaptcha = value; OnPropertyChanged(); }
        }

        private string chucVu;
        public string ChucVu
        {
            get => chucVu;
            set { chucVu = value; OnPropertyChanged(); }
        }

        private int soLanSai;
        public int SoLanSai
        {
            get => soLanSai;
            set { soLanSai = value; OnPropertyChanged(); }
        }


        public DangNhapVM()
        {
            db = new HighlandsCoffeeDBEntities();
            TaoCaptcha();
        }

   
        public void LamMoi()
        {
            TaiKhoan = string.Empty;
            MatKhau = string.Empty;
            MaXacNhan = string.Empty;
        }

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


        public void DangNhap()
        {
            if (SoLanSai >= 5)
            {
                MessageBox.Show("Bạn đã nhập sai quá 5 lần.\nVui lòng đăng nhập lại sau!",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TaiKhoan))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!");
                LamMoi();
                TaoCaptcha();
                return;
            }

            if (string.IsNullOrWhiteSpace(MatKhau))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!");
                LamMoi();
                TaoCaptcha();
                return;
            }

            if (string.IsNullOrWhiteSpace(MaXacNhan))
            {
                MessageBox.Show("Vui lòng nhập mã xác nhận!");
                LamMoi();
                TaoCaptcha();
                return;
            }

            if (!string.Equals(MaXacNhan?.Trim(), CurrentCaptcha?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                SoLanSai++;
                MessageBox.Show("Mã xác nhận không đúng!",
                                "Sai mã xác nhận", MessageBoxButton.OK, MessageBoxImage.Warning);

                MaXacNhan = string.Empty;
                LamMoi();
                TaoCaptcha();         
                return;
            }

            try
            {
                string tkNhap = TaiKhoan.Trim();
                string mkNhap = MatKhau;
                
                var nv = db.NhanVien
                    .Where(t => t.TrangThai == true)
                    .AsEnumerable() 
                    .FirstOrDefault(t =>
                        string.Equals(t.TaiKhoan, tkNhap, StringComparison.Ordinal) &&
                        string.Equals(t.MatKhau, mkNhap, StringComparison.Ordinal));

                if (nv == null)
                {
                    SoLanSai++;
                    MessageBox.Show($"Tên đăng nhập hoặc mật khẩu không đúng!\nBạn đã nhập sai {SoLanSai}/5 lần.");
                    LamMoi();
                    TaoCaptcha();
                    return;
                }

                SoLanSai = 0;
                ChucVu = nv.ChucVu;
                MessageBox.Show($"Đăng nhập thành công - Chức vụ: {ChucVu}");

                if (ChucVu == "Nhân viên")
                {
                    POSWindow winNV = new POSWindow(nv.MaNV, nv.HoTen, nv.ChucVu);
                    Application.Current.MainWindow = winNV;
                    winNV.Show();
                }
                else
                {
                    var winQL = new AdminWindow();
                    Application.Current.MainWindow = winQL;
                    winQL.Show();
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
