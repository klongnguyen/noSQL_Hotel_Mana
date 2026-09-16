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

## 3. Phân công chi tiết cho nhóm 3 người (Làm việc song song độc lập)

### 👤 Thành viên 1: Quản lý Khách sạn & Phòng (M1 + M2 + M3)
- **Bảng phụ trách**: `hotels`, `rooms_by_hotel`, `rooms_by_hotel_status`.
- **Mã nguồn tự quản lý**:
  - `Data/` (`CassandraContext.cs`, cấu hình kết nối ban đầu M1)
  - `Models/Hotel.cs`, `Models/Room.cs`
  - `Repositories/IHotelRepository.cs`, `HotelRepository.cs`
  - `Repositories/IRoomRepository.cs`, `RoomRepository.cs`
  - `Controllers/HotelsController.cs`, `Controllers/RoomsController.cs`
  - `Views/Hotels/`, `Views/Rooms/`
  - `Extensions/HotelModuleExtensions.cs`
- **Nhiệm vụ trọng tâm**: Truy vấn phòng theo trạng thái bằng bảng `rooms_by_hotel_status` (Query-First, không dùng ALLOW FILTERING).

---

### 👤 Thành viên 2: Khách hàng & Nghiệp vụ Booking (M4 + M5)
- **Bảng phụ trách**: `guests`, `bookings_by_guest`, `bookings_by_hotel_date`.
- **Mã nguồn tự quản lý**:
  - `Models/Guest.cs`, `Models/Booking.cs`
  - `ViewModels/CreateBookingViewModel.cs`
  - `Repositories/IGuestRepository.cs`, `GuestRepository.cs`
  - `Repositories/IBookingRepository.cs`, `BookingRepository.cs`
  - `Controllers/GuestsController.cs`, `Controllers/BookingsController.cs`
  - `Views/Guests/`, `Views/Bookings/`
  - `Extensions/BookingModuleExtensions.cs`
- **Nhiệm vụ trọng tâm**: Thực hiện **Denormalization / Dual-Write** (ghi đồng thời vào `bookings_by_guest` và `bookings_by_hotel_date` bằng BatchStatement) và cập nhật trạng thái phòng.

---

### 👤 Thành viên 3: Tra cứu Booking, Hóa đơn & Giao diện (M6 + M7 + M8)
- **Bảng phụ trách**: `invoices_by_booking`, đọc dữ liệu `bookings_by_guest`, `bookings_by_hotel_date`.
- **Mã nguồn tự quản lý**:
  - `Models/Invoice.cs`
  - `ViewModels/BookingQueryViewModel.cs`, `ViewModels/DashboardViewModel.cs`
  - `Repositories/IInvoiceRepository.cs`, `InvoiceRepository.cs`
  - `Services/IBookingQueryService.cs`, `BookingQueryService.cs`
  - `Controllers/InvoicesController.cs`, `Controllers/BookingQueriesController.cs`, `Controllers/HomeController.cs`
  - `Views/Invoices/`, `Views/BookingQueries/`, `Views/Home/`, `Views/Shared/_Layout.cshtml`
  - `Extensions/InvoiceModuleExtensions.cs`
- **Nhiệm vụ trọng tâm**: Tra cứu lịch sử đặt phòng theo khách / ngày; lập và xem hóa đơn; thiết kế giao diện Bootstrap chung & Dashboard thống kê hệ thống.

---

## 4. Quy trình phối hợp Git song song

1. **Bước 0**: Tạo khung project `HotelManagement` + `CassandraContext` dùng chung đưa lên nhánh `main`.
2. **Bước 1**: Mỗi thành viên tạo branch riêng:
   - Thành viên 1: `git checkout -b feature/hotel-room`
   - Thành viên 2: `git checkout -b feature/guest-booking`
   - Thành viên 3: `git checkout -b feature/query-invoice-ui`
3. **Bước 2**: Từng người code trọn vẹn module của mình (do file và thư mục tách biệt hoàn toàn nên không bị conflict).
4. **Bước 3**: Lần lượt tạo Pull Request / Merge vào `main`, test tích hợp toàn diện.

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

## 7. Lưu ý
- Không commit Astra DB Application Token hoặc Secure Connect Bundle lên GitHub.
- Booking là module quan trọng nhất, cần đảm bảo dữ liệu được ghi đúng vào các bảng denormalized liên quan.
- Mỗi query chính nên có bảng Cassandra được thiết kế tối ưu riêng.
