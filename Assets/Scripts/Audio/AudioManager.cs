using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("--Audio Sources--")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("--Audio Clips--")]
    public List<AudioClip> background = new();
    public List<AudioClip> effects = new();

    public static AudioManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if(SessionManager.GetFirstPlay())
        {
            musicSource.clip = background[1];
        }
        else
        {    
            musicSource.clip = background[0];
        }

        musicSource.loop = true;
        musicSource.volume = (float)SessionManager.GetVolume() / 100;
        musicSource.Play();
    }

    public void ChangeMusic(int index)
    {
        musicSource.Stop();
        musicSource.clip = background[index];
        musicSource.loop = true;
        musicSource.volume = (float)SessionManager.GetVolume() / 100;
        musicSource.Play();
    }

    public IEnumerator UpdateVolume(float newVolume)
    {
        musicSource.volume = newVolume / 100;
        sfxSource.volume = newVolume / 100;
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
            default:
                soundIndex = 0;
                break;
        }
        sfxSource.PlayOneShot(effects[soundIndex]);
    }
}
