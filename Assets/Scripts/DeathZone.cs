using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    PlayerController player;
    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        player = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //  if (collision.tag == "Player")
        //  {
        /*player = collision.transform.GetComponent<PlayerController>();

        if (player != null)
        {
            player.GetComponent<PolygonCollider2D>().enabled = false;
            player.OnDeath();
        }*/
        Debug.Log("COLLIDER: " + collision.name);
            collision.GetComponent<IDamageable>().IsAlive = false;

      //  }
    }
}