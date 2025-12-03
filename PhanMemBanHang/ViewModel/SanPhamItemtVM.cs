using System.ComponentModel;
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

        // ---- SIZE ĐANG CHỌN TRÊN POS: S / M / L ----
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

        // Giá theo size hiện tại
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
