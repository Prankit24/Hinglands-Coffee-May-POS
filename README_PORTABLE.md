# MayPOS Portable (SQLite)

Bản này đã bỏ phụ thuộc SQL Server / SSMS / LocalDB. Dữ liệu chạy bằng **SQLite** trong file `Database\MayPOS.db` đi kèm ứng dụng.

## Tài khoản demo
- Quản lý: `admin` / `123456`
- Nhân viên: `nhanvien` / `123456`

## Build lần đầu trên máy phát triển
1. Mở `MayPOS.sln` bằng Visual Studio 2022.
2. Chờ **NuGet Restore** tải package `System.Data.SQLite 2.0.4`.
3. Chọn `Release` + `Any CPU`.
4. `Build -> Rebuild Solution`.
5. Mở `PhanMemBanHang\bin\Release`.

Project đã có sẵn `Database\MayPOS.db` demo và file này được copy sang `bin\Release\Database`. Nếu DB bị xóa, app cũng có thể tự tạo lại. Không cần chạy file SQL.

## Tạo ZIP gửi HR
Sau khi build Release thành công, copy **toàn bộ** nội dung `PhanMemBanHang\bin\Release` vào một folder mới, ví dụ `MayPOS-Portable`.

Folder HR nhận phải có tối thiểu:
- `MayPOS.exe`
- `System.Data.SQLite.dll` (và file runtime SQLite mà NuGet đặt cạnh app, nếu có)
- thư mục `Image`
- thư mục `Database` có `MayPOS.db`
- các DLL mà Visual Studio copy ra Release

Nếu muốn reset dữ liệu demo, thay `Database\MayPOS.db` bằng bản seed trong source rồi build/copy lại.

## Tự kiểm tra trước khi gửi
1. Copy folder portable sang một thư mục khác (ví dụ Desktop).
2. Đóng Visual Studio.
3. Bấm `MayPOS.exe`.
4. Đăng nhập `admin / 123456`.
5. Tạo một đơn và đóng app.
6. Mở lại app, kiểm tra đơn vẫn còn -> SQLite đã lưu đúng.

## Git
Không commit dữ liệu phát sinh trong `bin/`, `obj/`, `.vs/`. File seed `PhanMemBanHang/Database/MayPOS.db` **nên commit** để bản demo luôn có dữ liệu ban đầu.

## Thay đổi chính
- Xóa connection string SQL Server và thông tin đăng nhập DB khỏi source.
- Không còn `EntityFramework.SqlServer`.
- Dùng `System.Data.SQLite` + `MayPOS.db`.
- Giữ API context cũ (`HighlandsCoffeeDBEntities`) để các ViewModel hiện tại thay đổi ít nhất.
- App tự tạo schema + dữ liệu demo khi chưa có DB.
