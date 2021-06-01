using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiscardUI: MonoBehaviour
{
    GameController gameController;
    [SerializeField] TextMeshProUGUI myText;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    private void Update()
    {
        myText.text = gameController.GetDiscardSize().ToString();
    }
}
