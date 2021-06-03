using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExploreController : MonoBehaviour
{
    const int MAX_TIER = 4;

    [SerializeField] List<RoomData> availableRooms;
    [SerializeField] GameObject roomPrefab;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform deckPosition;
    [SerializeField] Transform choicePosition;
    [SerializeField] GameObject blackout;
    [SerializeField] GameObject takeLifeButton;
    [SerializeField] Transform deckHolder;
    [SerializeField] GameObject viewDeckBlackout;
    [SerializeField] GameObject settingRooms;
    [SerializeField] TMPro.TextMeshProUGUI roomsRemaining;
    [SerializeField] int gameLength = 10;

    [SerializeField] AudioClip music;
    [SerializeField] GameObject infoImage;

    [SerializeField] GameObject options;

    List<Room> currentRooms;
    private int spawnedRooms;

    private PersistentData data;
    private ExploreUIUpdater uiUpdater;
    private List<GameObject> cardsToBePicked;

    float roomOffset = 0.5f;
    Room lastExploredRoom;

    float lastTierWeight = 2;
    float currentTierWeight = 4;
    float nextTierWeight = 4;

    int healthToAdd = 100;

    int choicesRemaining = 0;

    private bool viewDeck = false;

    private IEnumerator Start()
    {
        //Get rooms from persistent data & set up
        data = FindObjectOfType<PersistentData>();
        uiUpdater = FindObjectOfType<ExploreUIUpdater>();
        uiUpdater.UpdateTopBar();
        currentRooms = data.GetRooms();
        spawnedRooms = 0;

        data.SetGameLength(gameLength);
        if(data.showInfo)
        {
            infoImage.SetActive(true);
        }

        while(data.showInfo)
        {
            if(Input.GetMouseButton(0))
            {
                data.showInfo = false;
                infoImage.SetActive(false);
            }

            yield return new WaitForEndOfFrame();
        }

        FindObjectOfType<MusicPlayer>().PlayMusic(music);
        if (currentRooms.Count == 0)
        {
            //Generate First Room 
            List<RoomData> tier1Rooms = GetRoomsOfTier(1);
            GameObject go = Instantiate(roomPrefab, new Vector2(0 + roomOffset, 0 + roomOffset), Quaternion.identity);
            go.GetComponent<Room>().SetRoom(tier1Rooms[Random.Range(0, tier1Rooms.Count)]);
            RegisterRoom(go.GetComponent<Room>());
            spawnedRooms++;

            yield return StartCoroutine(FindObjectOfType<Fade>().FadeIn());
        }
        else
        {
            lastExploredRoom = data.GetLastExploredRoom();

            foreach (Room room in currentRooms)
            {
                if (room == lastExploredRoom)
                {
                    room.SetCleared();
                    data.LevelCleared();
                }

                GameObject go = Instantiate(roomPrefab, room.GetPosition(), Quaternion.identity);
                go.GetComponent<Room>().SetRoom(room.GetRoomData());
                go.GetComponent<Room>().Setup(room.IsSkipped(), room.IsCleared());
            }

            roomsRemaining.text = "Rooms Until Boss : " + (gameLength - data.GetClearedLevels());

            //Before Generating new rooms Give player cards to pick from
            yield return StartCoroutine(PickRewards());

            if (data.GetClearedLevels() < gameLength)
            {
                //Add new rooms for player to explore from last room entered
                //Need check here to see if we should just spawn 1 room - which is the boss room
                List<GameObject> spawnedRoomObjects = new List<GameObject>();
                spawnedRoomObjects.Add(SpawnNewRoom(lastExploredRoom.GetPosition().x + 1, lastExploredRoom.GetPosition().y + 0));
                spawnedRoomObjects.Add(SpawnNewRoom(lastExploredRoom.GetPosition().x - 1, lastExploredRoom.GetPosition().y + 0));
                spawnedRoomObjects.Add(SpawnNewRoom(lastExploredRoom.GetPosition().x + 0, lastExploredRoom.GetPosition().y + 1));
                spawnedRoomObjects.Add(SpawnNewRoom(lastExploredRoom.GetPosition().x + 0, lastExploredRoom.GetPosition().y - 1));

                //Check here if only 1 room was spawned
                //Spawn boss if so
                if (spawnedRooms == 1)
                {
                    Debug.Log("Only 1 space left - spawning a boss");
                    foreach(GameObject room in spawnedRoomObjects)
                    {
                        if(room == null)
                        {
                            break;
                        }
                        room.GetComponent<Room>().SetRoom(GenerateBossRoom());
                    }
                }
            }
            else
            {
                //spawn a boss here
                Debug.Log("Spawning Boss");
                //Messy looking but gets the job done to guarantee a boss spawns
                if (SpawnBossRoom(lastExploredRoom.GetPosition().x + 1, lastExploredRoom.GetPosition().y + 0) == false)
                {
                    if (SpawnBossRoom(lastExploredRoom.GetPosition().x - 1, lastExploredRoom.GetPosition().y + 0) == false)
                    {
                        if (SpawnBossRoom(lastExploredRoom.GetPosition().x + 0, lastExploredRoom.GetPosition().y + 1) == false)
                        {
                            SpawnBossRoom(lastExploredRoom.GetPosition().x + 0, lastExploredRoom.GetPosition().y - 1);
                        }
                    }
                }
            }
        }

        GenerateViewDeck();
    }

    public bool IsViewingDeck()
    {
        return viewDeck;
    }

    private void GenerateViewDeck()
    {
        float xCount = 0;
        float xMax = 4;
        float yCount = 0;
        float yMax = data.GetStartingDeck().Count;

        float yOffset = 2.5f;
        float xOffset = 1.5f;

        float startingX = deckHolder.position.x - (xMax / 2f) * xOffset;
        float lastX = deckHolder.position.x + (xMax / 2f) * xOffset; 

        float startingY = deckHolder.position.y;
        float lastY = deckHolder.position.y - yMax * yOffset;

        foreach (Card card in data.GetStartingDeck())
        {
            Vector2 position = new Vector2(Mathf.Lerp(startingX, lastX, xCount / (xMax - 1)), Mathf.Lerp(startingY, lastY, yCount / (yMax - 1)));
            GameObject go = Instantiate(cardPrefab,
                position,
                Quaternion.identity,
                deckHolder
                );
            go.GetComponent<Rune>().SetCard(card);
            xCount++;
            if(xCount == xMax)
            {
                xCount = 0;
                yCount++;
            }
        }
    }

    public void ToggleDeck()
    {
        viewDeck = !viewDeck;
        viewDeckBlackout.SetActive(viewDeck);

        SetRoomHitboxes(!viewDeck);
    }

    private void Update()
    {
        if (viewDeck)
        {
            deckHolder.position = new Vector3(deckHolder.position.x, deckHolder.position.y - Input.mouseScrollDelta.y, deckHolder.position.z);
        }
    }

    private GameObject SpawnNewRoom(float x, float y)
    {
        if (Physics2D.OverlapBox(new Vector2(x, y), new Vector2(0.2f, 0.2f), 0f) == null)
        {
            GameObject go = Instantiate(roomPrefab, new Vector2(x, y), Quaternion.identity);
            go.GetComponent<Room>().SetRoom(GenerateRoom());
            RegisterRoom(go.GetComponent<Room>());
            spawnedRooms++;
            return go;
        }
        return null;
    }

    private bool SpawnBossRoom(float x, float y)
    {
        if (Physics2D.OverlapBox(new Vector2(x, y), new Vector2(0.2f, 0.2f), 0f) == null)
        {
            GameObject go = Instantiate(roomPrefab, new Vector2(x, y), Quaternion.identity);
            go.GetComponent<Room>().SetRoom(GenerateBossRoom());
            RegisterRoom(go.GetComponent<Room>());
            spawnedRooms++;
            return true;
        }

        return false;
    }

    private void SetRoomHitboxes(bool toSet)
    {
        LayerMask mask;
        if (!toSet)
        {
            mask = 2;
        }
        else
        {
            mask = 0;
        }
        settingRooms.layer = mask;
        foreach (Room room in currentRooms)
        {
            if (room == null)
            {
                break;
            }
            room.gameObject.layer = mask;

        }
    }

    private IEnumerator PickRewards()
    {
        SetRoomHitboxes(false);

        blackout.SetActive(true);
        choicesRemaining = lastExploredRoom.GetRewardAmount();
        uiUpdater.UpdateChoicesRemaining(choicesRemaining);

        UpdateHealthToAdd();
        uiUpdater.UpdateLifeGained(healthToAdd);

        List<Card> cardsToPick = GenerateCards(lastExploredRoom.GetRewardsShown());
        cardsToBePicked = new List<GameObject>();

        float cardOffset = 1.5f;

        float xOffset = ((float)cardsToPick.Count / 2 * cardOffset);
        Vector2 startingPosition = new Vector2(-xOffset + choicePosition.transform.position.x, choicePosition.transform.position.y);
        Vector2 endPosition = new Vector2(xOffset + choicePosition.transform.position.x, choicePosition.transform.position.y);

        foreach (Card card in cardsToPick)
        {
            Vector2 spawnPosition = Vector2.Lerp(startingPosition, endPosition, (float)cardsToBePicked.Count / (float)(cardsToPick.Count - 1));
            GameObject go = Instantiate(
                cardPrefab, 
                spawnPosition,
                Quaternion.identity);
            go.GetComponent<Rune>().SetCard(card);
            cardsToBePicked.Add(go);
        }

        yield return StartCoroutine(FindObjectOfType<Fade>().FadeIn());

        while (choicesRemaining > 0)
        {
            //Need to get player choice here
            yield return new WaitForEndOfFrame();
        }
        if(cardsToBePicked.Count > 0)
        {
            //Destroy the cards
            foreach(GameObject card in cardsToBePicked)
            {
                yield return StartCoroutine(card.GetComponent<Rune>().DestroyRune());
            }
            cardsToBePicked.Clear();
        }
        blackout.SetActive(false);
        SetRoomHitboxes(true);
    }

    private void UpdateHealthToAdd()
    {
        //Maybe make this have a bit of variance
        healthToAdd = (int)(healthToAdd * (data.GetClearedLevels() * 1.1f));
    }

    //Test
    List<Card> GenerateCards(int amount)
    {
        List<Card> randomedCards = new List<Card>();
        for(int i = 0; i < amount; i++)
        {
            //Determine tier to generate -- Put these weights as variables
            float currentTier = 0.7f;
            float nextTier = 0.1f;
            float lastTier = 0.2f;

            float tierChosen = Random.Range(0, currentTier + nextTier + lastTier);
            int tier = 0;
            if (tierChosen >= currentTier + lastTier)
            {
                tier = Mathf.Clamp(lastExploredRoom.GetRoomTier() + 1, 1, MAX_TIER);
            }
            else if (tierChosen >= lastTier)
            {
                tier = lastExploredRoom.GetRoomTier();
            }
            else
            {
                tier = Mathf.Clamp(lastExploredRoom.GetRoomTier() - 1, 1, MAX_TIER);
            }
            if (i < lastExploredRoom.GetRewardAmount())
            {
                List<Card> potentialCards = new List<Card>();

                while (potentialCards.Count == 0)
                {
                    potentialCards = data.GetCardListOfTypeAndTier(lastExploredRoom.GetRewardType(), tier);
                    tier--;
                }
                randomedCards.Add(potentialCards[Random.Range(0, potentialCards.Count)]);
            }
            else
            {
                //Add Weights for what type of card
                List<Card> potentialCards = new List<Card>();
                while (potentialCards.Count == 0)
                {
                    potentialCards = data.GetCardListOfTier(tier);
                    tier--;
                }
                randomedCards.Add(potentialCards[Random.Range(0, potentialCards.Count)]);
            }
        }
        return randomedCards;
    }

    RoomData GenerateBossRoom()
    {
        List<RoomData> bossRooms = GetRoomsOfTier(MAX_TIER + 1);

        return bossRooms[ Random.Range(0, bossRooms.Count)];
    }

    RoomData GenerateRoom()
    {
        //Very bad for performance when we call this 4 times - can definitely cache it somehow
        List<RoomData> nextTierRooms = GetRoomsOfTier(lastExploredRoom.GetRoomData().GetRoomTier() + 1);
        List<RoomData> currentTierRooms = GetRoomsOfTier(lastExploredRoom.GetRoomData().GetRoomTier());
        List<RoomData> lastTierRooms = GetRoomsOfTier(lastExploredRoom.GetRoomData().GetRoomTier() - 1);

        //Generate some weights based on if nextTierRooms has any in it
        float tier = Random.Range(0, nextTierWeight + currentTierWeight + lastTierWeight);
        if(tier >= lastTierWeight + nextTierWeight)
        {
            if (nextTierRooms.Count < 1 || lastExploredRoom.GetRoomTier() == MAX_TIER)
            {
                //Generate from current tier
                return currentTierRooms[Random.Range(0, currentTierRooms.Count)];
            }
            else
            {
                return nextTierRooms[Random.Range(0, nextTierRooms.Count)];
            }
        }
        else if(tier >= lastTierWeight)
        {
            return currentTierRooms[Random.Range(0, currentTierRooms.Count)];
        }
        else
        {
            if(lastTierRooms.Count < 1)
            {
                return currentTierRooms[Random.Range(0, currentTierRooms.Count)];
            }
            else
            {
                return lastTierRooms[Random.Range(0, lastTierRooms.Count)];
            }
        }
    }

    List<RoomData> GetRoomsOfTier(int tier)
    {
        List<RoomData> tierRooms = new List<RoomData>();

        foreach(RoomData room in availableRooms)
        {
            if(room.GetRoomTier() == tier)
            {
                tierRooms.Add(room);
            }
        }

        return tierRooms;
    }

    public void RegisterRoom(Room newRoom)
    {
        currentRooms.Add(newRoom);
    }

    public void SetLastExploredRoom(Room lastRoom)
    {
        lastExploredRoom = lastRoom;
    }

    public void LoadFightScene()
    {
        StartCoroutine(FightSceneLoad());
    }

    private IEnumerator FightSceneLoad()
    {
        for (int i = currentRooms.Count - spawnedRooms; i < currentRooms.Count; i++)
        {
            if (currentRooms[i] != lastExploredRoom)
            {
                currentRooms[i].SetSkipped();
            }
        }

        data.SetLastEnteredRoom(lastExploredRoom);
        data.SetBackgroundObject(lastExploredRoom.GetRoomData().GetBackgroundTiles());
        data.SetEnemyObject(lastExploredRoom.GetRoomData().GetEnemy().GetEnemy());
        data.SetCurrentRooms(currentRooms);

        yield return StartCoroutine(FindObjectOfType<Fade>().FadeOut());

        SceneManager.LoadScene("FightScene");
    }

    public void PickCard(Rune pickedCard)
    {
        //Move card to deck position & add it to teh deck
        //Probs need a boolean here to see when its complete
        StartCoroutine(HandlePickedCard(pickedCard));
    }

    private IEnumerator HandlePickedCard(Rune pickedCard)
    {
        yield return StartCoroutine(pickedCard.MoveCardAndScaleDown(deckPosition.position, 20));
        data.AddCard(pickedCard.GetCard());
        choicesRemaining--;
        uiUpdater.UpdateChoicesRemaining(choicesRemaining);
        uiUpdater.UpdateDeckCount();
        takeLifeButton.SetActive(false); //Potentially change how this is handled rather than just disabling, quick coroutine maybe? the first time its pressed
        cardsToBePicked.Remove(pickedCard.gameObject);
    }

    public void Addlife()
    {
        //TODO
        //Determine life to add
        data.AddHealth(healthToAdd);
        uiUpdater.UpdateHealth();
        choicesRemaining = 0;
    }

    private bool viewOptions = false;

    public bool IsViewingOptions()
    {
        return viewOptions;
    }

    private bool firstSetup = true;

    public void ShowOptions()
    {
        viewOptions = true;
        options.SetActive(true);
        SetupOptions();
    }

    public void ToggleOptions()
    {
        viewOptions = !viewOptions;
        options.SetActive(viewOptions);
        SetupOptions();
    }

    private void SetupOptions()
    {
        if (firstSetup)
        {
            firstSetup = false;
            GameObject.Find("VolumeSlider").GetComponent<Slider>().value = FindObjectOfType<GameSettings>().GetMusicVolume();
            GameObject.Find("SFX Slider").GetComponent<Slider>().value = FindObjectOfType<GameSettings>().GetSFXVolume();
        }
    }

    public void HideOptions()
    {
        viewOptions = false;
        options.SetActive(false);
    }
}
