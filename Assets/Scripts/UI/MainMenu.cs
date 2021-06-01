using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    //Need new class called GameSettings that lets other classes easily find the settings set
    [SerializeField] GameObject optionsScreen;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sfxSlider;

    public void StartGame()
    {
        //Potentially change to a loading screen to show the player how to play
        SceneManager.LoadScene("ExploreScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowOptions()
    {
        optionsScreen.SetActive(true);
        volumeSlider.value = GameSettings.instance.GetMusicVolume();
        sfxSlider.value = GameSettings.instance.GetSFXVolume();
    }

    public void ExitOptions()
    {
        optionsScreen.SetActive(false);
    }

    public void UpdateVolume()
    {
        GameSettings.instance.SetMusicVolume(volumeSlider.value);
    }

    public void UpdateSFX()
    {
        //Play SFX to indicate how loud it will be TODO
        GameSettings.instance.SetSFXVolume(sfxSlider.value);
    }
}
