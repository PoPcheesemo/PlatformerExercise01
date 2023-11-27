using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(IDamageable))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    public float walkSpeedSet;
    public float runSpeedSet;
    public float baseGrav;
    public float fallGrav;
    public float JumpMax = 10.0f;
    public float JumpPower;
    public float JumpTime = 0f;
    public float JumpTimeStart;
    public float jumpApexMin;
    public float jumpApexMax;
    public float jumpApexGrav;
    public float jumpLerpTime;
    public float jumpLerpTimeMax;
    public float jumpCoyoteTime;
    public float jumpCoyoteMax;
    public float fallTermVel;
    private static int deathCounter = 0;

    Rigidbody2D rb;
    Animator animator;
    TouchingDirections touchingDirections;
    SpriteRenderer spriteRenderer;
    public PlayerInput playerInput;
    IDamageable damageable;

    public Sprite deathSprite;
    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving)
                {
                    runSpeed = runSpeedSet;
                    walkSpeed = walkSpeedSet;
                    if (!touchingDirections.IsGrounded)
                    {
                        {
                            runSpeed = 0.84f * runSpeedSet;
                            walkSpeed = 0.84f * walkSpeedSet;
                        }
                    }
                    
                    if (IsRunning)
                    {
                        return runSpeed;
                    }
                    else
                    {
                        return walkSpeed;
                    }
                }
                else
                {
                    return 0;
                }
            }

            else { return 0; }

        }
    }


    Vector2 moveInput;

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
        set
        {
            CanMove = animator.GetBool(AnimationStrings.canMove);
        }
    }
    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
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

    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
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
        damageable = GetComponent<IDamageable>();
    }
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (!damageable.IsAlive)
        {
            while (deathCounter < 1)
            {
                deathCounter++;
                OnDeath();
                Debug.Log("DEATH FOR PLAYER!!!");
                playerInput.enabled = false;
            }
        }
        if (!touchingDirections.IsGrounded)
        {
            jumpCoyoteTime += Time.fixedDeltaTime;
            jumpLerpTime += Time.fixedDeltaTime;

            if (rb.velocity.y > jumpApexMax)
            {
                rb.gravityScale = baseGrav;
                rb.drag = 10f;
            }
            else if (rb.velocity.y < jumpApexMax && rb.velocity.y >= 0)
            {
                rb.gravityScale = Mathf.Lerp(baseGrav, jumpApexGrav, jumpLerpTime / jumpLerpTimeMax);
            }
            else if (rb.velocity.y < 0 && rb.velocity.y > jumpApexMin)
            {
                rb.gravityScale = Mathf.Lerp(jumpApexGrav, fallGrav, jumpLerpTime / jumpLerpTimeMax);
            }
            else if (rb.velocity.y < jumpApexMin)
            {
                rb.gravityScale = fallGrav;
                rb.drag = 2f;
            }
            if (rb.velocity.y < fallTermVel)
            {
                rb.velocity = new Vector2(rb.velocity.x, fallTermVel);
            }

        }
        else
        {
            jumpCoyoteTime = 0;
            jumpLerpTime = 0;
        }

        if (!damageable.IsBlocking)
        {
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);

        }
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
        }
        else if (moveInput.x < 0 && IsFacingRight)
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
        if (context.started && jumpCoyoteTime < jumpCoyoteMax && CanMove)
        {
            JumpTimeStart = Time.time;
        }

        if (context.canceled && jumpCoyoteTime < jumpCoyoteMax)
        {
            animator.SetTrigger(AnimationStrings.Jump);
            JumpTime = Time.time - JumpTimeStart;
            if (JumpTime > 0.167f) JumpTime = 0.167f;

            JumpPower = JumpMax * JumpTime * 6;
            rb.AddForce(new Vector2(rb.velocity.x * JumpPower / 10, JumpPower));
        }
    }
    public void OnDeath()
    {
        StartCoroutine(DeathCooldown());
    }
    IEnumerator DeathCooldown()
    {
        playerInput.DeactivateInput();
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.sprite = deathSprite;
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsTag(AnimationStrings.Death));
        spriteRenderer.sprite = deathSprite;
        Debug.Log("DEATH!");
    }
    public void OnHit(int damage, Vector2 knockback, float faceRight)
    {
        rb.AddForce(knockback);
    }
    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            damageable.IsBlocking = true;
            animator.SetBool(AnimationStrings.IsBlocking, true);
            runSpeed = 0f;
            walkSpeed = 0f;

        }
        if (context.canceled)
        {
            damageable.IsBlocking = false;
            animator.SetBool(AnimationStrings.IsBlocking, false);
            runSpeed = runSpeedSet;
            walkSpeed = walkSpeedSet;
        }
    }
}