using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("--Audio Sources--")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("--Audio Clips--")]
    [SerializeField] private List<AudioClip> _backgroundClips = new();
    [SerializeField] private List<AudioClip> _effectsClips = new();

    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetNewMusic(int index)
    {
        if (_musicSource.clip == _backgroundClips[index] && _musicSource.isPlaying)
        {
            return; //Same music
        }

        _musicSource.Stop();
        _musicSource.clip = _backgroundClips[index];
        _musicSource.loop = true;
        _musicSource.volume = (float)SessionManager.GetVolume() / 100;
        _musicSource.Play();
    }

    public IEnumerator UpdateVolume(float newVolume)
    {
        _musicSource.volume = newVolume / 100;
        _sfxSource.volume = newVolume / 100;
        yield return null;
    }

    public void PlaySoundEffect(string soundEffect)
    {   
        int soundIndex;
        switch (soundEffect)
        {
            case "Shot":
                soundIndex = 0;
                break;
            case "ExplosionAsteroid":
                soundIndex = 1;
                break;
            case "ExplosionPlayer":
                soundIndex = 2;
                break;
            case "ShotEnemy":
                soundIndex = 3;
                break;
            case "ExplosionEnemy":
                soundIndex = 4;
                break;
            default:
                soundIndex = 0;
                break;
        }
        _sfxSource.PlayOneShot(_effectsClips[soundIndex]);
    }
}
