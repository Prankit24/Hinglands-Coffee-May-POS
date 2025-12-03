using System;
using System.Windows;
using PhanMemBanHang.ViewModel;

namespace PhanMemBanHang.View
{
    public partial class AdminWindow : Window
    {
        private readonly AdminVM _viewModel;

        public AdminWindow()
        {
            InitializeComponent();

            _viewModel = new AdminVM();
            DataContext = _viewModel;
        }

        // ================== HEADER BUTTONS ==================

        private void Btn_DangXuat(object sender, RoutedEventArgs e)
        {
            // Quay về màn đăng nhập
            var login = new DangNhap();
            login.Show();
            Close();
        }

        // ================== MENU BUTTONS ==================

        private void BtnBanHang_Click(object sender, RoutedEventArgs e)
        {
            var posWindow = new POSWindow();
            posWindow.Show();
            Close();
        }

        private void BtnSanPham_Click(object sender, RoutedEventArgs e)
        {
            var qlSanPhamWindow = new QLSanPhamWindow();
            qlSanPhamWindow.Show();
            Close();
        }

        private void BtnNhanVien_Click(object sender, RoutedEventArgs e)
        {
            var main = new QLNhanVienWindow();
            main.Show();
            Close();
        }

        private void BtnBaoCao_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng báo cáo đang được cập nhật.",
                            "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDonHang_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Danh sách đơn hàng sẽ được bổ sung sau.",
                            "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ================== WINDOW EVENTS ==================

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Giải phóng DbContext
            _viewModel.Dispose();
        }
    }
}
