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
        sfxSource.PlayOneShot(moveSound);

    }
    public void PlayCoin()
    {

        sfxSource.PlayOneShot(coinSound);
        sfxSource.volume = 1.5f;
    }
    void PlayBGR(AudioClip music)
    {
        if (bgmSource.clip == music) return;

        bgmSource.clip = music;
        bgmSource.volume = 0.5f;
        bgmSource.Play();
    }
}
