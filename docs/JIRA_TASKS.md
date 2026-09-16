# Kế hoạch Quản lý Phân công Công việc trên JIRA

Tài liệu này được định dạng chuẩn Jira (Agile/Scrum), phân chia theo **Epics**, **User Stories**, **Tasks**, đi kèm **Story Points**, **Priority**, **Bảng Cassandra** và **Tiêu chí nghiệm thu (Acceptance Criteria)** cho từng thành viên.

---

## 📌 Tổng quan Phân bổ Jira (Backlog Overview)

| Epic Key | Tên Epic | Phụ trách chính | Tổng Story Points | Bảng Cassandra |
|---|---|:---:|:---:|---|
| **EPIC-1** | **M1: System Foundation & Astra DB Setup** | Thành viên 1 | 5 pts | `hotel_ks` (All) |
| **EPIC-2** | **M2 + M3: Hotel & Room Catalog Management** | Thành viên 1 | 13 pts | `hotels`, `rooms_by_hotel`, `rooms_by_hotel_status` |
| **EPIC-3** | **M4 + M5: Guest Management & Booking Core (Dual-Write)** | Thành viên 2 | 16 pts | `guests`, `bookings_by_guest`, `bookings_by_hotel_date` |
| **EPIC-4** | **M6 + M7 + M8: Query, Invoicing & Dashboard UI** | Thành viên 3 | 16 pts | `invoices_by_booking`, Dashboard & Layout |

---

## 👤 THÀNH VIÊN 1: Foundation & Quản lý Khách sạn / Phòng

### 🔹 [EPIC-1] System Foundation & Cassandra Setup (M1)

#### 🎫 Ticket: `TASK-101`
- **Issue Type**: `Task`
- **Summary**: Khởi tạo ASP.NET Core 9 MVC project và cài đặt CassandraCSharpDriver
- **Assignee**: Thành viên 1
- **Story Points**: `2` | **Priority**: `Highest`
- **Description**: 
  - Khởi tạo project trong `src/HotelManagement/`.
  - Cài đặt NuGet package `CassandraCSharpDriver` (version >= 3.20.0).
  - Cấu hình file `.gitignore` để không push bundle credentials nhạy cảm.
- **Acceptance Criteria (DoD)**:
  - [x] Project build thành công bằng lệnh `dotnet build`.
  - [x] Dependency `CassandraCSharpDriver` có mặt trong file `.csproj`.

#### 🎫 Ticket: `TASK-102`
- **Issue Type**: `Task`
- **Summary**: Xây dựng CassandraContext Singleton kết nối Astra DB
- **Assignee**: Thành viên 1
- **Story Points**: `3` | **Priority**: `Highest`
- **Description**:
  - Tạo `CassandraSettings.cs` đọc cấu hình Token, Bundle Zip, Keyspace từ `appsettings.json`.
  - Tạo `ICassandraContext.cs` và `CassandraContext.cs` quản lý vòng đời `ISession` duy nhất.
  - Đăng ký Singleton trong `Program.cs`.
- **Acceptance Criteria (DoD)**:
  - [x] Ứng dụng khởi động kết nối thành công tới Astra DB với keyspace `hotel_ks`.
  - [x] Chạy được câu lệnh test `SELECT release_version FROM system.local`.

---

### 🔹 [EPIC-2] Hotel & Room Catalog Management (M2 + M3)

#### 🎫 Ticket: `STORY-103`
- **Issue Type**: `Story`
- **Summary**: Quản lý danh sách và thông tin chi tiết Khách sạn (Module Hotel)
- **Assignee**: Thành viên 1
- **Story Points**: `5` | **Priority**: `High`
- **Cassandra Table**: `hotels` (PK: `hotel_id`)
- **Description**:
  - Tạo Model `Hotel.cs`.
  - Tạo `IHotelRepository.cs` và `HotelRepository.cs` (viết các câu lệnh `PreparedStatement`).
  - Tạo `HotelsController.cs` với 2 action: `Index` và `Details(string id)`.
  - Tạo View Razor giao diện hiển thị danh sách khách sạn dạng Card Bootstrap.
- **Acceptance Criteria (DoD)**:
  - [x] Trang `/Hotels` hiển thị đầy đủ danh sách khách sạn kèm số sao, địa chỉ, số điện thoại.
  - [x] Trang `/Hotels/Details/{id}` xem chi tiết 1 khách sạn theo đúng `hotel_id`.
  - [x] Truy vấn dùng `PreparedStatement` an toàn, không có lỗi injection.

#### 🎫 Ticket: `STORY-104`
- **Issue Type**: `Story`
- **Summary**: Quản lý và tra cứu phòng theo khách sạn (Module Room - All Rooms)
- **Assignee**: Thành viên 1
- **Story Points**: `3` | **Priority**: `High`
- **Cassandra Table**: `rooms_by_hotel` (PK: `hotel_id`, CK: `room_number`)
- **Description**:
  - Tạo Model `Room.cs`.
  - Tạo `IRoomRepository.cs` và `RoomRepository.cs` với hàm `GetRoomsByHotelAsync(string hotelId)`.
  - Hiển thị danh sách phòng ngay trên trang chi tiết khách sạn `/Hotels/Details/{id}` hoặc `/Rooms?hotelId={id}`.
- **Acceptance Criteria (DoD)**:
  - [x] Hiển thị danh sách phòng thuộc khách sạn được chọn sắp xếp theo `room_number`.
  - [x] Hiển thị giá tiền (`price_per_night`), loại phòng (`room_type`), trạng thái (`status`).

#### 🎫 Ticket: `STORY-105`
- **Issue Type**: `Story`
- **Summary**: Lọc danh sách phòng theo trạng thái không dùng ALLOW FILTERING
- **Assignee**: Thành viên 1
- **Story Points**: `5` | **Priority**: `High`
- **Cassandra Table**: `rooms_by_hotel_status` (PK: `hotel_id, status`, CK: `room_number`)
- **Description**:
  - Hiện thực hàm `GetRoomsByStatusAsync(string hotelId, string status)` trong `RoomRepository.cs`.
  - Bổ sung thanh lọc (Filter Dropdown: `AVAILABLE`, `OCCUPIED`, `MAINTENANCE`) trên giao diện phòng.
- **Acceptance Criteria (DoD)**:
  - [x] Truy vấn chính xác vào bảng `rooms_by_hotel_status` bằng câu lệnh `SELECT * FROM rooms_by_hotel_status WHERE hotel_id = ? AND status = ?`.
  - [x] **Cam kết**: Tuyệt đối không sử dụng từ khóa `ALLOW FILTERING`.

---

## 👤 THÀNH VIÊN 2: Khách hàng & Nghiệp vụ Booking Core

### 🔹 [EPIC-3] Guest Management & Booking Transaction (M4 + M5)

#### 🎫 Ticket: `STORY-201`
- **Issue Type**: `Story`
- **Summary**: Quản lý thông tin Khách hàng (Module Guest CRUD)
- **Assignee**: Thành viên 2
- **Story Points**: `5` | **Priority**: `High`
- **Cassandra Table**: `guests` (PK: `guest_id`)
- **Description**:
  - Tạo Model `Guest.cs`.
  - Tạo `IGuestRepository.cs` và `GuestRepository.cs` (GetAll, GetById, Create).
  - Tạo `GuestsController.cs` và các View `Index.cshtml`, `Create.cshtml`, `Details.cshtml`.
- **Acceptance Criteria (DoD)**:
  - [x] Trang `/Guests` xem được danh sách khách hàng.
  - [x] Trang `/Guests/Create` thêm mới được khách hàng vào bảng `guests`.
  - [x] Dữ liệu có validate số điện thoại, email, CCCD/CMND (`citizen_id`).

#### 🎫 Ticket: `STORY-202`
- **Issue Type**: `Story`
- **Summary**: Nghiệp vụ Đặt phòng với kỹ thuật Denormalization Dual-Write
- **Assignee**: Thành viên 2
- **Story Points**: `8` | **Priority**: `Highest`
- **Cassandra Tables**: `bookings_by_guest`, `bookings_by_hotel_date`, `rooms_by_hotel`, `rooms_by_hotel_status`
- **Description**:
  - Tạo Model `Booking.cs` và ViewModel `CreateBookingViewModel.cs`.
  - Tạo `IBookingRepository.cs` và `BookingRepository.cs`.
  - Khi người dùng Submit form đặt phòng:
    1. Sinh `booking_id` dạng `Guid (UUID)`.
    2. Sử dụng `BatchStatement` của Cassandra C# Driver để ghi đồng thời vào cả 2 bảng:
       - `bookings_by_guest` (phục vụ khách tra cứu)
       - `bookings_by_hotel_date` (phục vụ lễ tân khách sạn tra cứu)
    3. Cập nhật trạng thái phòng thành `OCCUPIED` trong `rooms_by_hotel` và `rooms_by_hotel_status`.
- **Acceptance Criteria (DoD)**:
  - [x] Đặt phòng thành công, dữ liệu xuất hiện đầy đủ ở cả 2 bảng booking.
  - [x] Không xảy ra trường hợp dữ liệu chỉ có ở 1 bảng (tính nhất quán của dual-write).
  - [x] Trạng thái phòng chuyển sang `OCCUPIED`.

#### 🎫 Ticket: `STORY-203`
- **Issue Type**: `Story`
- **Summary**: Hủy đặt phòng (Cancel Booking) và hoàn trả trạng thái phòng
- **Assignee**: Thành viên 2
- **Story Points**: `3` | **Priority**: `Medium`
- **Cassandra Tables**: `bookings_by_guest`, `bookings_by_hotel_date`, `rooms_by_hotel`
- **Description**:
  - Hiện thực tính năng Hủy đặt phòng trên giao diện chi tiết Booking.
  - Cập nhật trường `status = 'CANCELLED'` trên cả 2 bảng booking.
  - Trả lại trạng thái phòng thành `AVAILABLE`.
- **Acceptance Criteria (DoD)**:
  - [x] Trạng thái booking chuyển sang `CANCELLED` ở cả 2 bảng.
  - [x] Phòng quay lại trạng thái `AVAILABLE`.

---

## 👤 THÀNH VIÊN 3: Tra cứu, Hóa đơn & Giao diện Tích hợp

### 🔹 [EPIC-4] Booking Queries, Invoicing & Dashboard Integration (M6 + M7 + M8)

#### 🎫 Ticket: `STORY-301`
- **Issue Type**: `Story`
- **Summary**: Module Tra cứu Lịch sử đặt phòng (Query-First Demonstration)
- **Assignee**: Thành viên 3
- **Story Points**: `5` | **Priority**: `High`
- **Cassandra Tables**: `bookings_by_guest`, `bookings_by_hotel_date`
- **Description**:
  - Tạo Service `BookingQueryService.cs` và Controller `BookingQueriesController.cs`.
  - Viết 2 trang tra cứu chuyên biệt:
    1. `/BookingQueries/ByGuest?guestId={id}`: Truy vấn `SELECT * FROM bookings_by_guest WHERE guest_id = ?`.
    2. `/BookingQueries/ByHotelDate?hotelId={id}&date={yyyy-MM-dd}`: Truy vấn `SELECT * FROM bookings_by_hotel_date WHERE hotel_id = ? AND check_in_date = ?`.
- **Acceptance Criteria (DoD)**:
  - [x] Tìm kiếm theo khách hiển thị toàn bộ lịch sử các lần đặt phòng của khách đó.
  - [x] Tìm kiếm theo khách sạn + ngày nhận phòng hiển thị danh sách khách dự kiến check-in ngày hôm đó.
  - [x] **Cam kết**: Tốc độ phản hồi tức thì, không dùng ALLOW FILTERING.

#### 🎫 Ticket: `STORY-302`
- **Issue Type**: `Story`
- **Summary**: Quản lý và Xuất Hóa đơn theo Booking (Module Invoice)
- **Assignee**: Thành viên 3
- **Story Points**: `5` | **Priority**: `High`
- **Cassandra Table**: `invoices_by_booking` (PK: `booking_id`, CK: `invoice_id`)
- **Description**:
  - Tạo Model `Invoice.cs`.
  - Tạo `IInvoiceRepository.cs` và `InvoiceRepository.cs`.
  - Tạo `InvoicesController.cs` và View hiển thị chi tiết hóa đơn (tiền phòng, thuế VAT, phụ phí dịch vụ, trạng thái thanh toán `PAID`/`UNPAID`).
  - Cho phép xuất hoặc xem hóa đơn từ màn hình chi tiết Booking.
- **Acceptance Criteria (DoD)**:
  - [x] Truy vấn lấy hóa đơn theo đúng `booking_id`: `SELECT * FROM invoices_by_booking WHERE booking_id = ?`.
  - [x] Hiển thị tính toán chi tiết: `total_amount = room_charge + service_charge + tax`.

#### 🎫 Ticket: `STORY-303`
- **Issue Type**: `Story`
- **Summary**: Thiết kế Master Layout, Navigation Bar & Dashboard Thống kê (Module UI)
- **Assignee**: Thành viên 3
- **Story Points**: `6` | **Priority**: `Highest`
- **Description**:
  - Tinh chỉnh `Views/Shared/_Layout.cshtml`: Menu điều hướng chuẩn gồm Khách sạn, Phòng, Khách hàng, Đặt phòng, Tra cứu, Hóa đơn.
  - Xây dựng trang chủ [`Views/Home/Index.cshtml`](file:///d:/Study/2026/Nam04_HK1/noSQL_T.Binh/BaiTapNhom_Tuan05/src/HotelManagement/Views/Home/Index.cshtml) làm Dashboard tổng quan:
    - Widget thống kê trạng thái kết nối Astra DB (Online/Latency).
    - Thẻ thống kê số lượng Khách sạn, Phòng, Khách hàng, Lượt đặt phòng.
    - Bảng hoạt động đặt phòng gần đây.
- **Acceptance Criteria (DoD)**:
  - [x] Giao diện Bootstrap đẹp mắt, hiện đại, hỗ trợ Responsive trên Mobile/PC.
  - [x] Tất cả các đường link trên thanh Menu hoạt động mượt mà, không bị gãy link (404).

---

## 📅 Gợi ý Chia 2 Sprint trên Jira (2-Week Scrum)

```text
┌─────────────────────────────────────────────────────────────┐
│ SPRINT 1: Nền tảng, Khách sạn, Phòng & Khách hàng (21 pts)   │
├─────────────────────────────────────────────────────────────┤
│ • TASK-101: Khởi tạo project ASP.NET Core 9 (TV1 - 2 pts)   │
│ • TASK-102: Setup CassandraContext Singleton (TV1 - 3 pts)  │
│ • STORY-103: Module Quản lý Khách sạn (TV1 - 5 pts)         │
│ • STORY-104: Module Quản lý Phòng (TV1 - 3 pts)             │
│ • STORY-201: Module Quản lý Khách hàng (TV2 - 5 pts)        │
│ • STORY-303: Khung Master Layout & Dashboard (TV3 - 3 pts)  │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ SPRINT 2: Đặt phòng NoSQL, Tra cứu, Hóa đơn & Demo (27 pts) │
├─────────────────────────────────────────────────────────────┤
│ • STORY-105: Lọc phòng theo trạng thái NoSQL (TV1 - 5 pts)  │
│ • STORY-202: Tạo Booking Dual-Write Batch (TV2 - 8 pts)     │
│ • STORY-203: Hủy Booking & Update Room (TV2 - 3 pts)        │
│ • STORY-301: Tra cứu Query-First 2 chiều (TV3 - 5 pts)      │
│ • STORY-302: Lập và xem Hóa đơn (TV3 - 5 pts)               │
│ • STORY-303 (tiếp): Tích hợp hoàn thiện UI (TV3 - 1 pt)     │
└─────────────────────────────────────────────────────────────┘
```
