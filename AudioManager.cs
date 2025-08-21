using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] soundEffects;
    public AudioSource bgm;
    public static AudioManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        bgm.Play();
    }


    void Update()
    {

    }

    public void PlaySfx(int soundToPlay)
    {
        soundEffects[soundToPlay].Play();
    }

    public void StopBackgroundMusic()
    {
        if (bgm != null && bgm.isPlaying)
        {
            bgm.Stop();
        }
    }
}
