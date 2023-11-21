using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDamageable : MonoBehaviour
{
    private int deathCounter = 0;
    Animator animator;

    [SerializeField] private int _maxHP = 100;
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
    }
    private void Update()
    {
        if (!IsAlive)
        {
            while (deathCounter == 0)
            {
                deathCounter++;
                GetComponentInParent<Transform>().gameObject.layer = 13;
                animator.SetTrigger(AnimationStrings.Death);
            }
        }
    }

    public void Hit(int damage)
    {
        if (IsInvincible) { return; }
        if (IsAlive && !IsInvincible)
        {
            CurrentHP -= damage;
            Debug.LogError("OUCH");
            animator.SetTrigger(AnimationStrings.Hurt);
        }  
    }
}
