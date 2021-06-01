using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buffs : MonoBehaviour
{
    float burnAmount;
    int burnTurnsRemaining;

    float bleedAmount = 0;
    int bleedTurnsRemaining;
    int bleedCount = 0;

    bool isShocked;
    float shockedAmount; //number from 1 to 2

    float waterReduction = 1; //number from 0 to 1
    int waterReductionTurns; 

    float windDrawTurnsRemaining; //Extra draw for player, enemies random 2 abilities & pick higher potency 1

    Health myHealth;
    HealthBarUI myHealthBar;

    private void Start()
    {
        myHealth = GetComponent<Health>();
        myHealthBar = GetComponentInChildren<HealthBarUI>();
    }

    public IEnumerator EndOfTurnEffects()
    {
        yield return StartCoroutine(BleedEffects());
        yield return StartCoroutine(BurnEffects());

        yield return new WaitForEndOfFrame();
    }

    //Applied to the player
    public void SetBuffs(PlayerAttack curAttack)
    {
        SetBuffs(curAttack.GetAffinities(), curAttack.GetDamage(), curAttack.GetTotalRunes());
    }

    public void SetDebuffs(EnemyAttack curAttack)
    {
        SetDebuffs(curAttack.GetAffinities(), curAttack.GetDamage(), curAttack.GetTotalRunes(), curAttack.GetAnimationTriggerAmount());
    }

    //Applied to the enemy
    public void SetBuffs(EnemyAttack curAttack)
    {
        SetBuffs(curAttack.GetAffinities(), curAttack.GetDamage(), curAttack.GetTotalRunes());
    }

    public void SetDebuffs(PlayerAttack curAttack)
    {
        SetDebuffs(curAttack.GetAffinities(), curAttack.GetDamage(), curAttack.GetTotalRunes(), curAttack.GetAnimationTriggerAmount());
    }

    private void SetBuffs(PlayerAttack.RunesRequired[] affinities, float potency, float totalRunes)
    {
        foreach (PlayerAttack.RunesRequired runes in affinities)
        {
            switch (runes.rune)
            {
                case RuneTags.WATER:
                    waterReductionTurns = 2;
                    waterReduction = (potency * 0.5f) * runes.amount / totalRunes;
                    waterReduction = 1 - (waterReduction/100);
                    myHealthBar.AddBuff(RuneTags.WATER, waterReductionTurns, 0);
                    break;
                case RuneTags.WIND:
                    windDrawTurnsRemaining = Mathf.Ceil(potency * 0.02f * (float)runes.amount / totalRunes);
                    myHealthBar.AddBuff(RuneTags.WIND, (int)windDrawTurnsRemaining, 0);
                    break;
                default:
                    break;
            }
        }
    }

    private void SetDebuffs(PlayerAttack.RunesRequired[] affinities, float potency, float totalRunes, int animationTriggers)
    {
        foreach (PlayerAttack.RunesRequired runes in affinities
)
        {
            switch (runes.rune)
            {
                case RuneTags.FIRE:
                    burnTurnsRemaining = 2;
                    burnAmount = potency * 0.5f * runes.amount / totalRunes * animationTriggers;
                    myHealthBar.AddBuff(RuneTags.FIRE, burnTurnsRemaining, 0);
                    break;
                case RuneTags.THUNDER:
                    isShocked = true;
                    shockedAmount =  1f + (1f / (100 / potency) * runes.amount / totalRunes);
                    myHealthBar.AddBuff(RuneTags.THUNDER, 1, 0);
                    break;
                case RuneTags.PHYSICAL:
                    bleedCount++;
                    bleedTurnsRemaining = 2; //Reset back to 2
                    bleedAmount += potency * 0.25f * runes.amount / totalRunes;
                    myHealthBar.AddBuff(RuneTags.PHYSICAL, bleedTurnsRemaining, 0);
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator BleedEffects()
    {
        if(bleedTurnsRemaining > 0)
        {
            GetComponent<Animator>().SetTrigger("Bleed");
            //Hard coded time for animation
            yield return new WaitForSeconds(0.2f);

            bleedTurnsRemaining--;
            myHealthBar.UpdateBuff(RuneTags.PHYSICAL, bleedTurnsRemaining, 0);
            //Spawn a VFX for bleeding or something here
            myHealth.TakeDamage((int)bleedAmount);

            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            bleedAmount = 0;//Reset back to 0 as it can stack
            bleedCount = 0;
        }
    }

    IEnumerator BurnEffects()
    {
        if(burnTurnsRemaining > 0)
        {
            GetComponent<Animator>().SetTrigger("Burn");
            //Hard coded time for animation
            yield return new WaitForSeconds(0.2f);

            burnTurnsRemaining--;
            myHealthBar.UpdateBuff(RuneTags.FIRE, burnTurnsRemaining, 0);
            //SPawn a VFX for bleeding or something here
            myHealth.TakeDamage((int)burnAmount);

            yield return new WaitForSeconds(0.2f);
        }
    }

    public float GetShockAmount()
    {
        if (isShocked == true)
        {
            isShocked = false;
            myHealthBar.UpdateBuff(RuneTags.THUNDER, 0, 0);
            return shockedAmount;
        }

        return 1;
    }

    public bool IsWindActive()
    {
        if(windDrawTurnsRemaining > 0)
        {
            windDrawTurnsRemaining--;
            myHealthBar.UpdateBuff(RuneTags.WIND, (int)windDrawTurnsRemaining, 0);
            return true;
        }
        return false;
    }

    public float GetDamageReduction()
    {
        if( waterReductionTurns > 0)
        {
            waterReductionTurns--;
            myHealthBar.UpdateBuff(RuneTags.WATER, waterReductionTurns, 0);
            return waterReduction;
        }
        return 1;
    }
}
