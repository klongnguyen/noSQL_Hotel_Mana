-- Seed data for noSQL_Hotel_Mana
-- Mỗi bảng có ít nhất 35 bản ghi.
-- Có thể chạy lại file: INSERT trong Cassandra hoạt động theo cơ chế upsert trên cùng primary key.

USE hotel_ks;

-- =====================================================
-- 1) HOTELS: 35 rows
-- =====================================================
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL001', 'Hotel 01 HoChiMinhCity', 'Ho Chi Minh City', '101 Nguyen Van Linh, District 1', 3, '0280000001');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL002', 'Hotel 02 HaNoi', 'Ha Noi', '102 Nguyen Van Linh, Hoan Kiem', 4, '0280000002');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL003', 'Hotel 03 DaNang', 'Da Nang', '103 Nguyen Van Linh, Son Tra', 5, '0280000003');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL004', 'Hotel 04 NhaTrang', 'Nha Trang', '104 Nguyen Van Linh, Loc Tho', 3, '0280000004');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL005', 'Hotel 05 DaLat', 'Da Lat', '105 Nguyen Van Linh, Ward 1', 4, '0280000005');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL006', 'Hotel 06 VungTau', 'Vung Tau', '106 Nguyen Van Linh, Ward 2', 5, '0280000006');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL007', 'Hotel 07 CanTho', 'Can Tho', '107 Nguyen Van Linh, Ninh Kieu', 3, '0280000007');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL008', 'Hotel 08 HoChiMinhCity', 'Ho Chi Minh City', '108 Nguyen Van Linh, District 1', 4, '0280000008');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL009', 'Hotel 09 HaNoi', 'Ha Noi', '109 Nguyen Van Linh, Hoan Kiem', 5, '0280000009');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL010', 'Hotel 10 DaNang', 'Da Nang', '110 Nguyen Van Linh, Son Tra', 3, '0280000010');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL011', 'Hotel 11 NhaTrang', 'Nha Trang', '111 Nguyen Van Linh, Loc Tho', 4, '0280000011');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL012', 'Hotel 12 DaLat', 'Da Lat', '112 Nguyen Van Linh, Ward 1', 5, '0280000012');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL013', 'Hotel 13 VungTau', 'Vung Tau', '113 Nguyen Van Linh, Ward 2', 3, '0280000013');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL014', 'Hotel 14 CanTho', 'Can Tho', '114 Nguyen Van Linh, Ninh Kieu', 4, '0280000014');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL015', 'Hotel 15 HoChiMinhCity', 'Ho Chi Minh City', '115 Nguyen Van Linh, District 1', 5, '0280000015');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL016', 'Hotel 16 HaNoi', 'Ha Noi', '116 Nguyen Van Linh, Hoan Kiem', 3, '0280000016');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL017', 'Hotel 17 DaNang', 'Da Nang', '117 Nguyen Van Linh, Son Tra', 4, '0280000017');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL018', 'Hotel 18 NhaTrang', 'Nha Trang', '118 Nguyen Van Linh, Loc Tho', 5, '0280000018');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL019', 'Hotel 19 DaLat', 'Da Lat', '119 Nguyen Van Linh, Ward 1', 3, '0280000019');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL020', 'Hotel 20 VungTau', 'Vung Tau', '120 Nguyen Van Linh, Ward 2', 4, '0280000020');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL021', 'Hotel 21 CanTho', 'Can Tho', '121 Nguyen Van Linh, Ninh Kieu', 5, '0280000021');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL022', 'Hotel 22 HoChiMinhCity', 'Ho Chi Minh City', '122 Nguyen Van Linh, District 1', 3, '0280000022');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL023', 'Hotel 23 HaNoi', 'Ha Noi', '123 Nguyen Van Linh, Hoan Kiem', 4, '0280000023');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL024', 'Hotel 24 DaNang', 'Da Nang', '124 Nguyen Van Linh, Son Tra', 5, '0280000024');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL025', 'Hotel 25 NhaTrang', 'Nha Trang', '125 Nguyen Van Linh, Loc Tho', 3, '0280000025');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL026', 'Hotel 26 DaLat', 'Da Lat', '126 Nguyen Van Linh, Ward 1', 4, '0280000026');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL027', 'Hotel 27 VungTau', 'Vung Tau', '127 Nguyen Van Linh, Ward 2', 5, '0280000027');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL028', 'Hotel 28 CanTho', 'Can Tho', '128 Nguyen Van Linh, Ninh Kieu', 3, '0280000028');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL029', 'Hotel 29 HoChiMinhCity', 'Ho Chi Minh City', '129 Nguyen Van Linh, District 1', 4, '0280000029');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL030', 'Hotel 30 HaNoi', 'Ha Noi', '130 Nguyen Van Linh, Hoan Kiem', 5, '0280000030');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL031', 'Hotel 31 DaNang', 'Da Nang', '131 Nguyen Van Linh, Son Tra', 3, '0280000031');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL032', 'Hotel 32 NhaTrang', 'Nha Trang', '132 Nguyen Van Linh, Loc Tho', 4, '0280000032');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL033', 'Hotel 33 DaLat', 'Da Lat', '133 Nguyen Van Linh, Ward 1', 5, '0280000033');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL034', 'Hotel 34 VungTau', 'Vung Tau', '134 Nguyen Van Linh, Ward 2', 3, '0280000034');
INSERT INTO hotels (hotel_id, hotel_name, city, address, star_rating, phone) VALUES ('HTL035', 'Hotel 35 CanTho', 'Can Tho', '135 Nguyen Van Linh, Ninh Kieu', 4, '0280000035');

-- =====================================================
-- 2) ROOMS_BY_HOTEL: 35 rows
-- =====================================================
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL001', 101, 'Standard', 800000, 'AVAILABLE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL002', 102, 'Deluxe', 1200000, 'OCCUPIED');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL003', 103, 'Suite', 2500000, 'MAINTENANCE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL004', 104, 'Standard', 800000, 'AVAILABLE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL005', 105, 'Deluxe', 1200000, 'OCCUPIED');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL006', 106, 'Suite', 2500000, 'MAINTENANCE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL007', 107, 'Standard', 800000, 'AVAILABLE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL008', 108, 'Deluxe', 1200000, 'OCCUPIED');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL009', 109, 'Suite', 2500000, 'MAINTENANCE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL010', 110, 'Standard', 800000, 'AVAILABLE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL011', 111, 'Deluxe', 1200000, 'OCCUPIED');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL012', 112, 'Suite', 2500000, 'MAINTENANCE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL013', 113, 'Standard', 800000, 'AVAILABLE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL014', 114, 'Deluxe', 1200000, 'OCCUPIED');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL015', 115, 'Suite', 2500000, 'MAINTENANCE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL016', 116, 'Standard', 800000, 'AVAILABLE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL017', 117, 'Deluxe', 1200000, 'OCCUPIED');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL018', 118, 'Suite', 2500000, 'MAINTENANCE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL019', 119, 'Standard', 800000, 'AVAILABLE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL020', 120, 'Deluxe', 1200000, 'OCCUPIED');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL021', 121, 'Suite', 2500000, 'MAINTENANCE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL022', 122, 'Standard', 800000, 'AVAILABLE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL023', 123, 'Deluxe', 1200000, 'OCCUPIED');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL024', 124, 'Suite', 2500000, 'MAINTENANCE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL025', 125, 'Standard', 800000, 'AVAILABLE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL026', 126, 'Deluxe', 1200000, 'OCCUPIED');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL027', 127, 'Suite', 2500000, 'MAINTENANCE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL028', 128, 'Standard', 800000, 'AVAILABLE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL029', 129, 'Deluxe', 1200000, 'OCCUPIED');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL030', 130, 'Suite', 2500000, 'MAINTENANCE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL031', 131, 'Standard', 800000, 'AVAILABLE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL032', 132, 'Deluxe', 1200000, 'OCCUPIED');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL033', 133, 'Suite', 2500000, 'MAINTENANCE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL034', 134, 'Standard', 800000, 'AVAILABLE');
INSERT INTO rooms_by_hotel (hotel_id, room_number, room_type, price_per_night, status) VALUES ('HTL035', 135, 'Deluxe', 1200000, 'OCCUPIED');

-- =====================================================
-- 3) ROOMS_BY_HOTEL_STATUS: 35 rows (mirror of rooms_by_hotel)
-- =====================================================
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL001', 'AVAILABLE', 101, 'Standard', 800000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL002', 'OCCUPIED', 102, 'Deluxe', 1200000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL003', 'MAINTENANCE', 103, 'Suite', 2500000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL004', 'AVAILABLE', 104, 'Standard', 800000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL005', 'OCCUPIED', 105, 'Deluxe', 1200000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL006', 'MAINTENANCE', 106, 'Suite', 2500000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL007', 'AVAILABLE', 107, 'Standard', 800000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL008', 'OCCUPIED', 108, 'Deluxe', 1200000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL009', 'MAINTENANCE', 109, 'Suite', 2500000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL010', 'AVAILABLE', 110, 'Standard', 800000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL011', 'OCCUPIED', 111, 'Deluxe', 1200000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL012', 'MAINTENANCE', 112, 'Suite', 2500000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL013', 'AVAILABLE', 113, 'Standard', 800000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL014', 'OCCUPIED', 114, 'Deluxe', 1200000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL015', 'MAINTENANCE', 115, 'Suite', 2500000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL016', 'AVAILABLE', 116, 'Standard', 800000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL017', 'OCCUPIED', 117, 'Deluxe', 1200000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL018', 'MAINTENANCE', 118, 'Suite', 2500000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL019', 'AVAILABLE', 119, 'Standard', 800000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL020', 'OCCUPIED', 120, 'Deluxe', 1200000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL021', 'MAINTENANCE', 121, 'Suite', 2500000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL022', 'AVAILABLE', 122, 'Standard', 800000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL023', 'OCCUPIED', 123, 'Deluxe', 1200000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL024', 'MAINTENANCE', 124, 'Suite', 2500000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL025', 'AVAILABLE', 125, 'Standard', 800000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL026', 'OCCUPIED', 126, 'Deluxe', 1200000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL027', 'MAINTENANCE', 127, 'Suite', 2500000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL028', 'AVAILABLE', 128, 'Standard', 800000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL029', 'OCCUPIED', 129, 'Deluxe', 1200000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL030', 'MAINTENANCE', 130, 'Suite', 2500000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL031', 'AVAILABLE', 131, 'Standard', 800000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL032', 'OCCUPIED', 132, 'Deluxe', 1200000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL033', 'MAINTENANCE', 133, 'Suite', 2500000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL034', 'AVAILABLE', 134, 'Standard', 800000);
INSERT INTO rooms_by_hotel_status (hotel_id, status, room_number, room_type, price_per_night) VALUES ('HTL035', 'OCCUPIED', 135, 'Deluxe', 1200000);

-- =====================================================
-- 4) GUESTS: 35 rows
-- =====================================================
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST001', 'Guest Demo 01', '0910000001', 'guest001@gmail.com', '079200000001');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST002', 'Guest Demo 02', '0910000002', 'guest002@gmail.com', '079200000002');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST003', 'Guest Demo 03', '0910000003', 'guest003@gmail.com', '079200000003');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST004', 'Guest Demo 04', '0910000004', 'guest004@gmail.com', '079200000004');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST005', 'Guest Demo 05', '0910000005', 'guest005@gmail.com', '079200000005');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST006', 'Guest Demo 06', '0910000006', 'guest006@gmail.com', '079200000006');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST007', 'Guest Demo 07', '0910000007', 'guest007@gmail.com', '079200000007');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST008', 'Guest Demo 08', '0910000008', 'guest008@gmail.com', '079200000008');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST009', 'Guest Demo 09', '0910000009', 'guest009@gmail.com', '079200000009');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST010', 'Guest Demo 10', '0910000010', 'guest010@gmail.com', '079200000010');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST011', 'Guest Demo 11', '0910000011', 'guest011@gmail.com', '079200000011');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST012', 'Guest Demo 12', '0910000012', 'guest012@gmail.com', '079200000012');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST013', 'Guest Demo 13', '0910000013', 'guest013@gmail.com', '079200000013');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST014', 'Guest Demo 14', '0910000014', 'guest014@gmail.com', '079200000014');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST015', 'Guest Demo 15', '0910000015', 'guest015@gmail.com', '079200000015');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST016', 'Guest Demo 16', '0910000016', 'guest016@gmail.com', '079200000016');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST017', 'Guest Demo 17', '0910000017', 'guest017@gmail.com', '079200000017');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST018', 'Guest Demo 18', '0910000018', 'guest018@gmail.com', '079200000018');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST019', 'Guest Demo 19', '0910000019', 'guest019@gmail.com', '079200000019');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST020', 'Guest Demo 20', '0910000020', 'guest020@gmail.com', '079200000020');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST021', 'Guest Demo 21', '0910000021', 'guest021@gmail.com', '079200000021');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST022', 'Guest Demo 22', '0910000022', 'guest022@gmail.com', '079200000022');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST023', 'Guest Demo 23', '0910000023', 'guest023@gmail.com', '079200000023');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST024', 'Guest Demo 24', '0910000024', 'guest024@gmail.com', '079200000024');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST025', 'Guest Demo 25', '0910000025', 'guest025@gmail.com', '079200000025');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST026', 'Guest Demo 26', '0910000026', 'guest026@gmail.com', '079200000026');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST027', 'Guest Demo 27', '0910000027', 'guest027@gmail.com', '079200000027');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST028', 'Guest Demo 28', '0910000028', 'guest028@gmail.com', '079200000028');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST029', 'Guest Demo 29', '0910000029', 'guest029@gmail.com', '079200000029');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST030', 'Guest Demo 30', '0910000030', 'guest030@gmail.com', '079200000030');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST031', 'Guest Demo 31', '0910000031', 'guest031@gmail.com', '079200000031');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST032', 'Guest Demo 32', '0910000032', 'guest032@gmail.com', '079200000032');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST033', 'Guest Demo 33', '0910000033', 'guest033@gmail.com', '079200000033');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST034', 'Guest Demo 34', '0910000034', 'guest034@gmail.com', '079200000034');
INSERT INTO guests (guest_id, full_name, phone, email, national_id) VALUES ('GUEST035', 'Guest Demo 35', '0910000035', 'guest035@gmail.com', '079200000035');

-- =====================================================
-- 5) BOOKINGS_BY_GUEST: 35 rows
-- =====================================================
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST001', '2026-10-01', e58cccdd-ef47-5e15-8f54-afe3fc8fa3cc, 'HTL001', 'Hotel 01 HoChiMinhCity', 101, '2026-10-02', 800000, 'CONFIRMED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST002', '2026-10-02', 06f25466-020f-547c-989c-67dd62d9bf8b, 'HTL002', 'Hotel 02 HaNoi', 102, '2026-10-04', 2400000, 'CHECKED_IN');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST003', '2026-10-03', 6ac74fd5-c115-56b5-a2f5-c8df947deba6, 'HTL003', 'Hotel 03 DaNang', 103, '2026-10-06', 7500000, 'CHECKED_OUT');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST004', '2026-10-04', febb88e8-9900-55fc-b752-e0107a6e1feb, 'HTL004', 'Hotel 04 NhaTrang', 104, '2026-10-05', 800000, 'CANCELLED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST005', '2026-10-05', 772301c3-24c0-57a3-a57f-0b056c024fc2, 'HTL005', 'Hotel 05 DaLat', 105, '2026-10-07', 2400000, 'CONFIRMED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST006', '2026-10-06', a5d9a6dd-2e70-589d-90de-030e62fbf0db, 'HTL006', 'Hotel 06 VungTau', 106, '2026-10-09', 7500000, 'CHECKED_IN');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST007', '2026-10-07', e481a573-59d1-5b8c-a040-35c852b47f85, 'HTL007', 'Hotel 07 CanTho', 107, '2026-10-08', 800000, 'CHECKED_OUT');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST008', '2026-10-08', 39693cfd-f1b8-5e48-9458-7c5656331ba0, 'HTL008', 'Hotel 08 HoChiMinhCity', 108, '2026-10-10', 2400000, 'CANCELLED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST009', '2026-10-09', 8ef142f1-b28b-5c14-8562-3ed91d36e747, 'HTL009', 'Hotel 09 HaNoi', 109, '2026-10-12', 7500000, 'CONFIRMED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST010', '2026-10-10', 47a8b09a-816e-5adf-a956-30a045cef9dc, 'HTL010', 'Hotel 10 DaNang', 110, '2026-10-11', 800000, 'CHECKED_IN');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST011', '2026-10-11', 5ccbf1e7-d721-5df3-8ad6-05818c6759c5, 'HTL011', 'Hotel 11 NhaTrang', 111, '2026-10-13', 2400000, 'CHECKED_OUT');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST012', '2026-10-12', e0b7ee16-a328-56b5-bd16-3a2e8def2ee6, 'HTL012', 'Hotel 12 DaLat', 112, '2026-10-15', 7500000, 'CANCELLED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST013', '2026-10-13', 606c04d3-ce9f-5406-952c-30644a70a326, 'HTL013', 'Hotel 13 VungTau', 113, '2026-10-14', 800000, 'CONFIRMED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST014', '2026-10-14', aa186394-4668-5bc1-99a9-84482a0b7c33, 'HTL014', 'Hotel 14 CanTho', 114, '2026-10-16', 2400000, 'CHECKED_IN');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST015', '2026-10-15', f29db15c-abf0-5282-8a68-f95415e0c9c5, 'HTL015', 'Hotel 15 HoChiMinhCity', 115, '2026-10-18', 7500000, 'CHECKED_OUT');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST016', '2026-10-16', 245d1d53-e81f-5d0a-91df-8e5b65522301, 'HTL016', 'Hotel 16 HaNoi', 116, '2026-10-17', 800000, 'CANCELLED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST017', '2026-10-17', e9e47d80-1711-59a7-8779-b86182aafcb3, 'HTL017', 'Hotel 17 DaNang', 117, '2026-10-19', 2400000, 'CONFIRMED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST018', '2026-10-18', 1cecfc82-be09-5ae2-9b35-e3655d80a62f, 'HTL018', 'Hotel 18 NhaTrang', 118, '2026-10-21', 7500000, 'CHECKED_IN');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST019', '2026-10-19', 35a96c73-21a5-55a1-be7f-532c7b8dd949, 'HTL019', 'Hotel 19 DaLat', 119, '2026-10-20', 800000, 'CHECKED_OUT');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST020', '2026-10-20', a11bb652-6573-5caa-9786-6086f0d29307, 'HTL020', 'Hotel 20 VungTau', 120, '2026-10-22', 2400000, 'CANCELLED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST021', '2026-10-21', 5a7d8626-6b33-5931-8fa1-1ace3859877b, 'HTL021', 'Hotel 21 CanTho', 121, '2026-10-24', 7500000, 'CONFIRMED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST022', '2026-10-22', d2bc7d0b-d87b-5a7b-bbf2-7db03af639bf, 'HTL022', 'Hotel 22 HoChiMinhCity', 122, '2026-10-23', 800000, 'CHECKED_IN');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST023', '2026-10-23', 4620d1e4-05e6-5c11-8bd3-7327b0d9af91, 'HTL023', 'Hotel 23 HaNoi', 123, '2026-10-25', 2400000, 'CHECKED_OUT');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST024', '2026-10-24', 57f40a78-970b-5c9a-a323-a25bc0fe6374, 'HTL024', 'Hotel 24 DaNang', 124, '2026-10-27', 7500000, 'CANCELLED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST025', '2026-10-25', af18c176-c5b4-5983-a2e5-00d983b5f70e, 'HTL025', 'Hotel 25 NhaTrang', 125, '2026-10-26', 800000, 'CONFIRMED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST026', '2026-10-26', 20069455-6211-5460-b02e-9dcb8ef6079a, 'HTL026', 'Hotel 26 DaLat', 126, '2026-10-28', 2400000, 'CHECKED_IN');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST027', '2026-10-27', 235c8593-f528-520a-bc52-ab96cf528be8, 'HTL027', 'Hotel 27 VungTau', 127, '2026-10-30', 7500000, 'CHECKED_OUT');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST028', '2026-10-28', 8c7efcfd-7bc9-5226-b7b5-8a021d38d3b8, 'HTL028', 'Hotel 28 CanTho', 128, '2026-10-29', 800000, 'CANCELLED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST029', '2026-10-29', eb69a9b4-3b61-5d86-bcbd-1c5c3f65038c, 'HTL029', 'Hotel 29 HoChiMinhCity', 129, '2026-10-31', 2400000, 'CONFIRMED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST030', '2026-10-30', 3a142e6f-db95-5d9e-8dcc-30fa654a829c, 'HTL030', 'Hotel 30 HaNoi', 130, '2026-11-02', 7500000, 'CHECKED_IN');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST031', '2026-10-31', 4ad62146-506b-5a8f-b507-370fe59fa85e, 'HTL031', 'Hotel 31 DaNang', 131, '2026-11-01', 800000, 'CHECKED_OUT');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST032', '2026-11-01', c1aed183-3d8f-5e22-9bdb-5a12f77877c4, 'HTL032', 'Hotel 32 NhaTrang', 132, '2026-11-03', 2400000, 'CANCELLED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST033', '2026-11-02', 1846c38f-bce4-59f0-b91a-fc5ca1c30716, 'HTL033', 'Hotel 33 DaLat', 133, '2026-11-05', 7500000, 'CONFIRMED');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST034', '2026-11-03', e912c2ac-94f3-5508-af02-f9f097825d98, 'HTL034', 'Hotel 34 VungTau', 134, '2026-11-04', 800000, 'CHECKED_IN');
INSERT INTO bookings_by_guest (guest_id, check_in_date, booking_id, hotel_id, hotel_name, room_number, check_out_date, total_amount, status) VALUES ('GUEST035', '2026-11-04', 563d138c-6a06-5534-b986-b67c1f047d4e, 'HTL035', 'Hotel 35 CanTho', 135, '2026-11-06', 2400000, 'CHECKED_OUT');

-- =====================================================
-- 6) BOOKINGS_BY_HOTEL_DATE: 35 rows (same booking_id as bookings_by_guest)
-- =====================================================
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL001', '2026-10-01', e58cccdd-ef47-5e15-8f54-afe3fc8fa3cc, 'GUEST001', 'Guest Demo 01', 101, '2026-10-02', 800000, 'CONFIRMED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL002', '2026-10-02', 06f25466-020f-547c-989c-67dd62d9bf8b, 'GUEST002', 'Guest Demo 02', 102, '2026-10-04', 2400000, 'CHECKED_IN');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL003', '2026-10-03', 6ac74fd5-c115-56b5-a2f5-c8df947deba6, 'GUEST003', 'Guest Demo 03', 103, '2026-10-06', 7500000, 'CHECKED_OUT');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL004', '2026-10-04', febb88e8-9900-55fc-b752-e0107a6e1feb, 'GUEST004', 'Guest Demo 04', 104, '2026-10-05', 800000, 'CANCELLED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL005', '2026-10-05', 772301c3-24c0-57a3-a57f-0b056c024fc2, 'GUEST005', 'Guest Demo 05', 105, '2026-10-07', 2400000, 'CONFIRMED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL006', '2026-10-06', a5d9a6dd-2e70-589d-90de-030e62fbf0db, 'GUEST006', 'Guest Demo 06', 106, '2026-10-09', 7500000, 'CHECKED_IN');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL007', '2026-10-07', e481a573-59d1-5b8c-a040-35c852b47f85, 'GUEST007', 'Guest Demo 07', 107, '2026-10-08', 800000, 'CHECKED_OUT');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL008', '2026-10-08', 39693cfd-f1b8-5e48-9458-7c5656331ba0, 'GUEST008', 'Guest Demo 08', 108, '2026-10-10', 2400000, 'CANCELLED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL009', '2026-10-09', 8ef142f1-b28b-5c14-8562-3ed91d36e747, 'GUEST009', 'Guest Demo 09', 109, '2026-10-12', 7500000, 'CONFIRMED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL010', '2026-10-10', 47a8b09a-816e-5adf-a956-30a045cef9dc, 'GUEST010', 'Guest Demo 10', 110, '2026-10-11', 800000, 'CHECKED_IN');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL011', '2026-10-11', 5ccbf1e7-d721-5df3-8ad6-05818c6759c5, 'GUEST011', 'Guest Demo 11', 111, '2026-10-13', 2400000, 'CHECKED_OUT');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL012', '2026-10-12', e0b7ee16-a328-56b5-bd16-3a2e8def2ee6, 'GUEST012', 'Guest Demo 12', 112, '2026-10-15', 7500000, 'CANCELLED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL013', '2026-10-13', 606c04d3-ce9f-5406-952c-30644a70a326, 'GUEST013', 'Guest Demo 13', 113, '2026-10-14', 800000, 'CONFIRMED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL014', '2026-10-14', aa186394-4668-5bc1-99a9-84482a0b7c33, 'GUEST014', 'Guest Demo 14', 114, '2026-10-16', 2400000, 'CHECKED_IN');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL015', '2026-10-15', f29db15c-abf0-5282-8a68-f95415e0c9c5, 'GUEST015', 'Guest Demo 15', 115, '2026-10-18', 7500000, 'CHECKED_OUT');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL016', '2026-10-16', 245d1d53-e81f-5d0a-91df-8e5b65522301, 'GUEST016', 'Guest Demo 16', 116, '2026-10-17', 800000, 'CANCELLED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL017', '2026-10-17', e9e47d80-1711-59a7-8779-b86182aafcb3, 'GUEST017', 'Guest Demo 17', 117, '2026-10-19', 2400000, 'CONFIRMED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL018', '2026-10-18', 1cecfc82-be09-5ae2-9b35-e3655d80a62f, 'GUEST018', 'Guest Demo 18', 118, '2026-10-21', 7500000, 'CHECKED_IN');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL019', '2026-10-19', 35a96c73-21a5-55a1-be7f-532c7b8dd949, 'GUEST019', 'Guest Demo 19', 119, '2026-10-20', 800000, 'CHECKED_OUT');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL020', '2026-10-20', a11bb652-6573-5caa-9786-6086f0d29307, 'GUEST020', 'Guest Demo 20', 120, '2026-10-22', 2400000, 'CANCELLED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL021', '2026-10-21', 5a7d8626-6b33-5931-8fa1-1ace3859877b, 'GUEST021', 'Guest Demo 21', 121, '2026-10-24', 7500000, 'CONFIRMED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL022', '2026-10-22', d2bc7d0b-d87b-5a7b-bbf2-7db03af639bf, 'GUEST022', 'Guest Demo 22', 122, '2026-10-23', 800000, 'CHECKED_IN');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL023', '2026-10-23', 4620d1e4-05e6-5c11-8bd3-7327b0d9af91, 'GUEST023', 'Guest Demo 23', 123, '2026-10-25', 2400000, 'CHECKED_OUT');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL024', '2026-10-24', 57f40a78-970b-5c9a-a323-a25bc0fe6374, 'GUEST024', 'Guest Demo 24', 124, '2026-10-27', 7500000, 'CANCELLED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL025', '2026-10-25', af18c176-c5b4-5983-a2e5-00d983b5f70e, 'GUEST025', 'Guest Demo 25', 125, '2026-10-26', 800000, 'CONFIRMED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL026', '2026-10-26', 20069455-6211-5460-b02e-9dcb8ef6079a, 'GUEST026', 'Guest Demo 26', 126, '2026-10-28', 2400000, 'CHECKED_IN');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL027', '2026-10-27', 235c8593-f528-520a-bc52-ab96cf528be8, 'GUEST027', 'Guest Demo 27', 127, '2026-10-30', 7500000, 'CHECKED_OUT');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL028', '2026-10-28', 8c7efcfd-7bc9-5226-b7b5-8a021d38d3b8, 'GUEST028', 'Guest Demo 28', 128, '2026-10-29', 800000, 'CANCELLED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL029', '2026-10-29', eb69a9b4-3b61-5d86-bcbd-1c5c3f65038c, 'GUEST029', 'Guest Demo 29', 129, '2026-10-31', 2400000, 'CONFIRMED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL030', '2026-10-30', 3a142e6f-db95-5d9e-8dcc-30fa654a829c, 'GUEST030', 'Guest Demo 30', 130, '2026-11-02', 7500000, 'CHECKED_IN');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL031', '2026-10-31', 4ad62146-506b-5a8f-b507-370fe59fa85e, 'GUEST031', 'Guest Demo 31', 131, '2026-11-01', 800000, 'CHECKED_OUT');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL032', '2026-11-01', c1aed183-3d8f-5e22-9bdb-5a12f77877c4, 'GUEST032', 'Guest Demo 32', 132, '2026-11-03', 2400000, 'CANCELLED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL033', '2026-11-02', 1846c38f-bce4-59f0-b91a-fc5ca1c30716, 'GUEST033', 'Guest Demo 33', 133, '2026-11-05', 7500000, 'CONFIRMED');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL034', '2026-11-03', e912c2ac-94f3-5508-af02-f9f097825d98, 'GUEST034', 'Guest Demo 34', 134, '2026-11-04', 800000, 'CHECKED_IN');
INSERT INTO bookings_by_hotel_date (hotel_id, check_in_date, booking_id, guest_id, guest_name, room_number, check_out_date, total_amount, status) VALUES ('HTL035', '2026-11-04', 563d138c-6a06-5534-b986-b67c1f047d4e, 'GUEST035', 'Guest Demo 35', 135, '2026-11-06', 2400000, 'CHECKED_OUT');

-- =====================================================
-- 7) INVOICES_BY_BOOKING: 35 rows
-- =====================================================
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (e58cccdd-ef47-5e15-8f54-afe3fc8fa3cc, 8b460d8b-8920-50bf-ba79-6d0fc0a4f5a1, 'GUEST001', 'HTL001', 800000, 100000, 64000, 964000, 'UNPAID', '2026-10-01T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (06f25466-020f-547c-989c-67dd62d9bf8b, 91895f33-5ca8-5236-84cc-5ca046bfbd26, 'GUEST002', 'HTL002', 2400000, 120000, 192000, 2712000, 'PAID', '2026-10-02T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (6ac74fd5-c115-56b5-a2f5-c8df947deba6, a68886e5-2d76-542a-b94f-f56849c1038f, 'GUEST003', 'HTL003', 7500000, 140000, 600000, 8240000, 'REFUNDED', '2026-10-03T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (febb88e8-9900-55fc-b752-e0107a6e1feb, 94c24067-41c1-5d9b-b162-f1558ad5f26d, 'GUEST004', 'HTL004', 800000, 160000, 64000, 1024000, 'UNPAID', '2026-10-04T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (772301c3-24c0-57a3-a57f-0b056c024fc2, f6677506-7df3-5072-a94c-8802ef906182, 'GUEST005', 'HTL005', 2400000, 180000, 192000, 2772000, 'PAID', '2026-10-05T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (a5d9a6dd-2e70-589d-90de-030e62fbf0db, 333728b1-7625-5992-b9f3-09c8ce66ed4f, 'GUEST006', 'HTL006', 7500000, 100000, 600000, 8200000, 'REFUNDED', '2026-10-06T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (e481a573-59d1-5b8c-a040-35c852b47f85, 17a8f7ca-e9d0-5372-bf2b-f67dd05be192, 'GUEST007', 'HTL007', 800000, 120000, 64000, 984000, 'UNPAID', '2026-10-07T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (39693cfd-f1b8-5e48-9458-7c5656331ba0, c18bd6ae-8819-5d95-b8cf-6e2acd76a391, 'GUEST008', 'HTL008', 2400000, 140000, 192000, 2732000, 'PAID', '2026-10-08T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (8ef142f1-b28b-5c14-8562-3ed91d36e747, 84ec8e80-a3b3-5599-9364-3a7979cd4f70, 'GUEST009', 'HTL009', 7500000, 160000, 600000, 8260000, 'REFUNDED', '2026-10-09T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (47a8b09a-816e-5adf-a956-30a045cef9dc, 879b9c24-30ad-5980-8467-a141bf455719, 'GUEST010', 'HTL010', 800000, 180000, 64000, 1044000, 'UNPAID', '2026-10-10T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (5ccbf1e7-d721-5df3-8ad6-05818c6759c5, 1ed6709b-d43e-5892-a50c-cfa81e56e1c2, 'GUEST011', 'HTL011', 2400000, 100000, 192000, 2692000, 'PAID', '2026-10-11T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (e0b7ee16-a328-56b5-bd16-3a2e8def2ee6, a6aecd88-6473-50b1-b173-f6a9becb657c, 'GUEST012', 'HTL012', 7500000, 120000, 600000, 8220000, 'REFUNDED', '2026-10-12T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (606c04d3-ce9f-5406-952c-30644a70a326, 6cb90dcd-151d-5680-94cb-c43cf3357131, 'GUEST013', 'HTL013', 800000, 140000, 64000, 1004000, 'UNPAID', '2026-10-13T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (aa186394-4668-5bc1-99a9-84482a0b7c33, 8471dbd6-4d99-5bff-99b1-780e8289a1c7, 'GUEST014', 'HTL014', 2400000, 160000, 192000, 2752000, 'PAID', '2026-10-14T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (f29db15c-abf0-5282-8a68-f95415e0c9c5, 420fbe21-c430-56ec-9ac2-5a29caac0b9a, 'GUEST015', 'HTL015', 7500000, 180000, 600000, 8280000, 'REFUNDED', '2026-10-15T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (245d1d53-e81f-5d0a-91df-8e5b65522301, f2fbfbf9-0917-5344-a08f-4fd53a80e945, 'GUEST016', 'HTL016', 800000, 100000, 64000, 964000, 'UNPAID', '2026-10-16T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (e9e47d80-1711-59a7-8779-b86182aafcb3, 8b495c0e-d30f-5670-bda1-02c9151527a5, 'GUEST017', 'HTL017', 2400000, 120000, 192000, 2712000, 'PAID', '2026-10-17T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (1cecfc82-be09-5ae2-9b35-e3655d80a62f, e02506b2-2777-5290-8772-c931f10393ee, 'GUEST018', 'HTL018', 7500000, 140000, 600000, 8240000, 'REFUNDED', '2026-10-18T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (35a96c73-21a5-55a1-be7f-532c7b8dd949, 9270df34-c9bc-50af-9962-3bd3eb9b0ddc, 'GUEST019', 'HTL019', 800000, 160000, 64000, 1024000, 'UNPAID', '2026-10-19T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (a11bb652-6573-5caa-9786-6086f0d29307, 03dac2c0-da1f-533e-be45-b927ec4173c5, 'GUEST020', 'HTL020', 2400000, 180000, 192000, 2772000, 'PAID', '2026-10-20T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (5a7d8626-6b33-5931-8fa1-1ace3859877b, b72d9843-a601-535a-957e-34bac0620041, 'GUEST021', 'HTL021', 7500000, 100000, 600000, 8200000, 'REFUNDED', '2026-10-21T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (d2bc7d0b-d87b-5a7b-bbf2-7db03af639bf, 60a29436-f769-55b5-8c5a-15130d2fcaf5, 'GUEST022', 'HTL022', 800000, 120000, 64000, 984000, 'UNPAID', '2026-10-22T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (4620d1e4-05e6-5c11-8bd3-7327b0d9af91, 877d5fd1-deb7-50f7-a956-54365818298c, 'GUEST023', 'HTL023', 2400000, 140000, 192000, 2732000, 'PAID', '2026-10-23T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (57f40a78-970b-5c9a-a323-a25bc0fe6374, 8eb68a44-c40a-5f26-8cae-6b595de1a562, 'GUEST024', 'HTL024', 7500000, 160000, 600000, 8260000, 'REFUNDED', '2026-10-24T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (af18c176-c5b4-5983-a2e5-00d983b5f70e, db18b530-5029-56a4-a3a1-a4c04f935c26, 'GUEST025', 'HTL025', 800000, 180000, 64000, 1044000, 'UNPAID', '2026-10-25T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (20069455-6211-5460-b02e-9dcb8ef6079a, 38d09738-831f-545f-86ab-1d48d1c54cef, 'GUEST026', 'HTL026', 2400000, 100000, 192000, 2692000, 'PAID', '2026-10-26T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (235c8593-f528-520a-bc52-ab96cf528be8, 3783d587-b643-5cde-bd08-1be6f6350610, 'GUEST027', 'HTL027', 7500000, 120000, 600000, 8220000, 'REFUNDED', '2026-10-27T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (8c7efcfd-7bc9-5226-b7b5-8a021d38d3b8, 42e17a3b-f54f-5cff-a274-2175313a12d6, 'GUEST028', 'HTL028', 800000, 140000, 64000, 1004000, 'UNPAID', '2026-10-28T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (eb69a9b4-3b61-5d86-bcbd-1c5c3f65038c, aaf5eabd-732e-5869-accf-c43afe62009f, 'GUEST029', 'HTL029', 2400000, 160000, 192000, 2752000, 'PAID', '2026-10-01T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (3a142e6f-db95-5d9e-8dcc-30fa654a829c, 57610a07-d3f1-537c-bf9c-8bf6bc30c8b1, 'GUEST030', 'HTL030', 7500000, 180000, 600000, 8280000, 'REFUNDED', '2026-10-02T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (4ad62146-506b-5a8f-b507-370fe59fa85e, a9a3293d-4d6d-58af-a7ab-41d4013e5351, 'GUEST031', 'HTL031', 800000, 100000, 64000, 964000, 'UNPAID', '2026-10-03T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (c1aed183-3d8f-5e22-9bdb-5a12f77877c4, 646032e2-af22-5adb-97e7-c1af1e2d6b75, 'GUEST032', 'HTL032', 2400000, 120000, 192000, 2712000, 'PAID', '2026-10-04T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (1846c38f-bce4-59f0-b91a-fc5ca1c30716, 1947176f-da42-5e40-b923-61b1b1788c11, 'GUEST033', 'HTL033', 7500000, 140000, 600000, 8240000, 'REFUNDED', '2026-10-05T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (e912c2ac-94f3-5508-af02-f9f097825d98, 742d1316-0145-5cc5-8f97-dc3329a0793b, 'GUEST034', 'HTL034', 800000, 160000, 64000, 1024000, 'UNPAID', '2026-10-06T10:00:00Z');
INSERT INTO invoices_by_booking (booking_id, invoice_id, guest_id, hotel_id, room_charge, service_charge, tax, total_amount, payment_status, issued_at) VALUES (563d138c-6a06-5534-b986-b67c1f047d4e, 4eaaf822-487e-5c43-8cb8-a254ebdf80c4, 'GUEST035', 'HTL035', 2400000, 180000, 192000, 2772000, 'PAID', '2026-10-07T10:00:00Z');

-- =====================================================
-- VALIDATION
-- =====================================================
SELECT COUNT(*) FROM hotels;
SELECT COUNT(*) FROM rooms_by_hotel;
SELECT COUNT(*) FROM rooms_by_hotel_status;
SELECT COUNT(*) FROM guests;
SELECT COUNT(*) FROM bookings_by_guest;
SELECT COUNT(*) FROM bookings_by_hotel_date;
SELECT COUNT(*) FROM invoices_by_booking;
