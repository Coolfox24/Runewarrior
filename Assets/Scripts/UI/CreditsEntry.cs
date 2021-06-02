using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditsEntry : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI author;
    [SerializeField] TextMeshProUGUI website;

    [SerializeField] float moveSpeed;

    public void Setup(bool isTitle, string text1, string text2)
    {
        if(isTitle)
        {
            title.gameObject.SetActive(true);
            title.text = text1;
        }
        else
        {
            author.gameObject.SetActive(true);
            author.text = text1;

            website.gameObject.SetActive(true);
            website.text = text2;
        }
    }

    private void Update()
    {
        //Move up
        Vector3 movement = new Vector3(0, moveSpeed * Time.deltaTime, 0);
        gameObject.transform.position += movement;
    }

}
