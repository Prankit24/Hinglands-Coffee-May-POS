using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PhanMemBanHang.Model;

namespace PhanMemBanHang.ViewModel
{
    public class QLSanPhamVM : BaseViewModel, IDisposable
    {
        private readonly HighlandsCoffeeDBEntities _db = new HighlandsCoffeeDBEntities();

        // Danh sách
        private ObservableCollection<SanPham> _danhSachSanPham;
        public ObservableCollection<SanPham> DanhSachSanPham
        {
            get => _danhSachSanPham;
            set => SetProperty(ref _danhSachSanPham, value);
        }

        private ObservableCollection<LoaiSanPham> _danhSachLoaiSP;
        public ObservableCollection<LoaiSanPham> DanhSachLoaiSP
        {
            get => _danhSachLoaiSP;
            set => SetProperty(ref _danhSachLoaiSP, value);
        }

        // Selection
        private SanPham _sanPhamDangChon;
        public SanPham SanPhamDangChon
        {
            get => _sanPhamDangChon;
            set => SetProperty(ref _sanPhamDangChon, value);
        }

        // Filter
        private string _tuKhoaTimKiem;
        public string TuKhoaTimKiem
        {
            get => _tuKhoaTimKiem;
            set
            {
                SetProperty(ref _tuKhoaTimKiem, value);
                LocSanPham();
            }
        }

        private int? _loaiSPLoc;
        public int? LoaiSPLoc
        {
            get => _loaiSPLoc;
            set
            {
                SetProperty(ref _loaiSPLoc, value);
                LocSanPham();
            }
        }

        private int _tongSoSanPham;
        public int TongSoSanPham
        {
            get => _tongSoSanPham;
            set => SetProperty(ref _tongSoSanPham, value);
        }

        // Form fields
        private int _maSP;
        public int MaSP
        {
            get => _maSP;
            set => SetProperty(ref _maSP, value);
        }

        private string _tenSP;
        public string TenSP
        {
            get => _tenSP;
            set => SetProperty(ref _tenSP, value);
        }

        private int? _maLoaiSP;
        public int? MaLoaiSP
        {
            get => _maLoaiSP;
            set => SetProperty(ref _maLoaiSP, value);
        }

        private decimal? _giaSizeS;
        public decimal? GiaSizeS
        {
            get => _giaSizeS;
            set => SetProperty(ref _giaSizeS, value);
        }

        private decimal? _giaSizeM;
        public decimal? GiaSizeM
        {
            get => _giaSizeM;
            set => SetProperty(ref _giaSizeM, value);
        }

        private decimal? _giaSizeL;
        public decimal? GiaSizeL
        {
            get => _giaSizeL;
            set => SetProperty(ref _giaSizeL, value);
        }

        private int? _soLuongTon;
        public int? SoLuongTon
        {
            get => _soLuongTon;
            set => SetProperty(ref _soLuongTon, value);
        }

        private bool _trangThai = true;
        public bool TrangThai
        {
            get => _trangThai;
            set => SetProperty(ref _trangThai, value);
        }

        private string _hinhAnh;
        public string HinhAnh
        {
            get => _hinhAnh;
            set => SetProperty(ref _hinhAnh, value);
        }

        public QLSanPhamVM()
        {
            LoadLoaiSP();
            LoadSanPham();
        }

        private void LoadLoaiSP()
        {
            var list = _db.LoaiSanPham.ToList();

            // Thêm "Tất cả" cho filter
            var all = new LoaiSanPham { MaLoai = 0, TenLoai = "Tất cả loại"};
            list.Insert(0, all);
            LoaiSPLoc = 0;
            DanhSachLoaiSP = new ObservableCollection<LoaiSanPham>(list);
        }

        private void LoadSanPham()
        {
            var list = _db.SanPham
                .Where(sp => sp.HienThi == true)
                .OrderByDescending(sp => sp.MaSP)
                .ToList();

            DanhSachSanPham = new ObservableCollection<SanPham>(list);
            TongSoSanPham = DanhSachSanPham.Count;
        }

        private void LocSanPham()
        {
            var query = _db.SanPham.Where(sp => sp.HienThi == true);

            if (LoaiSPLoc.HasValue && LoaiSPLoc.Value > 0)
            {
                int maLoai = LoaiSPLoc.Value;
                query = query.Where(sp => sp.MaLoai == maLoai);
            }

            if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
            {
                string kw = TuKhoaTimKiem.ToLower();
                query = query.Where(sp => sp.TenSP.ToLower().Contains(kw));
            }

            var list = query.OrderByDescending(sp => sp.MaSP).ToList();
            DanhSachSanPham = new ObservableCollection<SanPham>(list);
            TongSoSanPham = DanhSachSanPham.Count;
        }

        public void ChonSanPham(SanPham sp)
        {
            if (sp == null) return;

            MaSP = sp.MaSP;
            TenSP = sp.TenSP;
            MaLoaiSP = sp.MaLoai;
            GiaSizeS = sp.GiaSizeS;
            GiaSizeM = sp.GiaSizeM;
            GiaSizeL = sp.GiaSizeL;
            TrangThai = sp.TrangThai;
            HinhAnh = sp.HinhAnh;
            SoLuongTon = 100;
        }

        public void ThemSanPham()
        {
            try
            {
                var sp = new SanPham
                {
                    TenSP = TenSP.Trim(),
                    MaLoai = MaLoaiSP.Value,
                    GiaSizeS = GiaSizeS,
                    GiaSizeM = GiaSizeM,
                    GiaSizeL = GiaSizeL,
                    TrangThai = TrangThai,
                    HienThi = true,
                    HinhAnh = HinhAnh,
                };

                _db.SanPham.Add(sp);
                _db.SaveChanges();
                LoadSanPham();
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SuaSanPham()
        {
            try
            {
                var sp = _db.SanPham.FirstOrDefault(x => x.MaSP == MaSP);
                if (sp == null) return;

                sp.TenSP = TenSP?.Trim();
                if (MaLoaiSP.HasValue)
                    sp.MaLoai = MaLoaiSP.Value;

                sp.GiaSizeS = GiaSizeS;
                sp.GiaSizeM = GiaSizeM;
                sp.GiaSizeL = GiaSizeL;
                sp.TrangThai = TrangThai;
                sp.HinhAnh = HinhAnh;

                _db.SaveChanges();
                LoadSanPham();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void XoaSanPham()
        {
            try
            {
                var sp = _db.SanPham.FirstOrDefault(x => x.MaSP == MaSP);
                if (sp == null) return;

                sp.HienThi = false;
                _db.SaveChanges();
                LoadSanPham();
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LamMoi()
        {
            MaSP = 0;
            TenSP = string.Empty;
            MaLoaiSP = null;
            GiaSizeS = GiaSizeM = GiaSizeL = null;
            SoLuongTon = null;
            TrangThai = true;
            HinhAnh = string.Empty;
            SanPhamDangChon = null;
        }

        public bool KiemTraHopLe(out string thongBao)
        {
            if (string.IsNullOrWhiteSpace(TenSP))
            {
                thongBao = "Vui lòng nhập tên sản phẩm!";
                return false;
            }

            if (!MaLoaiSP.HasValue || MaLoaiSP.Value == 0)
            {
                thongBao = "Vui lòng chọn loại sản phẩm!";
                return false;
            }

            if (!GiaSizeS.HasValue && !GiaSizeM.HasValue && !GiaSizeL.HasValue)
            {
                thongBao = "Vui lòng nhập ít nhất một mức giá!";
                return false;
            }

            if ((GiaSizeS.HasValue && GiaSizeS.Value < 0) ||
                (GiaSizeM.HasValue && GiaSizeM.Value < 0) ||
                (GiaSizeL.HasValue && GiaSizeL.Value < 0))
            {
                thongBao = "Giá không được âm!";
                return false;
            }

            thongBao = string.Empty;
            return true;
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}