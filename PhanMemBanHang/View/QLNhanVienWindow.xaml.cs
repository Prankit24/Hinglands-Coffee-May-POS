using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using PhanMemBanHang.ViewModel;

namespace PhanMemBanHang.View
{
    public partial class QLNhanVienWindow : Window
    {
        QLNhanVienVM nvVM = new QLNhanVienVM();
        public QLNhanVienWindow()
        {
            InitializeComponent();
            DataContext = nvVM;
        }

        private void BtnQuayLai_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow ql = new AdminWindow();
            ql.Show();
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (nvVM is IDisposable d)
            {
                d.Dispose();
            }
            base.OnClosing(e);
        }

        private void ThemMoi(object sender, RoutedEventArgs e)
        {
            nvVM.ThemMoiVM();
        }

        private void Sua(object sender, RoutedEventArgs e)
        {
            nvVM.CapNhatVM();
        }

        private void Xoa(object sender, RoutedEventArgs e)
        {
            nvVM.XoaVM();
        }

        private void LocChucVu(object sender, SelectionChangedEventArgs e)
        {
            nvVM.LocTheoChucVu();
        }

        private void LocVaiTro(object sender, SelectionChangedEventArgs e)
        {
            nvVM.LocTheoVaiTro();
        }

    }
}
