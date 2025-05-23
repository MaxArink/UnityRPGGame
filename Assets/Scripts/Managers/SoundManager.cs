using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private float _masterVolume = 1.0f;
    [SerializeField] private float _musicVolume = 1.0f;
    [SerializeField] private float _effectVolume = 1.0f;

    public float MasterVolume
    {
        get => _masterVolume = Mathf.Clamp01(MasterVolume);
        set => _masterVolume = Mathf.Clamp01(value);
    }

    public float MusicVolume
    {
        get => _musicVolume = Mathf.Clamp01(MusicVolume);
        set => _musicVolume = Mathf.Clamp01(value);
    }

    public float EffectVolume
    {
        get => _effectVolume = Mathf.Clamp01(EffectVolume);
        set => _effectVolume = Mathf.Clamp01(value);
    }


    [SerializeField] private AudioSource _effectSource = null;
    [SerializeField] private AudioSource _musicSource = null;

    [Range(0f, 1f)][SerializeField] private float _lowPitchRange = 0.8f;
    [Range(1f, 2f)][SerializeField] private float _highPitchRange = 1.0f;



    public static SoundManager instance {  get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Play(AudioClip pClip)
    {
        _effectSource.clip = pClip;
        _effectSource.Play();
    }

    public void PlayMusic(AudioClip pClip)
    {
        _musicSource.clip = pClip;
        _musicSource.Play();
    }

    public void RandomSoundEffect(params AudioClip[] pClip)
    {
        int randomIndex = Random.Range(0, pClip.Length);
        float randomPitch = Random.Range(_lowPitchRange, _highPitchRange);

        _effectSource.pitch = randomPitch;
        _effectSource.clip = pClip[randomIndex];
        _effectSource.Play();
    }
}
