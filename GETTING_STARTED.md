# 🚀 Getting Started - UNO Online Game

## 👋 Chào Mừng!

Đây là hướng dẫn nhanh để chạy game UNO multiplayer.

---

## ⚡ Quick Start (5 Phút)

### Bước 1: Fix Input System (Client)
```
1. Mở Unity Editor (UNO-Client project)
2. Edit → Project Settings → Player
3. Active Input Handling → Input Manager (Old)
4. Apply → Restart Unity
```

### Bước 2: Chạy Server
```
1. Mở Unity Editor (UNO-Sever project)
2. Play scene
3. Check Console: "Server started inside Unity"
```

### Bước 3: Chạy Client
```
1. Mở Unity Editor (UNO-Client project)
2. Mở scene Lobby.unity
3. Play scene
4. Click "Create Room"
```

✅ **Nếu nhận được room code → Thành công!**

---

## 📚 Tài Liệu Đầy Đủ

### Client Documentation (UNO-Client/):

| File | Mô Tả | Khi Nào Đọc |
|------|-------|-------------|
| **START_HERE.md** | Bắt đầu từ đây | Lần đầu tiên |
| **QUICK_FIX.md** | Fix lỗi nhanh | Gặp lỗi |
| **CHECKLIST.md** | Danh sách việc | Setup mới |
| **TROUBLESHOOTING.md** | Debug guide | Lỗi phức tạp |
| **FIX_INPUT_SYSTEM_ERROR.md** | Chi tiết Input | Hiểu sâu |
| **FIX_UI_LAYOUT.md** | Chi tiết UI | UI bị vỡ |
| **QUICK_START.md** | Setup UI | Setup lần đầu |
| **DOCS_INDEX.md** | Index tài liệu | Tìm docs |

### Đọc Gì Trước?
```
UNO-Client/START_HERE.md  ⭐ Đọc đầu tiên!
```

---

## 🐛 Lỗi Thường Gặp

### 1. Input System Error ⚠️
```
InvalidOperationException: You are trying to read Input...
```
**Fix:** Xem `UNO-Client/QUICK_FIX.md`

### 2. UI Bị Vỡ 🎨
**Fix:** Xem `UNO-Client/FIX_UI_LAYOUT.md`

### 3. Không Kết Nối Server 🔌
**Fix:** Kiểm tra server đang chạy, port 7777

**Chi tiết:** `UNO-Client/TROUBLESHOOTING.md`

---

## 📁 Cấu Trúc Project

```
UNO-main/
│
├── README.md                    # Tổng quan project
├── GETTING_STARTED.md          # File này
│
├── UNO-Client/                 # Unity Client
│   ├── Assets/
│   ├── START_HERE.md           ⭐ Đọc đầu tiên!
│   ├── QUICK_FIX.md            ⚡ Fix nhanh
│   ├── CHECKLIST.md            ✅ Danh sách việc
│   ├── TROUBLESHOOTING.md      🐛 Debug guide
│   ├── FIX_INPUT_SYSTEM_ERROR.md
│   ├── FIX_UI_LAYOUT.md
│   ├── QUICK_START.md
│   ├── DOCS_INDEX.md
│   └── README.md
│
└── UNO-Sever/                  # Unity Server
    └── Assets/
```

---

## 🎯 Lộ Trình Học

### Người Mới:
```
1. GETTING_STARTED.md (đang đọc)
2. UNO-Client/START_HERE.md
3. UNO-Client/QUICK_FIX.md
4. Test & Play!
```

### Gặp Lỗi:
```
1. UNO-Client/TROUBLESHOOTING.md
2. Tìm lỗi cụ thể
3. Làm theo hướng dẫn
```

### Setup Mới:
```
1. UNO-Client/QUICK_START.md
2. UNO-Client/CHECKLIST.md
3. UNO-Client/QUICK_FIX.md
```

---

## 🛠️ Yêu Cầu

- Unity 2022.3+ (hoặc tương thích)
- Windows/Mac/Linux
- .NET Standard 2.1
- TextMeshPro package (tự động)

---

## 🎮 Features

### Client:
- ✅ Lobby UI với room code
- ✅ 4 player slots
- ✅ Real-time networking
- ✅ Settings panel

### Server:
- ✅ TCP socket server
- ✅ Room management
- ✅ Game state sync
- ✅ Multiple rooms

---

## 🔧 Development

### Chạy Development:
1. Start server (UNO-Sever)
2. Start client (UNO-Client)
3. Test features

### Debug:
- Check Console logs (cả Client và Server)
- Xem `UNO-Client/TROUBLESHOOTING.md`

---

## 📞 Support

### Gặp Vấn Đề?
1. Đọc `UNO-Client/START_HERE.md`
2. Xem `UNO-Client/TROUBLESHOOTING.md`
3. Check Console logs
4. Hỏi với đầy đủ thông tin

### Format Hỏi:
```
**Lỗi:** [Error message]
**Đã làm:** [Các bước đã thử]
**Console Log:** [Copy log]
**Screenshot:** [Nếu có]
```

---

## 🎉 Next Steps

Sau khi chạy thành công:
- [ ] Đọc `UNO-Client/README.md` để hiểu cấu trúc
- [ ] Xem `UNO-Client/DOCS_INDEX.md` để tìm tài liệu
- [ ] Test multiplayer với 2+ clients
- [ ] Bắt đầu phát triển features mới!

---

## 🔗 Quick Links

- [Client START_HERE](UNO-Client/START_HERE.md) ⭐
- [Client QUICK_FIX](UNO-Client/QUICK_FIX.md) ⚡
- [Client TROUBLESHOOTING](UNO-Client/TROUBLESHOOTING.md) 🐛
- [Client DOCS_INDEX](UNO-Client/DOCS_INDEX.md) 📚
- [Project README](README.md) 📖

---

**Happy Gaming! 🎮**

**Bắt đầu ngay:** `UNO-Client/START_HERE.md`
