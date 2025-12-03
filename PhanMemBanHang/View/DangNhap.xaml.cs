using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PhanMemBanHang.ViewModel;

namespace PhanMemBanHang.View
{
    public partial class DangNhap : Window
    {
        public DangNhap()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is DangNhapVM vm && sender is PasswordBox pb)
            {
                vm.MatKhau = pb.Password;
            }
        }

        private void Btn_Close(object sender, RoutedEventArgs e)
        {
            var dr = MessageBox.Show(
                "Bạn có muốn thoát khỏi màn hình không?",
                "Thông báo",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (dr == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            else
            {
                return;
            }
        }

        private void QuenMatKhau(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Vui lòng liên hệ với quản lý để cấp lại mật khẩu và tài khoản ! ",
                "Thông báo", MessageBoxButton.OK);
        }
    }
}
