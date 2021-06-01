using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckUI : MonoBehaviour
{
    GameController gameController;
    [SerializeField] TextMeshProUGUI myText;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    private void Update()
    {
        myText.text = gameController.GetDeckSize().ToString();
    }
}
