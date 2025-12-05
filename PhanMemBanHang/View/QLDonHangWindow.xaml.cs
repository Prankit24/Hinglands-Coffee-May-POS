using System;
using System.Windows;
using System.Windows.Controls;
using PhanMemBanHang.Model;
using PhanMemBanHang.ViewModel;

namespace PhanMemBanHang.View
{
    public partial class QLDonHangWindow : Window
    {
        QLDonHangVM donhangVM = new QLDonHangVM();

        public QLDonHangWindow()
        {
            InitializeComponent();
            DataContext = donhangVM;
        }

        private void BtnQuayLai_Click(object sender, RoutedEventArgs e)
        {
            // Nếu muốn quay lại AdminWindow thì mở thêm:
            // var admin = new AdminWindow();
            // admin.Show();
            // rồi Close();
            this.Close();
        }

        // ========== DATA GRID ĐƠN HÀNG ==========

        private void DgDonHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ItemsSource đang là DanhSachDonHang (DonHangDTO), 
            // nên SelectedItem là DonHangDTO, không phải HoaDon
            if (sender is DataGrid grid && grid.SelectedItem is DonHangDTO dto)
            {
                donhangVM.DonHangDangChon = dto;
            }
        }

        // ========== BỘ LỌC (DATE / COMBO) – GỌI LỌC LẠI ==========

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            donhangVM.LocDonHang();
        }

        private void ComboBox_TrangThai_Changed(object sender, SelectionChangedEventArgs e)
        {
            donhangVM.LocDonHang();
        }

        private void ComboBox_PhuongThuc_Changed(object sender, SelectionChangedEventArgs e)
        {
            donhangVM.LocDonHang();
        }

        private void ComboBox_LoaiDon_Changed(object sender, SelectionChangedEventArgs e)
        {
            donhangVM.LocDonHang();
        }

        private void ComboBox_NhanVien_Changed(object sender, SelectionChangedEventArgs e)
        {
            donhangVM.LocDonHang();
        }

        private void ComboBox_SapXep_Changed(object sender, SelectionChangedEventArgs e)
        {
            donhangVM.ApDungSapXep();
        }

        // ========== CÁC NÚT BÊN BỘ LỌC (TÌM KIẾM / LÀM MỚI / EXCEL) ==========

        private void BtnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            donhangVM.LocDonHang();
        }

        private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            donhangVM.LamMoi();
        }

        private void BtnXuatExcel_Click(object sender, RoutedEventArgs e)
        {
            donhangVM.XuatExcel();
        }

        // ========== NÚT Ở KHU CHI TIẾT ĐƠN HÀNG ==========

        private void BtnCapNhatTrangThai_Click(object sender, RoutedEventArgs e)
        {
            donhangVM.CapNhatTrangThai();
        }

        private void BtnInPhieu_Click(object sender, RoutedEventArgs e)
        {
            donhangVM.InPhieu();
        }

        // ========== DISPOSE VM KHI ĐÓNG WINDOW ==========

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            donhangVM?.Dispose();
            base.OnClosing(e);
        }
    }
}
