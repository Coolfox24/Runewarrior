using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class BuffIcons : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI turnsRemaining;
    [SerializeField] TextMeshProUGUI stackCount;
    [SerializeField] SpriteRenderer spriteRenderer;
    RuneTags type;


    [SerializeField] Sprite burnSprite;
    [SerializeField] Sprite windSprite;
    [SerializeField] Sprite bleedSprite;
    [SerializeField] Sprite shockedSprite;
    [SerializeField] Sprite shieldSprite;

    private float potency;
    private int stackCountInt;

    public void OnMouseEnter()
    {
        switch (type)
        {
            case RuneTags.FIRE:
                GetComponentInParent<HealthBarUI>().SetTooltip(burnSprite, "Burn", "Turns Remaining: " + turnsRemaining.text, "Damage: " + potency, " ");
                break;
            case RuneTags.PHYSICAL:
                GetComponentInParent<HealthBarUI>().SetTooltip(bleedSprite, "Bleed", "Turns Remaining: " 
                    + turnsRemaining.text, "Damage: " + potency, "Stacks: " + stackCountInt);
                break;
            case RuneTags.THUNDER:
                GetComponentInParent<HealthBarUI>().SetTooltip(shockedSprite, "Shock", "Turns Remaining: " + turnsRemaining.text, "Multiplier: " + (potency * 100) + "%" , " ");
                break;
            case RuneTags.WATER:
                GetComponentInParent<HealthBarUI>().SetTooltip(shieldSprite, "Water Shield", "Turns Remaining: " + turnsRemaining.text, "DR: " + System.Math.Round(((1 - potency) * 100), 2) + "%", " ");
                break;
            case RuneTags.WIND:
                GetComponentInParent<HealthBarUI>().SetTooltip(windSprite, "Airspeed", "Extra Card Draw", "Turns Remaining: " + turnsRemaining.text, " ");
                break;
        }

    }

    public void OnMouseExit()
    {
        //Hide
        GetComponentInParent<HealthBarUI>().UnsetTooltip();
    }

    public void Setup(RuneTags type, int turnsRemaining, int stacks, float potency)
    {
        this.turnsRemaining.text = turnsRemaining.ToString();
        this.potency = potency;
        if (stacks == 0)
        {
            stackCount.text = " ";
        }
        else
        {
            //stackCount.text = stacks.ToString();
            stackCountInt = stacks;
            stackCount.text = " ";
        }
        this.type = type;
        switch (type)
        {
            case RuneTags.FIRE:
                spriteRenderer.sprite = burnSprite;
                break;
            case RuneTags.WATER:
                spriteRenderer.sprite = shieldSprite;
                break;
            case RuneTags.THUNDER:
                spriteRenderer.sprite = shockedSprite;
                break;
            case RuneTags.WIND:
                spriteRenderer.sprite = windSprite;
                break;
            case RuneTags.PHYSICAL:
                spriteRenderer.sprite = bleedSprite;
                break;
            default:
                break;
        }
    }

    public void UpdateIcon(int turnsRemaining, int stacks, float potency)
    {
        this.potency = potency;
        this.turnsRemaining.text = turnsRemaining.ToString();
        if (stacks == 0)
        {
            stackCount.text = " ";
        }
        else
        {
            //stackCount.text = stacks.ToString();
            stackCountInt = stacks;
            stackCount.text = " ";
        }
    }

    public RuneTags GetRune()
    {
        return type;
    }
}
