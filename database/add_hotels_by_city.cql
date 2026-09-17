-- ============================================================================
-- SCRIPT TẠO BẢNG VÀ SEED DỮ LIỆU BẢNG: hotels_by_city
-- Mục đích: Hỗ trợ truy vấn Query-First "Tìm khách sạn theo thành phố và số sao"
-- Thiết kế:
--   - Partition Key: city (phân tán theo thành phố)
--   - Clustering Column 1: star_rating (sắp xếp giảm dần để ưu tiên sao cao)
--   - Clustering Column 2: hotel_id (định danh duy nhất khách sạn)
-- ============================================================================

USE hotel_ks;

-- 1. Tạo bảng
CREATE TABLE IF NOT EXISTS hotels_by_city (
    city text,
    star_rating int,
    hotel_id text,
    hotel_name text,
    address text,
    phone text,
    PRIMARY KEY ((city), star_rating, hotel_id)
) WITH CLUSTERING ORDER BY (star_rating DESC, hotel_id ASC);

-- 2. Seed 35 bản ghi đồng bộ với bảng hotels
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Ho Chi Minh City', 3, 'HTL001', 'Hotel 01 HoChiMinhCity', '101 Nguyen Van Linh, District 1', '0280000001');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Ha Noi', 4, 'HTL002', 'Hotel 02 HaNoi', '102 Nguyen Van Linh, Hoan Kiem', '0280000002');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Da Nang', 5, 'HTL003', 'Hotel 03 DaNang', '103 Nguyen Van Linh, Son Tra', '0280000003');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Nha Trang', 3, 'HTL004', 'Hotel 04 NhaTrang', '104 Nguyen Van Linh, Loc Tho', '0280000004');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Da Lat', 4, 'HTL005', 'Hotel 05 DaLat', '105 Nguyen Van Linh, Ward 1', '0280000005');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Vung Tau', 5, 'HTL006', 'Hotel 06 VungTau', '106 Nguyen Van Linh, Ward 2', '0280000006');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Can Tho', 3, 'HTL007', 'Hotel 07 CanTho', '107 Nguyen Van Linh, Ninh Kieu', '0280000007');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Ho Chi Minh City', 4, 'HTL008', 'Hotel 08 HoChiMinhCity', '108 Nguyen Van Linh, District 1', '0280000008');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Ha Noi', 5, 'HTL009', 'Hotel 09 HaNoi', '109 Nguyen Van Linh, Hoan Kiem', '0280000009');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Da Nang', 3, 'HTL010', 'Hotel 10 DaNang', '110 Nguyen Van Linh, Son Tra', '0280000010');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Nha Trang', 4, 'HTL011', 'Hotel 11 NhaTrang', '111 Nguyen Van Linh, Loc Tho', '0280000011');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Da Lat', 5, 'HTL012', 'Hotel 12 DaLat', '112 Nguyen Van Linh, Ward 1', '0280000012');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Vung Tau', 3, 'HTL013', 'Hotel 13 VungTau', '113 Nguyen Van Linh, Ward 2', '0280000013');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Can Tho', 4, 'HTL014', 'Hotel 14 CanTho', '114 Nguyen Van Linh, Ninh Kieu', '0280000014');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Ho Chi Minh City', 5, 'HTL015', 'Hotel 15 HoChiMinhCity', '115 Nguyen Van Linh, District 1', '0280000015');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Ha Noi', 3, 'HTL016', 'Hotel 16 HaNoi', '116 Nguyen Van Linh, Hoan Kiem', '0280000016');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Da Nang', 4, 'HTL017', 'Hotel 17 DaNang', '117 Nguyen Van Linh, Son Tra', '0280000017');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Nha Trang', 5, 'HTL018', 'Hotel 18 NhaTrang', '118 Nguyen Van Linh, Loc Tho', '0280000018');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Da Lat', 3, 'HTL019', 'Hotel 19 DaLat', '119 Nguyen Van Linh, Ward 1', '0280000019');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Vung Tau', 4, 'HTL020', 'Hotel 20 VungTau', '120 Nguyen Van Linh, Ward 2', '0280000020');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Can Tho', 5, 'HTL021', 'Hotel 21 CanTho', '121 Nguyen Van Linh, Ninh Kieu', '0280000021');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Ho Chi Minh City', 3, 'HTL022', 'Hotel 22 HoChiMinhCity', '122 Nguyen Van Linh, District 1', '0280000022');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Ha Noi', 4, 'HTL023', 'Hotel 23 HaNoi', '123 Nguyen Van Linh, Hoan Kiem', '0280000023');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Da Nang', 5, 'HTL024', 'Hotel 24 DaNang', '124 Nguyen Van Linh, Son Tra', '0280000024');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Nha Trang', 3, 'HTL025', 'Hotel 25 NhaTrang', '125 Nguyen Van Linh, Loc Tho', '0280000025');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Da Lat', 4, 'HTL026', 'Hotel 26 DaLat', '126 Nguyen Van Linh, Ward 1', '0280000026');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Vung Tau', 5, 'HTL027', 'Hotel 27 VungTau', '127 Nguyen Van Linh, Ward 2', '0280000027');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Can Tho', 3, 'HTL028', 'Hotel 28 CanTho', '128 Nguyen Van Linh, Ninh Kieu', '0280000028');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Ho Chi Minh City', 4, 'HTL029', 'Hotel 29 HoChiMinhCity', '129 Nguyen Van Linh, District 1', '0280000029');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Ha Noi', 5, 'HTL030', 'Hotel 30 HaNoi', '130 Nguyen Van Linh, Hoan Kiem', '0280000030');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Da Nang', 3, 'HTL031', 'Hotel 31 DaNang', '131 Nguyen Van Linh, Son Tra', '0280000031');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Nha Trang', 4, 'HTL032', 'Hotel 32 NhaTrang', '132 Nguyen Van Linh, Loc Tho', '0280000032');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Da Lat', 5, 'HTL033', 'Hotel 33 DaLat', '133 Nguyen Van Linh, Ward 1', '0280000033');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Vung Tau', 3, 'HTL034', 'Hotel 34 VungTau', '134 Nguyen Van Linh, Ward 2', '0280000034');
INSERT INTO hotels_by_city (city, star_rating, hotel_id, hotel_name, address, phone) VALUES ('Can Tho', 4, 'HTL035', 'Hotel 35 CanTho', '135 Nguyen Van Linh, Ninh Kieu', '0280000035');

-- 3. Câu lệnh kiểm tra nhanh sau khi chạy
-- SELECT * FROM hotels_by_city WHERE city = 'Da Nang';
