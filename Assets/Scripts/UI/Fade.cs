using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    Image myImage;

    [SerializeField] Color fadeOutColor;
    [SerializeField] Color fadeInColor;

    [SerializeField] float fadeTime;

    private void Awake()
    {
        myImage = GetComponent<Image>();
    }

    public IEnumerator FadeIn()
    {
        float t = 0;
        while(myImage.color != fadeInColor)
        {
            myImage.color = Color.Lerp(fadeOutColor, fadeInColor, t);
            t += fadeTime * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        myImage.enabled = false;
    }

    public IEnumerator FadeOut()
    {
        myImage.enabled = true;
        float t = 0;
        while (myImage.color != fadeOutColor)
        {
            myImage.color = Color.Lerp(fadeInColor, fadeOutColor, t);
            t += fadeTime * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
