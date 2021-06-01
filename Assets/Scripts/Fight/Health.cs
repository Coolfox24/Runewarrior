using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth;
    HealthBarUI myHealthBar;
    int curHealth;
    int curShield;
    TargetPoints targetPoints;
    [SerializeField] GameObject damageText;
    [SerializeField] float offset = 1f;


    private void Start()
    {
        SetHealth(maxHealth);
        targetPoints = GetComponent<TargetPoints>();
    }

    public void SetHealth(int amount)
    {
        maxHealth = amount;

        myHealthBar = GetComponentInChildren<HealthBarUI>();
        curHealth = maxHealth;
        myHealthBar.UpdateHealthBar();
    }

    public void SetScaledHealth(float scaleFactor)
    {
        int newHealth = (int) (maxHealth * scaleFactor);
        SetHealth(newHealth);
    }

    public void TakeDamage( int amount)
    {
        //Spawn damage text even if its overkill
        Vector2 spawnLoc = targetPoints.GetPosition(TargetSpots.HEAD).position;
        spawnLoc.x += Random.Range(-offset, offset);
        Instantiate(damageText, spawnLoc, Quaternion.identity)
            .GetComponent<DamageText>().SetDamageText(amount.ToString());

        if(curHealth == 0)
        {
            //to avoid animations triggering 2 deaths
            return;
        }

        curShield -= amount;

        if(curShield < 0)
        {
            curHealth = Mathf.Max(curHealth + curShield, 0);
            curShield = 0;
        }

        if(curHealth <= 0)
        {
            PlayDieAnimation();
        }
        else
        {
            GetComponent<Animator>().SetTrigger("Damage"); //Don't start damage animation if dead
        }
        myHealthBar.UpdateHealthBar();
    }

    private void PlayDieAnimation()
    {
        GetComponent<Animator>().SetTrigger("Die");
    }

    public void AddHealth(int amount, bool CanOverHeal)
    {
        if (CanOverHeal == false)
        {
            curHealth = Mathf.Clamp(curHealth + amount, 0, maxHealth);
        }
        else
        {
            curHealth += amount;
        }
        myHealthBar.UpdateHealthBar();
    }

    public int GetHealth()
    {
        return curHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

   public bool IsDead()
    {
        return curHealth <= 0;
    }

    public int GetShield()
    {
        return curShield;
    }
    
    public void AddShield(int amount)
    {
        curShield += amount;
        myHealthBar.UpdateHealthBar();
    }
}
