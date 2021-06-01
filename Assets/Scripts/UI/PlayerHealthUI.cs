using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    Health health;

    private void Start()
    {
        health = FindObjectOfType<PlayerAttackController>().GetComponent<Health>();
    }

    private void Update()
    {
        text.text = health.GetHealth().ToString();
    }
}
