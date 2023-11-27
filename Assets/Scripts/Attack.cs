using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;
    public Vector2 knockback;
    public float faceRight;

    void Update()
    {
         
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        faceRight = GetComponentInParent<IDamageable>().faceRight;
        collision.GetComponent<IDamageable>().Hit(damage, knockback, faceRight);
    }


}
