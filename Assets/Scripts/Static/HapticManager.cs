using UnityEngine;

public static class HapticManager

{
    private static bool CanVibrate()
    {
        if (SaveManager.Instance == null)
        {
            Debug.Log("<color=red>TỬ HUYỆT: SaveManager.Instance đang bị NULL! Két sắt chưa tồn tại!</color>");
            return false;
        }

        // 2. NGHI PHẠM B: Có Két sắt nhưng dữ liệu bên trong bị rỗng
        if (SaveManager.Instance.gameData == null)
        {
            Debug.Log("<color=red>TỬ HUYỆT: SaveManager.Instance.gameData đang bị NULL! Lỗi Load JSON!</color>");
            return false;
        }

        // 3. NGHI PHẠM C: Dữ liệu đã load, nhưng biến isHapticOn trên điện thoại lại mang giá trị FALSE
        if (SaveManager.Instance.gameData.isHapticOff == true)
        {
            Debug.Log("<color=yellow>THỦ PHẠM: Dữ liệu trên điện thoại ghi isHapticOn là FALSE (Đang bị tắt)!</color>");
            return false;
        }

        return true;
    }
    // ==========================================

    // CÚ TÁP MẠNH (Dùng để test)

    // ==========================================

    public static void HeavyTaptic()

    {
        if (!CanVibrate())
        {
            Debug.Log("<color=red>LÍNH GÁC ĐÃ CHẶN LỆNH RUNG!</color>");
            return;
        }
#if UNITY_ANDROID && !UNITY_EDITOR

       

        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))

        {

            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");



            // CHÌA KHÓA NẰM Ở ĐÂY: Ép lệnh chạy trên luồng (Thread) gốc của điện thoại

            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>

            {

                try

                {

                    AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

                    AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");

                   

                    // Lực nảy mạnh: 40 mili-giây, lực 255

                    AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", 40L, 255);

                   

                    vibrator.Call("vibrate", effect);

                }

                catch (System.Exception e)

                {

                    Debug.Log("Lỗi Haptic Native: " + e.Message);

                }

            }));

        }

       

#else
        // ĐOẠN NÀY DÀNH CHO MÁY TÍNH (UNITY EDITOR) ĐỂ BẠN NHÌN THẤY LOGIC CHẠY
        Debug.Log("<color=green>HEAVY HAPTIC MÔ PHỎNG: Rung Rung Rung!</color>");
#endif

    }



    // ==========================================

    // CÚ TÁP NHẸ

    // ==========================================

    public static void LightTaptic()

    {
        if (!CanVibrate())
        {
            Debug.Log("<color=red>LÍNH GÁC ĐÃ CHẶN LỆNH RUNG!</color>");
            return;
        }

#if UNITY_ANDROID && !UNITY_EDITOR

        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))

        {

            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");



            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>

            {

                try

                {

                    AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

                    AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");

                   

                    // Lực nảy nhẹ: 15 mili-giây, lực 50

                    AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", 25L, 150);

                   

                    vibrator.Call("vibrate", effect);

                }

                catch {}

            }));

        }

#else
        // ĐOẠN NÀY DÀNH CHO MÁY TÍNH (UNITY EDITOR) ĐỂ BẠN NHÌN THẤY LOGIC CHẠY
        Debug.Log("<color=green>Light HAPTIC MÔ PHỎNG: Rung Rung Rung!</color>");
#endif

    }

}