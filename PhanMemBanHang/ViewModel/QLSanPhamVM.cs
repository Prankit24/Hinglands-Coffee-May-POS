using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PhanMemBanHang.Model;

namespace PhanMemBanHang.ViewModel
{
    public class QLSanPhamVM : BaseViewModel, IDisposable
    {
        private readonly HighlandsCoffeeDBEntities db = new HighlandsCoffeeDBEntities();

        // DANH SÁCH SẢN PHẨM
        private ObservableCollection<SanPham> danhSachSanPham;
        public ObservableCollection<SanPham> DanhSachSanPham
        {
            get => danhSachSanPham;
            set => SetProperty(ref danhSachSanPham, value);
        }

        // DANH SÁCH LOẠI SẢN PHẨM
        private ObservableCollection<LoaiSanPham> danhSachLoaiSP;
        public ObservableCollection<LoaiSanPham> DanhSachLoaiSP
        {
            get => danhSachLoaiSP;
            set => SetProperty(ref danhSachLoaiSP, value);
        }

        // SẢN PHẨM ĐANG CHỌN — MODEL TRỰC TIẾP
        private SanPham sanPhamDangChon;
        public SanPham SanPhamDangChon
        {
            get => sanPhamDangChon;
            set => SetProperty(ref sanPhamDangChon, value);
        }

        // LỌC
        private string tuKhoaTimKiem;
        public string TuKhoaTimKiem
        {
            get => tuKhoaTimKiem;
            set
            {
                SetProperty(ref tuKhoaTimKiem, value);
                LocSanPham();
            }
        }

        private int loaiSPLoc = 0;
        public int LoaiSPLoc
        {
            get => loaiSPLoc;
            set
            {
                SetProperty(ref loaiSPLoc, value);
                LocSanPham();
            }
        }

        public int TongSoSanPham { get; set; }

        public QLSanPhamVM()
        {
            LoadLoaiSP();
            LoadSanPham();
        }

        // -------------------------------------------------
        // LOAD LOẠI
        // -------------------------------------------------
        private void LoadLoaiSP()
        {
            var list = db.LoaiSanPham.ToList();
            list.Insert(0, new LoaiSanPham { MaLoai = 0, TenLoai = "Tất cả loại" });

            DanhSachLoaiSP = new ObservableCollection<LoaiSanPham>(list);
            LoaiSPLoc = 0;
        }

        // -------------------------------------------------
        // LOAD SẢN PHẨM
        // -------------------------------------------------
        private void LoadSanPham()
        {
            var list = db.SanPham
                .Where(x => x.HienThi == true)
                .OrderByDescending(x => x.MaSP)
                .ToList();

            DanhSachSanPham = new ObservableCollection<SanPham>(list);
            TongSoSanPham = DanhSachSanPham.Count;
        }

        // -------------------------------------------------
        // LỌC
        // -------------------------------------------------
        private void LocSanPham()
        {
            var query = db.SanPham.Where(x => x.HienThi == true);

            if (LoaiSPLoc > 0)
                query = query.Where(x => x.MaLoai == LoaiSPLoc);

            if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
            {
                var kw = TuKhoaTimKiem.ToLower();
                query = query.Where(x => x.TenSP.ToLower().Contains(kw));
            }

            DanhSachSanPham = new ObservableCollection<SanPham>(
                query.OrderByDescending(x => x.MaSP).ToList()
            );
            TongSoSanPham = DanhSachSanPham.Count;
        }

        // -------------------------------------------------
        // THÊM
        // -------------------------------------------------
        public bool KiemTraHopLe(out string err)
        {
            if (SanPhamDangChon == null)
            {
                err = "Không có dữ liệu sản phẩm.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(SanPhamDangChon.TenSP))
            {
                err = "Vui lòng nhập tên sản phẩm!";
                return false;
            }

            if (SanPhamDangChon.MaLoai <= 0)
            {
                err = "Vui lòng chọn loại sản phẩm!";
                return false;
            }

            if (SanPhamDangChon.GiaSizeS == null &&
                SanPhamDangChon.GiaSizeM == null &&
                SanPhamDangChon.GiaSizeL == null)
            {
                err = "Vui lòng nhập ít nhất một mức giá!";
                return false;
            }

            err = "";
            return true;
        }

        public void ThemSanPham()
        {
            var sp = new SanPham
            {
                TenSP = SanPhamDangChon.TenSP.Trim(),
                MaLoai = SanPhamDangChon.MaLoai,
                GiaSizeS = SanPhamDangChon.GiaSizeS,
                GiaSizeM = SanPhamDangChon.GiaSizeM,
                GiaSizeL = SanPhamDangChon.GiaSizeL,
                TrangThai = SanPhamDangChon.TrangThai,
                HienThi = true,
                HinhAnh = SanPhamDangChon.HinhAnh
            };

            db.SanPham.Add(sp);
            db.SaveChanges();

            LoadSanPham();
            LamMoi();
        }

        // -------------------------------------------------
        // SỬA
        // -------------------------------------------------
        public void SuaSanPham()
        {
            var sp = db.SanPham.FirstOrDefault(x => x.MaSP == SanPhamDangChon.MaSP);
            if (sp == null) return;

            sp.TenSP = SanPhamDangChon.TenSP;
            sp.MaLoai = SanPhamDangChon.MaLoai;
            sp.GiaSizeS = SanPhamDangChon.GiaSizeS;
            sp.GiaSizeM = SanPhamDangChon.GiaSizeM;
            sp.GiaSizeL = SanPhamDangChon.GiaSizeL;
            sp.TrangThai = SanPhamDangChon.TrangThai;
            sp.HinhAnh = SanPhamDangChon.HinhAnh;

            db.SaveChanges();
            LoadSanPham();
        }

        // -------------------------------------------------
        // XOÁ (HienThi = false)
        // -------------------------------------------------
        public void XoaSanPham()
        {
            var sp = db.SanPham.FirstOrDefault(x => x.MaSP == SanPhamDangChon.MaSP);
            if (sp == null) return;

            sp.HienThi = false;
            db.SaveChanges();

            LoadSanPham();
            LamMoi();
        }

        // -------------------------------------------------
        // LÀM MỚI
        // -------------------------------------------------
        public void LamMoi()
        {
            SanPhamDangChon = new SanPham
            {
                TrangThai = true,
                HienThi = true
            };
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}
