using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PhanMemBanHang.Model;
using PhanMemBanHang.ViewModel;

namespace PhanMemBanHang.View
{
    public partial class POSWindow : Window
    {
        private readonly POSVM vm;
        public POSWindow() : this(1, "Admin", "Quản lý")
        {
        }

        public POSWindow(int maNV, string hoTen, string chucVu)
        {
            InitializeComponent();
            vm = new POSVM(maNV, hoTen, chucVu);
            DataContext = vm;

            cbPhuongThucTT.ItemsSource = vm.DSPhuongThucTT();
            cbPhuongThucTT.SelectedIndex = 0;

            cbLoaiDon.ItemsSource = vm.DSLoaiDon();
            cbLoaiDon.SelectedIndex = 1;
            dgSanPham.ItemsSource = vm.DanhSachSanPham;

            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.DanhSachSanPham))
                {
                    dgSanPham.ItemsSource = vm.DanhSachSanPham;
                }
            };
        }

        private void LocSanPham(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                int? maLoai = null;
                if (btn.Tag != null && int.TryParse(btn.Tag.ToString(), out int loai))
                {
                    maLoai = loai;
                }

                vm.LocTheoLoai(maLoai);
            }
        }
        private void ChonSize(object s, RoutedEventArgs e)
        {
            if (s is Button btn && btn.DataContext is SanPham sp)
            {
                string size = btn.Tag?.ToString();
                if (!string.IsNullOrEmpty(size))
                {
                    vm.ThemVaoGioHang(sp, size);
                }
            }
        }
        private void ThanhToan(object sender, RoutedEventArgs e)
        {
            vm.ThanhToan(cbPhuongThucTT.SelectedItem.ToString(), cbLoaiDon.SelectedItem.ToString());
        }

        private void HuyDon(object sender, RoutedEventArgs e)
        {
            vm.HuyDonHang();
        }

        private void LuuTam(object sender, RoutedEventArgs e)
        {
            if (!vm.GioHang.Any())
            {
                MessageBox.Show("Giỏ hàng trống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Chức năng lưu tạm sẽ được cập nhật sau \nBạn hãy huỷ bỏ rồi tạo đơn hàng khác nhé! \n" +
                "Chúc bạn ngày làm việc tốt lành", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Xoa(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is GioHangItem item)
            {
                vm.XoaKhoiGioHang(item);
            }
        }

        private void ThemSPVaoGioHang(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is GioHangItem item)
            {
                vm.TangSoLuong(item);
            }
        }

        private void GiamSPKhoiGioHang(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is GioHangItem item)
            {
                vm.GiamSoLuong(item);
            }
        }

        private void QuayLai(object sender, RoutedEventArgs e)
        {
            if (vm.CheckTaiKhoan())
            {
                AdminWindow main = new AdminWindow();
                main.Show();
                this.Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            vm?.Dispose();
            base.OnClosed(e);
        }

    
    }
}
