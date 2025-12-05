using System.Windows;
using System.Windows.Controls;
using PhanMemBanHang.ViewModel;

namespace PhanMemBanHang.View
{
    public partial class DangNhap : Window
    {
        DangNhapVM dnVM = new DangNhapVM();

        public DangNhap()
        {
            InitializeComponent();
            DataContext = dnVM;
        }


        private void Btn_DangNhap_Click(object sender, RoutedEventArgs e)
        {
            dnVM.DangNhap();
        }

        private void Btn_RefreshCaptcha_Click(object sender, RoutedEventArgs e)
        {
            dnVM.TaoCaptcha();
        }

        private void Btn_Close(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Thoát ứng dụng?", "Xác nhận",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void QuenMatKhau(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("Liên hệ quản lý để cấp lại mật khẩu.");
        }
    }
}
