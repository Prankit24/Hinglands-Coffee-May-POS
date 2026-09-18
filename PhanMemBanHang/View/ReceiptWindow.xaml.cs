using Microsoft.Win32;
using PhanMemBanHang.Model;
using PhanMemBanHang.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PhanMemBanHang.View
{
    public partial class ReceiptWindow : Window
    {
        private readonly HoaDon hoaDon;
        private readonly List<GioHangItem> items;
        private readonly string tenNhanVien;
        private readonly string tenKhachHang;
        private readonly decimal giamGia;


        private const string BANK_CODE = "MB";
        private const string BANK_ACCOUNT = "0963453170";
        private const string BANK_ACCOUNT_NAME = "";


        public ReceiptWindow(
            HoaDon hoaDon,
            IEnumerable<GioHangItem> gioHang,
            string tenNhanVien,
            string tenKhachHang,
            decimal giamGia)
        {
            InitializeComponent();

            this.hoaDon =
                hoaDon ?? throw new ArgumentNullException(nameof(hoaDon));


            this.items = gioHang?
                .Select(x => new GioHangItem
                {
                    MaSP = x.MaSP,
                    TenSP = x.TenSP,
                    Size = x.Size,
                    SoLuong = x.SoLuong,
                    DonGia = x.DonGia
                })
                .ToList()
                ?? new List<GioHangItem>();


            this.tenNhanVien =
                string.IsNullOrWhiteSpace(tenNhanVien)
                    ? "Nhân viên"
                    : tenNhanVien;


            this.tenKhachHang =
                string.IsNullOrWhiteSpace(tenKhachHang)
                    ? "Khách vãng lai"
                    : tenKhachHang;


            this.giamGia = giamGia;


            HienThiHoaDon();
        }


        // ==========================================================
        // HIỂN THỊ HÓA ĐƠN
        // ==========================================================

        private void HienThiHoaDon()
        {
            txtMaHD.Text = $"#{hoaDon.MaHD}";

            txtThoiGian.Text =
                hoaDon.NgayLap.ToString("HH:mm dd/MM/yyyy");

            txtNhanVien.Text = tenNhanVien;

            txtKhachHang.Text = tenKhachHang;

            txtLoaiDon.Text = hoaDon.LoaiDon;

            txtPhuongThuc.Text = hoaDon.PhuongThucTT;


            itemsHoaDon.ItemsSource = items;


            txtTamTinh.Text =
                $"{hoaDon.TongTien:N0}đ";

            txtGiamGia.Text =
                $"{giamGia:N0}đ";

            txtThanhTien.Text =
                $"{hoaDon.ThanhTien:N0}đ";

            if (!string.IsNullOrWhiteSpace(hoaDon.PhuongThucTT)
        && hoaDon.PhuongThucTT.Trim()
            .Equals("Chuyển khoản", StringComparison.OrdinalIgnoreCase))
            {
                qrPanel.Visibility = Visibility.Visible;

                string noiDungChuyenKhoan = $"HD{hoaDon.MaHD}";

                LoadQRCode(
                    BANK_ACCOUNT,
                    BANK_ACCOUNT_NAME,
                    hoaDon.ThanhTien,
                    noiDungChuyenKhoan
                );
            }
            else
            {
                qrPanel.Visibility = Visibility.Collapsed;
            }
        }


        // ==========================================================
        // TẠO QR VIETQR
        // ==========================================================

        private void LoadQRCode(
       string soTaiKhoan,
       string tenTaiKhoan,
       decimal soTien,
       string noiDung)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(soTaiKhoan))
                {
                    qrPanel.Visibility = Visibility.Collapsed;
                    return;
                }

                string accountName = Uri.EscapeDataString(tenTaiKhoan);
                string addInfo = Uri.EscapeDataString(noiDung);
                string amount = Convert.ToInt64(soTien).ToString();

                string qrUrl =
                    $"https://img.vietqr.io/image/" +
                    $"{BANK_CODE}-{soTaiKhoan}-compact2.png" +
                    $"?amount={amount}" +
                    $"&addInfo={addInfo}" +
                    $"&accountName={accountName}";

                BitmapImage bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.UriSource = new Uri(qrUrl, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.EndInit();

                // KHÔNG bitmap.Freeze();

                imgQR.Source = bitmap;

                txtNoiDungCK.Text = $"Nội dung CK: {noiDung}";
                txtSoTienQR.Text = $"Số tiền: {soTien:N0}đ";
            }
            catch (Exception ex)
            {
                qrPanel.Visibility = Visibility.Collapsed;

                MessageBox.Show(
                    "Không tải được mã QR.\n\n" + ex.Message,
                    "Lỗi QR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }


        // ==========================================================
        // IN HÓA ĐƠN
        // ==========================================================

        private void InHoaDon_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                PrintDialog dialog =
                    new PrintDialog();


                if (dialog.ShowDialog() == true)
                {
                    dialog.PrintVisual(
     receiptBorder,
     $"Highlands Coffee - Hóa đơn #{hoaDon.MaHD}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể in hóa đơn: "
                    + ex.Message,
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ==========================================================
        // LƯU TXT
        // ==========================================================

        private void LuuTxt_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dialog =
                    new SaveFileDialog
                    {
                        Title = "Lưu hóa đơn",

                        Filter =
                            "Text file (*.txt)|*.txt",

                        FileName =
                            $"MayPOS_HoaDon_{hoaDon.MaHD}.txt"
                    };


                if (dialog.ShowDialog() != true)
                    return;


                StringBuilder sb =
                    new StringBuilder();


                sb.AppendLine(
                    "================ HINGLANDS COFFEE ================");

                sb.AppendLine(
                    "             HÓA ĐƠN BÁN HÀNG");

                sb.AppendLine(
                    $"Mã hóa đơn : #{hoaDon.MaHD}");

                sb.AppendLine(
                    $"Thời gian  : " +
                    $"{hoaDon.NgayLap:HH:mm dd/MM/yyyy}");

                sb.AppendLine(
                    $"Nhân viên  : {tenNhanVien}");

                sb.AppendLine(
                    $"Khách hàng : {tenKhachHang}");

                sb.AppendLine(
                    $"Loại đơn   : {hoaDon.LoaiDon}");

                sb.AppendLine(
                    $"Thanh toán : {hoaDon.PhuongThucTT}");

                sb.AppendLine(
                    "----------------------------------------");


                foreach (GioHangItem item in items)
                {
                    sb.AppendLine(
                        item.TenHienThi);

                    sb.AppendLine(
                        $"  {item.SoLuong} x " +
                        $"{item.DonGia:N0}đ = " +
                        $"{item.ThanhTien:N0}đ");
                }


                sb.AppendLine(
                    "----------------------------------------");


                sb.AppendLine(
                    $"Tạm tính        : " +
                    $"{hoaDon.TongTien:N0}đ");


                sb.AppendLine(
                    $"Giảm giá        : " +
                    $"{giamGia:N0}đ");


                sb.AppendLine(
                    $"TỔNG THANH TOÁN : " +
                    $"{hoaDon.ThanhTien:N0}đ");


                // Nếu chuyển khoản thì ghi thêm nội dung CK
                if (!string.IsNullOrWhiteSpace(
                        hoaDon.PhuongThucTT)
                    &&
                    hoaDon.PhuongThucTT
                        .Trim()
                        .Equals(
                            "Chuyển khoản",
                            StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine();

                    sb.AppendLine(
    $"Nội dung CK     : HD{hoaDon.MaHD}");
                }


                sb.AppendLine(
                    "========================================");


                sb.AppendLine(
                    "Cảm ơn quý khách và hẹn gặp lại!");


                File.WriteAllText(
                    dialog.FileName,
                    sb.ToString(),
                    Encoding.UTF8);


                MessageBox.Show(
                    "Đã lưu hóa đơn thành công.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể lưu hóa đơn: "
                    + ex.Message,
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ==========================================================
        // ĐÓNG
        // ==========================================================

        private void Dong_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}