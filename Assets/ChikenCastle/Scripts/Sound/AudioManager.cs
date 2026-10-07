using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Serializable]
    private class Sound
    {
        public SoundType type;

        [Header("Варианты звука")]
        public AudioClip[] clips;

        [Header("Шанс воспроизведения")]
        [Range(0f, 1f)]
        public float playChance = 1f;

        [Header("Громкость")]
        [Range(0f, 1f)]
        public float volume = 1f;

        [Header("Минимальная задержка")]
        public float cooldown = 0f;

        [HideInInspector]
        public float lastPlayTime = -Mathf.Infinity;
    }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Sound[] sounds;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(SoundType type)
    {
        Sound sound = GetSound(type);

        if (sound == null)
            return;

        // Проверяем задержку
        if (Time.time < sound.lastPlayTime + sound.cooldown)
            return;

        // Проверяем шанс
        if (UnityEngine.Random.value > sound.playChance)
            return;

        // Получаем случайный клип
        AudioClip clip = GetRandomClip(sound);

        if (clip == null)
            return;

        sound.lastPlayTime = Time.time;

        audioSource.PlayOneShot(
            clip,
            sound.volume
        );
    }

    private Sound GetSound(SoundType type)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.type == type)
                return sound;
        }

        Debug.LogWarning(
            $"AudioManager: звук {type} не найден!"
        );

        return null;
    }

    private AudioClip GetRandomClip(Sound sound)
    {
        if (sound.clips == null || sound.clips.Length == 0)
            return null;

        return sound.clips[
            UnityEngine.Random.Range(
                0,
                sound.clips.Length
            )
        ];
    }
}