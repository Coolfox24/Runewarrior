using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttacks", menuName = "Player Attack", order = 1)]
public class PlayerAttack : ScriptableObject
{
    [Serializable]
    public struct RunesRequired
    {
        public RuneTags rune;
        public int amount;
    }

    [SerializeField] RunesRequired[] runesRequired;
    [SerializeField] int totalPotency;
    [SerializeField] int potencyTriggersInAnimation = 1;
    [SerializeField] AnimationClip attackAnimation;
    [SerializeField] bool isMelee = false;
    [SerializeField] VFX spellVFX = null;
    [SerializeField] AudioClip animationSFX = null;
    [SerializeField] string attackName;

    public bool CanCast(Dictionary<RuneTags, int> activeRunes)
    {
        for(int i = 0; i < runesRequired.Length; i++)
        {
            if(activeRunes[runesRequired[i].rune] < runesRequired[i].amount)
            {
                return false;
            }
            //Should minus the runes -- depends if active runes passes the value
            //activeRunes[runesRequired[i].rune] -= runesRequired[i].amount;
        }
        return true;
    }

    public void ReduceRunes(ref Dictionary<RuneTags, int> activeRunes)
    {
        //Only called if required runes already in active runes
        for (int i = 0; i < runesRequired.Length; i++)
        {
             activeRunes[runesRequired[i].rune] -= runesRequired[i].amount;
        }
    }

    public VFX GetSpellVFX()
    {
        return spellVFX;
    }

    public AnimationClip GetClip()
    {
        return attackAnimation;
    }

    public int GetDamage()
    {
        if (potencyTriggersInAnimation == 0)
        {
            Debug.LogError(name + ": Currently has a 0 set in damageFramesInAnimation. Until fixed this attack will deal 0 damage");
            return 0;
        }

        return totalPotency / potencyTriggersInAnimation;
    }

    public bool IsMelee()
    {
        return isMelee;
    }

    public int GetHealing()
    {
        return GetDamage();
    }

    public AudioClip GetSFX()
    {
        return animationSFX;
    }

    public string GetName()
    {
        return attackName;
    }

    public RunesRequired[] GetAffinities()
    {
        return runesRequired;
    }

    public int GetTotalRunes()
    {
        int totalRunes = 0;
        foreach(RunesRequired runes in runesRequired)
        {
            totalRunes += runes.amount;
        }

        return totalRunes;
    }

    public int GetAnimationTriggerAmount()
    {
        return potencyTriggersInAnimation;
    }
}
