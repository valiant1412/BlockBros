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
            DontDestroyOnLoad(instance);
            if (sfxSource != null)
            {
                sfxSource.ignoreListenerPause = true;
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }
    void Start()
    {
        if (backGround != null)
        {
            PlayBGR(backGround);
        }
    }
    public void PlayClickSFX()
    {
        sfxSource.PlayOneShot(sound);
    }
    public void PlayWin()
    {
        sfxSource.PlayOneShot(winSound);
    }
    public void PlayLose()
    {
        sfxSource.PlayOneShot(loseSound);
    }
    public void PlayMoving()
    {
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

        sfxSource.PlayOneShot(coinSound, 0.8f);
    }
    void PlayBGR(AudioClip music)
    {
        if (bgmSource.clip == music) return;

        bgmSource.clip = music;
        bgmSource.volume = 0.5f;
        bgmSource.Play();
    }
}
