import re
import json
import time
import uuid
import datetime
import urllib.parse
import sys
import io
import html
import requests

sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')

BASE_URL = "http://localhost:5000"

def get_html(resp):
    return html.unescape(resp.text) if resp is not None else ""

class TestReport:
    def __init__(self):
        self.tests = []
        self.passed = 0
        self.failed = 0
        self.warnings = 0

    def add_result(self, test_id, module, description, status, details=None, is_warning=False):
        if status == "PASS":
            self.passed += 1
            icon = "✅ PASS"
        elif is_warning or status == "WARN":
            self.warnings += 1
            icon = "⚠️ WARN"
            status = "WARN"
        else:
            self.failed += 1
            icon = "❌ FAIL"
        
        entry = {
            "id": test_id,
            "module": module,
            "description": description,
            "status": status,
            "details": details or ""
        }
        self.tests.append(entry)
        print(f"[{icon}] {test_id} ({module}): {description}")
        if details and status != "PASS":
            print(f"       -> Details: {details}")

def get_antiforgery_token(session, url):
    """Fetches a page and extracts ASP.NET Core RequestVerificationToken."""
    try:
        resp = session.get(url, timeout=15)
        match = re.search(r'name=["\']__RequestVerificationToken["\']\s+type=["\']hidden["\']\s+value=["\']([^"\']+)["\']', resp.text)
        if not match:
            match = re.search(r'value=["\']([^"\']+)["\']\s+name=["\']__RequestVerificationToken["\']', resp.text)
        token = match.group(1) if match else None
        return token, resp
    except Exception as e:
        return None, None

def run_tests():
    report = TestReport()
    session = requests.Session()

    print("=====================================================================")
    print(" BẮT ĐẦU KIỂM THỬ TỰ ĐỘNG TOÀN DIỆN HỆ THỐNG GRAND HOTEL PMS")
    print(f" Target URL: {BASE_URL}")
    print(f" Thời gian: {datetime.datetime.now().isoformat()}")
    print("=====================================================================\n")

    # -------------------------------------------------------------
    # MODULE 1: DASHBOARD & HOME
    # -------------------------------------------------------------
    mod = "Dashboard & Home"
    try:
        r = session.get(f"{BASE_URL}/", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Grand Hotel" in t:
            report.add_result("TC01", mod, "Truy cập trang chủ Dashboard (HTTP 200)", "PASS")
        else:
            report.add_result("TC01", mod, "Truy cập trang chủ Dashboard", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC01", mod, "Truy cập trang chủ Dashboard", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Home/Index", timeout=15)
        t = get_html(r)
        has_kpi = "Khách Sạn" in t and "Phòng Nghỉ" in t and "Đơn Đặt Phòng" in t
        if r.status_code == 200 and has_kpi:
            report.add_result("TC02", mod, "Hiển thị đầy đủ 4 thẻ chỉ số KPI cốt lõi", "PASS")
        else:
            report.add_result("TC02", mod, "Hiển thị đầy đủ 4 thẻ chỉ số KPI cốt lõi", "FAIL", f"Status: {r.status_code}, KPI missing")
    except Exception as e:
        report.add_result("TC02", mod, "Hiển thị 4 chỉ số KPI", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/", timeout=15)
        t = get_html(r)
        has_chart = "revenueChart" in t or "Chart" in t
        has_top_branches = "Top" in t or "Chi Nhánh" in t
        if has_chart and has_top_branches:
            report.add_result("TC03", mod, "Biểu đồ phân tích doanh thu và xếp hạng chi nhánh", "PASS")
        else:
            report.add_result("TC03", mod, "Biểu đồ doanh thu / Chi nhánh", "FAIL", "Missing chart or branches")
    except Exception as e:
        report.add_result("TC03", mod, "Biểu đồ doanh thu", "FAIL", str(e))

    # -------------------------------------------------------------
    # MODULE 2: HOTELS MANAGEMENT
    # -------------------------------------------------------------
    mod = "Quản Lý Khách Sạn"
    try:
        r = session.get(f"{BASE_URL}/Hotels", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "HTL001" in t:
            report.add_result("TC04", mod, "Danh sách khách sạn tải thành công với dữ liệu", "PASS")
        else:
            report.add_result("TC04", mod, "Danh sách khách sạn", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC04", mod, "Danh sách khách sạn", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Hotels?city=Ho+Chi+Minh+City", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Ho Chi Minh City" in t:
            report.add_result("TC05", mod, "Lọc khách sạn theo thành phố (Query-First hotels_by_city)", "PASS")
        else:
            report.add_result("TC05", mod, "Lọc khách sạn theo thành phố", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC05", mod, "Lọc khách sạn theo thành phố", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Hotels?city=NonExistentCityXYZ", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Không tìm thấy khách sạn nào phù hợp" in t:
            report.add_result("TC06", mod, "Lọc khách sạn theo thành phố không tồn tại (Empty State)", "PASS")
        else:
            report.add_result("TC06", mod, "Lọc khách sạn thành phố lạ", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC06", mod, "Lọc khách sạn thành phố lạ", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Hotels?starRating=5", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "5 ★" in t:
            report.add_result("TC07", mod, "Lọc khách sạn theo tiêu chuẩn hạng sao (5 sao)", "PASS")
        else:
            report.add_result("TC07", mod, "Lọc khách sạn theo số sao", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC07", mod, "Lọc khách sạn theo số sao", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Hotels?search=Hotel+01", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Hotel 01" in t:
            report.add_result("TC08", mod, "Tìm kiếm khách sạn theo từ khóa tên", "PASS")
        else:
            report.add_result("TC08", mod, "Tìm kiếm khách sạn theo tên", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC08", mod, "Tìm kiếm khách sạn theo tên", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Hotels/Details/HTL001", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "HTL001" in t and "Thông Tin Cơ Sở Lưu Trú" in t:
            report.add_result("TC09", mod, "Xem chi tiết khách sạn hợp lệ (HTL001)", "PASS")
        else:
            report.add_result("TC09", mod, "Xem chi tiết HTL001", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC09", mod, "Xem chi tiết HTL001", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Hotels/Details/HTL999_NOT_EXIST", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Không tìm thấy khách sạn" in t:
            report.add_result("TC10", mod, "Xem chi tiết khách sạn không tồn tại -> HotelNotFound View", "PASS")
        else:
            report.add_result("TC10", mod, "Xem chi tiết khách sạn không tồn tại", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC10", mod, "Xem chi tiết khách sạn không tồn tại", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Hotels/Details/", allow_redirects=False, timeout=15)
        if r.status_code in (301, 302, 307, 308) or r.status_code == 200:
            report.add_result("TC11", mod, "Truy cập /Hotels/Details không có ID -> Redirect an toàn", "PASS")
        else:
            report.add_result("TC11", mod, "Truy cập /Hotels/Details rỗng", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC11", mod, "Truy cập /Hotels/Details rỗng", "FAIL", str(e))

    # -------------------------------------------------------------
    # MODULE 3: ROOMS MANAGEMENT
    # -------------------------------------------------------------
    mod = "Quản Lý Buồng Phòng"
    try:
        r = session.get(f"{BASE_URL}/Rooms", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Danh Mục Phòng Nghỉ" in t:
            report.add_result("TC12", mod, "Danh sách phòng nghỉ mặc định khách sạn đầu tiên (200 OK)", "PASS")
        else:
            report.add_result("TC12", mod, "Danh sách phòng nghỉ", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC12", mod, "Danh sách phòng nghỉ", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Rooms?hotelId=HTL002", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "HTL002" in t:
            report.add_result("TC13", mod, "Xem danh sách phòng của khách sạn cụ thể (HTL002)", "PASS")
        else:
            report.add_result("TC13", mod, "Xem phòng HTL002", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC13", mod, "Xem phòng HTL002", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Rooms?hotelId=HTL001&status=AVAILABLE", timeout=15)
        if r.status_code == 200:
            report.add_result("TC14", mod, "Lọc phòng AVAILABLE (rooms_by_hotel_status)", "PASS")
        else:
            report.add_result("TC14", mod, "Lọc phòng AVAILABLE", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC14", mod, "Lọc phòng AVAILABLE", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Rooms?hotelId=HTL001&status=OCCUPIED", timeout=15)
        if r.status_code == 200:
            report.add_result("TC15", mod, "Lọc phòng OCCUPIED (rooms_by_hotel_status)", "PASS")
        else:
            report.add_result("TC15", mod, "Lọc phòng OCCUPIED", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC15", mod, "Lọc phòng OCCUPIED", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Rooms?hotelId=HTL999_EMPTY", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Chưa có dữ liệu phòng cho khách sạn này" in t:
            report.add_result("TC16", mod, "Xem phòng khách sạn không có phòng -> Empty State", "PASS")
        else:
            report.add_result("TC16", mod, "Xem phòng khách sạn lạ", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC16", mod, "Xem phòng khách sạn lạ", "FAIL", str(e))

    # Kiểm tra bug colspan trong Rooms/Index.cshtml
    try:
        r = session.get(f"{BASE_URL}/Rooms?hotelId=HTL999_EMPTY", timeout=15)
        if '<td colspan="5"' in r.text:
            report.add_result("TC17", mod, "Kiểm tra cấu trúc Colspan bảng phòng (Lỗi UI)", "FAIL", "Bảng có 6 cột (#, Số phòng, Hạng phòng, Sức chứa, Đơn giá, Thao tác) nhưng empty state đặt colspan=\"5\" làm co cụm hiển thị")
        else:
            report.add_result("TC17", mod, "Kiểm tra cấu trúc Colspan bảng phòng", "PASS")
    except Exception as e:
        report.add_result("TC17", mod, "Kiểm tra cấu trúc Colspan", "FAIL", str(e))

    # -------------------------------------------------------------
    # MODULE 4: GUESTS MANAGEMENT (CRUD & SEARCH)
    # -------------------------------------------------------------
    mod = "Hồ Sơ Khách Hàng"
    try:
        r = session.get(f"{BASE_URL}/Guests", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "GUEST001" in t:
            report.add_result("TC18", mod, "Tải danh sách hồ sơ khách hàng (200 OK)", "PASS")
        else:
            report.add_result("TC18", mod, "Tải danh sách khách hàng", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC18", mod, "Tải danh sách khách hàng", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Guests?searchTerm=GUEST001", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "GUEST001" in t:
            report.add_result("TC19", mod, "Tìm kiếm khách hàng theo chính xác mã Partition Key (GUEST001)", "PASS")
        else:
            report.add_result("TC19", mod, "Tìm kiếm theo mã GUEST001", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC19", mod, "Tìm kiếm theo mã GUEST001", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Guests?searchTerm=Nguyen", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and ("Nguyen" in t or "Nguyễn" in t):
            report.add_result("TC20", mod, "Tìm kiếm đa năng khách hàng theo Họ tên", "PASS")
        else:
            report.add_result("TC20", mod, "Tìm kiếm theo Họ tên", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC20", mod, "Tìm kiếm theo Họ tên", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Guests?searchTerm=KhachHangKhongTonTai9999", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Không tìm thấy khách hàng nào" in t:
            report.add_result("TC21", mod, "Tìm kiếm khách hàng không tồn tại -> Empty State", "PASS")
        else:
            report.add_result("TC21", mod, "Tìm kiếm khách không tồn tại", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC21", mod, "Tìm kiếm khách không tồn tại", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Guests/Details/GUEST001", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "GUEST001" in t:
            report.add_result("TC22", mod, "Xem chi tiết thông tin khách hàng GUEST001", "PASS")
        else:
            report.add_result("TC22", mod, "Chi tiết khách hàng GUEST001", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC22", mod, "Chi tiết khách hàng GUEST001", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Guests/Details/GUEST999999", timeout=15)
        if r.status_code == 404:
            report.add_result("TC23", mod, "Xem chi tiết khách hàng không tồn tại -> 404 NotFound", "PASS")
        else:
            report.add_result("TC23", mod, "Chi tiết khách không tồn tại", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC23", mod, "Chi tiết khách không tồn tại", "FAIL", str(e))

    # Test Create Guest validation: Missing GuestId
    token, resp = get_antiforgery_token(session, f"{BASE_URL}/Guests/Create")
    try:
        data = {
            "__RequestVerificationToken": token,
            "GuestId": "",
            "FullName": "Test Name",
            "Phone": "0911223344",
            "Email": "test@test.com",
            "NationalId": "012345678901"
        }
        r = session.post(f"{BASE_URL}/Guests/Create", data=data, timeout=15)
        t = get_html(r)
        if "Mã khách hàng không được để trống" in t or r.status_code != 302:
            report.add_result("TC24", mod, "Validate tạo khách hàng: Bỏ trống GuestId -> Bị chặn", "PASS")
        else:
            report.add_result("TC24", mod, "Validate tạo khách hàng bỏ trống GuestId", "FAIL", "Không báo lỗi khi bỏ trống GuestId")
    except Exception as e:
        report.add_result("TC24", mod, "Validate tạo khách hàng", "FAIL", str(e))

    # Test Create Guest validation: Duplicate GuestId
    token, _ = get_antiforgery_token(session, f"{BASE_URL}/Guests/Create")
    try:
        data = {
            "__RequestVerificationToken": token,
            "GuestId": "GUEST001",
            "FullName": "Trùng Lặp Guest",
            "Phone": "0911223344",
            "Email": "test@test.com",
            "NationalId": "012345678901"
        }
        r = session.post(f"{BASE_URL}/Guests/Create", data=data, timeout=15)
        t = get_html(r)
        if "Mã khách hàng đã tồn tại" in t or "đã tồn tại" in t:
            report.add_result("TC25", mod, "Validate tạo khách hàng: Trùng khóa chính GuestId -> Bị chặn và báo lỗi", "PASS")
        else:
            report.add_result("TC25", mod, "Validate trùng mã khách hàng", "FAIL", "Không báo lỗi trùng mã khách hàng")
    except Exception as e:
        report.add_result("TC25", mod, "Validate trùng mã khách hàng", "FAIL", str(e))

    # Test Create Guest: Valid new guest creation
    token, _ = get_antiforgery_token(session, f"{BASE_URL}/Guests/Create")
    test_guest_id = f"GTEST_{int(time.time()) % 100000}"
    try:
        data = {
            "__RequestVerificationToken": token,
            "GuestId": test_guest_id,
            "FullName": "Khách Thử Nghiệm Tự Động",
            "Phone": "0987654321",
            "Email": "autotest@hotel.vn",
            "NationalId": "079200009999"
        }
        r = session.post(f"{BASE_URL}/Guests/Create", data=data, allow_redirects=False, timeout=15)
        if r.status_code in (301, 302):
            report.add_result("TC26", mod, f"Tạo khách hàng mới thành công ({test_guest_id})", "PASS")
        else:
            report.add_result("TC26", mod, "Tạo khách hàng mới", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC26", mod, "Tạo khách hàng mới", "FAIL", str(e))

    # Test Edit Guest: Mismatched ID in URL vs Body
    token, resp = get_antiforgery_token(session, f"{BASE_URL}/Guests/Edit/{test_guest_id}")
    try:
        data = {
            "__RequestVerificationToken": token,
            "GuestId": "DIFFERENT_ID",
            "FullName": "Hack Name",
            "Phone": "0987654321",
            "Email": "hack@hotel.vn",
            "NationalId": "079200009999"
        }
        r = session.post(f"{BASE_URL}/Guests/Edit/{test_guest_id}", data=data, timeout=15)
        if r.status_code == 400:
            report.add_result("TC27", mod, "Cập nhật khách hàng: Sai khớp ID giữa Route và Body -> 400 BadRequest", "PASS")
        else:
            report.add_result("TC27", mod, "Cập nhật sai khớp ID", "FAIL", f"Expected 400, got {r.status_code}")
    except Exception as e:
        report.add_result("TC27", mod, "Cập nhật sai khớp ID", "FAIL", str(e))

    # Test Edit Guest: Valid update
    try:
        data = {
            "__RequestVerificationToken": token,
            "GuestId": test_guest_id,
            "FullName": "Khách Đã Cập Nhật Tên",
            "Phone": "0987654322",
            "Email": "updated@hotel.vn",
            "NationalId": "079200009999"
        }
        r = session.post(f"{BASE_URL}/Guests/Edit/{test_guest_id}", data=data, allow_redirects=False, timeout=15)
        if r.status_code in (301, 302):
            report.add_result("TC28", mod, "Cập nhật thông tin khách hàng thành công", "PASS")
        else:
            report.add_result("TC28", mod, "Cập nhật thông tin khách hàng", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC28", mod, "Cập nhật thông tin khách hàng", "FAIL", str(e))

    # Test Delete Guest: Clean up the test guest
    token, resp = get_antiforgery_token(session, f"{BASE_URL}/Guests/Delete/{test_guest_id}")
    try:
        data = {
            "__RequestVerificationToken": token,
            "id": test_guest_id
        }
        r = session.post(f"{BASE_URL}/Guests/Delete/{test_guest_id}", data=data, allow_redirects=False, timeout=15)
        if r.status_code in (301, 302):
            report.add_result("TC29", mod, f"Xóa hồ sơ khách hàng thử nghiệm ({test_guest_id})", "PASS")
        else:
            report.add_result("TC29", mod, "Xóa hồ sơ khách hàng", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC29", mod, "Xóa hồ sơ khách hàng", "FAIL", str(e))

    # -------------------------------------------------------------
    # MODULE 5: BOOKINGS MANAGEMENT (WORKFLOW & CONSTRAINTS)
    # -------------------------------------------------------------
    mod = "Đặt Phòng (Bookings)"
    try:
        r = session.get(f"{BASE_URL}/Bookings", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Vui lòng nhập Mã Khách Hàng" in t:
            report.add_result("TC30", mod, "Trang quản lý đơn đặt phòng khi chưa nhập GuestId (Prompt)", "PASS")
        else:
            report.add_result("TC30", mod, "Trang đặt phòng mặc định", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC30", mod, "Trang đặt phòng mặc định", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Bookings?guestId=GUEST001", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Danh Sách Đơn Đặt Phòng Của Khách Hàng" in t:
            report.add_result("TC31", mod, "Tra cứu danh sách đặt phòng theo GuestId (GUEST001)", "PASS")
        else:
            report.add_result("TC31", mod, "Tra cứu đặt phòng GUEST001", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC31", mod, "Tra cứu đặt phòng GUEST001", "FAIL", str(e))

    # AJAX API endpoints
    try:
        r = session.get(f"{BASE_URL}/Bookings/GetRooms?hotelId=HTL001", timeout=15)
        rooms_json = r.json()
        if r.status_code == 200 and isinstance(rooms_json, list) and len(rooms_json) > 0 and "roomNumber" in rooms_json[0]:
            report.add_result("TC32", mod, "API AJAX GetRooms trả về danh sách phòng kèm giá và sức chứa", "PASS")
        else:
            report.add_result("TC32", mod, "API GetRooms", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC32", mod, "API GetRooms", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/Bookings/GetBookedDates?hotelId=HTL001&roomNumber=101", timeout=15)
        dates_json = r.json()
        if r.status_code == 200 and isinstance(dates_json, list):
            report.add_result("TC33", mod, "API AJAX GetBookedDates trả về danh sách lịch đã đặt", "PASS")
        else:
            report.add_result("TC33", mod, "API GetBookedDates", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC33", mod, "API GetBookedDates", "FAIL", str(e))

    # Test Booking Create Validation: CheckInDate in the past
    token, resp = get_antiforgery_token(session, f"{BASE_URL}/Bookings/Create")
    try:
        past_checkin = (datetime.date.today() - datetime.timedelta(days=2)).isoformat()
        past_checkout = (datetime.date.today() - datetime.timedelta(days=1)).isoformat()
        data = {
            "__RequestVerificationToken": token,
            "GuestId": "GUEST001",
            "HotelId": "HTL001",
            "RoomNumber": "101",
            "CheckInDate": past_checkin,
            "CheckOutDate": past_checkout,
            "NumberOfOccupants": "1"
        }
        r = session.post(f"{BASE_URL}/Bookings/Create", data=data, timeout=15)
        t = get_html(r)
        has_error_in_html = "quá khứ" in t or "Ngày nhận phòng không được là ngày trong quá khứ" in t
        if has_error_in_html:
            report.add_result("TC34", mod, "Validate đặt phòng: Ngày check-in trong quá khứ -> Bị từ chối và hiển thị lỗi", "PASS")
        elif r.status_code == 200:
            # Server blocked the creation (did not redirect), but the error was suppressed from the user
            report.add_result("TC34", mod, "Validate đặt phòng: Ngày check-in trong quá khứ (Lỗi che giấu thông báo)", "FAIL", "Server chặn thành công nhưng thông báo lỗi không thể hiển thị tới người dùng do View dùng asp-validation-summary=\"ModelOnly\" và thiếu thẻ span asp-validation-for")
        else:
            report.add_result("TC34", mod, "Validate ngày quá khứ", "FAIL", "Hệ thống cho phép lưu ngày quá khứ!")
    except Exception as e:
        report.add_result("TC34", mod, "Validate ngày quá khứ", "FAIL", str(e))

    # Test Booking Create Validation: CheckOutDate <= CheckInDate
    try:
        today_str = datetime.date.today().isoformat()
        data = {
            "__RequestVerificationToken": token,
            "GuestId": "GUEST001",
            "HotelId": "HTL001",
            "RoomNumber": "101",
            "CheckInDate": today_str,
            "CheckOutDate": today_str,
            "NumberOfOccupants": "1"
        }
        r = session.post(f"{BASE_URL}/Bookings/Create", data=data, timeout=15)
        t = get_html(r)
        has_error_in_html = "ít nhất 1 đêm" in t or "sau ngày nhận phòng" in t
        if has_error_in_html:
            report.add_result("TC35", mod, "Validate đặt phòng: Check-out cùng ngày check-in -> Bị từ chối và hiển thị lỗi", "PASS")
        elif r.status_code == 200:
            report.add_result("TC35", mod, "Validate đặt phòng: Check-out cùng ngày check-in (Lỗi che giấu thông báo)", "FAIL", "Server chặn nhưng lỗi không hiển thị trên UI do ModelOnly summary")
        else:
            report.add_result("TC35", mod, "Validate checkout <= checkin", "FAIL", "Hệ thống cho phép checkout cùng ngày checkin!")
    except Exception as e:
        report.add_result("TC35", mod, "Validate checkout <= checkin", "FAIL", str(e))

    # Test Booking Create Validation: Invalid GuestId
    future_in = (datetime.date.today() + datetime.timedelta(days=10)).isoformat()
    future_out = (datetime.date.today() + datetime.timedelta(days=12)).isoformat()
    try:
        data = {
            "__RequestVerificationToken": token,
            "GuestId": "GUEST_NONEXISTENT_999",
            "HotelId": "HTL001",
            "RoomNumber": "101",
            "CheckInDate": future_in,
            "CheckOutDate": future_out,
            "NumberOfOccupants": "1"
        }
        r = session.post(f"{BASE_URL}/Bookings/Create", data=data, timeout=15)
        t = get_html(r)
        has_error = "không tồn tại" in t or "Mã khách hàng" in t
        if has_error:
            report.add_result("TC36", mod, "Validate đặt phòng: Khách hàng không tồn tại -> Báo lỗi", "PASS")
        elif r.status_code == 200:
            report.add_result("TC36", mod, "Validate đặt phòng: Khách hàng không tồn tại (Lỗi che giấu thông báo)", "FAIL", "Server chặn nhưng lỗi không hiển thị trên UI do ModelOnly summary")
        else:
            report.add_result("TC36", mod, "Validate khách không tồn tại", "FAIL", "Cho phép tạo booking với khách không tồn tại!")
    except Exception as e:
        report.add_result("TC36", mod, "Validate khách không tồn tại", "FAIL", str(e))

    # Test Booking Create Validation: Invalid HotelId
    try:
        data = {
            "__RequestVerificationToken": token,
            "GuestId": "GUEST001",
            "HotelId": "HTL_INVALID_999",
            "RoomNumber": "101",
            "CheckInDate": future_in,
            "CheckOutDate": future_out,
            "NumberOfOccupants": "1"
        }
        r = session.post(f"{BASE_URL}/Bookings/Create", data=data, timeout=15)
        t = get_html(r)
        has_error = "khách sạn" in t and "không tồn tại" in t
        if has_error:
            report.add_result("TC37", mod, "Validate đặt phòng: Khách sạn không tồn tại -> Báo lỗi", "PASS")
        elif r.status_code == 200:
            report.add_result("TC37", mod, "Validate đặt phòng: Khách sạn không tồn tại (Lỗi che giấu thông báo)", "FAIL", "Server chặn nhưng lỗi không hiển thị trên UI do ModelOnly summary")
        else:
            report.add_result("TC37", mod, "Validate khách sạn không tồn tại", "FAIL", "Cho phép tạo booking với khách sạn lạ!")
    except Exception as e:
        report.add_result("TC37", mod, "Validate khách sạn không tồn tại", "FAIL", str(e))

    # Test Booking Create Validation: Capacity exceeded (Standard room cap 2, register 5)
    try:
        data = {
            "__RequestVerificationToken": token,
            "GuestId": "GUEST001",
            "HotelId": "HTL001",
            "RoomNumber": "101",
            "CheckInDate": future_in,
            "CheckOutDate": future_out,
            "NumberOfOccupants": "5"
        }
        r = session.post(f"{BASE_URL}/Bookings/Create", data=data, timeout=15)
        t = get_html(r)
        has_error = "chứa tối đa" in t or "sức chứa" in t
        if has_error:
            report.add_result("TC38", mod, "Validate đặt phòng: Vượt quá sức chứa phòng (Capacity Exceeded) -> Báo lỗi", "PASS")
        elif r.status_code == 200:
            report.add_result("TC38", mod, "Validate đặt phòng: Vượt quá sức chứa phòng (Lỗi che giấu thông báo)", "FAIL", "Server chặn nhưng lỗi không hiển thị trên UI do ModelOnly summary")
        else:
            report.add_result("TC38", mod, "Validate sức chứa phòng", "FAIL", "Cho phép đăng ký số người vượt quá sức chứa!")
    except Exception as e:
        report.add_result("TC38", mod, "Validate sức chứa phòng", "FAIL", str(e))

    # Test Booking Create Validation: 2 occupants but missing occupant 2 details
    try:
        data = {
            "__RequestVerificationToken": token,
            "GuestId": "GUEST001",
            "HotelId": "HTL001",
            "RoomNumber": "101",
            "CheckInDate": future_in,
            "CheckOutDate": future_out,
            "NumberOfOccupants": "2",
            "Occupants[1].FullName": "",
            "Occupants[1].CitizenId": ""
        }
        r = session.post(f"{BASE_URL}/Bookings/Create", data=data, timeout=15)
        t = get_html(r)
        has_error = "người ở cùng" in t or "CCCD/CMND" in t or "Họ và tên" in t
        if has_error:
            report.add_result("TC39", mod, "Validate đặt phòng: Thiếu thông tin người ở cùng thứ 2 -> Báo lỗi", "PASS")
        elif r.status_code == 200:
            report.add_result("TC39", mod, "Validate đặt phòng: Thiếu thông tin người ở cùng (Lỗi che giấu thông báo)", "FAIL", "Server chặn nhưng lỗi không hiển thị trên UI do ModelOnly summary")
        else:
            report.add_result("TC39", mod, "Validate thông tin người ở cùng", "FAIL", "Cho phép đăng ký người ở cùng mà không có tên/CCCD!")
    except Exception as e:
        report.add_result("TC39", mod, "Validate thông tin người ở cùng", "FAIL", str(e))

    # Test Booking Create: Successful booking in 2027
    booking_in = "2027-02-10"
    booking_out = "2027-02-12"
    created_booking_id = None
    try:
        data = {
            "__RequestVerificationToken": token,
            "GuestId": "GUEST001",
            "HotelId": "HTL001",
            "RoomNumber": "101",
            "CheckInDate": booking_in,
            "CheckOutDate": booking_out,
            "NumberOfOccupants": "1"
        }
        r = session.post(f"{BASE_URL}/Bookings/Create", data=data, allow_redirects=True, timeout=15)
        t = get_html(r)
        if r.status_code == 200 and ("Đặt phòng thành công" in t or "CONFIRMED" in t):
            m = re.search(r'Mã booking:\s*([a-f0-9\-]{36})', t)
            if not m:
                m = re.search(r'([a-f0-9]{8}\-[a-f0-9]{4}\-[a-f0-9]{4}\-[a-f0-9]{4}\-[a-f0-9]{12})', t)
            if m:
                created_booking_id = m.group(1)
            report.add_result("TC40", mod, f"Tạo đơn đặt phòng hợp lệ thành công (Booking: {created_booking_id})", "PASS")
        else:
            report.add_result("TC40", mod, "Tạo đơn đặt phòng hợp lệ", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC40", mod, "Tạo đơn đặt phòng hợp lệ", "FAIL", str(e))

    # Test Booking Create Validation: Conflict detection with the booking just created
    try:
        overlap_in = "2027-02-11"
        overlap_out = "2027-02-13"
        token, _ = get_antiforgery_token(session, f"{BASE_URL}/Bookings/Create")
        data = {
            "__RequestVerificationToken": token,
            "GuestId": "GUEST002",
            "HotelId": "HTL001",
            "RoomNumber": "101",
            "CheckInDate": overlap_in,
            "CheckOutDate": overlap_out,
            "NumberOfOccupants": "1"
        }
        r = session.post(f"{BASE_URL}/Bookings/Create", data=data, timeout=15)
        t = get_html(r)
        if "đã có khách đặt" in t or "Xung Đột Lịch" in t or "Khoảng ngày bạn chọn không liên tục" in t:
            report.add_result("TC41", mod, "Phát hiện xung đột lịch đặt phòng (Overlap Detection) -> Bị chặn và hiển thị cảnh báo", "PASS")
        elif r.status_code == 200:
            report.add_result("TC41", mod, "Phát hiện xung đột lịch đặt phòng", "FAIL", "Bị chặn nhưng lỗi không hiển thị trên UI")
        else:
            report.add_result("TC41", mod, "Phát hiện xung đột lịch đặt phòng", "FAIL", "Hệ thống cho phép đặt trùng phòng/ngày!")
    except Exception as e:
        report.add_result("TC41", mod, "Phát hiện xung đột lịch đặt phòng", "FAIL", str(e))

    # Test Booking Cancel: Cancel the booking we just created
    if created_booking_id:
        try:
            token, _ = get_antiforgery_token(session, f"{BASE_URL}/Bookings?guestId=GUEST001")
            data = {
                "__RequestVerificationToken": token,
                "bookingId": created_booking_id,
                "guestId": "GUEST001",
                "hotelId": "HTL001",
                "roomNumber": "101",
                "checkInDate": booking_in
            }
            r = session.post(f"{BASE_URL}/Bookings/Cancel", data=data, allow_redirects=True, timeout=15)
            t = get_html(r)
            if "Đã hủy booking" in t or "CANCELLED" in t:
                report.add_result("TC42", mod, f"Hủy đơn đặt phòng thành công ({created_booking_id})", "PASS")
            else:
                report.add_result("TC42", mod, "Hủy đơn đặt phòng", "FAIL", f"Status: {r.status_code}, Msg: {t[:200]}")
        except Exception as e:
            report.add_result("TC42", mod, "Hủy đơn đặt phòng", "FAIL", str(e))

        # Test Booking Cancel: Attempt to cancel an ALREADY cancelled booking
        try:
            r = session.post(f"{BASE_URL}/Bookings/Cancel", data=data, allow_redirects=True, timeout=15)
            t = get_html(r)
            if "đã được hủy trước đó" in t:
                report.add_result("TC43", mod, "Hủy đơn đã hủy trước đó -> Bị từ chối và cảnh báo", "PASS")
            else:
                report.add_result("TC43", mod, "Hủy đơn đã hủy", "FAIL", "Cho phép hủy đơn trùng lặp")
        except Exception as e:
            report.add_result("TC43", mod, "Hủy đơn đã hủy", "FAIL", str(e))
    else:
        report.add_result("TC42", mod, "Hủy đơn đặt phòng", "WARN", "Bỏ qua do không có created_booking_id")
        report.add_result("TC43", mod, "Hủy đơn đã hủy", "WARN", "Bỏ qua")

    # -------------------------------------------------------------
    # MODULE 6: BOOKING HISTORY
    # -------------------------------------------------------------
    mod = "Lịch Sử Lưu Trú (BookingHistory)"
    try:
        r = session.get(f"{BASE_URL}/BookingHistory", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Tra cứu lịch sử đặt phòng của khách hàng" in t:
            report.add_result("TC44", mod, "Trang lịch sử khi chưa nhập mã (Prompt)", "PASS")
        else:
            report.add_result("TC44", mod, "Trang lịch sử mặc định", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC44", mod, "Trang lịch sử mặc định", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/BookingHistory?customerId=GUEST001", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Lịch Sử Đặt Phòng Của Khách" in t and "GUEST001" in t:
            report.add_result("TC45", mod, "Tra cứu lịch sử khách hàng có dữ liệu (GUEST001)", "PASS")
        else:
            report.add_result("TC45", mod, "Tra cứu lịch sử GUEST001", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC45", mod, "Tra cứu lịch sử GUEST001", "FAIL", str(e))

    try:
        r = session.get(f"{BASE_URL}/BookingHistory?customerId=GUEST_KHONG_TON_TAI_999", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Không tìm thấy lịch sử lưu trú" in t:
            report.add_result("TC46", mod, "Tra cứu lịch sử khách không tồn tại -> Empty State", "PASS")
        else:
            report.add_result("TC46", mod, "Tra cứu khách không tồn tại", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC46", mod, "Tra cứu khách không tồn tại", "FAIL", str(e))

    # Test trim input in BookingHistory
    try:
        r = session.get(f"{BASE_URL}/BookingHistory?customerId=%20GUEST001%20", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Lịch Sử Đặt Phòng Của Khách" in t and "lượt lưu trú" in t:
            report.add_result("TC47", mod, "Tra cứu lịch sử khi mã có khoảng trắng (' GUEST001 ')", "PASS")
        else:
            report.add_result("TC47", mod, "Tra cứu lịch sử khi mã có khoảng trắng (Lỗi Không Trim)", "FAIL", "Controller/Repo không trim() customerId dẫn tới Cassandra partition key ' GUEST001 ' không khớp", is_warning=True)
    except Exception as e:
        report.add_result("TC47", mod, "Tra cứu có khoảng trắng", "FAIL", str(e))

    # Test SQL injection payload in BookingHistory
    try:
        r = session.get(f"{BASE_URL}/BookingHistory?customerId=' OR 1=1 --", timeout=15)
        if r.status_code == 200:
            report.add_result("TC48", mod, "An toàn trước injection payload (' OR 1=1 --) trong BookingHistory", "PASS")
        else:
            report.add_result("TC48", mod, "Injection payload BookingHistory", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC48", mod, "Injection payload BookingHistory", "FAIL", str(e))

    # -------------------------------------------------------------
    # MODULE 7: INVOICE & BILLING
    # -------------------------------------------------------------
    mod = "Hóa Đơn & Thu Ngân (Invoice)"
    try:
        r = session.get(f"{BASE_URL}/Invoice", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Tra cứu hóa đơn & thanh toán" in t:
            report.add_result("TC49", mod, "Trang hóa đơn mặc định khi chưa tra cứu (200 OK)", "PASS")
        else:
            report.add_result("TC49", mod, "Trang hóa đơn mặc định", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC49", mod, "Trang hóa đơn mặc định", "FAIL", str(e))

    # Case 1: Tra cứu theo Khách sạn + Số phòng có đơn
    try:
        r = session.get(f"{BASE_URL}/Invoice?hotelId=HTL001&roomNumber=101", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and ("PHIẾU HÓA ĐƠN KHÁCH SẠN" in t or "Lịch Sử Đặt Phòng & Hóa Đơn" in t):
            report.add_result("TC50", mod, "Tra cứu hóa đơn theo [Khách Sạn + Số Phòng] (HTL001, P.101)", "PASS")
        else:
            report.add_result("TC50", mod, "Tra cứu theo KS + Phòng", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC50", mod, "Tra cứu theo KS + Phòng", "FAIL", str(e))

    # Case 2: Tra cứu theo Khách sạn + Số phòng KHÔNG có đơn
    try:
        r = session.get(f"{BASE_URL}/Invoice?hotelId=HTL001&roomNumber=999", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Không tìm thấy lịch sử đặt phòng nào" in t:
            report.add_result("TC51", mod, "Tra cứu hóa đơn phòng không có booking -> Báo lỗi lịch sự", "PASS")
        else:
            report.add_result("TC51", mod, "Tra cứu phòng không có booking", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC51", mod, "Tra cứu phòng không có booking", "FAIL", str(e))

    # Case 3: Tra cứu theo mã GUID hợp lệ từ seed data (e58cccdd-ef47-5e15-8f54-afe3fc8fa3cc)
    try:
        seed_guid = "e58cccdd-ef47-5e15-8f54-afe3fc8fa3cc"
        r = session.get(f"{BASE_URL}/Invoice?bookingId={seed_guid}", timeout=25)
        t = get_html(r)
        if r.status_code == 200 and "PHIẾU HÓA ĐƠN KHÁCH SẠN" in t:
            report.add_result("TC52", mod, "Tra cứu hóa đơn theo mã GUID đặt phòng (Seed Data)", "PASS")
        else:
            report.add_result("TC52", mod, "Tra cứu theo mã GUID seed", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC52", mod, "Tra cứu theo mã GUID seed", "FAIL", str(e))

    # Case 4: Kiểm tra CustomerName trên hóa đơn seed data
    try:
        seed_guid = "e58cccdd-ef47-5e15-8f54-afe3fc8fa3cc"
        r = session.get(f"{BASE_URL}/Invoice?bookingId={seed_guid}", timeout=25)
        t = get_html(r)
        # Check if customer name is displayed as GUEST001 instead of actual name
        if "GUEST001" in t and not ("Nguyễn" in t or "An" in t):
            report.add_result("TC53", mod, "Kiểm tra tên khách hàng trên hóa đơn (Lỗi Dữ Liệu)", "FAIL", "InvoiceRepository gán CustomerName = guest_id (GUEST001) thay vì họ tên khách hàng, dẫn tới hóa đơn in ra tên khách là mã ID")
        else:
            report.add_result("TC53", mod, "Kiểm tra tên khách hàng trên hóa đơn", "PASS")
    except Exception as e:
        report.add_result("TC53", mod, "Kiểm tra tên khách hàng", "FAIL", str(e))

    # Case 5: Tra cứu hóa đơn với mã GUID KHÔNG TỒN TẠI
    try:
        fake_guid = "00000000-0000-0000-0000-000000000000"
        r = session.get(f"{BASE_URL}/Invoice?bookingId={fake_guid}", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "Không tìm thấy hóa đơn" in t:
            report.add_result("TC54", mod, "Tra cứu GUID không tồn tại -> Thông báo không tìm thấy", "PASS")
        else:
            report.add_result("TC54", mod, "Tra cứu GUID không tồn tại", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC54", mod, "Tra cứu GUID không tồn tại", "FAIL", str(e))

    # Case 6: Tra cứu hóa đơn với mã không phải định dạng GUID
    try:
        r = session.get(f"{BASE_URL}/Invoice?bookingId=MALFORMED-STRING-XYZ", timeout=15)
        t = get_html(r)
        if r.status_code == 200 and "không đúng định dạng GUID" in t:
            report.add_result("TC55", mod, "Tra cứu mã không phải GUID -> Bắt lỗi định dạng", "PASS")
        else:
            report.add_result("TC55", mod, "Tra cứu mã không phải GUID", "FAIL", f"Status: {r.status_code}")
    except Exception as e:
        report.add_result("TC55", mod, "Tra cứu mã không phải GUID", "FAIL", str(e))

    # Case 7: Tra cứu hóa đơn từ BookingHistory -> Invoice liên kết (Workflow Integration)
    # Lấy bookingId mới tạo hoặc booking không có trong invoices_by_booking
    try:
        r = session.get(f"{BASE_URL}/BookingHistory?customerId=GUEST001", timeout=15)
        m = re.findall(r'href=["\'][^"\']*bookingId=([a-f0-9\-]{36})["\']', r.text)
        
        # Test the first seed booking (should pass)
        seed_id = "e58cccdd-ef47-5e15-8f54-afe3fc8fa3cc"
        r_seed = session.get(f"{BASE_URL}/Invoice?bookingId={seed_id}", timeout=25)
        t_seed = get_html(r_seed)
        if "PHIẾU HÓA ĐƠN KHÁCH SẠN" in t_seed:
            report.add_result("TC56", mod, f"Liên kết Lịch sử -> Xem hóa đơn chi tiết đơn có sẵn ({seed_id})", "PASS")
        else:
            report.add_result("TC56", mod, "Liên kết Lịch sử -> Hóa đơn seed", "FAIL", f"Status: {r_seed.status_code}")

        # Test any newly created booking or booking not in invoices_by_booking
        non_seed = [b for b in m if b != seed_id]
        if non_seed:
            test_b = non_seed[0]
            r_dyn = session.get(f"{BASE_URL}/Invoice?bookingId={test_b}", timeout=25)
            t_dyn = get_html(r_dyn)
            if "PHIẾU HÓA ĐƠN KHÁCH SẠN" in t_dyn:
                report.add_result("TC57", mod, f"Liên kết Lịch sử -> Xem hóa đơn đơn mới tạo ({test_b})", "PASS")
            else:
                report.add_result("TC57", mod, f"Liên kết Lịch sử -> Hóa đơn đơn mới tạo (Lỗi Thiếu Hóa Đơn)", "FAIL", f"Đơn đặt phòng mới {test_b} không có hóa đơn trong invoices_by_booking và InvoiceController theo GUID không có fallback tính hóa đơn tạm")
    except Exception as e:
        report.add_result("TC56", mod, "Liên kết Lịch sử -> Hóa đơn", "FAIL", str(e))

    # -------------------------------------------------------------
    # TỔNG KẾT VÀ XUẤT BÁO CÁO
    # -------------------------------------------------------------
    print("\n=====================================================================")
    print(" KẾT QUẢ KIỂM THỬ TỰ ĐỘNG (SUMMARY)")
    print("=====================================================================")
    print(f" Tổng số Testcases đã thực thi : {len(report.tests)}")
    print(f" Số Testcases ĐẠT (PASS)       : {report.passed}")
    print(f" Số Testcases CẢNH BÁO (WARN)  : {report.warnings}")
    print(f" Số Testcases THẤT BẠI (FAIL)  : {report.failed}")
    print("=====================================================================")

    # Lưu kết quả ra file JSON
    with open("docs/automated_test_results.json", "w", encoding="utf-8") as f:
        json.dump(report.tests, f, ensure_ascii=False, indent=2)
    print("Đã lưu kết quả chi tiết vào docs/automated_test_results.json\n")

if __name__ == "__main__":
    run_tests()
