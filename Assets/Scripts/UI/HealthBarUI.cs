using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Health healthToTrack;
    [SerializeField] Slider mySlider;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] GameObject shield;
    [SerializeField] TextMeshProUGUI shieldText;
    [SerializeField] Transform buffStart;
    [SerializeField] Transform debuffStart;
    [SerializeField] bool buffGrowRight = true;
    [SerializeField] float iconOffset = 0.5f;

    [SerializeField] GameObject buffPrefab;

    List<BuffIcons> currentBuffs;

    // Update is called once per frame
    private void Start()
    {
        mySlider.maxValue = healthToTrack.GetMaxHealth();
        currentBuffs = new List<BuffIcons>();
    }

    public void UpdateHealthBar()
    {
        mySlider.maxValue =  healthToTrack.GetMaxHealth();
        hpText.text = healthToTrack.GetHealth().ToString() + "/" + healthToTrack.GetMaxHealth().ToString();
        mySlider.value = healthToTrack.GetHealth();

        if(healthToTrack.GetShield() > 0)
        {
            shield.SetActive(true);
            shieldText.text = healthToTrack.GetShield().ToString();
        }
        else
        {
            shield.SetActive(false);
        }
    }

    public void AddBuff(RuneTags type, int turnsRemaining, int stackCount)
    {
        foreach(BuffIcons buff in currentBuffs)
        {
            if(buff.GetRune() == type)
            {
                buff.UpdateIcon(turnsRemaining, stackCount);
                return;
            }
        }
        if (IsBuff(type))
        {
            Vector2 buffSpot = buffStart.position;
            if(buffGrowRight)
            {
                buffSpot.x += iconOffset * GetBuffCount();
            }
            else
            {
                buffSpot.x -= iconOffset * GetBuffCount();
            }

            GameObject go = Instantiate(buffPrefab, buffSpot, Quaternion.identity, this.gameObject.transform);
            go.GetComponent<BuffIcons>().Setup(type, turnsRemaining, stackCount);
            currentBuffs.Add(go.GetComponent<BuffIcons>());
        }
        else
        {
            Vector2 buffSpot = debuffStart.position;
            if (!buffGrowRight)
            {
                buffSpot.x += iconOffset * GetDebuffCount();
            }
            else
            {
                buffSpot.x -= iconOffset * GetDebuffCount();
            }

            GameObject go = Instantiate(buffPrefab, buffSpot, Quaternion.identity, this.gameObject.transform);
            go.GetComponent<BuffIcons>().Setup(type, turnsRemaining, stackCount);
            currentBuffs.Add(go.GetComponent<BuffIcons>());
        }
        ResetBuffPositions();
    }

    private bool IsBuff(RuneTags type)
    {
        if (type == RuneTags.FIRE || type == RuneTags.PHYSICAL || type == RuneTags.THUNDER)
        {
            return false;
        }
        return true;
    }

    private int GetBuffCount()
    {
        int count = 0;

        foreach(BuffIcons buff in currentBuffs)
        {
            if(IsBuff(buff.GetRune()))
            {
                count++;
            }
        }
        return count;
    }

    private int GetDebuffCount()
    {
        int count = 0;

        foreach (BuffIcons buff in currentBuffs)
        {
            if (!IsBuff(buff.GetRune()))
            {
                count++;
            }
        }
        return count;
    }

    public void UpdateBuff(RuneTags type, int turnsRemaining, int stackCount)
    {
        if(turnsRemaining == 0)
        {
            //destroy  the gameobject of type
            foreach(BuffIcons buff in currentBuffs)
            {
                if(buff.GetRune() == type)
                {
                    currentBuffs.Remove(buff);
                    Destroy(buff.gameObject);
                    return;
                }
            }
        }

        foreach(BuffIcons buff in currentBuffs)
        {
            if(buff.GetRune() == type)
            {
                buff.UpdateIcon(turnsRemaining, stackCount);
            }
        }
        ResetBuffPositions();
    }

    public void ResetBuffPositions()
    {
        int buffCount = 0;
        int debuffCount = 0;

        float iconOffsetNew = iconOffset;

        if(!buffGrowRight)
        {
            iconOffsetNew = -iconOffsetNew;
        }

        foreach(BuffIcons buff in currentBuffs)
        {
            if(IsBuff(buff.GetRune()))
            {
                Transform t = buff.gameObject.GetComponent<Transform>();
                t.position = new Vector2(buffStart.position.x + (iconOffsetNew * buffCount), t.position.y);
                buffCount++;
            }
            else
            {
                Transform t = buff.gameObject.GetComponent<Transform>();
                t.position = new Vector2(debuffStart.position.x - (iconOffsetNew * debuffCount), t.position.y);
                debuffCount++;
            }
        }
    }
}
