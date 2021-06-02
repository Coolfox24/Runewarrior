using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Will be destroyed between runs
public class PersistentData : MonoBehaviour
{
    [SerializeField] Card[] _ListOfAllCards;
    [SerializeField] int startingDeckSize = 8;

    [SerializeField] List<Card> _PlayerStartingDeck = new List<Card>();
    [SerializeField] int _PlayerHealth = 2000;
    private int clearedRooms = 0;
    //TODO remove serialize
    [Header("For Testing Purposes")]
    [SerializeField] GameObject _PlayerCharacter;
    [SerializeField] GameObject _EnemyCharacter;
    [SerializeField] GameObject _Background;

    [SerializeField] GameObject PLAYER_CHARACTER;

    List<Room> currentlyExploredRooms;
    Room lastEntered;

    public bool showInfo = true;

    // Start is called before the first frame update
    void Awake()
    {
        if(FindObjectsOfType<PersistentData>().Length > 1)
        {
            Destroy(gameObject);
            this.gameObject.SetActive(false);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        _PlayerCharacter = PLAYER_CHARACTER;
        GenerateStartingDeck();
        currentlyExploredRooms = new List<Room>();
    }

    private void GenerateStartingDeck()
    {
        List<Card> tier1Cards = GetTier1Cards();
        for(int i = 0; i < startingDeckSize; i ++)
        {
            int cardNum = Random.Range(0, tier1Cards.Count);
            AddCard(tier1Cards[cardNum]);
        }
    }

    private List<Card> GetTier1Cards()
    {
        List<Card> tier1Cards = new List<Card> ();

        foreach (Card c in _ListOfAllCards)
        {
            if(c.GetTier() ==1)
            {
                tier1Cards.Add(c);
            }
        }
        return tier1Cards;
    }

    public void AddCard(Card newCard)
    {
        _PlayerStartingDeck.Add(newCard);
    }

    public List<Card> GetStartingDeck()
    {
        return _PlayerStartingDeck;
    }

    //Called on start of fight scene
    public int GetCurHealth()
    {
        return _PlayerHealth;
    }

    //Called on end of fight scene
    public void SetHealth(int newAmount)
    {
        _PlayerHealth = newAmount;
    }

    public void ResetGame()
    {
        Destroy(gameObject);
    }

    public void SetPlayerObject(GameObject player)
    {
        _PlayerCharacter = player;
    }

    public void SetEnemyObject(GameObject enemy)
    {
        _EnemyCharacter = enemy;
    }

    public void SetBackgroundObject(GameObject background)
    {
        _Background = background;
    }

    public GameObject GetPlayerObject()
    {
        return _PlayerCharacter;
    }

    public GameObject GetEnemyObject()
    {
        return _EnemyCharacter;
    }
    
    public GameObject GetBackgroundObject()
    {
        return _Background;
    }

    public void SetCurrentRooms(List<Room> rooms)
    {
        currentlyExploredRooms = rooms;
    }

    public List<Room> GetRooms()
    {
        return currentlyExploredRooms;
    }

    public void SetLastEnteredRoom(Room room)
    {
        lastEntered = room;
    }

    public Room GetLastExploredRoom()
    {
        return lastEntered;
    }

    public void LevelCleared()
    {
        clearedRooms++;
    }

    public int GetClearedLevels()
    {
        return clearedRooms;
    }

    public List<Card> GetCardListOfTypeAndTier(RuneTags type, int tier)
    {
        List<Card> typeCards = new List<Card>();
        foreach (Card card in _ListOfAllCards)
        {
            if (card.GetAffinity() == type && card.GetTier() == tier)
            {
                typeCards.Add(card);
            }
        }

        return typeCards;
    }

    public List<Card> GetCardListOfTier(int tier)
    {
        List<Card> typeCards = new List<Card>();
        foreach (Card card in _ListOfAllCards)
        {
            if (card.GetTier() == tier)
            {
                typeCards.Add(card);
            }
        }

        return typeCards;
    }

    public List<Card> GetCardListOfType(RuneTags type)
    {
        List<Card> typeCards = new List<Card>();
        foreach (Card card in _ListOfAllCards)
        {
            if (card.GetAffinity() == type)
            {
                typeCards.Add(card);
            }
        }

        return typeCards;
    }

    public void AddHealth(int amount)
    {
        _PlayerHealth += amount;
    }
}
