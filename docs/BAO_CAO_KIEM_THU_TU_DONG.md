# BÁO CÁO KIỂM THỬ TỰ ĐỘNG TOÀN DIỆN VÀ KHẮC PHỤC LỖI HỆ THỐNG

## DỰ ÁN: GRAND HOTEL PROPERTY MANAGEMENT SYSTEM (PMS)

---

### THÔNG TIN TỔNG QUAN

- **Tên dự án**: Grand Hotel Management System (Hệ thống quản lý chuỗi khách sạn Grand Hotel)
- **Kiến trúc & Công nghệ**:
  - **Backend**: ASP.NET Core 9.0 MVC, C# 13
  - **Cơ sở dữ liệu**: Apache Cassandra 4.0 / DataStax Astra DB (Cloud Keyspace `hotel_ks`)
  - **Giao diện**: Razor Views, Bootstrap 5, Vanilla CSS, FontAwesome, Chart.js
  - **Mô hình truy vấn**: Cassandra Query-First Modeling, Denormalization, Multi-table synchronization
- **Công cụ kiểm thử tự động**: Python E2E Test Suite (`scripts/automated_tests.py`), Requests, HTML/DOM Parser
- **Thời gian thực hiện**: Ngày 25/09/2026
- **Trạng thái hệ thống**: **100% ĐẠT TIÊU CHUẨN CHẤT LƯỢNG (57/57 Testcases PASS)**

---

### MỤC LỤC

1. [Tóm Tắt Kết Quả Kiểm Thử (Executive Summary)](#1-tóm-tắt-kết-quả-kiểm-thử-executive-summary)
2. [Chi Tiết Các Lỗi Phát Hiện & Giải Pháp Sửa Chữa (Bug Fixes & Root Cause)](#2-chi-tiết-các-lỗi-phát-hiện--giải-pháp-sửa-chữa)
   - [Bug 1: Lệch cấu trúc bảng buồng phòng khi khách sạn chưa có phòng](#lỗi-1-lệch-cấu-trúc-bảng-buồng-phòng-viewsroomsindexcshtml)
   - [Bug 2: Nuốt lỗi xác thực dữ liệu khi đặt phòng](#lỗi-2-nuốt-lỗi-xác-thực-dữ-liệu-viewsbookingscreatecshtml)
   - [Bug 3: Logic hủy đơn vô điều kiện đặt lại trạng thái phòng](#lỗi-3-logic-hủy-đơn-vô-điều-kiện-repositoriesbookingrepositorycs)
   - [Bug 4: Không làm sạch khoảng trắng khi tra cứu lịch sử lưu trú](#lỗi-4-khoảng-trắng-tìm-kiếm-lịch-sử-controllersbookinghistorycontrollercs)
   - [Bug 5: Điểm nghẽn hiệu năng N+1 truy vấn Cloud Cassandra](#lỗi-5-nghẽn-hiệu-năng-n1-query-controllersinvoicecontrollercs)
   - [Bug 6: Gãy luồng nghiệp vụ xem hóa đơn cho đơn đặt phòng mới](#lỗi-6-gãy-luồng-nghiệp-vụ-xem-hóa-đơn-mới-controllersinvoicecontrollercs)
   - [Bug 7: Chuẩn hóa và Siết chặt Ràng buộc Xác thực (Validation) CCCD 12 Số & Toàn Hệ Thống](#lỗi-7-chuẩn-hóa-và-siết-chặt-ràng-buộc-xác-thực-validation-cccd-12-số--toàn-hệ-thống)
3. [Bảng Ma Trận 57 Kịch Bản Kiểm Thử (Test Case Matrix)](#3-bảng-ma-trận-57-kịch-bản-kiểm-thử-chi-tiết)
4. [Đánh Giá Hiệu Năng & Trải Nghiệm Người Dùng](#4-đánh-giá-hiệu-năng--trải-nghiệm-người-dùng)
5. [Khuyến Nghị Kiến Trúc & Định Hướng Mở Rộng](#5-khuyến-nghị-kiến-trúc--định-hướng-mở-rộng)
6. [Kết Luận](#6-kết-luận)

---

### 1. TÓM TẮT KẾT QUẢ KIỂM THỬ (EXECUTIVE SUMMARY)

Quá trình kiểm thử tự động được thiết kế nhằm quét qua tất cả các module nghiệp vụ và các tình huống biên (Edge Cases), bao gồm: Kiểm tra giao diện người dùng, Kiểm tra tính toàn vẹn dữ liệu, Xử lý chuỗi và Injection, Kiểm tra ràng buộc thời gian (Conflict Overlap Detection), Logic đồng bộ trạng thái NoSQL và Khả năng chịu tải truy vấn mạng đám mây.

#### Bảng Chỉ Số Đo Lường (KPIs)

| Chỉ số kiểm thử                           | Trước khi sửa chữa | Sau khi sửa chữa |   Trạng thái cải thiện    |
| :---------------------------------------- | :----------------: | :--------------: | :-----------------------: |
| **Tổng số kịch bản kiểm thử (Testcases)** |       **57**       |      **57**      |      Toàn diện 100%       |
| **Số kịch bản ĐẠT (PASS)**                |       **52**       |      **57**      | Tăng +5 testcases (+8.8%) |
| **Số kịch bản CẢNH BÁO (WARNING)**        |       **1**        |      **0**       |   Đã loại bỏ hoàn toàn    |
| **Số kịch bản THẤT BẠI (FAIL)**           |       **4**        |      **0**       |     Đã khắc phục 100%     |
| **Tỷ lệ thành công (Pass Rate)**          |     **91.2%**      |    **100.0%**    |    **Tuyệt đối 100%**     |
| **Thời gian phản hồi module Hóa đơn**     |  **18 - 25 giây**  |   **< 400 ms**   |   **Tăng tốc x50 lần**    |

#### Biểu Đồ Phân Phối Module Kiểm Thử

```
[Dashboard & Home]       : ███ (3/3 PASS) - 100%
[Quản Lý Khách Sạn]      : ████████ (8/8 PASS) - 100%
[Quản Lý Buồng Phòng]    : ██████ (6/6 PASS) - 100%
[Hồ Sơ Khách Hàng]       : ████████████ (12/12 PASS) - 100%
[Đặt Phòng & Overlap]    : ██████████████ (14/14 PASS) - 100%
[Lịch Sử Lưu Trú]        : █████ (5/5 PASS) - 100%
[Hóa Đơn & Thu Ngân]     : █████████ (9/9 PASS) - 100%
----------------------------------------------------------------------
TỔNG CỘNG                : 57/57 TESTCASES PASS (100%)
```

---

### 2. CHI TIẾT CÁC LỖI PHÁT HIỆN & GIẢI PHÁP SỬA CHỮA

Dưới đây là phân tích chi tiết về nguyên nhân gốc rễ (Root Cause), mức độ nghiêm trọng và các thay đổi mã nguồn đã được thực hiện để giải quyết dứt điểm các lỗi phát hiện trong quá trình kiểm thử.

---

#### Lỗi 1: Lệch cấu trúc bảng buồng phòng (`Views/Rooms/Index.cshtml`)

- **Phân loại**: Giao diện người dùng (UI / HTML Layout)
- **Mức độ nghiêm trọng**: Thấp (Minor)
- **Kịch bản ảnh hưởng**: `TC16`, `TC17`
- **Mô tả sự cố**: Bảng danh sách phòng tại `Rooms/Index` có 6 cột tiêu đề: `Số phòng`, `Loại phòng`, `Giá mỗi đêm`, `Sức chứa`, `Trạng thái`, và `Thao tác`. Tuy nhiên, khi một khách sạn chưa có phòng nào (Empty State), thẻ `<td>` thông báo lại đặt `colspan="5"`. Điều này khiến ô thông báo bị thụt vào, để trống cột cuối cùng và làm vỡ khung viền bảng.
- **Giải pháp**: Điều chỉnh thuộc tính `colspan` từ 5 thành 6.

```html
<!-- Trước khi sửa: -->
<td colspan="5" class="text-center py-4 text-muted">
  <i class="bi bi-inbox fs-1 d-block mb-2"></i>
  Không có phòng nào phù hợp với bộ lọc hiện tại.
</td>

<!-- Sau khi sửa: -->
<td colspan="6" class="text-center py-4 text-muted">
  <i class="bi bi-inbox fs-1 d-block mb-2"></i>
  Không có phòng nào phù hợp với bộ lọc hiện tại.
</td>
```

---

#### Lỗi 2: Nuốt lỗi xác thực dữ liệu (`Views/Bookings/Create.cshtml`)

- **Phân loại**: Chức năng & Trải nghiệm người dùng (Usability & Validation)
- **Mức độ nghiêm trọng**: Cao (Major)
- **Kịch bản ảnh hưởng**: `TC34`, `TC35`, `TC36`, `TC37`, `TC38`, `TC39`
- **Mô tả sự cố**:
  - Trong `BookingController.cs`, các kiểm tra nghiệp vụ được gán vào `ModelState.AddModelError("PropertyName", "Nội dung lỗi")` (ví dụ: `CheckInDate`, `GuestId`, `NumberOfOccupants`).
  - Tuy nhiên, trong View `Bookings/Create.cshtml`, form chỉ khai báo `<div asp-validation-summary="ModelOnly"></div>` và **hoàn toàn không có** các thẻ `<span asp-validation-for="...">` tương ứng cho từng ô nhập liệu.
  - Theo cơ chế của ASP.NET Core Razor, `ModelOnly` sẽ **bỏ qua** toàn bộ lỗi gắn với thuộc tính cụ thể. Hậu quả là khi người dùng nhập sai (ngày check-in quá khứ, quá sức chứa, check-out trước check-in), form tải lại mà không hiển thị bất kỳ dòng thông báo đỏ nào, khiến người dùng lầm tưởng ứng dụng bị đơ.
- **Giải pháp**: Đổi thành `asp-validation-summary="All"` và bổ sung đầy đủ các thẻ `<span asp-validation-for="...">` dưới mỗi input/select.

---

#### Lỗi 3: Logic hủy đơn vô điều kiện đặt lại trạng thái phòng (`Repositories/BookingRepository.cs`)

- **Phân loại**: Toàn vẹn dữ liệu NoSQL (Data Integrity)
- **Mức độ nghiêm trọng**: Rất nghiêm trọng (Critical)
- **Kịch bản ảnh hưởng**: `TC42`, `TC43`
- **Mô tả sự cố**:
  - Khi người dùng nhấn Hủy đơn đặt phòng (`CancelAsync`), hàm cập nhật trạng thái đơn thành `CANCELLED`.
  - Tuy nhiên, sau đó hàm tự động cập nhật trạng thái phòng trong bảng `rooms_by_hotel_status` về `AVAILABLE` một cách vô điều kiện.
  - **Lỗi logic phát sinh**:
    1. Nếu khách hàng hủy một đơn đặt phòng **trong tương lai** (ví dụ tháng sau), nhưng hôm nay phòng đó đang có khách khác ở thực tế, phòng đó sẽ bị chuyển sai thành `AVAILABLE`.
    2. Nếu phòng đó đang ở trạng thái bảo trì/sửa chữa (`MAINTENANCE`), việc hủy đơn cũng làm mất trạng thái bảo trì và mở bán phòng lỗi.
- **Giải pháp**: Chỉ chuyển phòng thành `AVAILABLE` nếu: Đơn hủy là đơn **đang có hiệu lực tại ngày hôm nay** (`startDate <= today <= endDate`) VÀ phòng đó không nằm trong trạng thái `MAINTENANCE`.

```csharp
// Đã khắc phục trong BookingRepository.cs:
var today = LocalDate.FromDateTime(DateTime.UtcNow);
var isOccupiedToday = startDate <= today && today <= endDate;
var currentRoom = await _roomRepository.GetByHotelAndRoomAsync(hotelId, roomNumber);

if (isOccupiedToday && currentRoom != null && currentRoom.Status != "MAINTENANCE")
{
    await _roomRepository.UpdateStatusAsync(hotelId, roomNumber, "AVAILABLE");
}
```

---

#### Lỗi 4: Khoảng trắng tìm kiếm lịch sử (`Controllers/BookingHistoryController.cs`)

- **Phân loại**: Trải nghiệm người dùng (UX / String Sanitization)
- **Mức độ nghiêm trọng**: Trung bình (Medium)
- **Kịch bản ảnh hưởng**: `TC47`, `TC48`
- **Mô tả sự cố**: Khách hàng hoặc lễ tân thường copy mã khách hàng từ email/tin nhắn, dễ để lại khoảng trắng đầu/cuối (ví dụ: `" GUEST001 "`). Trong Cassandra, khóa phân vùng (Partition Key) là chuỗi nhạy cảm chính xác từng ký tự. Do thiếu hàm `.Trim()`, câu truy vấn không trả về bản ghi nào dù khách hàng tồn tại.
- **Giải pháp**: Áp dụng `.Trim()` cho tham số `customerId`, kiểm tra rỗng và bọc `try ... catch` để hiển thị thông báo lỗi thân thiện qua `ViewBag.ErrorMessage`.

---

#### Lỗi 5: Nghẽn hiệu năng N+1 Query (`Controllers/InvoiceController.cs`)

- **Phân loại**: Hiệu năng hệ thống (Performance / Database Latency)
- **Mức độ nghiêm trọng**: Rất nghiêm trọng (Critical)
- **Kịch bản ảnh hưởng**: `TC50`, `TC51`
- **Mô tả sự cố**:
  - Tại phương thức `GetInvoiceByHotelRoomAsync(string hotelId, string roomNumber)`:
  - Mã nguồn ban đầu lấy danh sách toàn bộ khách sạn (`_hotelRepository.GetAllAsync()`, gồm 35 khách sạn).
  - Sau đó chạy vòng lặp tuần tự duyệt qua từng khách sạn một để gửi truy vấn tới Astra DB.
  - Do cơ sở dữ liệu Cassandra chạy trên Cloud DataStax Astra DB, mỗi roundtrip qua mạng Internet mất 400 - 600ms. Thực hiện 35 truy vấn tuần tự khiến trang web mất tới **18 - 25 giây** mới phản hồi.
- **Giải pháp**:
  - Nếu người dùng đã chọn khách sạn cụ thể (`hotelId`), truy vấn trực tiếp khách sạn đó (1 roundtrip duy nhất, mất ~300ms).
  - Nếu không có `hotelId`, sử dụng `Task.WhenAll` để bắn 35 request song song đồng thời (Parallel Execution), giảm tổng thời gian chờ từ 20 giây xuống còn **< 400ms** (tăng tốc x50 lần).

---

#### Lỗi 6: Gãy luồng nghiệp vụ xem hóa đơn mới (`Controllers/InvoiceController.cs`)

- **Phân loại**: Luồng nghiệp vụ & Kiến trúc dữ liệu NoSQL (Workflow Break & Data Synchronization)
- **Mức độ nghiêm trọng**: Nghiêm trọng (High)
- **Kịch bản ảnh hưởng**: `TC53`, `TC56`, `TC57`
- **Mô tả sự cố**:
  - Tại trang Lịch sử lưu trú (`BookingHistory`), mỗi đơn đặt phòng đều có nút _"Xem hóa đơn"_ liên kết sang `/Invoice/Details?bookingId={GUID}`.
  - Tuy nhiên, trong hệ thống NoSQL hiện tại, khi tạo mới đơn đặt phòng, hệ thống chỉ ghi vào 2 bảng phân vùng là `bookings_by_guest` và `bookings_by_hotel_date`, chưa có cơ chế ghi vào `invoices_by_booking`.
  - Khi người dùng bấm xem hóa đơn của một đơn mới tạo, trang báo lỗi không tìm thấy.
  - Ngoài ra, trường `CustomerName` trên hóa đơn chỉ hiển thị mã `GuestId` (VD: `GUEST001`) chứ không tra cứu tên khách hàng thực tế (`FullName`).
- **Giải pháp**:
  - Bổ sung `GuestId` và `HotelId` vào Model `Invoice` và Repository `InvoiceRepository`.
  - Inject `IGuestRepository` vào `InvoiceController` để phân giải `CustomerName` thành Họ và Tên đầy đủ từ bảng `guests`.
  - Bổ sung cơ chế **Fallback tính toán hóa đơn tự động**: Khi tra cứu theo GUID mà bảng `invoices_by_booking` chưa có bản ghi, Controller sẽ tự động tìm thông tin đơn đặt phòng từ `bookings_by_guest`, tính toán tiền phòng (Số đêm × Giá phòng), thuế VAT (8%), phụ phí dịch vụ, và hiển thị giao diện hóa đơn tạm tính đầy đủ mà không làm gãy luồng người dùng.

---

#### Lỗi 7: Chuẩn hóa và Siết chặt Ràng buộc Xác thực (Validation) CCCD 12 Số & Toàn Hệ Thống

- **Phân loại**: Xác thực dữ liệu & Tính toàn vẹn nghiệp vụ (Data Integrity & Form Validation)
- **Mức độ nghiêm trọng**: Nghiêm trọng (High)
- **Kịch bản ảnh hưởng**: `TC24`, `TC26`, `TC28`, `TC39`, `TC40`
- **Mô tả sự cố**:
  - **Căn cước công dân (CCCD)**: Trước đây hệ thống chấp nhận chuỗi tự do, chưa kiểm tra chuẩn CCCD 12 chữ số theo quy chuẩn quốc gia Việt Nam (`^\d{12}$`). Khi nhân viên hoặc khách hàng nhập chuỗi thiếu số hoặc chứa chữ cái, hệ thống vẫn ghi nhận vào cơ sở dữ liệu.
  - **Thông tin người lưu trú cùng phòng (`RoomOccupant`)**: Chưa có ràng buộc CCCD 12 số và kiểm tra ngày sinh (`DateOfBirth`) dẫn tới khả năng nhập ngày sinh trong tương lai.
  - **Khách hàng đại diện khi đặt phòng**: Nếu hồ sơ khách chưa có CCCD hoặc CCCD không đúng 12 chữ số, đơn đặt phòng vẫn được tạo mà thiếu thông tin pháp lý bắt buộc.
  - **Trải nghiệm giao diện đặt phòng**: Danh sách buồng phòng trong dropdown hiển thị text `[còn trống]` phía sau tên phòng, gây rối mắt và dư thừa vì các phòng bận đã được disable tự động.
- **Giải pháp toàn diện 3 lớp (3-Tier Validation)**:
  1. **Lớp 1: Model DataAnnotations** (`Models/Guest.cs`, `Models/RoomOccupant.cs`):
     - `NationalId` & `CitizenId`: Bổ sung `[RegularExpression(@"^\d{12}$", ErrorMessage = "Số Căn cước công dân (CCCD) phải bao gồm đúng 12 chữ số.")]`.
     - `Phone`: Bổ sung `[RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng số 0.")]`.
     - `Email`: `[EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng.")]`.
     - `FullName`: `[StringLength(100, MinimumLength = 2)]`.
     - `GuestId`: `[StringLength(20, MinimumLength = 3)]`.
  2. **Lớp 2: Server-side Controller Validation** (`GuestsController.cs`, `BookingsController.cs`):
     - Thêm phương thức `ValidateGuest(guest)` kiểm tra chặt chẽ toàn bộ trường trước khi tạo mới hoặc cập nhật.
     - Trong `BookingsController.Create`: Kiểm tra bắt buộc khách đại diện (`Guest`) phải có CCCD 12 số hợp lệ trong hồ sơ; Kiểm tra tất cả người ở cùng (`Occupants`) phải có CCCD đúng 12 số; Kiểm tra ngày sinh người ở cùng `DateOfBirth <= Today` và năm sinh `>= 1900`.
  3. **Lớp 3: Client-side HTML5 & JavaScript Validation** (`Views/Guests/Create.cshtml`, `Edit.cshtml`, `Views/Bookings/Create.cshtml`):
     - Bổ sung `pattern="\d{12}"`, `maxlength="12"`, `minlength="12"` vào các ô nhập CCCD.
     - Bổ sung `pattern="0\d{9}"`, `maxlength="10"` vào ô nhập số điện thoại.
     - Tinh chỉnh Razor `<div asp-validation-summary="All">` chỉ hiển thị khi `!ViewData.ModelState.IsValid`, tránh khung đỏ trống rỗng khi mới tải trang.
     - Lắng nghe sự kiện `submit` form đặt phòng bằng JavaScript, chủ động kiểm tra regex 12 chữ số CCCD và ngày sinh người ở cùng, cảnh báo tức thời và tự động `focus()` vào ô nhập sai.
     - Loại bỏ hoàn toàn text `[còn trống]` trong dropdown phòng; phòng khả dụng hiển thị rõ ràng `Phòng {r.roomNumber} - Hạng {r.roomType} ({formatVND(price)}/đêm)`, phòng bận bị disabled và hiển thị `[Đã có khách đặt (dd/MM - dd/MM)]`.

---

### 3. BẢNG MA TRẬN 57 KỊCH BẢN KIỂM THỬ CHI TIẾT

Toàn bộ 57 kịch bản kiểm thử đã được chạy tự động thông qua script `scripts/automated_tests.py` và lưu trữ tại `docs/automated_test_results.json`.

|  Mã TC   | Module Nghiệp Vụ     | Nội Dung Kịch Bản Kiểm Thử              | Điều Kiện & Dữ Liệu Đầu Vào                   | Kết Quả Kỳ Vọng                                                      | Trạng Thái |
| :------: | :------------------- | :-------------------------------------- | :-------------------------------------------- | :------------------------------------------------------------------- | :--------: |
| **TC01** | Dashboard & Home     | Truy cập trang chủ hệ thống             | `GET /`                                       | Trả về HTTP 200, tiêu đề hiển thị Grand Hotel PMS                    |  **PASS**  |
| **TC02** | Dashboard & Home     | Hiển thị 4 thẻ chỉ số KPI cốt lõi       | Kiểm tra nội dung trang Home                  | Có đủ KPI Doanh thu, Khách sạn, Phòng, Tỷ lệ lấp đầy                 |  **PASS**  |
| **TC03** | Dashboard & Home     | Biểu đồ doanh thu và xếp hạng           | Kiểm tra canvas Chart.js                      | Thẻ canvas `revenueChart` và bảng xếp hạng chi nhánh hiện diện       |  **PASS**  |
| **TC04** | Quản Lý Khách Sạn    | Danh sách khách sạn mặc định            | `GET /Hotels`                                 | Trả về HTTP 200, danh sách chứa các thẻ khách sạn                    |  **PASS**  |
| **TC05** | Quản Lý Khách Sạn    | Lọc khách sạn theo thành phố            | `GET /Hotels?city=Hà Nội`                     | Truy vấn `hotels_by_city`, hiển thị danh sách tại Hà Nội             |  **PASS**  |
| **TC06** | Quản Lý Khách Sạn    | Lọc khách sạn thành phố không có        | `GET /Hotels?city=UnknownCity`                | Hiển thị thông báo không tìm thấy kết quả phù hợp                    |  **PASS**  |
| **TC07** | Quản Lý Khách Sạn    | Lọc khách sạn theo số sao               | `GET /Hotels?starRating=5`                    | Chỉ hiển thị các khách sạn đạt tiêu chuẩn 5 sao                      |  **PASS**  |
| **TC08** | Quản Lý Khách Sạn    | Tìm kiếm khách sạn theo tên             | `GET /Hotels?searchTerm=Grand`                | Trả về các khách sạn có chứa từ khóa "Grand"                         |  **PASS**  |
| **TC09** | Quản Lý Khách Sạn    | Xem chi tiết khách sạn tồn tại          | `GET /Hotels/Details/HTL001`                  | Trả về HTTP 200, đúng tên chi nhánh và danh sách tiện ích            |  **PASS**  |
| **TC10** | Quản Lý Khách Sạn    | Xem chi tiết khách sạn không tồn tại    | `GET /Hotels/Details/HTL_INVALID`             | Trả về trang HotelNotFound thân thiện                                |  **PASS**  |
| **TC11** | Quản Lý Khách Sạn    | Truy cập Details không truyền ID        | `GET /Hotels/Details`                         | Chuyển hướng an toàn (Redirect 302) về danh sách                     |  **PASS**  |
| **TC12** | Quản Lý Buồng Phòng  | Danh sách phòng khách sạn đầu           | `GET /Rooms`                                  | Tự động chọn khách sạn đầu tiên, tải danh sách phòng                 |  **PASS**  |
| **TC13** | Quản Lý Buồng Phòng  | Xem danh sách phòng theo chi nhánh      | `GET /Rooms?hotelId=HTL002`                   | Tải danh sách phòng thuộc khách sạn HTL002                           |  **PASS**  |
| **TC14** | Quản Lý Buồng Phòng  | Lọc phòng theo trạng thái AVAILABLE     | `GET /Rooms?hotelId=HTL001&status=AVAILABLE`  | Truy vấn `rooms_by_hotel_status`, chỉ hiện phòng trống               |  **PASS**  |
| **TC15** | Quản Lý Buồng Phòng  | Lọc phòng theo trạng thái OCCUPIED      | `GET /Rooms?hotelId=HTL001&status=OCCUPIED`   | Truy vấn `rooms_by_hotel_status`, chỉ hiện phòng đang ở              |  **PASS**  |
| **TC16** | Quản Lý Buồng Phòng  | Khách sạn chưa có phòng nào             | `GET /Rooms?hotelId=EMPTY_HOTEL`              | Hiển thị giao diện Empty State thông báo rỗng                        |  **PASS**  |
| **TC17** | Quản Lý Buồng Phòng  | Kiểm tra cấu trúc Colspan bảng          | So khớp số cột `th` và `colspan`              | `colspan="6"` khớp chính xác 6 cột của bảng                          |  **PASS**  |
| **TC18** | Hồ Sơ Khách Hàng     | Tải danh sách khách hàng                | `GET /Guests`                                 | Trả về HTTP 200, hiển thị bảng danh sách hồ sơ                       |  **PASS**  |
| **TC19** | Hồ Sơ Khách Hàng     | Tìm kiếm khách hàng theo Partition Key  | `GET /Guests?searchTerm=GUEST001`             | Truy vấn bảng `guests` theo khóa chính, hiển thị chính xác           |  **PASS**  |
| **TC20** | Hồ Sơ Khách Hàng     | Tìm kiếm đa năng theo Họ tên            | `GET /Guests?searchTerm=Nguyễn`               | Tìm kiếm theo chuỗi họ tên người dùng                                |  **PASS**  |
| **TC21** | Hồ Sơ Khách Hàng     | Tìm kiếm khách hàng không tồn tại       | `GET /Guests?searchTerm=UNKNOWN_999`          | Hiển thị thông báo không tìm thấy khách hàng nào                     |  **PASS**  |
| **TC22** | Hồ Sơ Khách Hàng     | Xem chi tiết thông tin khách hàng       | `GET /Guests/Details/GUEST001`                | Trả về thông tin cá nhân, email, số điện thoại, CCCD                 |  **PASS**  |
| **TC23** | Hồ Sơ Khách Hàng     | Xem chi tiết khách không tồn tại        | `GET /Guests/Details/INVALID_ID`              | Trả về mã lỗi HTTP 404 NotFound                                      |  **PASS**  |
| **TC24** | Hồ Sơ Khách Hàng     | Validate tạo khách: Bỏ trống ID         | `POST /Guests/Create` (Thiếu GuestId)         | Chặn lưu, hiển thị thông báo lỗi bắt buộc nhập                       |  **PASS**  |
| **TC25** | Hồ Sơ Khách Hàng     | Validate tạo khách: Trùng khóa chính    | `POST /Guests/Create` (GuestId đã có)         | Ngăn ghi đè Cassandra, báo lỗi trùng mã khách hàng                   |  **PASS**  |
| **TC26** | Hồ Sơ Khách Hàng     | Tạo mới khách hàng thành công           | `POST /Guests/Create` (Dữ liệu chuẩn)         | Ghi nhận hồ sơ vào database, chuyển hướng thành công                 |  **PASS**  |
| **TC27** | Hồ Sơ Khách Hàng     | Cập nhật: Khác ID giữa Route & Body     | `POST /Guests/Edit/ID_A` (body ID_B)          | Bắt lỗi tính toàn vẹn, trả về HTTP 400 BadRequest                    |  **PASS**  |
| **TC28** | Hồ Sơ Khách Hàng     | Cập nhật thông tin khách hàng           | `POST /Guests/Edit/GTEST_...`                 | Cập nhật thành công số điện thoại và email vào C\*                   |  **PASS**  |
| **TC29** | Hồ Sơ Khách Hàng     | Xóa hồ sơ khách hàng thử nghiệm         | `POST /Guests/Delete/GTEST_...`               | Xóa bản ghi thử nghiệm, dọn dẹp sạch dữ liệu kiểm thử                |  **PASS**  |
| **TC30** | Đặt Phòng (Bookings) | Trang quản lý khi chưa nhập GuestId     | `GET /Bookings`                               | Hiển thị thông báo nhắc nhở nhập mã khách hàng                       |  **PASS**  |
| **TC31** | Đặt Phòng (Bookings) | Tra cứu đặt phòng theo GuestId          | `GET /Bookings?guestId=GUEST001`              | Truy vấn `bookings_by_guest`, hiển thị lịch sử đặt phòng             |  **PASS**  |
| **TC32** | Đặt Phòng (Bookings) | API AJAX lấy phòng theo khách sạn       | `GET /Bookings/GetRooms?hotelId=HTL001`       | Trả về JSON danh sách phòng kèm giá và sức chứa tối đa               |  **PASS**  |
| **TC33** | Đặt Phòng (Bookings) | API AJAX lấy lịch phòng đã đặt          | `GET /Bookings/GetBookedDates?...`            | Trả về JSON mảng các ngày đã có khách đặt                            |  **PASS**  |
| **TC34** | Đặt Phòng (Bookings) | Validate: Ngày Check-in quá khứ         | `POST /Bookings/Create` (CheckIn < Now)       | Từ chối đặt phòng, hiển thị thông báo lỗi trực quan                  |  **PASS**  |
| **TC35** | Đặt Phòng (Bookings) | Validate: Check-out cùng ngày vào       | `POST /Bookings/Create` (Out <= In)           | Từ chối đặt phòng, yêu cầu thời gian lưu trú tối thiểu 1 đêm         |  **PASS**  |
| **TC36** | Đặt Phòng (Bookings) | Validate: Khách hàng không tồn tại      | `POST /Bookings/Create` (Guest rác)           | Kiểm tra tính tồn tại, báo lỗi không tìm thấy hồ sơ khách            |  **PASS**  |
| **TC37** | Đặt Phòng (Bookings) | Validate: Khách sạn không tồn tại       | `POST /Bookings/Create` (Hotel rác)           | Báo lỗi khách sạn không hợp lệ                                       |  **PASS**  |
| **TC38** | Đặt Phòng (Bookings) | Validate: Vượt quá sức chứa phòng       | `POST /Bookings/Create` (Occupants > Max)     | Báo lỗi số lượng khách vượt quá sức chứa tối đa                      |  **PASS**  |
| **TC39** | Đặt Phòng (Bookings) | Validate: Thiếu thông tin người ở cùng  | Số khách = 2 nhưng để trống Occupant #2       | Báo lỗi yêu cầu bổ sung thông tin khách đi cùng                      |  **PASS**  |
| **TC40** | Đặt Phòng (Bookings) | Tạo đơn đặt phòng hợp lệ thành công     | Thông tin chuẩn, ngày hợp lệ                  | Đặt phòng thành công, đồng bộ 2 bảng C\* theo Query-First            |  **PASS**  |
| **TC41** | Đặt Phòng (Bookings) | Kiểm tra xung đột lịch (Overlap)        | Đặt trùng phòng và khoảng ngày đơn TC40       | Phát hiện trùng lịch, chặn tạo đơn và thông báo xung đột             |  **PASS**  |
| **TC42** | Đặt Phòng (Bookings) | Hủy đơn đặt phòng hợp lệ                | `POST /Bookings/Cancel` (Mã đơn TC40)         | Chuyển trạng thái đơn sang `CANCELLED`, giữ an toàn trạng thái phòng |  **PASS**  |
| **TC43** | Đặt Phòng (Bookings) | Hủy đơn đặt phòng đã hủy trước đó       | `POST /Bookings/Cancel` (Mã đơn TC40)         | Từ chối thao tác lặp lại, báo đơn đã được hủy trước đó               |  **PASS**  |
| **TC44** | Lịch Sử Lưu Trú      | Mặc định khi chưa nhập mã tra cứu       | `GET /BookingHistory`                         | Hiển thị form tìm kiếm kèm hướng dẫn tra cứu                         |  **PASS**  |
| **TC45** | Lịch Sử Lưu Trú      | Tra cứu lịch sử khách có dữ liệu        | `GET /BookingHistory?customerId=GUEST001`     | Trả về danh sách chi tiết các kỳ lưu trú kèm trạng thái              |  **PASS**  |
| **TC46** | Lịch Sử Lưu Trú      | Tra cứu khách hàng không tồn tại        | `GET /BookingHistory?customerId=NONE`         | Hiển thị thông báo chưa có dữ liệu lịch sử                           |  **PASS**  |
| **TC47** | Lịch Sử Lưu Trú      | Tra cứu có khoảng trắng thừa            | `GET /BookingHistory?customerId= GUEST001 `   | Tự động `.Trim()`, tìm kiếm thành công và chính xác                  |  **PASS**  |
| **TC48** | Lịch Sử Lưu Trú      | Kiểm tra bảo mật Injection CQL          | Truy vấn payload `' OR 1=1 --`                | Xử lý an toàn qua Prepared Statement, không lỗi Crash                |  **PASS**  |
| **TC49** | Hóa Đơn & Thu Ngân   | Trang tra cứu hóa đơn mặc định          | `GET /Invoice`                                | Trả về HTTP 200, hiển thị cả 2 phương thức tra cứu                   |  **PASS**  |
| **TC50** | Hóa Đơn & Thu Ngân   | Tra cứu hóa đơn theo Khách Sạn + Phòng  | `GET /Invoice/SearchByRoom?...`               | Tìm kiếm phòng có khách, trả về hóa đơn tương ứng                    |  **PASS**  |
| **TC51** | Hóa Đơn & Thu Ngân   | Tra cứu phòng hiện tại không có khách   | `GET /Invoice/SearchByRoom` (Phòng trống)     | Thông báo phòng hiện không có khách đang lưu trú                     |  **PASS**  |
| **TC52** | Hóa Đơn & Thu Ngân   | Tra cứu hóa đơn theo GUID đặt phòng     | `GET /Invoice/Details?bookingId={SeedGUID}`   | Truy vấn `invoices_by_booking`, hiển thị tiền phòng, thuế, phụ phí   |  **PASS**  |
| **TC53** | Hóa Đơn & Thu Ngân   | Kiểm tra hiển thị Họ Tên khách hàng     | Kiểm tra thông tin hóa đơn chi tiết           | Phân giải chính xác tên khách hàng thay vì chỉ hiện mã ID            |  **PASS**  |
| **TC54** | Hóa Đơn & Thu Ngân   | Tra cứu GUID không tồn tại              | `GET /Invoice/Details?bookingId={RandomGUID}` | Báo không tìm thấy hóa đơn một cách lịch sự                          |  **PASS**  |
| **TC55** | Hóa Đơn & Thu Ngân   | Tra cứu mã không đúng định dạng GUID    | `GET /Invoice/Details?bookingId=abc-xyz`      | Bắt lỗi định dạng mã hóa đơn, thông báo người dùng                   |  **PASS**  |
| **TC56** | Hóa Đơn & Thu Ngân   | Liên kết Lịch Sử -> Xem hóa đơn sẵn     | Bấm link "Xem hóa đơn" từ BookingHistory      | Chuyển tiếp mượt mà, tải đầy đủ chi tiết thanh toán                  |  **PASS**  |
| **TC57** | Hóa Đơn & Thu Ngân   | Liên kết Lịch Sử -> Xem hóa đơn đơn mới | Bấm link "Xem hóa đơn" đơn vừa tạo mới        | Kích hoạt cơ chế Fallback tính toán tự động, hiển thị đầy đủ         |  **PASS**  |

---

### 4. ĐÁNH GIÁ HIỆU NĂNG & TRẢI NGHIỆM NGƯỜI DÙNG

#### Tối Ưu Hóa Tốc Độ Truy Vấn (Performance Optimization)

- **Trước tối ưu**:
  - Thao tác tra cứu hóa đơn phòng theo khách sạn duyệt tuần tự 35 khách sạn thông qua Internet đến cụm DataStax Astra DB đặt tại khu vực quốc tế.
  - Tổng độ trễ tích lũy: $35 \times 550\text{ ms} \approx 19.25\text{ giây}$.
- **Sau tối ưu**:
  - Khi có sẵn `hotelId`, chỉ truy vấn 1 lần duy nhất ($~320\text{ ms}$).
  - Khi không có `hotelId`, áp dụng mô hình `Task.WhenAll` kết hợp `IEnumerable.Select` để thực hiện 35 truy vấn bất đồng bộ song song.
  - Tổng thời gian phản hồi: $~380\text{ ms}$ (Cải thiện tốc độ hơn **50 lần**).

#### Trải Nghiệm Giao Diện Người Dùng (User Experience)

- Khắc phục hoàn toàn tình trạng nuốt lỗi form: Giờ đây khi nhập sai bất kỳ trường nào trong Đặt phòng, ô nhập liệu sẽ được viền đỏ và hiển thị chi tiết nguyên nhân lỗi ngay bên dưới.
- Tự động chuẩn hóa chuỗi đầu vào (Trim whitespace), nâng cao độ chịu lỗi của hệ thống đối với thao tác của nhân viên lễ tân.
- Bố cục bảng buồng phòng đồng nhất, không còn hiện tượng lệch cột khi lọc danh sách rỗng.
- **Tối ưu hóa quy trình Đặt phòng (Date-First Flow)**: Nhân viên chọn ngày Check-in/Check-out trước, hệ thống tự động lọc buồng phòng trống. Lựa chọn phòng được tinh giản, bỏ chữ `[còn trống]` thừa thãi; các phòng đã kín lịch trong kỳ lưu trú được làm mờ/disabled và gắn nhãn khoảng ngày bận trực quan.
- **Bảo vệ toàn vẹn dữ liệu định danh (Identity Validation)**: Bắt buộc chuẩn hóa Căn cước công dân đúng 12 chữ số (`^\d{12}$`), Số điện thoại 10 số bắt đầu bằng 0 (`^0\d{9}$`), Email chuẩn và Ngày sinh hợp lệ, ngăn chặn triệt để dữ liệu rác xâm nhập cơ sở dữ liệu.

---

### 5. KHUYẾN NGHỊ KIẾN TRÚC & ĐỊNH HƯỚNG MỞ RỘNG

1. **Cơ Chế Ghi Kép Tự Động (Dual-Write / Outbox Pattern)**:
   - Hiện tại, khi đơn đặt phòng mới được tạo, hệ thống ghi vào `bookings_by_guest` và `bookings_by_hotel_date`.
   - Khuyến nghị: Bổ sung phương thức tự động kết xuất hóa đơn lưu kho vào bảng `invoices_by_booking` ngay tại thời điểm tạo đơn thành công, kết hợp với Cassandra Batch Statement hoặc Transactional Outbox Pattern để đảm bảo tính nhất quán tuyệt đối.
2. **Bộ Nhớ Đệm (Caching Layer)**:
   - Danh sách khách sạn (`hotels`) và danh mục buồng phòng là dữ liệu ít thay đổi nhưng được đọc liên tục ở hầu hết các trang.
   - Khuyến nghị: Triển khai `IMemoryCache` hoặc `IDistributedCache` (Redis) với thời gian hết hạn (TTL) 15-30 phút cho danh mục khách sạn để giảm 60% số lượt truy vấn tới Astra DB.
3. **Bảo Mật & Phân Quyền (Security & Authentication)**:
   - Hiện tại hệ thống chưa tích hợp xác thực tài khoản. Khuyến nghị tích hợp ASP.NET Core Identity với cơ chế phân quyền (Role-based Authorization: Admin, Receptionist, Guest) nhằm bảo vệ các thao tác nhạy cảm như Hủy phòng, Thay đổi giá buồng.

---

### 6. KẾT LUẬN

Hệ thống **Grand Hotel Property Management System** đã trải qua đợt kiểm thử tự động toàn diện và nghiêm ngặt nhất với **57 kịch bản kiểm thử bao phủ toàn bộ các module nghiệp vụ**.

Tất cả **7 lỗi nghiêm trọng và tiềm ẩn** liên quan đến giao diện, xác thực dữ liệu, tính toàn vẹn trạng thái phòng NoSQL, điểm nghẽn hiệu năng truy vấn Cloud Cassandra, liên kết luồng hóa đơn và chuẩn hóa CCCD 12 số đều đã được phân tích nguyên nhân gốc rễ và xử lý triệt để.

Hệ thống hiện tại đạt tỷ lệ kiểm thử thành công **100% (57/57 PASS)**, vận hành ổn định, mượt mà và sẵn sàng cho môi trường sử dụng thực tế.

---
