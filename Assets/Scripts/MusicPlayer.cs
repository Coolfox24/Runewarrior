using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioSource musicToPlay;

    private void Awake()
    {
        if(FindObjectsOfType<MusicPlayer>().Length > 1)
        {
            Destroy(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if(GameSettings.instance == null)
        {
            return;
        }
        musicToPlay.volume = GameSettings.instance.GetMusicVolume();
    }

    public void PlayMusic(AudioClip newClip)
    {
        musicToPlay.clip = newClip;
        musicToPlay.Play();
    }
}
