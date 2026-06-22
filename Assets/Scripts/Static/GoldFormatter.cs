using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GoldFormatter
{
    public static string FormatGold(int gold)
    {
        if (gold >= 1000000)
        {
            // BẪY TOÁN HỌC: Chữ 'f' ở cuối 1000000f là BẮT BUỘC để ép máy tính hiểu đây là phép chia số thực, giúp giữ lại phần thập phân.
            float millions = gold / 1000000f;

            // Kỹ thuật Format "0.#": 
            // - Luôn hiện số ở hàng đơn vị (số 0).
            // - Dấu # nghĩa là CHỈ hiện 1 số thập phân nếu nó khác 0.
            // Ví dụ: 1.5 triệu -> "1.5M", 1.0 triệu -> "1M" (không bị lỗi 1.0M lóa mắt).
            return millions.ToString("0.#") + "M";
        }

        // TRƯỜNG HỢP 2: Từ 100,000 đến 999,999 (Giới hạn 5 số, hiển thị chữ K)
        else if (gold >= 100000)
        {
            // Do ở đây ta không cần số thập phân, nên cứ chia số nguyên bình thường
            int thousands = gold / 1000;
            return thousands.ToString() + "K";
        }

        // TRƯỜNG HỢP 3: Dưới 100,000 (Dưới 6 chữ số)
        else
        {
            // Định dạng "N0" sẽ tự động thêm dấu phẩy ngăn cách hàng nghìn cực kỳ chuyên nghiệp.
            // Ví dụ: 99999 -> "99,999"
            return gold.ToString("N0");
        }
    }
    // Start is called before the first frame update.

}
