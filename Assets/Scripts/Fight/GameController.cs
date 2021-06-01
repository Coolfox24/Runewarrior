using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using Cinemachine;

public enum GameState
{
    setup,
    player_turn,
    enemy_turn
}


public class GameController : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] Transform handPosition;
    [SerializeField] Transform deckPosition;
    [SerializeField] Transform enemySpawnPoint;
    [SerializeField] Transform playerSpawnPoint;
    [SerializeField] Transform cardEmphasisPosition;

    [Header("Prefabs & References")]
    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject attackTextContainer;
    [SerializeField] CurrentAttackUI attackText;
    [SerializeField] GameObject infoBox;
    [SerializeField] TextMeshProUGUI infoText;

    [Header("Music & Victory")]
    [SerializeField] AudioClip music;
    [SerializeField] AudioClip victoryFanfare;
    [SerializeField] CinemachineVirtualCamera victoryCam;

    [Header("For Testing")]
    [SerializeField] bool useScaling = true;

    private List<Card> deck;
    private List<Card> hand;
    private List<Card> discard;

    private GameState curState;
    private int runesDrawn = 2;

    private PlayerAttackController playerAttackController;
    private EnemyAttackController enemyAttackController;

    float enemyScaling;

    private IEnumerator Start()
    {
        
        PersistentData data = FindObjectOfType<PersistentData>();

        curState = GameState.setup;
        deck = data.GetStartingDeck();
        hand = new List<Card>(); 
        discard = new List<Card>();

        SetEnemyScaling(data);

        GameObject player = Instantiate(data.GetPlayerObject(), playerSpawnPoint.position, Quaternion.identity);
        GameObject enemy =  Instantiate(data.GetEnemyObject(), enemySpawnPoint.position, Quaternion.identity);

        Instantiate(data.GetBackgroundObject(), Vector3.zero, Quaternion.identity);

        playerAttackController = player.GetComponent<PlayerAttackController>();
        enemyAttackController = enemy.GetComponent<EnemyAttackController>();
        playerAttackController.GetComponent<Health>().SetHealth(data.GetCurHealth());

        infoBox.SetActive(false);

        ReshuffleDeck();

        FindObjectOfType<MusicPlayer>().PlayMusic(music);
        yield return StartCoroutine(FindObjectOfType<Fade>().FadeIn());

        StartCoroutine(DrawCard(3));
        yield return new WaitForEndOfFrame();
    }

    public int GetDeckSize()
    {
        return deck.Count;
    }

    public int GetDiscardSize()
    {
        return discard.Count;
    }

    private IEnumerator DrawCard(int amount)
    {
        attackTextContainer.SetActive(false);

        for (int i = 0; i < amount; i++)
        {

            if (deck.Count == 0)
            {
                ReshuffleDeck();
                if (deck.Count == 0)
                {
                    //If player has <5 total runes and all are in hand
                    continue;
                }
            }

            GameObject go = Instantiate(cardPrefab, handPosition);

            yield return StartCoroutine(AnimateCard(go));
            SortHandGraphics();
        }

        //All this to delay the attack text popping up before the last card is in position
        //Other cards we don't care if 2 are moving at the same time
        var handCards = handPosition.GetComponentsInChildren<Rune>();
        Rune lastCard = handCards[handCards.Length - 1];

        while(lastCard.IsMoving())
        {
            yield return new WaitForEndOfFrame();
        }

        attackTextContainer.SetActive(true);
        attackText.UpdateText();
        curState = GameState.player_turn;
    }

    private IEnumerator AnimateCard(GameObject card)
    {
        //Set Starting position to deck position
        Rune curRune = card.GetComponent<Rune>();
        Card curCard = deck[0];
        curRune.SetCard(curCard);
        if (hand.Count < 5)
        {
            hand.Add(curCard);
            deck.Remove(curCard);
            card.transform.position = deckPosition.position;
            yield return StartCoroutine(curRune.MoveCard(cardEmphasisPosition.position, 8));   
            
            yield return new WaitForSeconds(1);

            curRune.SetPosition(0, handPosition.position.y, true);
        }
        else
        {
            deck.Remove(curCard);
            card.transform.position = deckPosition.position;
            yield return StartCoroutine(curRune.MoveCard(cardEmphasisPosition.position, 8));
            yield return StartCoroutine(curRune.DestroyRune());
            discard.Add(curCard);
        }
    }

    private void SortHandGraphics()
    {
        Rune[] cardGraphics = handPosition.GetComponentsInChildren<Rune>();

        if(cardGraphics.Length == 0 )
        {
            return;
        }

        if (cardGraphics.Length == 1)
        {
            cardGraphics[0].transform.position = new Vector2(0, cardGraphics[0].transform.position.y);
            return;
        }

        float xOffset = (cardGraphics.Length / 2 * 1.5f);
        Vector2 startingPosition = new Vector2(-xOffset, cardGraphics[0].transform.position.y);
        Vector2 endPosition = new Vector2(xOffset, cardGraphics[0].transform.position.y);

        for (int index = 0; index <cardGraphics.Length; index ++)
        {
            Rune card = cardGraphics[index];
            Vector2 newPosition = Vector2.Lerp(startingPosition, endPosition, (float)index / (cardGraphics.Length - 1));
            card.SetPosition(newPosition.x, newPosition.y, false);
            
        }
    }

    private void ReshuffleDeck()
    {
        foreach(Card card in discard)
        {
            deck.Add(card);
        }
        discard.Clear();

        System.Random rng = new System.Random();
        deck = deck.OrderBy(a => rng.Next()).ToList();
    }

    public void DiscardCard(int handIndex)
    {
        discard.Add(hand[handIndex]);
        hand.Remove(hand[handIndex]);
    }

    public string GetAttack()
    {
        //Determine what attack to do based on currently selected runes
        //Count Runes
        Dictionary<RuneTags, int> activeRunes = new Dictionary<RuneTags, int>();
        foreach(RuneTags rune in Enum.GetValues(typeof(RuneTags)))
        {
            activeRunes.Add(rune, 0);
        }

        foreach (Rune card in handPosition.GetComponentsInChildren<Rune>())
        {
            if (card.IsSelected())
            {
                //Do something
                RuneTags[] cardsTags = card.GetRuneTags();
                for (int i = 0; i < cardsTags.Length; i++)
                {
                    activeRunes[cardsTags[i]]++;
                }
            }
        }

        ////debug rune count
        //foreach (KeyValuePair<RuneTags, int> cur in activeRunes)
        //{
        //    Debug.Log(cur.Key + " = " + cur.Value);
        //}

        return playerAttackController.DetermineAttack(activeRunes);
    }
    
    public void Attack()
    {
        StartCoroutine(EndTurn());
    }

    IEnumerator EndTurn()
    {
        curState = GameState.enemy_turn;
        //Disable Text interactiveness
        attackTextContainer.SetActive(false);

        //Discard selected tiles from hand
        Rune[] runes = FindObjectsOfType<Rune>();

        foreach (Rune rune in runes)
        {
            if (rune.IsSelected())
            {
                hand.Remove(rune.GetCard());
                discard.Add(rune.GetCard());

                //Before Destroying (Fire some animation that makes them burn)
                yield return StartCoroutine(rune.DestroyRune());

            }
        }
        SortHandGraphics();
        yield return StartCoroutine(playerAttackController.DoAttack());

        //After player turn, determine if enemy is dead
        if(enemyAttackController.IsDead())
        {
            yield return StartCoroutine(EnemyDead());
        }

        //Do Enemy Turn
        yield return StartCoroutine(enemyAttackController.DoAttack(infoBox, infoText));

        //Determine if Player is dead
        if (playerAttackController.IsDead())
        {
            yield return StartCoroutine(PlayerDead());
        }
        else //start next turn
        {
            //End of Turn Effects
            yield return StartCoroutine(playerAttackController.GetComponent<Buffs>().EndOfTurnEffects());
            yield return StartCoroutine(enemyAttackController.GetComponent<Buffs>().EndOfTurnEffects());


            //Determine if Player is dead
            if (playerAttackController.IsDead())
            {
                yield return StartCoroutine(PlayerDead());
            }
            else if(enemyAttackController.IsDead())
            {
                yield return StartCoroutine(EnemyDead());
            }
            else //start next turn
            {
                //Draw Cards
                curState = GameState.setup;
                int drawnRunes = runesDrawn;
                if(playerAttackController.GetComponent<Buffs>().IsWindActive())
                {
                    drawnRunes += 1;
                }
                yield return StartCoroutine(DrawCard(drawnRunes));
            }
        }
    }

    private IEnumerator PlayerDead()
    {
        yield return StartCoroutine(playerAttackController.Die());

        //Bit of extra padding after lose
        StartCoroutine(FindObjectOfType<DeathController>().DeathScene());
    }

    private IEnumerator EnemyDead()
    {
        yield return StartCoroutine(enemyAttackController.Die());

        victoryCam.Priority = 30;
        playerAttackController.GetComponent<Animator>().SetTrigger("Victory");

        AudioPlayer.PlayClipAtPoint(victoryFanfare, Camera.main.transform.position);
        yield return new WaitForSeconds(4);

        yield return StartCoroutine(FindObjectOfType<Fade>().FadeOut());
        FindObjectOfType<PersistentData>().SetHealth(
            playerAttackController.GetComponent<Health>().GetHealth()
            );
        if (FindObjectOfType<PersistentData>().GetLastExploredRoom().GetRoomTier() < 6)
        {
            SceneManager.LoadScene("ExploreScene");
        }
        else
        {
            //load win screen
            SceneManager.LoadScene("WinScreen");
        }
        //Probs wont fire as we're going to load a new scene here
        yield return new WaitForSeconds(2);
    }

    public bool CanInteract()
    {
        if(curState == GameState.player_turn)
        {
            return true;
        }
        return false;
    }

    private void SetEnemyScaling(PersistentData data)
    {
        if(!useScaling)
        {
            enemyScaling = 1;
            return;
        }

        float levelsCleared = data.GetClearedLevels();
        float roomTier = data.GetLastExploredRoom().GetRoomTier();
        float scalingFactor = roomTier / 10f;
        if (roomTier < 5)
        {
            enemyScaling = 1 + (scalingFactor * ((levelsCleared) - roomTier));
        }
        else
        {
            enemyScaling = 1;
        }
    }

    public float GetEnemyScaling()
    {
        return enemyScaling;
    }
}
