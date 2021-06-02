using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;

public class DeathController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera deathCam;
    [SerializeField] GameObject deathLight;
    [SerializeField] Light2D backgroundLight;
    [SerializeField] GameObject defaultCanvas;
    [SerializeField] GameObject deathCanvas;
    [SerializeField] Color fadedOutColor;
    [SerializeField] float fadeOutTime = 2f;

    GameObject playerHealthbar;

    public IEnumerator DeathScene()
    {
        playerHealthbar = FindObjectOfType<PlayerAttackController>().GetComponentInChildren<HealthBarUI>().gameObject;
        deathCam.Priority = 100;
        defaultCanvas.SetActive(false);
        playerHealthbar.SetActive(false);
        Color startingColor = backgroundLight.color;
        float t = 0;

        while(t <= 1)
        {
            backgroundLight.color = Color.Lerp(startingColor, fadedOutColor, t);
            t += Time.deltaTime * (1 / fadeOutTime);
            yield return new WaitForEndOfFrame();
        }

        while(Camera.main.GetComponent<CinemachineBrain>().IsBlending)
        {
            yield return new WaitForEndOfFrame();
        }

        deathLight.SetActive(true);
        deathCanvas.SetActive(true);
    }

    public void MainMenu()
    {
        FindObjectOfType<PersistentData>().ResetGame();
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
