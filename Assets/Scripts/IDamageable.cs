using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class IDamageable : MonoBehaviour
{
    private int deathCounter = 0;

    Animator animator;
    PlayerInput playerInput;

    public UnityEvent<int, Vector2, float> damageableHit;

    [SerializeField] private float hurtTime = 0f;
    [SerializeField] private float iTime = 0.2f;

    [SerializeField] private int _maxHP = 100;

    public Vector2 knockback;
    public float faceRight;
    public int MaxHP { get 
        { 
            return _maxHP; 
        }
        set
        {
            _maxHP = value;
        }
    }
    [SerializeField] private int _currentHP = 100;
    public int CurrentHP { get
        {
            return _currentHP;
        }
        set
        {
            _currentHP = value;

            if (_currentHP <= 0) {
                IsAlive = false;
                
            }
        }
    }
    [SerializeField] private int _damage = 10;
    public int Damage { get 
        {
            return _damage;
        }
        set 
        {
            _damage = value;
        }
    }
    [SerializeField] private bool _isAlive = true;
    public bool IsAlive { get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.IsAlive, value);
        }
    }
    [SerializeField] private bool _isInvincible;
    public bool IsInvincible { get 
        {
        return _isInvincible;
        } 
        set
        {
            _isInvincible = value;
        }
    }
    [SerializeField] private bool _isBlocking;
    public float blockDamMod;
    public Vector2 blockKnockMod;

    public bool IsBlocking { get
        {
            return _isBlocking;
        } set
        {
            _isBlocking = value;
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }
    private void Update()
    {
        faceRight = transform.localScale.x;
        if (IsInvincible)
        {
            if (hurtTime > iTime)
            {
                IsInvincible = false;
                if (GetComponentInParent<PlayerInput>() != null)
                {
                    playerInput.enabled = true;
                }
                hurtTime = 0;
            }
            hurtTime += Time.deltaTime;
        }
        
        if (!IsAlive)
        {
            
            if (GetComponentInParent<PlayerController>() == true )
            {
                GetComponentInParent<PlayerController>().playerInput.enabled = false;
            }
            if(GetComponentInParent<Knight>() == true )
            {
                GetComponentInParent<Knight>().moveSpeed = 0;
                GetComponentInParent<Knight>().walkSpeed = 0;
                GetComponentInParent<Knight>().runSpeed = 0;
            }
            while (deathCounter == 0)
            {
                deathCounter++;
                GetComponentInParent<Transform>().gameObject.layer = 13;
                StartCoroutine(DeathDelay());
                
            }
        }
    }
    public void Hit(int damage, Vector2 knockback, float faceRight)
    {
        if (IsInvincible) { return; }
        if (IsAlive && !IsInvincible)
        {
            if (IsBlocking)
            {
                damage = (int) ((damage * (100 - blockDamMod)) / 100);
                knockback *= (new Vector2(1, 1) - blockKnockMod);
            }
            this.knockback = knockback;
            this.faceRight = faceRight;
            CurrentHP -= damage;
            if (CurrentHP < 0)
            {
                IsAlive = false; return;
            }
            if(GetComponentInParent<PlayerInput>() != null )
            {
            playerInput.enabled = false;
            }
            damageableHit.Invoke(damage, knockback, faceRight);
            animator.SetTrigger(AnimationStrings.Hurt);
            IsInvincible = true;
        }  
    }
    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetTrigger(AnimationStrings.Death);
    }
}
