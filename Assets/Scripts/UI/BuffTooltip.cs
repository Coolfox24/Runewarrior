using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffTooltip : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI textname;
    [SerializeField] TextMeshProUGUI text1;
    [SerializeField] TextMeshProUGUI text2;
    [SerializeField] TextMeshProUGUI text3;

    public void Setup(Sprite icon, string name, string text1, string text2, string text3)
    {
        image.sprite = icon;
        this.textname.text = name;
        this.text1.text = text1;
        this.text2.text = text2;
        this.text3.text = text3;
        //Do shit here
    }
}
