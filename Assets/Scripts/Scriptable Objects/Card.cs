using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum RuneTags
{
    PHYSICAL = 1,
    FIRE = 2,
    WATER = 3,
    THUNDER = 4,
    WIND = 5,
    ARCANE = 6,
    NOT_SELECTED
}

[CreateAssetMenu(fileName = "Card", menuName = "Runes", order = 1)]
public class Card : ScriptableObject
{
    [Serializable]
    struct RunesProvided
    {
        public RuneTags rune;
        public int amount;
    }

    RuneTags[] returnTags;

    [SerializeField] Sprite sprite;
    [SerializeField] RunesProvided[] runeTags;

    public Sprite GetSprite()
    {
        return sprite;
    }

    public int GetRuneAmount(RuneTags type)
    {
        foreach(RunesProvided r in runeTags)
        {
            if(r.rune == type)
            {
                return r.amount;
            }
        }

        return 0;
    }

    private void CalcRunes()
    {
        List<RuneTags> tags = new List<RuneTags>();
        foreach (RunesProvided r in runeTags)
        {
            for (int i = 0; i < r.amount; i++)
            {
                tags.Add(r.rune);
            }
        }

        returnTags = tags.ToArray();
    }

    //Used for generating rewards for player
    public int GetTier()
    {
        int tier = 0;

        foreach (RunesProvided r in runeTags)
        {
            tier += r.amount;
        }

        return tier;
    }

    //Used for generating rewards for player
    public RuneTags GetAffinity()
    {
        RunesProvided curAffinity = new RunesProvided
        {
            amount = 0
        };

        foreach (RunesProvided r in runeTags)
        {
            if(r.amount > curAffinity.amount)
            {
                curAffinity = r;
            }
        }

        return curAffinity.rune;
    }

    public RuneTags[] GetRuneTags()
    {
        CalcRunes();
        return returnTags;
    }
}
