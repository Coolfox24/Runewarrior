using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy Data", order = 1)]
public class EnemyData : ScriptableObject
{
    [SerializeField] GameObject enemyObject;
    [SerializeField] Sprite exploreSprite;

    public GameObject GetEnemy()
    {
        return enemyObject;
    }

    public Sprite GetExploreSprite()
    {
        return exploreSprite;
    }
}
