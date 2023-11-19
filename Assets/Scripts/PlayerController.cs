using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]

public class PlayerController : MonoBehaviour
{
    private float walkSpeed;
    private float runSpeed;
    public float walkSpeedSet;
    public float runSpeedSet;
    public float JumpMax = 10.0f;
    public float JumpPower;
    public float JumpTime = 0f;
    public float JumpTimeStart;

    public bool CanMove {  get
        {
            return animator.GetBool(AnimationStrings.canMove);
        } set
        {
            CanMove = animator.GetBool(AnimationStrings.canMove);
        }
    }

    Rigidbody2D rb;
    Animator animator;
    TouchingDirections touchingDirections;
    SpriteRenderer spriteRenderer;
    PlayerInput playerInput;

    public Sprite deathSprite;
    public float CurrentMoveSpeed {get
        {
            if (CanMove)
            {
                 if (IsMoving)
                 {
                
                    if (!touchingDirections.IsGrounded)
                    {
                        runSpeed = 0.66f * runSpeedSet;
                        walkSpeed = 0.66f * walkSpeedSet;
                    }
                    else
                    {
                    runSpeed = runSpeedSet;
                    walkSpeed = walkSpeedSet;
                }
                if(IsRunning)
                {
                    return runSpeed;
                } else 
                { 
                    return walkSpeed; 
                } 
            } else
                {
                    return 0;
                }
            }
            else { return 0; }
           
        }
    }

    Vector2 moveInput;
    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving { get
        {
            return _isMoving;
        } private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.IsMoving, value);
        }
    }
    [SerializeField]
    private bool _isRunning = false;
    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }
        private set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.IsRunning, value);
        }
    }

    public bool _isFacingRight = true;

    public bool IsFacingRight { get { return _isFacingRight; } private set {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);

    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.Attack);
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        IsMoving = moveInput != Vector2.zero;

        SetFacingDirection(moveInput);
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        } else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        } 
        if (context.canceled)
        {
            IsRunning = false;

        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded && CanMove)
        {
            JumpTimeStart = Time.time;
        }

        if (context.canceled && touchingDirections.IsGrounded)
        {
            animator.SetTrigger(AnimationStrings.Jump);
            JumpTime = Time.time - JumpTimeStart;
            if (JumpTime > 0.33f) { JumpTime = 0.33f; }

            JumpPower = JumpMax * JumpTime * 3;
            rb.velocity = new Vector2(rb.velocityX, JumpPower);
        }
    }
    public void OnDeath()
    {
        animator.SetTrigger(AnimationStrings.Death);
        StartCoroutine(DeathCooldown());
        
    }
    IEnumerator DeathCooldown()
    {
        playerInput.DeactivateInput();
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.sprite = deathSprite;
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimationStrings.Death));
        animator.enabled = false;
        spriteRenderer.sprite = deathSprite;
        Debug.Log("DEATH!");
    }
}