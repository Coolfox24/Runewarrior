using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    Health health;

    private void Start()
    {
        health = FindObjectOfType<EnemyAttackController>().GetComponent<Health>();
    }

    private void Update()
    {
        text.text = health.GetHealth().ToString();
    }
}
