using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;

    float musicVolume;
    float sfxVolume;

    private void Awake()
    {
        if (FindObjectsOfType<GameSettings>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }

        if(PlayerPrefs.HasKey("MUSIC_VOLUME"))
        {
            instance.musicVolume = PlayerPrefs.GetFloat("MUSIC_VOLUME");
        }
        else
        {
            SetMusicVolume(1);
        }

        if(PlayerPrefs.HasKey("SFX_VOLUME"))
        {
            instance.sfxVolume = PlayerPrefs.GetFloat("SFX_VOLUME");
        }
        else
        {
            SetSFXVolume(1);
        }
    }

    public  void SetMusicVolume(float amount)
    {
        instance.musicVolume = amount;
        PlayerPrefs.SetFloat("MUSIC_VOLUME", amount);
    }

    public  void SetSFXVolume(float amount)
    {
        instance.sfxVolume = amount;
        PlayerPrefs.SetFloat("SFX_VOLUME", amount);
    }

    public  float GetMusicVolume()
    {
        return instance.musicVolume;
    }

    public  float GetSFXVolume()
    {
        return instance.sfxVolume;
    }
}
