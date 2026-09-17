USE hotel_ks;

-- Kiểm tra schema/key của các bảng mà 3 công việc sử dụng.
SELECT table_name, column_name, kind, position, type
FROM system_schema.columns
WHERE keyspace_name = 'hotel_ks'
  AND table_name IN (
      'guests',
      'rooms_by_hotel',
      'rooms_by_hotel_status',
      'bookings_by_guest',
      'bookings_by_hotel_date'
  )
ORDER BY table_name, kind, position;

-- Các primary key mà code hiện tại giả định:
-- guests: PRIMARY KEY (guest_id)
-- rooms_by_hotel: PRIMARY KEY ((hotel_id), room_number)
-- rooms_by_hotel_status: PRIMARY KEY ((hotel_id, status), room_number)
-- bookings_by_guest: PRIMARY KEY ((guest_id), check_in_date, booking_id)
-- bookings_by_hotel_date: PRIMARY KEY ((hotel_id), check_in_date, booking_id)
