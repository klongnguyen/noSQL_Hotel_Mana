-- ============================================================================
-- SCRIPT CQL: TẠO BẢNG VÀ SEED DỮ LIỆU THỐNG KÊ DASHBOARD (ASTRA DB / CASSANDRA)
-- ============================================================================
-- Mục đích:
--   1. monthly_revenue_stats   : Thống kê doanh thu theo từng tháng phục vụ biểu đồ xu hướng (Line / Area Chart).
--   2. hotel_revenue_stats     : Thống kê doanh thu và lượt đặt theo chi nhánh phục vụ Top chi nhánh (Horizontal Bar Chart).
--   3. city_guest_density      : Thống kê lượng khách và mật độ theo khu vực/thành phố phục vụ Bản đồ phân bố (Choropleth Map).
--
-- Hướng dẫn chạy trên Astra DB:
--   1. Truy cập DataStax Astra Portal -> Database của bạn -> Tab CQL Console.
--   2. Sao chép và dán toàn bộ nội dung script này vào console và nhấn Enter.
-- ============================================================================

USE hotel_ks;

-- ============================================================================
-- 1. BẢNG THỐNG KÊ DOANH THU THEO THÁNG (monthly_revenue_stats)
-- ============================================================================
CREATE TABLE IF NOT EXISTS monthly_revenue_stats (
    year_month text,              -- Định dạng YYYY-MM (Ví dụ: '2026-05', '2026-06')
    month_name text,              -- Tên hiển thị (Ví dụ: 'Tháng 5', 'Tháng 6')
    total_revenue decimal,        -- Tổng doanh thu (VNĐ)
    booking_count int,            -- Tổng số lượt đặt phòng
    guest_count int,              -- Số khách phục vụ
    PRIMARY KEY (year_month)
);

-- Seed dữ liệu 10 tháng gần nhất năm 2026
INSERT INTO monthly_revenue_stats (year_month, month_name, total_revenue, booking_count, guest_count) 
VALUES ('2026-01', 'Tháng 1', 1450000000, 310, 520);

INSERT INTO monthly_revenue_stats (year_month, month_name, total_revenue, booking_count, guest_count) 
VALUES ('2026-02', 'Tháng 2', 1890000000, 395, 680);

INSERT INTO monthly_revenue_stats (year_month, month_name, total_revenue, booking_count, guest_count) 
VALUES ('2026-03', 'Tháng 3', 1620000000, 340, 590);

INSERT INTO monthly_revenue_stats (year_month, month_name, total_revenue, booking_count, guest_count) 
VALUES ('2026-04', 'Tháng 4', 2150000000, 430, 740);

INSERT INTO monthly_revenue_stats (year_month, month_name, total_revenue, booking_count, guest_count) 
VALUES ('2026-05', 'Tháng 5', 2480000000, 490, 860);

INSERT INTO monthly_revenue_stats (year_month, month_name, total_revenue, booking_count, guest_count) 
VALUES ('2026-06', 'Tháng 6', 3120000000, 620, 1080);

INSERT INTO monthly_revenue_stats (year_month, month_name, total_revenue, booking_count, guest_count) 
VALUES ('2026-07', 'Tháng 7', 3580000000, 710, 1250);

INSERT INTO monthly_revenue_stats (year_month, month_name, total_revenue, booking_count, guest_count) 
VALUES ('2026-08', 'Tháng 8', 3240000000, 645, 1140);

INSERT INTO monthly_revenue_stats (year_month, month_name, total_revenue, booking_count, guest_count) 
VALUES ('2026-09', 'Tháng 9', 2760000000, 530, 930);

INSERT INTO monthly_revenue_stats (year_month, month_name, total_revenue, booking_count, guest_count) 
VALUES ('2026-10', 'Tháng 10', 2950000000, 580, 1020);


-- ============================================================================
-- 2. BẢNG THỐNG KÊ DOANH THU THEO CHI NHÁNH / KHÁCH SẠN (hotel_revenue_stats)
-- ============================================================================
CREATE TABLE IF NOT EXISTS hotel_revenue_stats (
    hotel_id text,                -- Mã khách sạn (Ví dụ: HTL015, HTL003)
    hotel_name text,              -- Tên chi nhánh
    city text,                    -- Thành phố
    star_rating int,              -- Số sao (3, 4, 5)
    total_revenue decimal,        -- Tổng doanh thu lũy kế (VNĐ)
    booking_count int,            -- Số lượt đặt
    occupancy_rate double,        -- Tỷ lệ lấp đầy phòng (%)
    PRIMARY KEY (hotel_id)
);

-- Seed dữ liệu các chi nhánh tiêu biểu (Top doanh thu hàng đầu hệ thống)
INSERT INTO hotel_revenue_stats (hotel_id, hotel_name, city, star_rating, total_revenue, booking_count, occupancy_rate)
VALUES ('HTL015', 'Grand Hotel Ho Chi Minh Center', 'Ho Chi Minh City', 5, 4850000000, 980, 92.5);

INSERT INTO hotel_revenue_stats (hotel_id, hotel_name, city, star_rating, total_revenue, booking_count, occupancy_rate)
VALUES ('HTL009', 'Grand Hotel Ha Noi Old Quarter', 'Ha Noi', 5, 4320000000, 890, 88.0);

INSERT INTO hotel_revenue_stats (hotel_id, hotel_name, city, star_rating, total_revenue, booking_count, occupancy_rate)
VALUES ('HTL003', 'Grand Hotel Da Nang Beachfront', 'Da Nang', 5, 3960000000, 810, 85.2);

INSERT INTO hotel_revenue_stats (hotel_id, hotel_name, city, star_rating, total_revenue, booking_count, occupancy_rate)
VALUES ('HTL018', 'Grand Hotel Nha Trang Bay', 'Nha Trang', 5, 3450000000, 730, 81.0);

INSERT INTO hotel_revenue_stats (hotel_id, hotel_name, city, star_rating, total_revenue, booking_count, occupancy_rate)
VALUES ('HTL006', 'Grand Hotel Vung Tau Resort', 'Vung Tau', 5, 2980000000, 640, 77.4);

INSERT INTO hotel_revenue_stats (hotel_id, hotel_name, city, star_rating, total_revenue, booking_count, occupancy_rate)
VALUES ('HTL012', 'Grand Hotel Da Lat Heritage', 'Da Lat', 5, 2750000000, 590, 74.8);

INSERT INTO hotel_revenue_stats (hotel_id, hotel_name, city, star_rating, total_revenue, booking_count, occupancy_rate)
VALUES ('HTL021', 'Grand Hotel Can Tho Riverside', 'Can Tho', 5, 2180000000, 480, 69.5);

INSERT INTO hotel_revenue_stats (hotel_id, hotel_name, city, star_rating, total_revenue, booking_count, occupancy_rate)
VALUES ('HTL001', 'Hotel 01 HoChiMinhCity', 'Ho Chi Minh City', 3, 1950000000, 430, 66.0);

INSERT INTO hotel_revenue_stats (hotel_id, hotel_name, city, star_rating, total_revenue, booking_count, occupancy_rate)
VALUES ('HTL002', 'Hotel 02 HaNoi', 'Ha Noi', 4, 1820000000, 410, 64.5);

INSERT INTO hotel_revenue_stats (hotel_id, hotel_name, city, star_rating, total_revenue, booking_count, occupancy_rate)
VALUES ('HTL017', 'Hotel 17 DaNang', 'Da Nang', 4, 1650000000, 370, 61.2);


-- ============================================================================
-- 3. BẢNG MẬT ĐỘ LƯỢNG KHÁCH THEO KHU VỰC / THÀNH PHỐ (city_guest_density)
-- ============================================================================
-- Phục vụ bản đồ Choropleth Map hiển thị độ đậm nhạt tương ứng với lượng khách
CREATE TABLE IF NOT EXISTS city_guest_density (
    city text,                    -- Tên thành phố
    guest_count int,              -- Tổng lượng khách lưu trú
    booking_count int,            -- Tổng số đơn đặt phòng
    total_revenue decimal,        -- Tổng doanh thu khu vực (VNĐ)
    percentage_share double,      -- Tỷ trọng lượng khách (%)
    density_rank int,             -- Thứ hạng mật độ (1: cao nhất -> 7)
    density_level text,           -- Cấp độ: 'VERY_HIGH', 'HIGH', 'MEDIUM', 'MODERATE'
    PRIMARY KEY (city)
);

-- Seed số liệu 7 khu vực trọng điểm của chuỗi khách sạn
-- TP.HCM: Mật độ cao nhất (Đậm nhất - Black/Deep Gray)
INSERT INTO city_guest_density (city, guest_count, booking_count, total_revenue, percentage_share, density_rank, density_level)
VALUES ('Ho Chi Minh City', 2850, 1420, 8950000000, 31.5, 1, 'VERY_HIGH');

-- Hà Nội: Mật độ rất cao (Rất đậm - Charcoal Gray)
INSERT INTO city_guest_density (city, guest_count, booking_count, total_revenue, percentage_share, density_rank, density_level)
VALUES ('Ha Noi', 2340, 1180, 7620000000, 25.8, 2, 'VERY_HIGH');

-- Đà Nẵng: Mật độ cao (Đậm vừa - Dark Slate Gray)
INSERT INTO city_guest_density (city, guest_count, booking_count, total_revenue, percentage_share, density_rank, density_level)
VALUES ('Da Nang', 1580, 790, 5240000000, 17.4, 3, 'HIGH');

-- Nha Trang: Mật độ trung bình khá (Xám trung tính đậm - Medium Dark Gray)
INSERT INTO city_guest_density (city, guest_count, booking_count, total_revenue, percentage_share, density_rank, density_level)
VALUES ('Nha Trang', 1020, 510, 3860000000, 11.3, 4, 'HIGH');

-- Vũng Tàu: Mật độ trung bình (Xám trung tính - Gray)
INSERT INTO city_guest_density (city, guest_count, booking_count, total_revenue, percentage_share, density_rank, density_level)
VALUES ('Vung Tau', 590, 310, 2150000000, 6.5, 5, 'MEDIUM');

-- Đà Lạt: Mật độ trung bình (Xám nhạt vừa - Silver Gray)
INSERT INTO city_guest_density (city, guest_count, booking_count, total_revenue, percentage_share, density_rank, density_level)
VALUES ('Da Lat', 440, 240, 1890000000, 4.9, 6, 'MEDIUM');

-- Cần Thơ: Mật độ cơ bản (Xám nhạt - Light Gray)
INSERT INTO city_guest_density (city, guest_count, booking_count, total_revenue, percentage_share, density_rank, density_level)
VALUES ('Can Tho', 240, 130, 980000000, 2.6, 7, 'MODERATE');


-- ============================================================================
-- 4. TRUY VẤN KIỂM TRA DỮ LIỆU SAU KHI SEED
-- ============================================================================
SELECT * FROM monthly_revenue_stats;
SELECT * FROM hotel_revenue_stats;
SELECT * FROM city_guest_density;
