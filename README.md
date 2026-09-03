# 🧩 Rubik's Cube 3D - Unity

Mô phỏng khối Rubik 3x3 tương tác trực quan.


## 🎮 Hướng dẫn cách chơi

### 🖱️ Điều khiển bằng chuột

**1. Xoay góc nhìn toàn bộ khối Rubik theo 1 chiều nhất định (2d)**
Sử dụng **Chuột phải (Nhấn giữ & Vuốt)** để thay đổi góc nhìn bao quát khối Rubik:
| Thao tác vuốt | Kết quả |
| :--- | :--- |
| ➡️ Sang phải / ⬅️ Sang trái | Xoay khối sang phải / trái |
| ↖️ Lên-trái / ↗️ Lên-phải | Xoay lên / Nghiêng phải |
| ↙️ Xuống-trái / ↘️ Xuống-phải | Nghiêng trái / Xoay xuống |

**2. Xoay từng mặt Rubik (Live Drag Rotation)**
Sử dụng **Chuột trái** để thao tác trực tiếp lên các mặt:
* **Thao tác:** Nhấn giữ chuột trái vào một ô bất kỳ và kéo theo hướng muốn xoay. Mặt Rubik sẽ xoay bám sát theo tay ngay lập tức.
* **Cơ chế Snap thông minh (Thả chuột):**
  * Kéo vượt quá **45°** ➡️ Khối tự động xoay hoàn tất vào góc 90°.
  * Kéo chưa tới **45°** ➡️ Khối tự động hoàn về vị trí cũ.

**3. Xoay góc nhìn toàn bộ khối Rubik (3d): dùng chuột giữa**
---

### ⌨️ Điều khiển bằng bàn phím
Hỗ trợ phím tắt theo ký hiệu Rubik quốc tế. 
> 💡 Giữ phím `Shift` + Phím tương ứng để xoay **ngược chiều kim đồng hồ**.

| Phím | Mặt thao tác | Phím | Lớp giữa (Middle Layers) |
| :---: | :--- | :---: | :--- |
| **U** | Mặt trên (Up) | **M** | Lớp giữa Trái - Phải |
| **D** | Mặt dưới (Down) | **E** | Lớp giữa Trên - Dưới |
| **F** | Mặt trước (Front) | **S** | Lớp giữa Trước - Sau |
| **B** | Mặt sau (Back) | | |
| **R** | Mặt phải (Right) | | |
| **L** | Mặt trái (Left) | | |
