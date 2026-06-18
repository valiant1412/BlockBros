using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingBtnManager
 : MonoBehaviour
{
    public enum SettingType
    {
        BGMusic, SFX, Haptic

    }
    [Header("Loại Nút Bấm")]
    public SettingType toggleType;
    [SerializeField] private Sprite firstSprites;

    [SerializeField] private Sprite secondSprites;


    [SerializeField] private Image image;


    void Start()
    {
        ChangeIcon();
    }
    public void ToggleBtn()
    {

        if (toggleType.Equals(SettingType.BGMusic))
        {
            bool isMute = SaveManager.Instance.gameData.isMuted;
            SaveManager.Instance.gameData.isMuted = !isMute;

            AudioManager.instance.bgmSource.mute = SaveManager.Instance.gameData.isMuted;

            if (SaveManager.Instance.gameData.isMuted == false)
            {
                // Kiểm tra xem loa có đang cầm đúng bài nhạc không và chưa chạy thì ép chạy
                if (AudioManager.instance.bgmSource.clip != null && !AudioManager.instance.bgmSource.isPlaying)
                {
                    AudioManager.instance.bgmSource.Play();
                }
            }
            Debug.Log("Âm thanh được mute:" + SaveManager.Instance.gameData.isMuted);
        }
        else if (toggleType.Equals(SettingType.SFX))
        {
            bool isSFXOff = SaveManager.Instance.gameData.isSFXOff;
            SaveManager.Instance.gameData.isSFXOff = !isSFXOff;

            Debug.Log("SFX được mute:" + SaveManager.Instance.gameData.isSFXOff);
        }
        else if (toggleType.Equals(SettingType.Haptic))
        {
            bool isHapticOff = SaveManager.Instance.gameData.isHapticOff;
            SaveManager.Instance.gameData.isHapticOff = !isHapticOff;

        }
        SaveManager.Instance.SaveGame();
        ChangeIcon();
    }

    void ChangeIcon()
    {
        bool changeValue = true;
        if (toggleType.Equals(SettingType.BGMusic))
        {
            changeValue = SaveManager.Instance.gameData.isMuted;
        }
        else if (toggleType.Equals(SettingType.SFX))
        {
            changeValue = SaveManager.Instance.gameData.isSFXOff;
        }
        else if (toggleType.Equals(SettingType.Haptic))
        {
            changeValue = SaveManager.Instance.gameData.isHapticOff;
        }
        image.sprite = changeValue ? firstSprites : secondSprites;

    }
    // Viết một hàm lừa tình, không bao giờ được gọi đến
}
