using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PhanMemBanHang.Model;

namespace PhanMemBanHang.ViewModel
{
    public class QLSanPhamVM : BaseViewModel, IDisposable
    {
        private readonly HighlandsCoffeeDBEntities db;


        private ObservableCollection<SanPham> danhSachSanPham;
        public ObservableCollection<SanPham> DanhSachSanPham
        {
            get => danhSachSanPham;
            set => SetProperty(ref danhSachSanPham, value);
        }


        private ObservableCollection<LoaiSanPham> danhSachLoaiSP;
        public ObservableCollection<LoaiSanPham> DanhSachLoaiSP
        {
            get => danhSachLoaiSP;
            set => SetProperty(ref danhSachLoaiSP, value);
        }


        private ObservableCollection<LoaiSanPham> danhSachLoaiLoc;
        public ObservableCollection<LoaiSanPham> DanhSachLoaiLoc
        {
            get => danhSachLoaiLoc;
            set => SetProperty(ref danhSachLoaiLoc, value);
        }


        private SanPham sanPhamDangChon;
        public SanPham SanPhamDangChon
        {
            get => sanPhamDangChon;
            set => SetProperty(ref sanPhamDangChon, value);
        }


        private string tuKhoaTimKiem;
        public string TuKhoaTimKiem
        {
            get => tuKhoaTimKiem;
            set
            {
                if (SetProperty(ref tuKhoaTimKiem, value))
                {
                    LocSanPham();
                }
            }
        }

        private int loaiSPLoc;
        public int LoaiSPLoc
        {
            get => loaiSPLoc;
            set
            {
                if (SetProperty(ref loaiSPLoc, value))
                {
                    LocSanPham();
                }
            }
        }

        private int tongSoSanPham;
        public int TongSoSanPham
        {
            get => tongSoSanPham;
            set => SetProperty(ref tongSoSanPham, value);
        }

        public QLSanPhamVM()
        {
            db = new HighlandsCoffeeDBEntities();

            LoadLoaiSP();
            LoadSanPham();
            LamMoi();   
        }
        private void LoadLoaiSP()
        {
            var list = db.LoaiSanPham.ToList();

            DanhSachLoaiSP = new ObservableCollection<LoaiSanPham>(list);

            var listLoc = list.ToList();
            listLoc.Insert(0, new LoaiSanPham
            {
                MaLoai = 0,
                TenLoai = "Tất cả loại"
            });

            DanhSachLoaiLoc = new ObservableCollection<LoaiSanPham>(listLoc);
            LoaiSPLoc = 0;
        }

        private void LoadSanPham()
        {
            var list = db.SanPham
                .Where(x => x.HienThi == true)
                .OrderByDescending(x => x.MaSP)
                .ToList();

            DanhSachSanPham = new ObservableCollection<SanPham>(list);
            TongSoSanPham = DanhSachSanPham.Count;
        }

        private void LocSanPham()
        {
            var query = db.SanPham.Where(x => x.HienThi == true);

            if (LoaiSPLoc > 0)
            {
                query = query.Where(x => x.MaLoai == LoaiSPLoc);
            }

            if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
            {
                var kw = TuKhoaTimKiem.ToLower();
                query = query.Where(x => x.TenSP.ToLower().Contains(kw));
            }

            var list = query
                .OrderByDescending(x => x.MaSP)
                .ToList();

            DanhSachSanPham = new ObservableCollection<SanPham>(list);
            TongSoSanPham = DanhSachSanPham.Count;
        }

        public bool KiemTraNoiDung(SanPham sp, out string err)
        {
            if (sp == null)
            {
                err = "Không có dữ liệu sản phẩm.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(sp.TenSP))
            {
                err = "Vui lòng nhập tên sản phẩm!";
                return false;
            }

            if (sp.MaLoai <= 0)
            {
                err = "Vui lòng chọn loại sản phẩm!";
                return false;
            }

            if (sp.GiaSizeS == null &&
                sp.GiaSizeM == null &&
                sp.GiaSizeL == null)
            {
                err = "Vui lòng nhập ít nhất một mức giá!";
                return false;
            }

            if ((sp.GiaSizeS ?? 0) < 0 ||
                (sp.GiaSizeM ?? 0) < 0 ||
                (sp.GiaSizeL ?? 0) < 0)
            {
                err = "Giá không được âm!";
                return false;
            }

            err = "";
            return true;
        }


        public void ThemSanPham()
        {
            try
            {
                if (!KiemTraNoiDung(SanPhamDangChon, out string err))
                {
                    MessageBox.Show(err, "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var sp = new SanPham
                {
                    TenSP    = SanPhamDangChon.TenSP?.Trim(),
                    MaLoai   = SanPhamDangChon.MaLoai,
                    GiaSizeS = SanPhamDangChon.GiaSizeS,
                    GiaSizeM = SanPhamDangChon.GiaSizeM,
                    GiaSizeL = SanPhamDangChon.GiaSizeL,
                    TrangThai = SanPhamDangChon.TrangThai,
                    HienThi   = SanPhamDangChon.HienThi,
                    HinhAnh   = SanPhamDangChon.HinhAnh ?? string.Empty,
                };

                db.SanPham.Add(sp);
                var result = db.SaveChanges();

                if (result > 0)
                {
                    MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadSanPham();
                    LamMoi(); 
                }
                else
                {
                    MessageBox.Show("Thêm sản phẩm thất bại!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm sản phẩm: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SuaSanPham()
        {
            try
            {
                if (SanPhamDangChon == null || SanPhamDangChon.MaSP == 0)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!KiemTraNoiDung(SanPhamDangChon, out string err))
                {
                    MessageBox.Show(err, "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var sp = db.SanPham.FirstOrDefault(x => x.MaSP == SanPhamDangChon.MaSP);
                if (sp == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm trong cơ sở dữ liệu!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                sp.TenSP    = SanPhamDangChon.TenSP?.Trim();
                sp.MaLoai   = SanPhamDangChon.MaLoai;
                sp.GiaSizeS = SanPhamDangChon.GiaSizeS;
                sp.GiaSizeM = SanPhamDangChon.GiaSizeM;
                sp.GiaSizeL = SanPhamDangChon.GiaSizeL;
                sp.TrangThai = SanPhamDangChon.TrangThai;
                sp.HienThi   = SanPhamDangChon.HienThi;
                sp.HinhAnh   = SanPhamDangChon.HinhAnh ?? string.Empty;

                var result = db.SaveChanges();

                if (result > 0)
                {
                    MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadSanPham();
                }
                else
                {
                    MessageBox.Show("Cập nhật sản phẩm thất bại!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật sản phẩm: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void XoaSanPham()
        {
            try
            {
                if (SanPhamDangChon == null || SanPhamDangChon.MaSP == 0)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var resultConfirm = MessageBox.Show(
                    $"Bạn có chắc muốn xóa sản phẩm '{SanPhamDangChon.TenSP}'?\n\n" +
                    "Lưu ý: Thao tác này không thể hoàn tác!",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultConfirm != MessageBoxResult.Yes)
                    return;

                var sp = db.SanPham.FirstOrDefault(x => x.MaSP == SanPhamDangChon.MaSP);
                if (sp == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm trong cơ sở dữ liệu!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                db.SanPham.Remove(sp);
                var result = db.SaveChanges();

                if (result > 0)
                {
                    MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadSanPham();
                    LamMoi();
                    SanPhamDangChon = null;
                }
                else
                {
                    MessageBox.Show("Xóa sản phẩm thất bại!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi xóa sản phẩm: {ex.Message}\n\n" +
                    "Có thể sản phẩm này đang được dùng trong đơn hàng hoặc bảng khác.",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LamMoi()
        {
            TuKhoaTimKiem = string.Empty;
            LoaiSPLoc = 0;

            SanPhamDangChon = new SanPham
            {
                TrangThai = true,
                HienThi = true,
            };

            LocSanPham();
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}
