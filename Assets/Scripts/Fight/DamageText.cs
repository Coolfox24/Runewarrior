using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float liveTime = 3f;

    private void Update()
    {
        //float upwards a little bit
        transform.position = new Vector2(transform.position.x, transform.position.y + moveSpeed * Time.deltaTime);
        liveTime -= Time.deltaTime;
        if(liveTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamageText(string damageAmount)
    {
        damageText.text = damageAmount;
    }
}
