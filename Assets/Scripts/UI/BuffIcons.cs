using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public void Setup(RuneTags type, int turnsRemaining, int stacks)
    {
        this.turnsRemaining.text = turnsRemaining.ToString();
        
        if(stacks == 0)
        {
            stackCount.text = " ";
        }
        else
        {
            stackCount.text = stacks.ToString();
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

    public void UpdateIcon(int turnsRemaining, int stacks)
    {
        this.turnsRemaining.text = turnsRemaining.ToString();
        if (stacks == 0)
        {
            stackCount.text = " ";
        }
        else
        {
            stackCount.text = stacks.ToString();
        }
    }

    public RuneTags GetRune()
    {
        return type;
    }
}
