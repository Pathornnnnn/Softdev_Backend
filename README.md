# Softdev_Backend

🛠 สิ่งที่ต้องเตรียม (Prerequisites)
Docker Desktop
Git
บัญชี Supabase (สำหรับ Database และ Authentication)

🏃‍♂️ ขั้นตอนการรันโปรเจกต์ (Quick Start)

1. Clone Project

git clone https://github.com/Pathornnnnn/Softdev_Backend.git
cd Softdev_Backend

2. Setup Environment Variables
คัดลอกไฟล์ตัวอย่างและเปลี่ยนชื่อเป็น .env:

cp .env.example .env


จากนั้นเปิดไฟล์ .env และกรอกข้อมูลจากหน้า Supabase Settings ของคุณ:
SUPABASE_URL: Project URL
SUPABASE_KEY: API Key (anon/public)
DB_HOST: Hostname ของฐานข้อมูล
DB_PASSWORD: รหัสผ่านฐานข้อมูลที่คุณตั้งไว้

3. Run with Docker 🐳
สั่ง Build และรัน Container ทั้งหมดด้วยคำสั่งเดียว:

docker-compose up --build



###เมื่อรันสำเร็จ API จะพร้อมใช้งานที่: http://localhost:5069/swagger