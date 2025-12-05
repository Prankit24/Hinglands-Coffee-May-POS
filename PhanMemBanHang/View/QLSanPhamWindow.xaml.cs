using System.Windows;
using PhanMemBanHang.ViewModel;

namespace PhanMemBanHang.View
{
    public partial class QLSanPhamWindow : Window
    {
        QLSanPhamVM spVM = new QLSanPhamVM();

        public QLSanPhamWindow()
        {
            InitializeComponent();
            DataContext = spVM;
        }

        private void QuayLai(object sender, RoutedEventArgs e)
        {
            var ql = new AdminWindow();
            ql.Show();
            this.Close();
        }

        private void Them(object sender, RoutedEventArgs e)
        {
            if (!spVM.KiemTraHopLe(out string thongBao))
            {
                MessageBox.Show(thongBao, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            spVM.ThemSanPham();
            MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Sua(object sender, RoutedEventArgs e)
        {
            if (spVM.SanPhamDangChon == null || spVM.SanPhamDangChon.MaSP == 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!spVM.KiemTraHopLe(out string thongBao))
            {
                MessageBox.Show(thongBao, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            spVM.SuaSanPham();
            MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Xoa(object sender, RoutedEventArgs e)
        {
            if (spVM.SanPhamDangChon == null || spVM.SanPhamDangChon.MaSP == 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa sản phẩm '{spVM.SanPhamDangChon.TenSP}'?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                spVM.XoaSanPham();
                MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LamMoi(object sender, RoutedEventArgs e)
        {
            spVM.LamMoi();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            spVM?.Dispose();
            base.OnClosing(e);
        }
    }
}
