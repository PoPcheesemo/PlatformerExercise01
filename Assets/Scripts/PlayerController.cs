using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float JumpMax = 4f;
    public float JumpPower;
    public float JumpTime = 0f;
    public float CurrentMoveSpeed {get
        {
            if (IsMoving)
            {
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
/*    [SerializeField]
    private bool _isJumping = false;
    public bool IsJumping
    {
        get
        {
            return _isJumping;
        }
        private set
        {
            _isJumping = value;
            animator.SetBool(AnimationStrings.IsJumping, value);
        }
    }*/

    public bool _isFacingRight = true;
    public bool IsFacingRight { get { return _isFacingRight; } private set {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
      //  JumpPower = JumpTime * JumpMax * Time.deltaTime;
        rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);

    }

    public void OnAttack()
    {

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
        /*if ((float)context.duration < 2.0f && (float)context.duration > 0f)
        {
            JumpTime = (float)context.duration;
        }   
        else if ((float)context.duration > 2.0f)
        {
            JumpTime = 2.0f;
        }*/

        if (context.started)
        {
            JumpTime = 1.5f;
            JumpPower = JumpMax * JumpTime;
        }

        rb.velocity = new Vector2 (rb.velocityX, JumpPower);
    }
}
