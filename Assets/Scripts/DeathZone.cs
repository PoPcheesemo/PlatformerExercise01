using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public Vector2 knockback;
    public float faceRight;
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
        collision.GetComponent<IDamageable>().Hit(20, knockback, faceRight);
    }
}