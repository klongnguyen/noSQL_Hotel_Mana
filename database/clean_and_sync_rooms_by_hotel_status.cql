-- ====================================================================
-- SCRIPT CQL: DỌN DẸP DÒNG MA (NULL) VÀ ĐỒNG BỘ CAPACITY CHO ROOMS_BY_HOTEL_STATUS
-- Khóa chính của bảng: PRIMARY KEY ((hotel_id, status), room_number)
-- ====================================================================

USE hotel_ks;

-- ====================================================================
-- BƯỚC 1: TRUY VẤN XEM CÁC DÒNG ĐANG BỊ DÍNH NULL
-- (Lưu ý: Vì Partition Key là cặp (hotel_id, status), khi chỉ lọc theo
--  hotel_id trong cqlsh bắt buộc phải thêm ALLOW FILTERING)
-- ====================================================================
SELECT hotel_id, status, room_number, room_type, price_per_night, capacity 
FROM rooms_by_hotel_status 
WHERE hotel_id = 'HTL001' ALLOW FILTERING;

SELECT hotel_id, status, room_number, room_type, price_per_night, capacity 
FROM rooms_by_hotel_status 
WHERE hotel_id = 'HTL007' ALLOW FILTERING;


-- ====================================================================
-- BƯỚC 2: XÓA 13 DÒNG MA (PHANTOM ROWS CHỨA ROOM_TYPE / PRICE = NULL)
-- (Đã truyền đủ cả hotel_id, status và room_number nên chạy 100% chuẩn xác)
-- ====================================================================
DELETE FROM rooms_by_hotel_status WHERE hotel_id = 'HTL001' AND status = 'AVAILABLE' AND room_number = 101;
DELETE FROM rooms_by_hotel_status WHERE hotel_id = 'HTL007' AND status = 'AVAILABLE' AND room_number = 201;
DELETE FROM rooms_by_hotel_status WHERE hotel_id = 'HTL016' AND status = 'AVAILABLE' AND room_number = 202;
DELETE FROM rooms_by_hotel_status WHERE hotel_id = 'HTL017' AND status = 'OCCUPIED'  AND room_number = 202;
DELETE FROM rooms_by_hotel_status WHERE hotel_id = 'HTL018' AND status = 'AVAILABLE' AND room_number = 202;
DELETE FROM rooms_by_hotel_status WHERE hotel_id = 'HTL022' AND status = 'AVAILABLE' AND room_number = 202;
DELETE FROM rooms_by_hotel_status WHERE hotel_id = 'HTL023' AND status = 'OCCUPIED'  AND room_number = 202;
DELETE FROM rooms_by_hotel_status WHERE hotel_id = 'HTL024' AND status = 'AVAILABLE' AND room_number = 202;
DELETE FROM rooms_by_hotel_status WHERE hotel_id = 'HTL028' AND status = 'AVAILABLE' AND room_number = 202;
DELETE FROM rooms_by_hotel_status WHERE hotel_id = 'HTL029' AND status = 'OCCUPIED'  AND room_number = 202;
DELETE FROM rooms_by_hotel_status WHERE hotel_id = 'HTL030' AND status = 'AVAILABLE' AND room_number = 202;
DELETE FROM rooms_by_hotel_status WHERE hotel_id = 'HTL034' AND status = 'AVAILABLE' AND room_number = 202;
DELETE FROM rooms_by_hotel_status WHERE hotel_id = 'HTL035' AND status = 'OCCUPIED'  AND room_number = 202;


-- ====================================================================
-- BƯỚC 3: CẬP NHẬT CAPACITY ĐẦY ĐỦ CHO 13 DÒNG THẬT THEO ĐÚNG TRẠNG THÁI HIỆN TẠI
-- ====================================================================
UPDATE rooms_by_hotel_status SET capacity = 2 WHERE hotel_id = 'HTL001' AND status = 'OCCUPIED'  AND room_number = 101;
UPDATE rooms_by_hotel_status SET capacity = 2 WHERE hotel_id = 'HTL007' AND status = 'OCCUPIED'  AND room_number = 201;
UPDATE rooms_by_hotel_status SET capacity = 3 WHERE hotel_id = 'HTL016' AND status = 'OCCUPIED'  AND room_number = 202;
UPDATE rooms_by_hotel_status SET capacity = 3 WHERE hotel_id = 'HTL017' AND status = 'AVAILABLE' AND room_number = 202;
UPDATE rooms_by_hotel_status SET capacity = 3 WHERE hotel_id = 'HTL018' AND status = 'OCCUPIED'  AND room_number = 202;
UPDATE rooms_by_hotel_status SET capacity = 3 WHERE hotel_id = 'HTL022' AND status = 'OCCUPIED'  AND room_number = 202;
UPDATE rooms_by_hotel_status SET capacity = 3 WHERE hotel_id = 'HTL023' AND status = 'AVAILABLE' AND room_number = 202;
UPDATE rooms_by_hotel_status SET capacity = 3 WHERE hotel_id = 'HTL024' AND status = 'OCCUPIED'  AND room_number = 202;
UPDATE rooms_by_hotel_status SET capacity = 3 WHERE hotel_id = 'HTL028' AND status = 'OCCUPIED'  AND room_number = 202;
UPDATE rooms_by_hotel_status SET capacity = 3 WHERE hotel_id = 'HTL029' AND status = 'AVAILABLE' AND room_number = 202;
UPDATE rooms_by_hotel_status SET capacity = 3 WHERE hotel_id = 'HTL030' AND status = 'OCCUPIED'  AND room_number = 202;
UPDATE rooms_by_hotel_status SET capacity = 3 WHERE hotel_id = 'HTL034' AND status = 'OCCUPIED'  AND room_number = 202;
UPDATE rooms_by_hotel_status SET capacity = 3 WHERE hotel_id = 'HTL035' AND status = 'AVAILABLE' AND room_number = 202;


-- ====================================================================
-- BƯỚC 4: KIỂM TRA LẠI KẾT QUẢ SAU KHI SỬA
-- ====================================================================
SELECT hotel_id, status, room_number, room_type, price_per_night, capacity 
FROM rooms_by_hotel_status 
WHERE hotel_id = 'HTL001' ALLOW FILTERING;

SELECT hotel_id, status, room_number, room_type, price_per_night, capacity 
FROM rooms_by_hotel_status 
WHERE hotel_id = 'HTL016' ALLOW FILTERING;
