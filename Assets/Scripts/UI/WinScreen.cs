using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Linq;

public class WinScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI runesText;
    [SerializeField] TextMeshProUGUI typeText;

    PersistentData data;

    // Start is called before the first frame update
    void Start()
    {
        data = FindObjectOfType<PersistentData>();

        scoreText.text = data.GetCurHealth().ToString();
        runesText.text = data.GetStartingDeck().Count.ToString();
        typeText.text = GetDeckAffinity(data.GetStartingDeck()).ToString();
    }

    private RuneTags GetDeckAffinity(List<Card> deck)
    {
        Dictionary<RuneTags, int> tags = new Dictionary<RuneTags, int>();
        foreach (RuneTags rune in Enum.GetValues(typeof(RuneTags)))
        {
            tags.Add(rune, 0);
        }

        foreach(Card c in deck)
        {
            tags[c.GetAffinity()]++;
        }

        RuneTags prefRune = RuneTags.NOT_SELECTED;
        
        foreach(KeyValuePair<RuneTags, int> entry in tags)
        {
            if(entry.Value > tags[prefRune])
            {
                prefRune = entry.Key;
            }
        }

        return prefRune;
    }

    public void MainMenu()
    {
        data.ResetGame();
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

