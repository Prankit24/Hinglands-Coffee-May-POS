using System;
using System.ComponentModel;
using System.IO;
using PhanMemBanHang.Model;

namespace PhanMemBanHang.ViewModel
{
    public class SanPhamItemVM : INotifyPropertyChanged
    {
        private readonly SanPham _sp;

        public int MaSP => _sp.MaSP;
        public string TenSP => _sp.TenSP;
        public int MaLoai => _sp.MaLoai;

        public decimal GiaSizeS => _sp.GiaSizeS ?? 0M;
        public decimal GiaSizeM => _sp.GiaSizeM ?? 0M;
        public decimal GiaSizeL => _sp.GiaSizeL ?? 0M;

        // ==== ĐƯỜNG DẪN ẢNH ĐẦY ĐỦ ====
        public string HinhAnhPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_sp.HinhAnh))
                    return null;

                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var relative = _sp.HinhAnh.Trim().Replace('/', '\\');
                var full = Path.Combine(baseDir, relative);
                // System.Diagnostics.Debug.WriteLine($"IMG: {full} - {File.Exists(full)}");

                return full;
            }
        }

        private string _sizeHienTai = "M";
        public string SizeHienTai
        {
            get => _sizeHienTai;
            set
            {
                if (_sizeHienTai != value)
                {
                    _sizeHienTai = value;
                    OnPropertyChanged(nameof(SizeHienTai));
                    OnPropertyChanged(nameof(GiaTheoSize));
                }
            }
        }

        public decimal GiaTheoSize
        {
            get
            {
                switch (SizeHienTai)
                {
                    case "S": return GiaSizeS;
                    case "L": return GiaSizeL;
                    default: return GiaSizeM;
                }
            }
        }

        public SanPhamItemVM(SanPham sanPham)
        {
            _sp = sanPham;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
