using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Music Settings")]
    [SerializeField] private AudioClip backgroundMusicClip;
    private AudioSource musicSource;

    private float sfxVolume = 0.5f;
    private float musicVolume = 0.2f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true; 
        musicSource.playOnAwake = false;

        UpdateVolumeFromPrefs();
        PlayBackgroundMusic();
    }

    public void UpdateVolumeFromPrefs()
    {
        sfxVolume = PlayerPrefs.GetFloat("GeneralSoundVolume", 80f) / 100f;

        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 60f) / 100f;
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusicClip != null && musicSource != null)
        {
            musicSource.clip = backgroundMusicClip;
            musicSource.Play();
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        if (clip != null)
        {
            GameObject tempGO = new GameObject("TempAudio");
            tempGO.transform.position = position;
            
            AudioSource source = tempGO.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = sfxVolume;
            
            source.Play();
            Destroy(tempGO, clip.length);
        }
    }
}