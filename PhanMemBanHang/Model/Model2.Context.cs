using System;
using System.Linq;

namespace PhanMemBanHang.Model
{
    /// <summary>
    /// Portable replacement for the old Entity Framework / SQL Server context.
    /// Data is stored in MayPOS.db (SQLite) beside MayPOS.exe.
    /// </summary>
    public partial class HighlandsCoffeeDBEntities : IDisposable
    {
        public PortableDbSet<ChiTietHoaDon> ChiTietHoaDon { get; private set; }
        public PortableDbSet<HoaDon> HoaDon { get; private set; }
        public PortableDbSet<KhachHang> KhachHang { get; private set; }
        public PortableDbSet<LoaiSanPham> LoaiSanPham { get; private set; }
        public PortableDbSet<Pager> Pager { get; private set; }
        public PortableDbSet<SanPham> SanPham { get; private set; }
        public PortableDbSet<NhanVien> NhanVien { get; private set; }

        public HighlandsCoffeeDBEntities()
        {
            PortableDatabase.EnsureCreated();
            LoadAll();
        }

        private void LoadAll()
        {
            NhanVien = new PortableDbSet<NhanVien>(PortableDatabase.LoadNhanVien());
            KhachHang = new PortableDbSet<KhachHang>(PortableDatabase.LoadKhachHang());
            LoaiSanPham = new PortableDbSet<LoaiSanPham>(PortableDatabase.LoadLoaiSanPham());
            SanPham = new PortableDbSet<SanPham>(PortableDatabase.LoadSanPham());
            Pager = new PortableDbSet<Pager>(PortableDatabase.LoadPager());
            HoaDon = new PortableDbSet<HoaDon>(PortableDatabase.LoadHoaDon());
            ChiTietHoaDon = new PortableDbSet<ChiTietHoaDon>(PortableDatabase.LoadChiTietHoaDon());
            WireNavigationProperties();
        }

        public int SaveChanges()
        {
            AssignMissingIds();
            WireNavigationProperties();
            PortableDatabase.SaveAll(this);
            return 1;
        }

        private void AssignMissingIds()
        {
            int nextNV = NhanVien.Any() ? NhanVien.Max(x => x.MaNV) + 1 : 1;
            foreach (var item in NhanVien.Where(x => x.MaNV <= 0)) item.MaNV = nextNV++;

            int nextKH = KhachHang.Any() ? KhachHang.Max(x => x.MaKH) + 1 : 1;
            foreach (var item in KhachHang.Where(x => x.MaKH <= 0)) item.MaKH = nextKH++;

            int nextLoai = LoaiSanPham.Any() ? LoaiSanPham.Max(x => x.MaLoai) + 1 : 1;
            foreach (var item in LoaiSanPham.Where(x => x.MaLoai <= 0)) item.MaLoai = nextLoai++;

            int nextSP = SanPham.Any() ? SanPham.Max(x => x.MaSP) + 1 : 1;
            foreach (var item in SanPham.Where(x => x.MaSP <= 0)) item.MaSP = nextSP++;

            int nextPager = Pager.Any() ? Pager.Max(x => x.MaPager) + 1 : 1;
            foreach (var item in Pager.Where(x => x.MaPager <= 0)) item.MaPager = nextPager++;

            int nextHD = HoaDon.Any() ? HoaDon.Max(x => x.MaHD) + 1 : 1;
            foreach (var item in HoaDon.Where(x => x.MaHD <= 0)) item.MaHD = nextHD++;

            int nextCT = ChiTietHoaDon.Any() ? ChiTietHoaDon.Max(x => x.MaCT) + 1 : 1;
            foreach (var item in ChiTietHoaDon.Where(x => x.MaCT <= 0)) item.MaCT = nextCT++;
        }

        private void WireNavigationProperties()
        {
            foreach (var loai in LoaiSanPham)
                loai.SanPham.Clear();
            foreach (var sp in SanPham)
            {
                sp.ChiTietHoaDon.Clear();
                sp.LoaiSanPham = LoaiSanPham.FirstOrDefault(x => x.MaLoai == sp.MaLoai);
                sp.LoaiSanPham?.SanPham.Add(sp);
            }

            foreach (var nv in NhanVien)
                nv.HoaDon.Clear();
            foreach (var kh in KhachHang)
                kh.HoaDon.Clear();
            foreach (var pager in Pager)
                pager.HoaDon.Clear();
            foreach (var hd in HoaDon)
            {
                hd.ChiTietHoaDon.Clear();
                hd.NhanVien = NhanVien.FirstOrDefault(x => x.MaNV == hd.MaNV);
                hd.KhachHang = KhachHang.FirstOrDefault(x => x.MaKH == hd.MaKH);
                hd.Pager = Pager.FirstOrDefault(x => x.MaPager == hd.MaPager);
                hd.NhanVien?.HoaDon.Add(hd);
                hd.KhachHang?.HoaDon.Add(hd);
                hd.Pager?.HoaDon.Add(hd);
            }

            foreach (var ct in ChiTietHoaDon)
            {
                ct.HoaDon = HoaDon.FirstOrDefault(x => x.MaHD == ct.MaHD);
                ct.SanPham = SanPham.FirstOrDefault(x => x.MaSP == ct.MaSP);
                ct.HoaDon?.ChiTietHoaDon.Add(ct);
                ct.SanPham?.ChiTietHoaDon.Add(ct);
            }
        }

        public void Dispose()
        {
            // Connections are opened only for each read/write operation.
        }
    }
}
