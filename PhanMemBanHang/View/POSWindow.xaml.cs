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
            cbLoaiDon.SelectedIndex = 0;

            dgSanPham.ItemsSource = vm.DanhSachSanPham;

            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.DanhSachSanPham))
                    dgSanPham.ItemsSource = vm.DanhSachSanPham;
            };
        }

        private void LocSanPham(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                int? maLoai = null;
                if (btn.Tag != null && int.TryParse(btn.Tag.ToString(), out int loai))
                    maLoai = loai;

                vm.LocTheoLoai(maLoai);
            }
        }

        private void ChonSize(object s, RoutedEventArgs e)
        {
            if (s is Button btn && btn.DataContext is SanPham sp)
            {
                string size = btn.Tag?.ToString();
                if (!string.IsNullOrEmpty(size))
                    vm.ThemVaoGioHang(sp, size);
            }
        }

        private void TimKhachHang_Click(object sender, RoutedEventArgs e)
        {
            bool found = vm.TimKhachHangTheoSDT(txtSDTKhach.Text);
            if (found)
            {
                txtTenKhach.Text = vm.KhachHangDangChon?.TenKH ?? string.Empty;
            }
            else if (!string.IsNullOrWhiteSpace(txtSDTKhach.Text))
            {
                txtTenKhach.Focus();
            }
        }

        private void ThemKhachHang_Click(object sender, RoutedEventArgs e)
        {
            if (vm.ThemKhachHangNhanh(txtTenKhach.Text, txtSDTKhach.Text, out string message))
            {
                txtTenKhach.Text = vm.KhachHangDangChon?.TenKH ?? txtTenKhach.Text;
                MessageBox.Show(message, "Khách hàng", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(message, "Khách hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ThanhToan(object sender, RoutedEventArgs e)
        {
            var phuongThuc = cbPhuongThucTT.SelectedItem?.ToString();
            var loaiDon = cbLoaiDon.SelectedItem?.ToString();

            var itemsSnapshot = vm.GioHang.Select(x => new GioHangItem
            {
                MaSP = x.MaSP,
                TenSP = x.TenSP,
                Size = x.Size,
                SoLuong = x.SoLuong,
                DonGia = x.DonGia
            }).ToList();

            var tenKhach = vm.KhachHangDangChon?.TenKH ?? "Khách vãng lai";
            var giamGia = vm.GiamGia;

            var hoaDon = vm.ThanhToan(phuongThuc, loaiDon);
            if (hoaDon == null) return;

            var receipt = new ReceiptWindow(hoaDon, itemsSnapshot, vm.TenNhanVien, tenKhach, giamGia)
            {
                Owner = this
            };
            receipt.ShowDialog();

            vm.HoanTatDonHang();
            txtTenKhach.Clear();
            txtSDTKhach.Clear();
        }

        private void HuyDon(object sender, RoutedEventArgs e)
        {
            vm.HuyDonHang();
            txtTenKhach.Clear();
            txtSDTKhach.Clear();
        }

        private void LuuTam(object sender, RoutedEventArgs e)
        {
            if (!vm.GioHang.Any())
            {
                MessageBox.Show("Giỏ hàng trống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Chức năng lưu tạm sẽ được cập nhật sau.\nBạn có thể hoàn tất hoặc hủy đơn hiện tại.",
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Xoa(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is GioHangItem item)
                vm.XoaKhoiGioHang(item);
        }

        private void ThemSPVaoGioHang(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is GioHangItem item)
                vm.TangSoLuong(item);
        }

        private void GiamSPKhoiGioHang(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is GioHangItem item)
                vm.GiamSoLuong(item);
        }

        private void QuayLai(object sender, RoutedEventArgs e)
        {
            if (vm.CheckTaiKhoan())
            {
                var main = new AdminWindow();
                main.Show();
                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            vm?.Dispose();
            base.OnClosed(e);
        }
    }
}
