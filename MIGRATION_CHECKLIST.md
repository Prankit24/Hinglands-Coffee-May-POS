# Chuyển repo hiện tại sang SQLite Portable

## Cách an toàn để giữ lịch sử Git
Copy các file/thư mục từ bản này đè lên repo MayPOS hiện tại, sau đó xóa các file SQL Server/EF cũ sau (nếu còn):

- `PhanMemBanHang/HighlandsCoffeeDB.mdf`
- `PhanMemBanHang/HighlandsCoffeeDB_log.ldf`
- `PhanMemBanHang/packages.config`
- `PhanMemBanHang/Model/Model2.edmx`
- `PhanMemBanHang/Model/Model2.edmx.diagram`
- `PhanMemBanHang/Model/Model2.tt`
- `PhanMemBanHang/Model/Model2.Context.tt`
- `PhanMemBanHang/Model/Model2.cs`
- `PhanMemBanHang/Model/Model2.Designer.cs`
- thư mục `packages/EntityFramework.6.2.0` nếu repo đang track nó

Không xóa các model POCO: `NhanVien.cs`, `SanPham.cs`, `HoaDon.cs`, ...

## Sau khi copy
1. Đóng Visual Studio.
2. Xóa `.vs`, `PhanMemBanHang/bin`, `PhanMemBanHang/obj`.
3. Mở lại `MayPOS.sln`.
4. Cho Visual Studio Restore NuGet (`System.Data.SQLite 2.0.4`).
5. Chọn Release -> Rebuild Solution.
6. Test `PhanMemBanHang/bin/Release/MayPOS.exe`.

## Git
Chạy:

```bash
git status
git add .
git commit -m "Migrate MayPOS database from SQL Server to portable SQLite"
git push
```

`Database/MayPOS.db` là DB seed demo và nên commit. Các DB phát sinh trong `bin/` sẽ bị `.gitignore` bỏ qua.

## Bảo mật
Bản này không còn connection string SQL Server hay mật khẩu DB trong `App.config`. Nếu credential SQL Server cũ từng được commit/push, nên đổi credential đó vì Git history có thể vẫn giữ bản cũ.
