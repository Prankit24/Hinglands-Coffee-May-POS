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
        private decimal  tongThanhToan;
        private int maNV;
        private string chucVu;

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


        private List<SanPham> danhSachSanPham;
        public List<SanPham> DanhSachSanPham
        {
            get => danhSachSanPham;
            set
            {
                danhSachSanPham = value;
                OnPropertyChanged(nameof(DanhSachSanPham));
            }
        }
        private int? maLoaiDangLoc;

     

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

        public String caLam { get; set; }
        public String XacDinhCaLam()
        {
            TimeSpan gio = DateTime.Now.TimeOfDay;

            if( gio >= new TimeSpan(6, 0, 0) && gio <= new TimeSpan(11, 30, 00))
                return "Ca sáng 6:00 - 11:30";
            if (gio >= new TimeSpan(11, 30, 0) && gio <= new TimeSpan(17, 0, 0))
                return "Ca chiều 11:30 - 17:00";
            if (gio >= new TimeSpan(17, 0, 0) && gio <= new TimeSpan(23, 0, 0))
                return "Ca tối 17:00 - 23:00";
            return "Bạn đi làm giờ này làm gì ?";
        }
        public void HienThiNgayGio()
        {
            NgayBan = DateTime.Now;
            caLam = XacDinhCaLam();
            OnPropertyChanged(nameof(caLam));

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
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
            get =>  tongThanhToan;
            set
            {
                 tongThanhToan = value;
                OnPropertyChanged(nameof(TongThanhToan));
            }
        }

        public List<string> DSPhuongThucTT()
        {
            var list = db.HoaDon.Select(x => x.PhuongThucTT).Distinct().ToList();
            return list;
        }

        public List<string> DSLoaiDon()
        {
            var list = db.HoaDon.Select(x => x.LoaiDon).Distinct().ToList();
            return list;
        }
        public List<SanPham> TimKiemSP()
        {
            return db.SanPham.ToList();
        }

        public List<SanPham> TimKiemSP(string tuKhoa, int? maLoai)
        {
            tuKhoa = tuKhoa?.ToLower() ?? "";

            var query = db.SanPham.Where(x => x.TenSP.ToLower().Contains(tuKhoa));

            if (maLoai != null)
            {
                query = query.Where(x => x.MaLoai == maLoai);
            }

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
            {
                item.SoLuong++;
            }
            else
            {
                GioHang.Add(new GioHangItem{MaSP = sp.MaSP, TenSP = sp.TenSP,Size = size,SoLuong = 1, DonGia = donGia });
            }
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
            if (item != null)
            {
                if (item.SoLuong > 1)
                {
                    item.SoLuong--;
                }
                else
                {
                    GioHang.Remove(item);
                }
                TinhTongTien();
            }
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
            TamTinh = GioHang.Sum(m => m.ThanhTien);
            tongThanhToan = TamTinh - GiamGia;
        }

        public void ThanhToan(string phuongThucTT, string loaiDon)
        {
            if (!GioHang.Any())
            {
                MessageBox.Show("Chưa có sản phẩm trong giỏ hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Tổng thanh toán: {TongThanhToan:N0} ₫\n\nXác nhận thanh toán?",
                "Thanh toán",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                int maPagerMacDinh = 1;
                int maKhachVangLai = 1;

                var hoaDon = new HoaDon
                {
                    NgayLap = DateTime.Now,
                    ThoiGianGoi = DateTime.Now,
                    MaNV = this.MaNV,
                    MaPager = maPagerMacDinh,
                    MaKH = maKhachVangLai,
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
                db.SaveChanges();

                MessageBox.Show($"Thanh toán thành công!\nMã hóa đơn: {hoaDon.MaHD}",
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                GioHang.Clear();
                GiamGia = 0;
                HienThiMaDon();
                TinhTongTien();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thanh toán: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void HuyDonHang()
        {
            if (GioHang.Any())
            {
                var result = MessageBox.Show("Bạn có chắc muốn hủy đơn hàng này?", "Xác nhận",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    GioHang.Clear();
                    GiamGia = 0;
                    TinhTongTien();
                    MessageBox.Show("Đã hủy đơn hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
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