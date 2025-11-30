using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    [Header("Audio Clip")]
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Settings")]
    [SerializeField] private float defaultVolume = 0.5f;

    private AudioSource audioSource;
    private static BGMManager instance;

    public static BGMManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        // ถ้ามี Instance อยู่แล้ว ทำลายตัวใหม่
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // ⭐ ไม่ถูกทำลายเมื่อเปลี่ยน Scene

        // Setup Audio Source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = true;
        audioSource.volume = defaultVolume;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        // เล่นเพลงทันที
        PlayMusic();
    }

    private void PlayMusic()
    {
        if (backgroundMusic == null)
        {
            Debug.LogWarning("⚠️ ไม่มีไฟล์เพลง! ลืมลาก Audio Clip?");
            return;
        }

        if (audioSource.isPlaying && audioSource.clip == backgroundMusic)
        {
            return;
        }

        audioSource.clip = backgroundMusic;
        audioSource.Play();
        Debug.Log($"🎵 เล่นเพลง: {backgroundMusic.name}");
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        audioSource.UnPause();
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }
}