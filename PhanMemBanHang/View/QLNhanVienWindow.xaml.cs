using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using PhanMemBanHang.ViewModel;

namespace PhanMemBanHang.View
{
    public partial class QLNhanVienWindow : Window
    {
        private readonly QLNhanVienVM _viewModel;

        public QLNhanVienWindow()
        {
            InitializeComponent();
            _viewModel = new QLNhanVienVM();
            DataContext = _viewModel;
        }

        // Nút quay lại màn quản lý (Admin / QuanLyWindow)
        private void BtnQuayLai_Click(object sender, RoutedEventArgs e)
        {
            var ql = new AdminWindow();
            ql.Show();
            Close();
        }

        // Combobox lọc vai trò
        private void ComboBox_VaiTroLoc_Changed(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.LocTheoVaiTro();
        }

        // Combobox lọc chức vụ
        private void ComboBox_ChucVuLoc_Changed(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.LocTheoChucVu();
        }

        // Xử lý khi nhập mật khẩu
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null && _viewModel != null)
            {
                _viewModel.MatKhauMoi = passwordBox.Password;
            }
        }

        // Đóng window -> nếu ViewModel có IDisposable thì dispose luôn
        protected override void OnClosing(CancelEventArgs e)
        {
            if (_viewModel is IDisposable d)
            {
                d.Dispose();
            }
            base.OnClosing(e);
        }

       
    }
}