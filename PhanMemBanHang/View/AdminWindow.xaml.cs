using System;
using System.Windows;
using PhanMemBanHang.ViewModel;

namespace PhanMemBanHang.View
{
    public partial class AdminWindow : Window
    {
        AdminVM qlVM = new AdminVM();
        private bool checkOut = false;
        public AdminWindow()
        {
            InitializeComponent();
            DataContext = qlVM;
        }
        private void DangXuat(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn đăng xuất không?",
                                     "Đăng xuất",
                                     MessageBoxButton.YesNo,
                                     MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;
            checkOut = true;
            var login = new DangNhap();
            login.Show();
            this.Close();
        }
        private void BanHang(object sender, RoutedEventArgs e)
        {
            checkOut = true;
            var posWindow = new POSWindow();
            posWindow.Show();
            Close();
        }

        private void QuanLySanPham(object sender, RoutedEventArgs e)
        {
            checkOut = true;
            QLSanPhamWindow qlSanPhamWindow = new QLSanPhamWindow();
            qlSanPhamWindow.Show();
            Close();
        }

        private void QuanLyNhanVien(object sender, RoutedEventArgs e)
        {
            checkOut = true;
            QLNhanVienWindow main = new QLNhanVienWindow();
            main.Show();
            Close();
        }

        private void QuanLyBaoCao(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng báo cáo đang được cập nhật.",
                            "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void QuanLyDonHang(object sender, RoutedEventArgs e)
        {
            checkOut = true;
            QLDonHangWindow main = new QLDonHangWindow();
            main.Show();
            this.Close();
        }


        private void QuanLyKhoHang(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng đang được phát triển",
                           "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void QuanLyKhachHang(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng đang được phát triển",
                           "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CaiDat(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng đang được phát triển",
                           "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (checkOut)
                return;
            MessageBoxResult result = MessageBox.Show("Bạn có muốn thoát hệ thống quản lý chứ ?",
                "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;  
            }
            else
            {
                Application.Current.Shutdown();
            }

        }
    }
}
