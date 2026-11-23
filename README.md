\- Họ và tên:NGUYỄN VĂN CƯƠNG

\- Mã sinh viên: K235480106006

\- Lớp: K59KMT

\- Mô tả:Bài tập tìm hiểu về các thuật toán mã hóa đối xứng (DES, AES), mã hóa bất đối xứng (RSA) và mô hình mã hóa kết hợp.



\---



 1. Thuật toán mã hóa hiện đại DES và AES



a. Thuật toán DES (Data Encryption Standard)

\- Tổng quan: DES là thuật toán mã hóa đối xứng theo khối (Block cipher). Kích thước khối dữ liệu là 64-bit và độ dài khóa thực tế là 56-bit (8-bit còn lại dùng kiểm tra lỗi parity).

\- Cấu trúc: Sử dụng cấu trúc mạng Feistel gồm 16 vòng biến đổi liên tiếp.

\- Quy trình mã hóa:

&#x20; 1. Khối văn bản rõ 64-bit đầu vào được đưa qua phép Hoán vị ban đầu (Initial Permutation - IP).

&#x20; 2. Khối dữ liệu được tách thành 2 nửa 32-bit là Trái L0 và Phải R0.

&#x20; 3. Trải qua 16 vòng biến đổi Feistel theo công thức:

&#x20;    - L(i) = R(i-1)

&#x20;    - R(i) = L(i-1) XOR F(R(i-1), K\_i)

&#x20;    - Trong đó: K\_i là khóa vòng 48-bit được tạo ra từ khóa chính, F là hàm biến đổi Feistel thực hiện mở rộng R(i-1) từ 32-bit lên 48-bit, XOR với K\_i, nén qua 8 hộp S-Box về 32-bit và hoán vị qua P-Box.

&#x20; 4. Sau 16 vòng, ghép hai nửa (R16, L16) lại và cho qua phép Hoán vị ngược IP^(-1) để thu được bản mã 64-bit.

\- Quy trình giải mã: Sử dụng cùng thuật toán và sơ đồ như mã hóa, nhưng thứ tự khóa vòng được đưa vào ngược lại (từ K16 về K1).



\---



 b. Thuật toán AES (Advanced Encryption Standard)

\- Tổng quan:AES là thuật toán mã hóa đối xứng mã hóa theo từng khối 128-bit. Thuật toán hỗ trợ các độ dài khóa 128-bit (10 vòng), 192-bit (12 vòng) và 256-bit (14 vòng).

\- Quy trình mã hóa (với khóa 128-bit - 10 vòng):

&#x20; 1. Khởi tạo:Cộng khóa vòng đầu tiên (AddRoundKey) giữa khối dữ liệu và K0.

&#x20; 2. Vòng 1 đến Vòng 9: Lặp lại 4 bước biến đổi:

&#x20;    - SubBytes: Thay thế phi tuyến từng byte qua bảng S-Box cố định.

&#x20;    - ShiftRows: Dịch chuyển vòng các hàng của ma trận trạng thái (Hàng 0 giữ nguyên, Hàng 1 dịch trái 1 byte, Hàng 2 dịch 2 byte, Hàng 3 dịch 3 byte).

&#x20;    - MixColumns: Trộn các cột bằng phép nhân ma trận trên trường Galois GF(2^8).

&#x20;    - AddRoundKey: XOR dữ liệu với khóa vòng K\_i tương ứng.

&#x20; 3. Vòng 10 (Vòng cuối): Thực hiện 3 bước SubBytes -> ShiftRows -> AddRoundKey (bỏ qua bước MixColumns).

\- Quy trình giải mã: Thực hiện các bước biến đổi ngược (InvShiftRows -> InvSubBytes -> AddRoundKey -> InvMixColumns) theo thứ tự ngược lại từ vòng cuối về vòng đầu.

\- Cài đặt chương trình: Em đã cài đặt demo thuật toán AES chế độ CBC bằng ngôn ngữ Python trong file `aes\_demo.py` kèm theo repository nà
