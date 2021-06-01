using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Rune : MonoBehaviour
{
    bool isSelected;
    [SerializeField] float ySelectedOffset = 0.3f;
    float yDefault;
    float ySelected;
    Card myCard;
    SpriteRenderer spriteRenderer;
    GameController gameController;
    ExploreController exploreController;
    private bool moving = false;
    [SerializeField] GameObject tooltipUI;

    [SerializeField] TextMeshProUGUI physRunes;
    [SerializeField] TextMeshProUGUI fireRunes;
    [SerializeField] TextMeshProUGUI waterRunes;
    [SerializeField] TextMeshProUGUI thunderRunes;
    [SerializeField] TextMeshProUGUI windRunes;
    [SerializeField] TextMeshProUGUI arcaneRunes;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        exploreController = FindObjectOfType<ExploreController>();
        tooltipUI.SetActive(false);
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void SetCard(Card card)
    {
        myCard = card;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = card.GetSprite();
        SetTooltipRunes();
       // GetComponent<Animator>().enabled = true;
    }

    private void SetTooltipRunes()
    {
        physRunes.text = myCard.GetRuneAmount(RuneTags.PHYSICAL).ToString();
        fireRunes.text = myCard.GetRuneAmount(RuneTags.FIRE).ToString();
        waterRunes.text = myCard.GetRuneAmount(RuneTags.WATER).ToString();
        thunderRunes.text = myCard.GetRuneAmount(RuneTags.THUNDER).ToString();
        windRunes.text = myCard.GetRuneAmount(RuneTags.WIND).ToString();
        arcaneRunes.text = myCard.GetRuneAmount(RuneTags.ARCANE).ToString();
    }

    private void OnMouseDown()
    {
        if (gameController != null)
        {
            if (gameController.CanInteract() == true)
            {
                isSelected = !isSelected;
                SetPosition(transform.position.x, transform.position.y, false);

                FindObjectOfType<CurrentAttackUI>().UpdateText();
            }
        }
        if(exploreController != null)
        {
            if(exploreController.IsViewingDeck())
            {
                return;
            }

            exploreController.PickCard(this);
        }
    }

    private void OnMouseEnter()
    {
        tooltipUI.SetActive(true);
    }

    private void OnMouseExit()
    {
        tooltipUI.SetActive(false);
    }

    public void SetPosition(float x, float y, bool firstTime)
    {
        if (!firstTime)
        {
            if (isSelected)
            {
                y = ySelected;
            }
            else
            {
                y = yDefault;
            }
        }
        else
        {
            yDefault = y;
            ySelected = y + ySelectedOffset;
        }
        StartCoroutine(MoveCard(new Vector2(x, y), 12));
    }


    public IEnumerator MoveCard(Vector2 newPosition, float moveSpeed)
    {
        moving = true;
        float currentFrame = 0.0f;
        Vector2 startingPosition = transform.position;
        Vector2 curPosition = transform.position;
        while (curPosition != newPosition)
        {
            transform.position =  Vector2.Lerp(startingPosition, newPosition, currentFrame);
            curPosition = transform.position;
            currentFrame += Time.deltaTime * moveSpeed;
            yield return new WaitForEndOfFrame();
        }
        moving = false;
    }

    public IEnumerator MoveCardAndScaleDown(Vector2 newPosition, float moveSpeed)
    {
        moving = true;
        float currentFrame = 0.0f;
        Vector2 startingPosition = transform.position;
        Vector2 curPosition = transform.position;
        while (curPosition != newPosition)
        {
            transform.position = Vector2.Lerp(startingPosition, newPosition, currentFrame);
            curPosition = transform.position;
            transform.localScale = new Vector3(1 - currentFrame, 1 - currentFrame, 0);
            currentFrame += Time.deltaTime * moveSpeed;
            yield return new WaitForEndOfFrame();
        }
        moving = false;
    }

    public bool IsMoving()
    {
        return moving;
    }

    public RuneTags[] GetRuneTags()
    {
        return myCard.GetRuneTags();
    }

    public Card GetCard()
    {
        return myCard;
    }

    public IEnumerator DestroyRune()
    {
        //Do stuff
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("DestroyCard");
        AnimationClip[] acs = animator.runtimeAnimatorController.animationClips;
        foreach(AnimationClip ac in acs)
        {
            if(ac.name == "DestroyCard")
            {
                yield return new WaitForSeconds(ac.length);
            }
        }
        Destroy(this.gameObject);
        yield return new WaitForEndOfFrame();
    }
}
