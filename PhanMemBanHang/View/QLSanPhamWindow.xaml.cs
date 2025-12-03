using System.Windows;
using System.Windows.Controls;
using PhanMemBanHang.Model;
using PhanMemBanHang.ViewModel;
using PhanMemBanHang.View;

namespace PhanMemBanHang.View
{
    public partial class QLSanPhamWindow : Window
    {
        private QLSanPhamVM _viewModel;

        public QLSanPhamWindow()
        {
            InitializeComponent();
            _viewModel = new QLSanPhamVM();
            this.DataContext = _viewModel;
        }

        private void BtnQuayLai_Click(object sender, RoutedEventArgs e)
        {
            var ql = new AdminWindow();
            ql.Show();
            this.Close();
            
        }

        private void DgSanPham_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSanPham.SelectedItem is SanPham sp)
            {
                _viewModel.ChonSanPham(sp);
            }
        }

        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.KiemTraHopLe(out string thongBao))
            {
                MessageBox.Show(thongBao, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _viewModel.ThemSanPham();
            MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.MaSP == 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!_viewModel.KiemTraHopLe(out string thongBao))
            {
                MessageBox.Show(thongBao, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _viewModel.SuaSanPham();
            MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.MaSP == 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa sản phẩm '{_viewModel.TenSP}'?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _viewModel.XoaSanPham();
                MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LamMoi();
            dgSanPham.SelectedItem = null;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _viewModel?.Dispose();
            base.OnClosing(e);
        }
    }
}