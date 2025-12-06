using System.Windows;
using PhanMemBanHang.ViewModel;

namespace PhanMemBanHang.View
{
    public partial class DangNhap : Window
    {
        private readonly DangNhapVM dnVM = new DangNhapVM();

        public DangNhap()
        {
            InitializeComponent();
            DataContext = dnVM;
        }

        private void btnDangNhap(object sender, RoutedEventArgs e)
        {
            dnVM.DangNhap();
        }

        private void LamMoiCaptCha(object sender, RoutedEventArgs e)
        {
            dnVM.LamMoi();     
            dnVM.TaoCaptcha();  
        }

        private void Thoat(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Bạn muốn thoát khỏi màn hình đăng nhập chứ ?",
                "Thông báo", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        private void QuenMatKhau(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("Liên hệ quản lý để cấp lại mật khẩu.");
        }
    }
}
