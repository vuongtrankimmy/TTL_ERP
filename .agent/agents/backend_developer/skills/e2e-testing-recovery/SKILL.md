---
name: e2e-testing-recovery
description: Use this skill to recover context, track the latest completed tasks, and resume work after a system crash, model limit, or session restart. Gives instructions on how to use track records to seamlessly pick up remaining items.
---

# Tác Vụ Khôi Phục Bối Cảnh Hệ Thống (Context Recovery Tracker)

## 1. Mục đích
Skill này được thiết lập để giúp Antigravity AI Agent nhanh chóng khôi phục lại bối cảnh (context) và trạng thái công việc mới nhất trong trường hợp phiên làm việc bị ngắt quãng do các vấn đề như: system crash, vượt quá giới hạn token (out of context window), hoặc rate limit từ API.

Mục tiêu là **không phải hỏi lại User** hoặc **không lặp lại các thao tác đã làm**, mà tự động quét các file Artifacts để tìm ra chính xác "Điểm dừng cuối cùng" và đi tiếp.

## 2. Các Hệ Sinh Thái Dữ Liệu Đang Theo Dõi (Current Tracking State)
Hiện tại, dự án TTL.HR đang ở giai đoạn **Nghiệm thu Dữ liệu thực E2E (Phase 1 Finalization)**. 

Bất cứ khi nào Agent sử dụng Skill này, hãy lập tức đọc nội dung của các file báo cáo quan trọng nhất (được lưu trong thư mục Artifacts của dự án - truy xuất thông qua cấu trúc `.gemini/antigravity/brain/...`):

*   **`task.md`:** Bảng Master Task List chứa toàn bộ log công việc từ trước đến nay. Đây là xương sống của Project.
*   **`master_execution_plan_summary.md`:** Bức tranh tổng quan kiến trúc hệ thống (Rất quan trọng để hiểu bối cảnh lớn).
*   **`testing_execution_sequence.md`:** Thứ tự chạy các file test.
*   **`real_data_creation_tasks.md`:** Các bước tạo dữ liệu mẫu/thực để test E2E.
*   **`e2e_real_data_flow_report.md`:** Báo cáo luồng xử lý ngầm (Backend/DB) cho các module.
*   **`e2e_real_data_checklist.md`:** Giấy chấm điểm/nghiệm thu cuối cùng rà soát các test cases.

## 3. Quy trình Khôi phục (Resume Protocol)
Khi Agent được gọi lại (hoặc khởi động lại) và được User yêu cầu "tiếp tục", "resume", hoặc chạy lại luồng bị gián đoạn, hãy nghiêm ngặt thực hiện 4 bước sau bằng cách sử dụng Tools:

1.  **Quét lại Artifacts:** Sử dụng Tool `grep_search` hoặc `view_file` để trích xuất file `task.md` nhằm xác định chính xác checkbox cuối cùng được đánh dấu `[x]`. Nếu đang thực thi E2E, quét thêm file `e2e_real_data_checklist.md` xem các Test Case E2E đã tick `[x]` đến đâu rồi.
2.  **Thông báo Trạng thái (Acknowledge):** Báo cáo lại cho User biết task cuối cùng vừa hoàn thành theo dấu vết là gì và Next Action (Task tiếp theo) cần làm (dựa trên các ô `[ ]` chưa check hoặc danh sách Phase kế tiếp).
3.  **Hỏi Quyền Đi Tiếp:** Xác nhận lại với User xem họ muốn tự động chạy tiếp kịch bản, hay rẽ nhánh giải quyết Bug (nếu crash do lỗi code/DB).
4.  **Kế Thừa Code (Nguyên tắc Vàng):** Tuyệt đối **KHÔNG viết lại từ đầu** hay xóa sổ hạ tầng Code/Database đã có. Kế thừa toàn bộ code và cấu hình hiện tại. Nếu sinh ra lỗi, đi thẳng vào việc sửa lỗi (Fixing) tại chính xác module bị văng.

## 4. Trạng thái Dừng Gần Nhất (Last Known State Snapshot - 01/03/2026)
*Lưu ý: Snapshot này ghi nhận mức độ hoàn thiện ở thời điểm lưu Skill. Thực tế có thể đã tiến xa hơn khi bạn đọc file này.*

-   **Hoàn thành (Đóng đinh):** Toàn bộ Source Code và Giao diện của Phần 1 (Core HR, Chấm Công, Quy trình Duyệt Đơn, Bộ Engine Tính Lương & Thuế lũy tiến, Cảnh báo tài sản, Tự động hóa Email notification, và Tối ưu hóa giao diện di động Mobile UI Optimization).
-   **Trạng thái tức thời:** Hệ thống dừng ở bước đóng gói Kịch bản Test E2E Dữ liệu thật (**KHÔNG VIẾT CODE ở đoạn này**). Đang chờ User thực thi Nhập liệu test ở phía Front-end/Database dựa trên các file Hướng dẫn checklist.
-   **Next Phase Pipeline (Phase 2):** Nếu quá trình QA/E2E test không phát sinh lỗi (Bug-free), hệ thống đã sẵn sàng bước sang Phase 2 (Premium Experience & Advanced Workflows) bao gồm:
    1.  Duyệt Đơn Đa Cấp (Multi-level Approval).
    2.  Báo Cáo Nâng Cao (Smart Analytics Reporting).
    3.  Tích hợp AI Tuyển Dụng.
