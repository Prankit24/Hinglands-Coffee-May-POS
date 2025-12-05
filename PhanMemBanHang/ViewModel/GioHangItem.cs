using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemBanHang.ViewModel
{
    public class GioHangItem : BaseViewModel
    {
        private int soLuong;
        private decimal donGia;

        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public string Size { get; set; }

        public int SoLuong
        {
            get => soLuong;
            set
            {
                soLuong = value;
                OnPropertyChanged(nameof(SoLuong));
                OnPropertyChanged(nameof(ThanhTien));
            }
        }

        public decimal DonGia
        {
            get => donGia;
            set
            {
                donGia = value;
                OnPropertyChanged(nameof(DonGia));
                OnPropertyChanged(nameof(ThanhTien));
            }
        }

        public decimal ThanhTien => SoLuong * DonGia;

        public string TenHienThi => $"{TenSP} - {Size}";
    }
}
