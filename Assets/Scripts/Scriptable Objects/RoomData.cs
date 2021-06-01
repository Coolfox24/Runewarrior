using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "Room", order = 1)]
public class RoomData : ScriptableObject
{
    [SerializeField] Color backgroundColor;
    [SerializeField] RuneTags rewardType;
    [SerializeField] int rewardAmount;
    [SerializeField] int rewardsShown;
    [SerializeField] GameObject backgroundTiles;
    [SerializeField] Sprite roomSprite;
    [SerializeField] EnemyData enemy ;
    [SerializeField] int roomTier;

    public EnemyData GetEnemy()
    {
        return enemy;
    }

    public Color Getbackground()
    {
        return backgroundColor;
    }
    
    public Sprite GetRoomSprite()
    {
        return roomSprite;
    }

    public RuneTags GetRewardType()
    {
        return rewardType;
    }

    public int GetRewardAmount()
    {
        return rewardAmount;
    }

    public GameObject GetBackgroundTiles()
    {
        return backgroundTiles;
    }

    public int GetRoomTier()
    {
        return roomTier;
    }

    public int GetRewardsShown()
    {
        return rewardsShown;
    }
}
