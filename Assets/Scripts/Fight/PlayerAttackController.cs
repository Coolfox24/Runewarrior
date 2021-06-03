using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] [Tooltip("Higher up increases priority")] PlayerAttack[] listPlayerAttacks;
    [SerializeField] float moveVelocity = 6;
    [SerializeField] Transform spellVFXPosition;
    [SerializeField] AudioClip walkSFX = null;
    [SerializeField] AudioClip idleSFX = null;
    [SerializeField] Transform noFlip;
    
    Transform spellVFXContainer;
    private List<PlayerAttack> curAttack;

    Buffs mybuffs;
    Animator animator;

    Vector2 startingPosition;


    private void Start()
    {
        animator = GetComponent<Animator>();
        startingPosition = transform.position;
        curAttack = new List<PlayerAttack>();
        mybuffs = GetComponent<Buffs>();

        spellVFXContainer = GameObject.FindGameObjectWithTag("vfxContainer").transform;
    }

    public string DetermineAttack(Dictionary<RuneTags, int> activeRunes)
    {
        List<PlayerAttack> playerAttacks = new List<PlayerAttack>();
        foreach(PlayerAttack playerAttack in listPlayerAttacks)
        {
            if(playerAttack.CanCast(activeRunes) == true)
            {
                playerAttacks.Add(playerAttack);
                // return playerAttacks[0].name;
                playerAttack.ReduceRunes(ref activeRunes);
            }
        }

        curAttack = playerAttacks;

        //TODO implement multiple player attacks a turn if needed
        if (curAttack.Count > 0)
        {
            string allAttacks = "";
            foreach(PlayerAttack playerAttack in curAttack)
            {
                allAttacks += playerAttack.name + " + ";
            }

            return "(" + curAttack[0].GetTotalRunes() + ") " +  curAttack[0].GetName();
            //return allAttacks;
        }
        //If no runes selected, player can pass to draw more
        curAttack = null;
        return "Pass Turn";
    }

    public IEnumerator DoAttack()
    {
        if (curAttack == null)
        {
            yield return null;
        }
        else
        {
            if (curAttack[0].IsMelee())
            {
                yield return StartCoroutine(MoveToMelee());
                yield return StartCoroutine(PlayAttackAnimation());
                yield return StartCoroutine(MoveBackToStartingSpot());
            }
            else
            {
                yield return StartCoroutine(PlayAttackAnimation());
            }
        }
    }

    private IEnumerator MoveToMelee()
    {
        animator.SetTrigger("StartRunning");
        BoxCollider2D myCollider = GetComponent<BoxCollider2D>();
        Rigidbody2D myRb = GetComponent<Rigidbody2D>();

        while (myCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")) == false)
        {
            //Move Here
            myRb.velocity = new Vector2(x: moveVelocity, 0);
            yield return new WaitForEndOfFrame();
        }

        myRb.velocity = Vector2.zero;
    }

    private IEnumerator PlayAttackAnimation()
    {
        animator.Play(curAttack[0].GetClip().name);

        yield return new WaitForSeconds(curAttack[0].GetClip().length);

        while (spellVFXContainer.childCount > 0)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator MoveBackToStartingSpot()
    {
        Vector2 flipX = new Vector2(-1, 1);
        transform.localScale = flipX;
        noFlip.localScale = flipX;

        Rigidbody2D myRb = GetComponent<Rigidbody2D>();
        myRb.velocity = new Vector2(x: -moveVelocity, 0);
        animator.SetTrigger("StartRunning");
        while (transform.position.x != startingPosition.x)
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

    public void AnimationFinished()
    {
        //Called from animation events to signify it has finished its animation
    }

    public void DealDamage()
    {
        //Called from animation events
        Health eacHealth = FindObjectOfType<EnemyAttackController>().GetComponent<Health>();
        Buffs enemyBuffs = eacHealth.GetComponent<Buffs>();
        eacHealth.TakeDamage((int)(curAttack[0].GetDamage() * enemyBuffs.GetShockAmount() * enemyBuffs.GetDamageReduction()));
        enemyBuffs.SetDebuffs(curAttack[0]);
        mybuffs.SetBuffs(curAttack[0]);
    }

    public void HealDamage()
    {
        //Called from animation events
        GetComponent<Health>().AddHealth(curAttack[0].GetHealing(), false);
        mybuffs.SetBuffs(curAttack[0]);
    }

    public void GainShield()
    {
        //Called from animation events
        GetComponent<Health>().AddShield(curAttack[0].GetHealing());
        mybuffs.SetBuffs(curAttack[0]);
    }

    public void TriggerSpellVFX()
    { 
        //Called from animation events to spawn the spell vfx
        VFX spellVFX = curAttack[0].GetSpellVFX();

        if(spellVFX == null)
        {
            Debug.Log("No VFX Set for " + curAttack[0].GetName() + ". Did you mean to trigger one on this spell?");
            return;
        }

        GameObject spell = SpawnSpell(spellVFX);

        spell.GetComponent<VFX>().SetPlayer(true);

        if (spellVFX.IsProjectile() == false)
        {
            Destroy(spell, spellVFX.GetDuration());
        }
        else
        {                                                                               
            spell.GetComponent<VFX>().SetUpProjectile(FindObjectOfType<EnemyAttackController>().GetComponent<TargetPoints>());
        }
    }

    GameObject SpawnSpell(VFX spellVFX)
    {
        if(spellVFX.IsStationary())
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
        if(curAttack[0].GetSFX() == null)
        {
            Debug.LogError(curAttack[0].name + " tried to generate a sound effect but 1 wasn't found. Either remove the animation event or " +
                "add an sfx to the PlayerAttack object");
            return;
        }

        AudioPlayer.PlayClipAtPoint(curAttack[0].GetSFX(), Camera.main.transform.position);
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
