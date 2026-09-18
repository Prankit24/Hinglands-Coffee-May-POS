using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PhanMemBanHang.Model;

namespace PhanMemBanHang.ViewModel
{
    public class POSVM : BaseViewModel, IDisposable
    {
        private readonly HighlandsCoffeeDBEntities db;
        private ObservableCollection<GioHangItem> gioHang;
        private string tuKhoaTimKiem;
        private DateTime ngayBan;
        private string tenNhanVien;
        private string thongTinDonHang;
        private string gioHienTai;
        private decimal tamTinh;
        private decimal giamGia;
        private decimal tongThanhToan;
        private int maNV;
        private string chucVu;
        private int? maLoaiDangLoc;
        private List<SanPham> danhSachSanPham;

        private KhachHang khachHangDangChon;
        private string soDienThoaiKhach;
        private string thongTinKhachHang = "Khách vãng lai";

        public int MaNV
        {
            get => maNV;
            set { maNV = value; OnPropertyChanged(nameof(MaNV)); }
        }

        public string TenNhanVien
        {
            get => tenNhanVien;
            set { tenNhanVien = value; OnPropertyChanged(nameof(TenNhanVien)); }
        }

        public string ChucVu
        {
            get => chucVu;
            set { chucVu = value; OnPropertyChanged(nameof(ChucVu)); }
        }

        public List<SanPham> DanhSachSanPham
        {
            get => danhSachSanPham;
            set
            {
                danhSachSanPham = value;
                OnPropertyChanged(nameof(DanhSachSanPham));
            }
        }

        public KhachHang KhachHangDangChon
        {
            get => khachHangDangChon;
            private set
            {
                khachHangDangChon = value;
                OnPropertyChanged(nameof(KhachHangDangChon));
                OnPropertyChanged(nameof(DiemKhachHang));
            }
        }

        public string SoDienThoaiKhach
        {
            get => soDienThoaiKhach;
            set
            {
                soDienThoaiKhach = value;
                OnPropertyChanged(nameof(SoDienThoaiKhach));
            }
        }

        public string ThongTinKhachHang
        {
            get => thongTinKhachHang;
            private set
            {
                thongTinKhachHang = value;
                OnPropertyChanged(nameof(ThongTinKhachHang));
            }
        }

        public int DiemKhachHang => KhachHangDangChon?.DiemTichLuy ?? 0;

        public POSVM(int maNv, string hoTen, string chucVu)
        {
            db = new HighlandsCoffeeDBEntities();
            GioHang = new ObservableCollection<GioHangItem>();

            MaNV = maNv;
            TenNhanVien = hoTen;
            ChucVu = chucVu;

            HienThiMaDon();
            HienThiNgayGio();
            DanhSachSanPham = TimKiemSP();
        }

        public ObservableCollection<GioHangItem> GioHang
        {
            get => gioHang;
            set
            {
                gioHang = value;
                OnPropertyChanged(nameof(GioHang));
                TinhTongTien();
            }
        }

        public string caLam { get; set; }

        public string XacDinhCaLam()
        {
            TimeSpan gio = DateTime.Now.TimeOfDay;

            if (gio >= new TimeSpan(6, 0, 0) && gio <= new TimeSpan(11, 30, 0))
                return "Ca sáng 6:00 - 11:30";
            if (gio >= new TimeSpan(11, 30, 0) && gio <= new TimeSpan(17, 0, 0))
                return "Ca chiều 11:30 - 17:00";
            if (gio >= new TimeSpan(17, 0, 0) && gio <= new TimeSpan(23, 0, 0))
                return "Ca tối 17:00 - 23:00";
            return "Ngoài giờ làm việc";
        }

        public void HienThiNgayGio()
        {
            NgayBan = DateTime.Now;
            caLam = XacDinhCaLam();
            OnPropertyChanged(nameof(caLam));

            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += (s, e) => GioHienTai = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
            timer.Start();
        }

        public void HienThiMaDon()
        {
            var lastHD = db.HoaDon.OrderByDescending(x => x.MaHD).FirstOrDefault();
            int nextSoHD = lastHD != null ? lastHD.MaHD + 1 : 1;
            ThongTinDonHang = $"Đơn hàng #{nextSoHD}";
        }

        public string TuKhoaTimKiem
        {
            get => tuKhoaTimKiem;
            set
            {
                tuKhoaTimKiem = value;
                OnPropertyChanged(nameof(TuKhoaTimKiem));
                LocSanPham();
            }
        }

        public DateTime NgayBan
        {
            get => ngayBan;
            set
            {
                ngayBan = value;
                OnPropertyChanged(nameof(NgayBan));
            }
        }

        public string ThongTinDonHang
        {
            get => thongTinDonHang;
            set
            {
                thongTinDonHang = value;
                OnPropertyChanged(nameof(ThongTinDonHang));
            }
        }

        public string GioHienTai
        {
            get => gioHienTai;
            set
            {
                gioHienTai = value;
                OnPropertyChanged(nameof(GioHienTai));
            }
        }

        public decimal TamTinh
        {
            get => tamTinh;
            set
            {
                tamTinh = value;
                OnPropertyChanged(nameof(TamTinh));
            }
        }

        public decimal GiamGia
        {
            get => giamGia;
            set
            {
                giamGia = value;
                OnPropertyChanged(nameof(GiamGia));
                TinhTongTien();
            }
        }

        public decimal TongThanhToan
        {
            get => tongThanhToan;
            set
            {
                tongThanhToan = value;
                OnPropertyChanged(nameof(TongThanhToan));
            }
        }

        // Không phụ thuộc dữ liệu hóa đơn cũ nữa. Database mới vẫn có lựa chọn thanh toán.
        public List<string> DSPhuongThucTT()
        {
            return new List<string> { "Tiền mặt", "Chuyển khoản", "Thẻ" };
        }

        public List<string> DSLoaiDon()
        {
            return new List<string> { "Tại quán", "Mang về" };
        }

        public List<SanPham> TimKiemSP()
        {
            return db.SanPham.Where(x => x.HienThi).ToList();
        }

        public List<SanPham> TimKiemSP(string tuKhoa, int? maLoai)
        {
            tuKhoa = tuKhoa?.ToLower() ?? "";

            var query = db.SanPham.Where(x => x.HienThi && x.TenSP.ToLower().Contains(tuKhoa));

            if (maLoai != null)
                query = query.Where(x => x.MaLoai == maLoai);

            return query.ToList();
        }

        private void LocSanPham()
        {
            DanhSachSanPham = TimKiemSP(tuKhoaTimKiem, maLoaiDangLoc);
        }

        public void LocTheoLoai(int? maLoai)
        {
            maLoaiDangLoc = maLoai;
            LocSanPham();
        }

        public bool TimKhachHangTheoSDT(string soDienThoai)
        {
            var sdt = (soDienThoai ?? string.Empty).Trim();
            SoDienThoaiKhach = sdt;

            if (string.IsNullOrWhiteSpace(sdt))
            {
                BoChonKhachHang();
                return false;
            }

            var kh = db.KhachHang.FirstOrDefault(x => x.SDT == sdt);
            if (kh == null)
            {
                KhachHangDangChon = null;
                ThongTinKhachHang = "Chưa có khách hàng - có thể thêm nhanh";
                return false;
            }

            KhachHangDangChon = kh;
            ThongTinKhachHang = $"{kh.TenKH} • {kh.DiemTichLuy} điểm";
            return true;
        }

        public bool ThemKhachHangNhanh(string tenKhach, string soDienThoai, out string message)
        {
            var ten = (tenKhach ?? string.Empty).Trim();
            var sdt = (soDienThoai ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(ten))
            {
                message = "Vui lòng nhập tên khách hàng.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(sdt) || sdt.Length < 9)
            {
                message = "Số điện thoại không hợp lệ.";
                return false;
            }

            var khCu = db.KhachHang.FirstOrDefault(x => x.SDT == sdt);
            if (khCu != null)
            {
                KhachHangDangChon = khCu;
                SoDienThoaiKhach = khCu.SDT;
                ThongTinKhachHang = $"{khCu.TenKH} • {khCu.DiemTichLuy} điểm";
                message = "Số điện thoại đã tồn tại. Đã chọn khách hàng này.";
                return true;
            }

            try
            {
                var kh = new KhachHang
                {
                    TenKH = ten,
                    SDT = sdt,
                    Email = string.Empty,
                    DiaChi = string.Empty,
                    DiemTichLuy = 0
                };

                db.KhachHang.Add(kh);
                db.SaveChanges();

                KhachHangDangChon = kh;
                SoDienThoaiKhach = kh.SDT;
                ThongTinKhachHang = $"{kh.TenKH} • 0 điểm";
                message = "Thêm khách hàng thành công.";
                return true;
            }
            catch (Exception ex)
            {
                message = "Không thể thêm khách hàng: " + ex.Message;
                return false;
            }
        }

        public void BoChonKhachHang()
        {
            KhachHangDangChon = null;
            SoDienThoaiKhach = string.Empty;
            ThongTinKhachHang = "Khách vãng lai";
        }

        public void ThemVaoGioHang(SanPham sp, string size)
        {
            if (sp == null) return;
            decimal donGia = 0;

            switch (size)
            {
                case "S": donGia = sp.GiaSizeS ?? 0; break;
                case "M": donGia = sp.GiaSizeM ?? 0; break;
                case "L": donGia = sp.GiaSizeL ?? 0; break;
            }

            if (donGia == 0)
            {
                MessageBox.Show("Size này không có giá!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var item = GioHang.FirstOrDefault(x => x.MaSP == sp.MaSP && x.Size == size);

            if (item != null)
                item.SoLuong++;
            else
                GioHang.Add(new GioHangItem { MaSP = sp.MaSP, TenSP = sp.TenSP, Size = size, SoLuong = 1, DonGia = donGia });

            TinhTongTien();
        }

        public void TangSoLuong(GioHangItem item)
        {
            if (item != null)
            {
                item.SoLuong++;
                TinhTongTien();
            }
        }

        public void GiamSoLuong(GioHangItem item)
        {
            if (item == null) return;

            if (item.SoLuong > 1)
                item.SoLuong--;
            else
                GioHang.Remove(item);

            TinhTongTien();
        }

        public void XoaKhoiGioHang(GioHangItem item)
        {
            if (item != null)
            {
                GioHang.Remove(item);
                TinhTongTien();
            }
        }

        private void TinhTongTien()
        {
            TamTinh = GioHang?.Sum(m => m.ThanhTien) ?? 0;
            TongThanhToan = Math.Max(0, TamTinh - GiamGia);
        }

        private int LayMaKhachHangChoDon()
        {
            if (KhachHangDangChon != null)
                return KhachHangDangChon.MaKH;

            // Ưu tiên khách mã 1 nếu dữ liệu demo đã có.
            var khachVangLai = db.KhachHang.FirstOrDefault(x => x.MaKH == 1)
                              ?? db.KhachHang.FirstOrDefault(x => x.SDT == "0000000000");

            if (khachVangLai != null)
                return khachVangLai.MaKH;

            khachVangLai = new KhachHang
            {
                TenKH = "Khách vãng lai",
                SDT = "0000000000",
                Email = string.Empty,
                DiaChi = string.Empty,
                DiemTichLuy = 0
            };
            db.KhachHang.Add(khachVangLai);
            db.SaveChanges();
            return khachVangLai.MaKH;
        }

        private int LayMaPagerMacDinh()
        {
            var pager = db.Pager.FirstOrDefault();
            if (pager == null)
                throw new InvalidOperationException("Chưa có Pager trong cơ sở dữ liệu. Hãy chạy script dữ liệu demo trước.");
            return pager.MaPager;
        }

        public HoaDon ThanhToan(string phuongThucTT, string loaiDon)
        {
            if (!GioHang.Any())
            {
                MessageBox.Show("Chưa có sản phẩm trong giỏ hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            if (string.IsNullOrWhiteSpace(phuongThucTT) || string.IsNullOrWhiteSpace(loaiDon))
            {
                MessageBox.Show("Vui lòng chọn phương thức thanh toán và loại đơn.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            var result = MessageBox.Show(
                $"Tổng thanh toán: {TongThanhToan:N0} ₫\n" +
                $"Khách hàng: {(KhachHangDangChon?.TenKH ?? "Khách vãng lai")}\n\n" +
                "Xác nhận thanh toán?",
                "Thanh toán",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return null;

            try
            {
                int maPagerMacDinh = LayMaPagerMacDinh();
                int maKhach = LayMaKhachHangChoDon();

                var hoaDon = new HoaDon
                {
                    NgayLap = DateTime.Now,
                    ThoiGianGoi = DateTime.Now,
                    MaNV = MaNV,
                    MaPager = maPagerMacDinh,
                    MaKH = maKhach,
                    TongTien = TamTinh,
                    ThanhTien = TongThanhToan,
                    PhuongThucTT = phuongThucTT,
                    LoaiDon = loaiDon,
                    TrangThai = "Hoàn Thành"
                };

                db.HoaDon.Add(hoaDon);
                db.SaveChanges();

                foreach (var mon in GioHang)
                {
                    db.ChiTietHoaDon.Add(new ChiTietHoaDon
                    {
                        MaHD = hoaDon.MaHD,
                        MaSP = mon.MaSP,
                        SoLuong = mon.SoLuong,
                        DonGia = mon.DonGia,
                        ThanhTien = mon.ThanhTien,
                        Size = mon.Size
                    });
                }

                // Thành viên nhận 1 điểm cho mỗi 10.000đ thanh toán.
                if (KhachHangDangChon != null)
                {
                    int diemCong = (int)Math.Floor(TongThanhToan / 10000m);
                    KhachHangDangChon.DiemTichLuy += diemCong;
                    ThongTinKhachHang = $"{KhachHangDangChon.TenKH} • {KhachHangDangChon.DiemTichLuy} điểm";
                    OnPropertyChanged(nameof(DiemKhachHang));
                }

                db.SaveChanges();

                MessageBox.Show(
                    $"Thanh toán thành công!\nMã hóa đơn: {hoaDon.MaHD}" +
                    (KhachHangDangChon != null ? $"\nĐiểm hiện tại: {KhachHangDangChon.DiemTichLuy}" : string.Empty),
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                return hoaDon;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thanh toán: {ex.Message}\n\nChi tiết: {ex.InnerException?.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public void HoanTatDonHang()
        {
            GioHang.Clear();
            GiamGia = 0;
            BoChonKhachHang();
            HienThiMaDon();
            TinhTongTien();
        }

        public void HuyDonHang()
        {
            if (!GioHang.Any()) return;

            var result = MessageBox.Show("Bạn có chắc muốn hủy đơn hàng này?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                GioHang.Clear();
                GiamGia = 0;
                BoChonKhachHang();
                TinhTongTien();
                MessageBox.Show("Đã hủy đơn hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public bool CheckTaiKhoan()
        {
            if (ChucVu != "Quản lý")
            {
                MessageBox.Show("Bạn chỉ có thể ở trang Order này !",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}
