using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentAttackUI : MonoBehaviour
{
    GameController gameController;
    TextMeshProUGUI myText;

    // Start is called before the first frame update
    void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        myText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText()
    {
        myText.text = gameController.GetAttack();
    }

    public void ConfirmAttack()
    {
        gameController.Attack();
    }

}
