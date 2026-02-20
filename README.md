# 🚀 PSDP Project

> เว็บแอปพร้อมระบบ Authentication และ Database ด้วย **Supabase**  
> รันง่ายด้วย **Docker**

---

## 🧰 Prerequisites (สิ่งที่ต้องเตรียม)

ก่อนเริ่มใช้งาน ให้ติดตั้งเครื่องมือเหล่านี้ก่อน

- 🐳 **Docker Desktop**
- 🌿 **Git**
- 🔐 **Supabase Account** (สำหรับ Database & Authentication)

---

## ⚡ Quick Start

### 1️⃣ Clone Project

```
git clone https://github.com/noonnoonzii/PSDP_Project.git
cd PSDP_Project
2️⃣ Setup Environment Variables
```

คัดลอกไฟล์ตัวอย่างแล้วเปลี่ยนชื่อเป็น .env

cp .env.example .env

จากนั้นเปิดไฟล์ .env แล้วใส่ค่าจาก Supabase Settings (ตัวอย่างที่ต้องใส่ .env.example)

SUPABASE_URL=your_project_url
SUPABASE_KEY=your_anon_public_key

📍 หาได้ที่:
Supabase Dashboard → Project Settings → API

3️⃣ Run with Docker 🐳

สั่ง Build และ Run Container

docker-compose up --build
✅ เมื่อรันสำเร็จ

🔗 API: http://localhost:5000

(หรือแล้วแต่พอร์ตที่กำหนดใน docker-compose)


🛠 Useful Commands

หยุด Container
docker-compose down

ดู Logs
docker-compose logs -f

Rebuild ใหม่
docker-compose up --build --force-recreate



💡 Tips

ถ้าแก้ .env ต้อง restart container

ตรวจสอบว่า Docker ทำงานก่อนรัน

ถ้าเชื่อม Supabase ไม่ได้ ให้เช็ค URL และ KEY
