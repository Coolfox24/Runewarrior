using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    [SerializeField] AnimationClip myAnimation;
    [SerializeField] AudioClip soundEffect;
    [SerializeField] bool isProjectile = false;
    [SerializeField] TargetSpots aimPosition = TargetSpots.BODY;
    [SerializeField] bool isStationary = false;
    [SerializeField] float speed = 1f;
    [SerializeField] bool isArc = false;

    bool isPlayer = false;
    Vector3 targetPosition;

    public float GetDuration()
    {
        return myAnimation.length;
    }

    public bool IsProjectile()
    {
        return isProjectile;
    }

    public bool IsStationary()
    {
        return isStationary;
    }

    public void PlaySFX()
    {
        //called from animation events
        AudioPlayer.PlayClipAtPoint(soundEffect, Camera.main.transform.position);
    }

    public void SetPlayer(bool isPlayer)
    {
        this.isPlayer = isPlayer;
    }

    public void SetUpProjectile(TargetPoints points)
    {
        targetPosition = points.GetPosition(aimPosition).position;
    }

    public void StartMoving()
    {
        StartCoroutine(Movement());
    }

    private IEnumerator Movement()
    {
        float currentFrame = 0f;
        Vector2 startingPosition = transform.position;
        if (!isArc)
        {
            while (transform.position != targetPosition)
            {
                transform.position = Vector2.Lerp(startingPosition, targetPosition, currentFrame);
                currentFrame += speed * Time.deltaTime;
                Debug.Log(currentFrame);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (transform.position != targetPosition)
            {
                
                float xPos = Mathf.Lerp(startingPosition.x, targetPosition.x, currentFrame);
                float yPos = GetYPos(startingPosition.y, targetPosition.y, 4, currentFrame);
                transform.position = new Vector2(xPos, yPos);
                currentFrame += speed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        //yield return new WaitForSeconds(5);
        GetComponent<Animator>().SetTrigger("Hit");
    }

    private float GetYPos(float startingY, float targetY, float arcHeight, float currentFrame)
    {
        float returnY = 0;
        if (currentFrame <= 0.5f)
        {
            float squaredCurrrentFrame = (currentFrame * 2) * (currentFrame * 2);
            returnY = Mathf.Lerp(startingY, targetY + arcHeight, squaredCurrrentFrame);
        }
        else
        {
            float squaredCurrrentFrame = ((currentFrame - 0.5f) * 2) * ((currentFrame - 0.5f) * 2);
            returnY = Mathf.Lerp(targetY + arcHeight, targetY, squaredCurrrentFrame);
        }
        return returnY;
    }

    //Folling methods called from animation events
    public void ProjDealDamage()
    {
        if(isPlayer)
        {
            FindObjectOfType<PlayerAttackController>().DealDamage();
        }
        else
        {
            FindObjectOfType<EnemyAttackController>().DealDamage();
        }
    }

    public void Delete()
    {
        Destroy(gameObject);
    }

    public void DealDamage()
    {
        if (isPlayer)
        {
            FindObjectOfType<PlayerAttackController>().DealDamage();
        }
        else
        {
            FindObjectOfType<EnemyAttackController>().DealDamage();
        }
    }

    public void GainShield()
    {
        if (isPlayer)
        {
            FindObjectOfType<PlayerAttackController>().GainShield();
        }
        else
        {
            FindObjectOfType<EnemyAttackController>().GainShield();
        }
    }

    public void HealDamage()
    {
        if(isPlayer)
        {
            FindObjectOfType<PlayerAttackController>().HealDamage();
        }
        else
        {
            FindObjectOfType<EnemyAttackController>().HealDamage();
        }
    }
}
