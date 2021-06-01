using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttack", menuName = "Enemy Attack", order = 1)]
public class EnemyAttack : ScriptableObject 
{
    [SerializeField] int totalPotency;
    [SerializeField] int potencyTriggersInAnimation = 1;

    [SerializeField] AnimationClip attackAnimation;
    [SerializeField] bool isMelee = false;
    [SerializeField] VFX spellVFX = null;
    [SerializeField] AudioClip animationSFX = null;
    [SerializeField] string attackName;

    [SerializeField] PlayerAttack.RunesRequired[] runesRequired;


    public AnimationClip GetClip()
    {
        return attackAnimation;
    }

    public int GetDamage()
    {
        if(potencyTriggersInAnimation == 0)
        {
            Debug.LogError(name + ": Currently has a 0 set in damageFramesInAnimation. Until fixed this attack will deal 0 damage");
            return 0;
        }

        return totalPotency/potencyTriggersInAnimation;
    }

    public bool IsMelee()
    {
        return isMelee;
    }

    public int GetHealing()
    {
        return GetDamage();
    }

    public VFX GetSpellVFX()
    {
        return spellVFX;
    }

    public AudioClip GetSFX()
    {
        return animationSFX;
    }

    public string GetName()
    {
        return attackName;
    }

    public PlayerAttack.RunesRequired[] GetAffinities()
    {
        return runesRequired;
    }

    public int GetTotalRunes()
    {
        int totalRunes = 0;
        foreach (PlayerAttack.RunesRequired runes in runesRequired)
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
