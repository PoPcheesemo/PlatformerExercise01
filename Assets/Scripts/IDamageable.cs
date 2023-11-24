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

    public UnityEvent<int, Vector2> damageableHit;

    [SerializeField] private float hurtTime = 0f;
    [SerializeField] private float iTime = 0.2f;

    [SerializeField] private int _maxHP = 100;

    public Vector2 knockback;
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

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
     //   knockback = GetComponentInParent<Knight>().GetComponent<Attack>().knockback;
    }
    private void Update()
    {
        if (IsInvincible)
        {
            if (hurtTime > iTime)
            {
                //Remove invincibility
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
            while (deathCounter == 0)
            {
                deathCounter++;
                GetComponentInParent<Transform>().gameObject.layer = 13;
                animator.SetTrigger(AnimationStrings.Death);
                
            }
            if (GetComponentInParent<PlayerController>() == true )
            {
                    Debug.LogWarning("PLAYER IS DEAD");
                    GetComponentInParent<PlayerController>().playerInput.enabled = false;
            }
            if(GetComponentInParent<Knight>() == true )
                {
                    Debug.LogWarning("KNIGHT IS DEAD");
                    GetComponentInParent<Knight>().moveSpeed = 0;
                }
        }
    }
    public void Hit(int damage, Vector2 knockback)
    {
        if (IsInvincible) { return; }
        if (IsAlive && !IsInvincible)
        {
            CurrentHP -= damage;
            if(GetComponentInParent<PlayerInput>() != null )
            {
            playerInput.enabled = false;

            }
            Debug.LogError("Knockback :" + knockback);
            damageableHit.Invoke(damage, knockback);

            Debug.LogError("OUCH");
            animator.SetTrigger(AnimationStrings.Hurt);
            IsInvincible = true;
        }  
    }
}
