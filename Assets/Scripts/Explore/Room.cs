using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    [SerializeField] SpriteRenderer enemySprite;
    [SerializeField] SpriteRenderer rewardsSprite;
    [SerializeField] SpriteRenderer background;
    [SerializeField] SpriteRenderer border;
    [SerializeField] GameObject fadeOut;
    [SerializeField] Color physRewardColor;
    [SerializeField] Color fireRewardColor;
    [SerializeField] Color thunderRewardColor;
    [SerializeField] Color waterRewardColor;
    [SerializeField] Color windRewardColor;
    [SerializeField] Color arcaneRewardColor;

    //TODO Make this randomed by Explore Controller
    [SerializeField] RoomData myRoom;
    bool isCleared = false;
    bool isSkipped = false;

    Vector2 myPosition;
    ExploreController exploreController;

    private void Start()
    {
        //SetRoom(myRoom);
        if(isSkipped)
        {
            fadeOut.SetActive(true);
        }
        else
        {
            fadeOut.SetActive(false);
        }
        exploreController = FindObjectOfType<ExploreController>();
    }

    public void Setup(bool skipped, bool cleared)
    {
        isCleared = cleared;
        isSkipped = skipped;
    }

    public void SetRoom(RoomData newRoom)
    {
        myPosition = transform.position;

        myRoom = newRoom;
        enemySprite.sprite = myRoom.GetEnemy().GetExploreSprite();
        background.color = myRoom.Getbackground();
        background.sprite = myRoom.GetRoomSprite();
        SetRewardsColor();
    }

    public int GetRoomTier()
    {
        return myRoom.GetRoomTier();
    }

    public Vector2 GetPosition()
    {
        return myPosition;
    }

    public RuneTags GetRewardType()
    {
        return myRoom.GetRewardType();
    }

    private void SetRewardsColor()
    {
        switch (myRoom.GetRewardType())
        {
            case RuneTags.PHYSICAL:
                rewardsSprite.color = physRewardColor;
                break;
            case RuneTags.ARCANE:
                rewardsSprite.color = arcaneRewardColor;
                break;
            case RuneTags.FIRE:
                rewardsSprite.color = fireRewardColor;
                break;
            case RuneTags.WATER:
                rewardsSprite.color = waterRewardColor;
                break;
            case RuneTags.WIND:
                rewardsSprite.color = windRewardColor;
                break;
            case RuneTags.THUNDER:
                rewardsSprite.color = thunderRewardColor;
                break;
            default:
                Debug.Log("Set a reward type to room: " + myRoom.name);
                break;
        }
    }

    private void OnMouseEnter()
    {
        if (!isSkipped && !isCleared && !exploreController.IsViewingDeck())
        {
            border.enabled = true;
        }
    }

    private void OnMouseExit()
    {
        if (!isSkipped && !isCleared)
        {
            border.enabled = false;
        }
    }

    private void OnMouseDown()
    {
        //Load Scene
        if (!isSkipped && !isCleared && !exploreController.IsViewingDeck())
        {  
            exploreController.SetLastExploredRoom(this);
            exploreController.LoadFightScene();
        }
    }

    public bool IsCleared()
    {
        return isCleared;
    }

    public void SetCleared()
    {
        isCleared = true;
    }

    public bool IsSkipped()
    {
        return isSkipped;
    }

    public void SetSkipped()
    {
        isSkipped = true;
    }

    public RoomData GetRoomData()
    {
        return myRoom;
    }

    public int GetRewardAmount()
    {
        return myRoom.GetRewardAmount();
    }

    public int GetRewardsShown()
    {
        return myRoom.GetRewardsShown();
    }
}
