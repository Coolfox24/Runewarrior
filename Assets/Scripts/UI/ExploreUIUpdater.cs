using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExploreUIUpdater : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI deckCount;
    [SerializeField] TextMeshProUGUI currentHealth;
    [SerializeField] TextMeshProUGUI lifeGained;
    [SerializeField] TextMeshProUGUI choicesRemaining;

    private PersistentData data;

    private void OnEnable()
    {
        data = FindObjectOfType<PersistentData>();
    }

    public void UpdateTopBar()
    {
        UpdateDeckCount();
        UpdateHealth();
    }

    public void UpdateDeckCount()
    {
        deckCount.text = data.GetStartingDeck().Count.ToString();
    }

    public void UpdateHealth()
    {
        currentHealth.text = data.GetCurHealth().ToString();
    }

    public void UpdateLifeGained(int amount)
    {
        lifeGained.text = "Take Life: " + amount;
    }

    public void UpdateChoicesRemaining(int amount)
    {
        choicesRemaining.text = "Choices Remaining: " + amount;
    }
}
