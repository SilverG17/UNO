# 🎮 UNO Online Game

Multiplayer UNO game với Unity client-server architecture.

## 🚨 LỖI KHI CHẠY? ĐỌC NGAY!

### ⚡ Quick Fix:
1. **Mở Unity Editor** (UNO-Client project)
2. **Edit** → **Project Settings** → **Player**
3. **Active Input Handling** → **Input Manager (Old)**
4. **Restart Unity**

**Chi tiết:** Xem `UNO-Client/QUICK_FIX.md`

---

## 📁 Cấu Trúc Project

```
UNO-main/
├── UNO-Client/          # Unity client project
│   ├── Assets/
│   ├── QUICK_FIX.md           ⭐ ĐỌC ĐẦU TIÊN!
│   ├── TROUBLESHOOTING.md     📚 Giải quyết mọi lỗi
│   ├── FIX_INPUT_SYSTEM_ERROR.md
│   ├── FIX_UI_LAYOUT.md
│   └── QUICK_START.md
│
└── UNO-Sever/           # Unity server project
    └── Assets/
```

---

## 🚀 Hướng Dẫn Chạy

### Bước 1: Fix Input System (Client)
```
Xem UNO-Client/QUICK_FIX.md
```

### Bước 2: Start Server
1. Mở **UNO-Sever** project trong Unity
2. Play scene
3. Check Console: "Server started inside Unity"

### Bước 3: Start Client
1. Mở **UNO-Client** project trong Unity
2. Mở scene **Lobby.unity**
3. Play scene

### Bước 4: Test
1. Click "Create Room"
2. Nhận room code
3. Test join với client khác

---

## ⚠️ Lỗi Thường Gặp

### 1. Input System Error ⚠️
```
InvalidOperationException: You are trying to read Input using the UnityEngine.Input class
```
**Fix:** `UNO-Client/QUICK_FIX.md`

### 2. UI Bị Vỡ 🎨
UI elements chồng lên nhau, text bị cắt

**Fix:** `UNO-Client/FIX_UI_LAYOUT.md`

### 3. Không Kết Nối Server 🔌
**Fix:** Kiểm tra server đang chạy, port 7777

---

## 📚 Tài Liệu

### Client (UNO-Client/):
- **QUICK_FIX.md** - Sửa lỗi nhanh nhất
- **TROUBLESHOOTING.md** - Debug guide đầy đủ
- **FIX_INPUT_SYSTEM_ERROR.md** - Chi tiết Input System
- **FIX_UI_LAYOUT.md** - Chi tiết UI Layout
- **QUICK_START.md** - Setup UI guide

### Server (UNO-Sever/):
- Server tự động chạy khi Play scene
- Port mặc định: 7777

---

## 🛠️ Yêu Cầu

- Unity 2022.3+ (hoặc tương thích)
- Windows/Mac/Linux
- .NET Standard 2.1

---

## 🎯 Features

### Client:
- ✅ Lobby UI với room code system
- ✅ 4 player slots
- ✅ Real-time networking
- ✅ Settings panel

### Server:
- ✅ TCP socket server
- ✅ Room management
- ✅ Game state synchronization
- ✅ Multiple rooms support

---

## 🔧 Development

### Architecture:
```
Client (Unity) ←→ TCP Socket ←→ Server (Unity)
     ↓                              ↓
  LobbyUI                      RoomManager
  GameUI                       GameManager
```

### Protocol:
- JSON-based message protocol
- Message types: CREATE_ROOM, JOIN_ROOM, GAME_STATE, etc.
- Newline-delimited messages

---

## 🐛 Debug

### Client Logs:
```
[CLIENT] CONNECTING TO 127.0.0.1:7777
[CLIENT] TCP CONNECT SUCCESS
[CLIENT] MESSAGE: {...}
```

### Server Logs:
```
[SERVER] STARTING SERVER...
[SERVER] CLIENT CONNECTED FROM: ...
[SERVER] BYTES RECEIVED: ...
```

---

## 📞 Support

Gặp lỗi?
1. Đọc `UNO-Client/QUICK_FIX.md`
2. Xem `UNO-Client/TROUBLESHOOTING.md`
3. Check Console logs (cả Client và Server)

---

## 📝 Notes

- Server và Client là 2 Unity projects riêng biệt
- Phải chạy Server trước, sau đó mới chạy Client
- Default host: 127.0.0.1, port: 7777
- Có thể thay đổi host/port trong Settings

---

**Happy Gaming! 🎮**

