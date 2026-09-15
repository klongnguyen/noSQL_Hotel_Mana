# PLAN - Hotel Management with Cassandra & ASP.NET Core MVC

## 1. Mục tiêu
Xây dựng bài tập nhóm nhỏ quản lý khách sạn bằng **ASP.NET Core MVC (C#)** và **DataStax Astra DB / Apache Cassandra**.

Trọng tâm:
- Thiết kế dữ liệu theo tư duy **Query-First**.
- Sử dụng **Partition Key / Clustering Column** hợp lý.
- Hạn chế JOIN và ALLOW FILTERING.
- Thể hiện denormalization khi lưu Booking ở nhiều bảng phục vụ nhiều truy vấn.

## 2. Các module

| Module | Nội dung chính | Kết quả |
|---|---|---|
| M1. Cassandra Setup | Tạo Astra DB, keyspace, cấu hình kết nối C# | Kết nối thành công tới Cassandra |
| M2. Hotel | Xem danh sách và chi tiết khách sạn | Model, Repository, Controller, View |
| M3. Room | Xem phòng theo khách sạn và trạng thái | `rooms_by_hotel`, `rooms_by_hotel_status` |
| M4. Guest | Quản lý thông tin khách hàng | Guest CRUD cơ bản |
| M5. Booking | Tạo / hủy booking, cập nhật trạng thái phòng | Ghi đồng thời các bảng booking |
| M6. Booking Query | Tra cứu lịch sử booking theo khách và khách sạn/ngày | `bookings_by_guest`, `bookings_by_hotel_date` |
| M7. Invoice | Tạo và tra cứu hóa đơn | `invoices_by_booking` |
| M8. UI & Integration | Bootstrap, ghép module, kiểm thử | Website hoàn chỉnh |

## 3. Phân công gợi ý cho nhóm 3 người
- **Thành viên 1:** M1 + M2 + M3
- **Thành viên 2:** M4 + M5
- **Thành viên 3:** M6 + M7 + M8

Cả nhóm cùng thực hiện:
- Thiết kế schema CQL.
- Seed dữ liệu mẫu.
- Kiểm thử tích hợp.
- Báo cáo và demo.

## 4. Thứ tự thực hiện

```text
M1 Cassandra Setup
       ↓
M2 Hotel ── M3 Room ── M4 Guest
                     ↓
                  M5 Booking
                     ↓
              M6 Booking Query
                     ↓
                  M7 Invoice
                     ↓
               M8 Integration
```

## 5. Cấu trúc thư mục

```text
noSQL_Hotel_Mana/
├── src/
│   └── HotelManagement/
│       ├── Controllers/
│       ├── Models/
│       ├── ViewModels/
│       ├── Repositories/
│       ├── Services/
│       ├── Data/
│       ├── Views/
│       │   ├── Hotels/
│       │   ├── Rooms/
│       │   ├── Guests/
│       │   ├── Bookings/
│       │   └── Invoices/
│       └── wwwroot/
├── database/
├── docs/
├── screenshots/
└── .gitignore
```

## 6. Tiến độ đề xuất
- **Ngày 1-3:** M1 + thiết kế Query-First + schema CQL.
- **Ngày 4-9:** M2 đến M7.
- **Ngày 10-12:** M8 + tích hợp + sửa lỗi.
- **Ngày 13-14:** Test, README, báo cáo và demo.

## 7. Lưu ý
- Không commit Astra DB Application Token hoặc Secure Connect Bundle lên GitHub.
- Booking là module quan trọng nhất, cần đảm bảo dữ liệu được ghi đúng vào các bảng denormalized liên quan.
- Mỗi query chính nên có bảng Cassandra được thiết kế tối ưu riêng.
