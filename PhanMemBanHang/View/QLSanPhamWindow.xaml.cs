using System.ComponentModel;
using System.Windows;
using PhanMemBanHang.ViewModel;

namespace PhanMemBanHang.View
{
    public partial class QLSanPhamWindow : Window
    {
        private readonly QLSanPhamVM spVM = new QLSanPhamVM();

        public QLSanPhamWindow()
        {
            InitializeComponent();
            DataContext = spVM;
        }

        private void QuayLai(object sender, RoutedEventArgs e)
        {
            var ql = new AdminWindow();
            ql.Show();
            Close();
        }

        private void Them(object sender, RoutedEventArgs e)
        {
            spVM.ThemSanPham();
        }

        private void Sua(object sender, RoutedEventArgs e)
        {
            spVM.SuaSanPham();
        }

        private void Xoa(object sender, RoutedEventArgs e)
        {
            spVM.XoaSanPham();
        }

        private void LamMoi(object sender, RoutedEventArgs e)
        {
            spVM.LamMoi();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            spVM?.Dispose();
            base.OnClosing(e);
        }
    }
}
