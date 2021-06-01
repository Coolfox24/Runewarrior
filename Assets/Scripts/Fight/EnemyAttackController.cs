using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyAttackController : MonoBehaviour
{
    [SerializeField] [Tooltip("Higher up increases priority")] EnemyAttack[] listEnemyAttacks;
    [SerializeField] float moveVelocity = 4;
    [SerializeField] Transform spellVFXPosition;
    [SerializeField] AudioClip walkSFX = null;
    [SerializeField] AudioClip idleSFX = null;
    [SerializeField] string enemyName;
    [SerializeField] bool isScripted;
    [SerializeField] Transform noFlip;

    Buffs mybuffs;
    private EnemyAttack curAttack;
    private int curAttackIndex = 0;
    Transform spellVFXContainer;
    Animator animator;
    //private bool currentlyAnimating = false;
    GameController gameController;

    Vector2 startingPosition;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        startingPosition = transform.position;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        gameController = FindObjectOfType<GameController>();

        mybuffs = GetComponent<Buffs>();
        spellVFXContainer = GameObject.FindGameObjectWithTag("vfxContainer").transform;
        GetComponent<Health>().SetScaledHealth(gameController.GetEnemyScaling());
    }

    private void DetermineAttack()
    {
        //Random Attack pattern
        if (!isScripted)
        {
            int randomIndex = Random.Range(0, listEnemyAttacks.Length);
            //Debug.Log(randomIndex);
            curAttack = listEnemyAttacks[randomIndex];
        }
        else
        {
            if (curAttackIndex >= listEnemyAttacks.Length)
            {
                curAttackIndex = 0;
            }
            curAttack = listEnemyAttacks[curAttackIndex];
            curAttackIndex++;
        }
    }

    public IEnumerator DoAttack(GameObject infoBox, TextMeshProUGUI infoText)
    {
        //Function to determine what attack to do
        DetermineAttack();
        infoBox.SetActive(true);
        infoText.SetText(enemyName +  " Used\n" + curAttack.GetName());

        if (curAttack.IsMelee())
        {
            yield return StartCoroutine(MoveToMelee());
            yield return StartCoroutine(PlayAttackAnimation());
            yield return StartCoroutine(MoveBackToStartingSpot());
        }
        else
        {
            yield return StartCoroutine(PlayAttackAnimation());
        }
        infoBox.SetActive(false);
    }

    private IEnumerator MoveToMelee()
    {
        animator.SetTrigger("StartRunning");
        BoxCollider2D myCollider = GetComponent<BoxCollider2D>();
        Rigidbody2D myRb = GetComponent<Rigidbody2D>();

        while (myCollider.IsTouchingLayers(LayerMask.GetMask("Player")) == false)
        {
            //Move Here
            myRb.velocity = new Vector2(x: -moveVelocity, 0);
            yield return new WaitForEndOfFrame();
        }

        myRb.velocity = Vector2.zero;
    }

    private IEnumerator PlayAttackAnimation()
    {
        spriteRenderer.sortingOrder += 25;

        animator.Play(curAttack.GetClip().name);

        yield return new WaitForSeconds(curAttack.GetClip().length);

        while (spellVFXContainer.childCount > 0)
        {
            yield return new WaitForEndOfFrame();
        }

        spriteRenderer.sortingOrder -= 25;
    }

    private IEnumerator MoveBackToStartingSpot()
    {
        Vector2 flipX = new Vector2(-1, 1);
        transform.localScale = flipX;
        noFlip.localScale = flipX;

        Rigidbody2D myRb = GetComponent<Rigidbody2D>();
        myRb.velocity = new Vector2(x: moveVelocity, 0);
        animator.SetTrigger("StartRunning");
        while(transform.position.x != startingPosition.x)
        {
            yield return new WaitForEndOfFrame();
        }
        myRb.velocity = Vector2.zero;
        animator.SetTrigger("StopRunning");

        transform.localScale = Vector2.one;
        noFlip.localScale = Vector2.one;
    }

    public IEnumerator Die()
    { 
        //Waits for death animation to finish
        while (isAnimating())
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private bool isAnimating()
    {
         return animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
    }

    public bool IsDead()
    {
        return GetComponent<Health>().IsDead();
    }

    public void AnimationComplete()
    {
        //called from animation
        //currentlyAnimating = false;
    }

    public void DealDamage()
    {
        //called from animation events
        Health playerHealth = FindObjectOfType<PlayerAttackController>().GetComponent<Health>();
        Buffs enemybuffs = playerHealth.GetComponent<Buffs>();
        playerHealth.TakeDamage((int)(curAttack.GetDamage() * gameController.GetEnemyScaling() * 
            enemybuffs.GetShockAmount() * enemybuffs.GetDamageReduction()));

        enemybuffs.SetDebuffs(curAttack);
        mybuffs.SetBuffs(curAttack);
    }

    public void HealDamage()
    {
        //Called from animation events
        GetComponent<Health>().AddHealth(curAttack.GetHealing(), false);
    }

    public void GainShield()
    {
        //Called from animation events
        GetComponent<Health>().AddShield(curAttack.GetHealing());
    }

    public void TriggerSpellVFX()
    {
        //Called from animation events to spawn the spell vfx
        VFX spellVFX = curAttack.GetSpellVFX();

        if (spellVFX == null)
        {
            Debug.Log("No VFX Set for " + curAttack.GetName() + ". Did you mean to trigger one on this spell?");
            return;
        }

        GameObject spell = SpawnSpell(spellVFX);

        spell.GetComponent<VFX>().SetPlayer(false);

        if (spellVFX.IsProjectile() == false)
        {
            Destroy(spell, spellVFX.GetDuration());
        }
        else
        {
            spell.GetComponent<VFX>().SetUpProjectile(FindObjectOfType<PlayerAttackController>().GetComponent<TargetPoints>());
        }
    }

    GameObject SpawnSpell(VFX spellVFX)
    {
        if (spellVFX.IsStationary())
        {
            return Instantiate(
                spellVFX.gameObject,
                FindObjectOfType<EnemyAttackController>().GetComponent<TargetPoints>().GetPosition(TargetSpots.ABOVE).position,
                Quaternion.identity,
                spellVFXContainer);
        }
        return Instantiate(spellVFX.gameObject, spellVFXPosition.position, Quaternion.identity, spellVFXContainer);
    }

    public void TriggerSFX()
    {
        //Called from animation events
        if (curAttack.GetSFX() == null)
        {
            Debug.LogError(curAttack.name + " tried to generate a sound effect but 1 wasn't found. Either remove the animation event or " +
                "add an sfx to the PlayerAttack object");
            return;
        }

        AudioPlayer.PlayClipAtPoint(curAttack.GetSFX(), Camera.main.transform.position);
        
       // AudioSource.PlayClipAtPoint(curAttack.GetSFX(), Camera.main.transform.position);
    }

    public void TriggerIdleSFX()
    {
        //Called from animation events
        if (idleSFX == null)
        {
            Debug.LogError("Idle SFX not set on: " + name);
            return;
        }

        AudioPlayer.PlayClipAtPoint(idleSFX, Camera.main.transform.position);
    }

    public void TriggerWalkSFX()
    {
        //Called from animation events
        if (walkSFX == null)
        {
            Debug.LogError("Walk SFX not set on: " + name);
            return;
        }

        AudioPlayer.PlayClipAtPoint(walkSFX, Camera.main.transform.position);
    }
}
