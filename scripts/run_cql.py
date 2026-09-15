import os
import sys
import re
from pathlib import Path
from cassandra.cluster import Cluster
from cassandra.auth import PlainTextAuthProvider
from dotenv import load_dotenv

# Đảm bảo stdout không lỗi encoding trên Windows console
if sys.stdout and hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")


# 1. Nạp biến môi trường từ .env
BASE_DIR = Path(__file__).resolve().parent.parent
load_dotenv(BASE_DIR / ".env")
load_dotenv()

TOKEN = os.getenv("ASTRA_DB_APPLICATION_TOKEN")

# Fallback: đọc token từ ~/.astrarc nếu chưa có trong env
if not TOKEN:
    astrarc_path = Path.home() / ".astrarc"
    if astrarc_path.exists():
        try:
            with open(astrarc_path, "r", encoding="utf-8") as f:
                for line in f:
                    if line.strip().startswith("ASTRA_DB_APPLICATION_TOKEN="):
                        TOKEN = line.strip().split("=", 1)[1].strip()
                        break
        except Exception:
            pass

if not TOKEN:
    raise Exception(
        "Chưa có biến môi trường ASTRA_DB_APPLICATION_TOKEN! "
        "Hãy tạo file .env hoặc cấu hình token Astra DB."
    )

# 2. Định vị file Secure Connect Bundle và file CQL
BUNDLE_PATH = os.getenv("ASTRA_DB_BUNDLE_PATH")
if not BUNDLE_PATH or not Path(BUNDLE_PATH).exists():
    # Tìm file zip trong thư mục gốc
    zip_files = list(BASE_DIR.glob("secure-connect-*.zip"))
    if zip_files:
        BUNDLE_PATH = str(zip_files[0])
    else:
        BUNDLE_PATH = str(BASE_DIR / "secure-connect-hotel-management-demo.zip")

KEYSPACE = os.getenv("ASTRA_DB_KEYSPACE", "hotel_ks")
CQL_FILE = BASE_DIR / "database" / "seed_35_each.cql"

print(f"--- THÔNG TIN KẾT NỐI ---")
print(f"Bundle   : {BUNDLE_PATH}")
print(f"Keyspace : {KEYSPACE}")
print(f"CQL File : {CQL_FILE}")
print(f"Token    : {TOKEN[:12]}...{TOKEN[-6:]}")
print("--------------------------\n")

cloud_config = {
    "secure_connect_bundle": BUNDLE_PATH
}

auth_provider = PlainTextAuthProvider(
    "token",
    TOKEN
)

print("Đang kết nối tới DataStax Astra DB...")
cluster = Cluster(
    cloud=cloud_config,
    auth_provider=auth_provider
)

session = cluster.connect(KEYSPACE)
print("Kết nối thành công!\n")

with open(CQL_FILE, "r", encoding="utf-8") as f:
    content = f.read()

# Lọc các dòng comment và lệnh USE
lines = []
for line in content.splitlines():
    stripped = line.strip()
    if not stripped or stripped.startswith("--"):
        continue
    if stripped.upper().startswith("USE "):
        continue
    lines.append(line)

content = "\n".join(lines)

statements = [
    stmt.strip()
    for stmt in content.split(";")
    if stmt.strip()
]

print(f"Tìm thấy tổng cộng {len(statements)} câu lệnh CQL.\n")

success = 0
failed = 0

for index, statement in enumerate(statements, start=1):
    # Rút gọn hiển thị statement
    first_line = statement.splitlines()[0]
    preview = first_line[:60] + ("..." if len(first_line) > 60 else "")
    try:
        result = session.execute(statement)
        success += 1
        
        # Nếu là câu lệnh SELECT (như validation count), hiển thị kết quả
        if statement.strip().upper().startswith("SELECT"):
            row = result.one()
            count_val = row[0] if row else "N/A"
            print(f"[{index:03d}/{len(statements)}] QUERY OK | {preview} => {count_val}")
        else:
            if index % 20 == 0 or index == len(statements):
                print(f"[{index:03d}/{len(statements)}] Đã thực thi xong...")
    except Exception as e:
        failed += 1
        print(f"\n[{index:03d}] LỖI khi chạy lệnh:")
        print(f"Statement: {statement}")
        print(f"Error    : {e}\n")

cluster.shutdown()

print("\n==============================")
print(f"Thành công : {success}")
print(f"Lỗi        : {failed}")
print("==============================")