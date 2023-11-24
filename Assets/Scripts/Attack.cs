using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;
    public Vector2 knockback;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*if (collision.GetComponent<IDamageable>())
        {

        }*/
            Debug.LogWarning("HIT: " + collision.name);
            collision.GetComponent<IDamageable>().Hit(damage, knockback);
    }

}
