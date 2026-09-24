-- ====================================================================
-- SCRIPT CQL: CẬP NHẬT DỮ LIỆU ĐẦY ĐỦ CHO CÁC VỊ TRÍ NULL TẠI HTL001
-- Hệ Thống Quản Lý Khách Sạn NoSQL Grand Hotel PMS (Astra DB)
-- ====================================================================

USE hotel_ks;

-- ====================================================================
-- PHƯƠNG ÁN 1: BỔ SUNG ĐẦY ĐỦ THÔNG TIN CHO PHÒNG 102 VÀ 103 (KHUYÊN DÙNG)
-- Phòng 102: Hạng Deluxe (Sức chứa 3 người, Đơn giá: 1.200.000 VND)
-- Phòng 103: Hạng Suite  (Sức chứa 4 người, Đơn giá: 2.500.000 VND)
-- ====================================================================

-- 1. Điền dữ liệu cho bảng rooms_by_hotel
UPDATE rooms_by_hotel 
SET room_type = 'Deluxe', 
    price_per_night = 1200000, 
    status = 'AVAILABLE',
    capacity = 3
WHERE hotel_id = 'HTL001' AND room_number = 102;

UPDATE rooms_by_hotel 
SET room_type = 'Suite', 
    price_per_night = 2500000, 
    status = 'AVAILABLE',
    capacity = 4
WHERE hotel_id = 'HTL001' AND room_number = 103;

-- 2. Đồng bộ sang bảng rooms_by_hotel_status (Composite Key: hotel_id, status, room_number)
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night, capacity)
VALUES ('HTL001', 'AVAILABLE', 102, 'Deluxe', 1200000, 3);

INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night, capacity)
VALUES ('HTL001', 'AVAILABLE', 103, 'Suite', 2500000, 4);


-- ====================================================================
-- KIỂM TRA LẠI DỮ LIỆU SAU KHI CẬP NHẬT
-- ====================================================================
SELECT hotel_id, room_number, capacity, price_per_night, room_type, status 
FROM rooms_by_hotel 
WHERE hotel_id = 'HTL001';

SELECT hotel_id, status, room_number, room_type, price_per_night, capacity 
FROM rooms_by_hotel_status 
WHERE hotel_id = 'HTL001' AND status = 'AVAILABLE';
