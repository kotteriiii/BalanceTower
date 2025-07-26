using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(AudioClip clip, float volume = 0.3f)
    {
        if (audioSource.clip == clip && audioSource.isPlaying)
            return; // ìØÇ∂ã»Ç™çƒê∂íÜÇ»ÇÁâΩÇ‡ÇµÇ»Ç¢

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }

    public void StopBGM()
    {
        audioSource.Stop();
    }
}
