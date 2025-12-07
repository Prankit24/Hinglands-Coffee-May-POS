using System.ComponentModel;
using System.Windows;
using PhanMemBanHang.ViewModel;

namespace PhanMemBanHang.View
{
    public partial class QLDonHangWindow : Window
    {
        private readonly QLDonHangVM donhangVM = new QLDonHangVM();

        public QLDonHangWindow()
        {
            InitializeComponent();
            DataContext = donhangVM;
        }

        private void BtnQuayLai_Click(object sender, RoutedEventArgs e)
        {
            var admin = new AdminWindow();
            admin.Show();
            Close();
        }

        private void BtnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            donhangVM.LocDonHang();
        }

        private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            donhangVM.TuKhoaTimKiem = string.Empty;
            donhangVM.TuNgay = null;
            donhangVM.DenNgay = null;
            donhangVM.TrangThaiLoc = "Tất cả";
            donhangVM.TrangThaiThanhToanLoc = "Tất cả";
            donhangVM.PhuongThucTTLoc = "Tất cả";
            donhangVM.LoaiDonLoc = "Tất cả";
            donhangVM.NhanVienLoc = null;
            donhangVM.SapXepTheo = "Mới nhất";

            donhangVM.LocDonHang();
            donhangVM.TinhThongKe();
        }

        private void BtnXuatExcel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng xuất Excel đang được demo. Bạn có thể cài thêm EPPlus / ClosedXML để export thật.",
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnXuatHoaDonVAT_Click(object sender, RoutedEventArgs e)
        {
            if (donhangVM.DonHangDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"Xuất hóa đơn VAT cho đơn #{donhangVM.DonHangDangChon.MaHD}",
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnHoanTra_Click(object sender, RoutedEventArgs e)
        {
            if (donhangVM.DonHangDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"Hoàn trả đơn #{donhangVM.DonHangDangChon.MaHD} (demo).",
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnInPhieu_Click(object sender, RoutedEventArgs e)
        {
            if (donhangVM.DonHangDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show(
                $"In phiếu đơn #{donhangVM.DonHangDangChon.MaHD}",
                "In phiếu",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            donhangVM?.Dispose();
            base.OnClosing(e);
        }

      
    }
}
