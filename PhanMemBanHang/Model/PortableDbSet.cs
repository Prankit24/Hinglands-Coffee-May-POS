using System;
using System.Collections.Generic;
using System.Linq;

namespace PhanMemBanHang.Model
{
    /// <summary>
    /// A tiny in-memory set used by the portable SQLite context. It intentionally
    /// exposes List/LINQ behavior so existing ViewModels can keep using Where,
    /// FirstOrDefault, Count, Add and Remove without Entity Framework.
    /// </summary>
    public class PortableDbSet<T> : List<T> where T : class
    {
        public PortableDbSet() { }
        public PortableDbSet(IEnumerable<T> items) : base(items ?? Enumerable.Empty<T>()) { }

        public T Find(params object[] keyValues)
        {
            if (keyValues == null || keyValues.Length == 0 || keyValues[0] == null)
                return null;

            int id;
            try { id = Convert.ToInt32(keyValues[0]); }
            catch { return null; }

            if (typeof(T) == typeof(NhanVien))
                return this.Cast<NhanVien>().FirstOrDefault(x => x.MaNV == id) as T;
            if (typeof(T) == typeof(KhachHang))
                return this.Cast<KhachHang>().FirstOrDefault(x => x.MaKH == id) as T;
            if (typeof(T) == typeof(LoaiSanPham))
                return this.Cast<LoaiSanPham>().FirstOrDefault(x => x.MaLoai == id) as T;
            if (typeof(T) == typeof(SanPham))
                return this.Cast<SanPham>().FirstOrDefault(x => x.MaSP == id) as T;
            if (typeof(T) == typeof(Pager))
                return this.Cast<Pager>().FirstOrDefault(x => x.MaPager == id) as T;
            if (typeof(T) == typeof(HoaDon))
                return this.Cast<HoaDon>().FirstOrDefault(x => x.MaHD == id) as T;
            if (typeof(T) == typeof(ChiTietHoaDon))
                return this.Cast<ChiTietHoaDon>().FirstOrDefault(x => x.MaCT == id) as T;

            return null;
        }
    }
}
