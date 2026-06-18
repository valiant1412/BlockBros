using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static AudioManager instance;

    [Header("Cái Loa")]
    public AudioSource sfxSource;

    public AudioSource bgmSource;

    public AudioClip sound;

    public AudioClip winSound;

    public AudioClip loseSound;

    public AudioClip moveSound;

    public AudioClip coinSound;

    public AudioClip backGround;

    private float lastMoveTime = 0f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // PHẢI LÀ gameObject, không phải instance
            DontDestroyOnLoad(gameObject);

            if (sfxSource != null)
            {
                sfxSource.ignoreListenerPause = true;
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            // BẮT BUỘC PHẢI CÓ LỆNH RETURN Ở ĐÂY ĐỂ CHẶN ĐỨNG LUỒNG CODE
            // Nếu không có return, nó vẫn sẽ chạy tuột xuống hàm Start() và phát nhạc!
            return;
        }


    }
    // Thay chữ "void" thành "IEnumerator"
    IEnumerator Start()
    {
        // 1. Kỷ luật thép: Vòng lặp này sẽ giam lỏng AudioManager lại, 
        // không cho chạy tiếp nếu Két sắt chưa load xong file JSON.
        while (SaveManager.Instance == null || SaveManager.Instance.gameData == null)
        {
            yield return null; // Chờ 1 khung hình rồi quay lại kiểm tra tiếp
        }

        // 2. KHI ĐÃ XUỐNG ĐƯỢC ĐẾN ĐÂY: Két sắt 100% đã có mặt và mở sẵn
        if (backGround != null)
        {
            PlayBGR(backGround);
        }
    }
    public void PlayClickSFX()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.gameData.isSFXOff) return;
        sfxSource.PlayOneShot(sound);
    }
    public void PlayWin()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.gameData.isSFXOff) return;
        sfxSource.PlayOneShot(winSound);
    }
    public void PlayLose()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.gameData.isSFXOff) return;
        sfxSource.PlayOneShot(loseSound);
    }
    public void PlayMoving()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.gameData.isSFXOff) return;
        if (Time.time - lastMoveTime > 0.05f)
        {
            // Nhét thẳng mức volume 0.5f vào hàm PlayOneShot như tôi đã hướng dẫn bài trước
            // ĐỪNG dùng lệnh sfxSource.volume = 0.5f nữa nhé!
            sfxSource.PlayOneShot(moveSound, 0.5f);

            // Cập nhật lại thời điểm vừa phát âm thanh
            lastMoveTime = Time.time;
        }
    }
    public void PlayCoin()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.gameData.isSFXOff) return;

        sfxSource.PlayOneShot(coinSound, 0.8f);
    }
    void PlayBGR(AudioClip music)
    {
        // 1. Luôn đồng bộ trạng thái Mute trước
        if (SaveManager.Instance != null && SaveManager.Instance.gameData != null)
        {
            bgmSource.mute = SaveManager.Instance.gameData.isMuted;
        }

        // 2. SỬA ĐIỀU KIỆN: Chỉ bỏ qua nếu TRÙNG BÀI VÀ LOA ĐANG PHÁT NHẠC
        if (bgmSource.clip == music && bgmSource.isPlaying) return;

        // 3. Nếu trùng bài nhưng loa đang tắt (do vừa mở game), hoặc là bài mới -> Phát nhạc
        bgmSource.clip = music;
        bgmSource.volume = 0.5f;
        bgmSource.Play();
    }

}
