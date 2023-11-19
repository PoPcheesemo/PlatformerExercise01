using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Knight : MonoBehaviour
{
    public ContactFilter2D castFilterGround;

    public int countdownTime;

    private bool _isAggro;
    public bool IsAggro { get { return _isAggro; }
        set
        {
            _isAggro = value;
        }
    }

    public enum EnemyDirection { Left, Right}
    private EnemyDirection _walkDirection;
    public EnemyDirection WalkDirection
    {
        get { return _walkDirection; }
        set { 
            if (_walkDirection != value)
            {
                Flip();
                StartCoroutine(KnightFlip());
            }
            
            _walkDirection = value; }
    }

    IEnumerator KnightFlip()
    {
        moveSpeed = 0f;
        yield return new WaitForSeconds(1f);
        if (!IsFacingRight)
        {
            moveSpeed = -walkSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }
    }
    public IEnumerator KnightAggro()
    {
        countdownTime = 5;
        Debug.Log("AGGRO!");
        if (!IsFacingRight)
        {
            moveSpeed = -runSpeed;
        }
        else
        {
        moveSpeed = runSpeed;
        }
         for (int i = countdownTime * 20; i > 0; i--)
        {
            yield return new WaitForSeconds(1f / 20f);
            Debug.Log("COUNTDOWN: " + ((float)i / 20));
            IsAggro = true;
        }

        if (!IsFacingRight)
        {
            moveSpeed = -walkSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }
        Debug.Log("AGGRO OVER!!!");
        IsAggro = false;
    }

    public float moveSpeed;
    public float walkSpeed = 1.75f;
    public float runSpeed = 5f;
    public float rayLength = 3.4f;

    CapsuleCollider2D capsuleCollider;
    CircleCollider2D circleCollider;

    public DetectionZone attackZone;

    Rigidbody2D rb;
    Animator animator;

    RaycastHit2D[] groundHits = new RaycastHit2D[1];
    RaycastHit2D[] wallHits = new RaycastHit2D[4];


    public bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (transform.localScale.x > 0)
            {
                _isFacingRight = true;
            }
            else if (transform.localScale.x < 0)
            {
                _isFacingRight = false;
            }
        }

    }
    [SerializeField] private bool _isGrounded = true;
    public bool IsGrounded
    {
        get
        {
            return _isGrounded;
        }
        private set
        {
            _isGrounded = value;
            animator.SetBool(AnimationStrings.IsGrounded, value);

        }
    }
    [SerializeField] private bool _isOnWall;
    public bool _hasTarget = false;
    public bool HasTarget { get {  return _hasTarget; }
    private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.HasTarget, value);
            
        }
    }
    public bool IsOnWall
    {
        get
        {
            return _isOnWall;
        }
        private set
        {
            _isOnWall = value;
            animator.SetBool(AnimationStrings.IsOnWall, value);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = rb.GetComponent<CapsuleCollider2D>();
        circleCollider = rb.GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;
        animator.SetFloat(AnimationStrings.rbVelocity, Mathf.Abs(rb.velocityX));
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveSpeed * Vector2.right.x, rb.velocity.y);

        IsGrounded = circleCollider.Cast(new Vector2(0f, -1.0f), castFilterGround, groundHits, 0.3f) > 0;

        IsOnWall = circleCollider.Cast(new Vector2(1f, 0.0f), castFilterGround, groundHits, 0.3f) > 0 ||
            circleCollider.Cast(new Vector2(-1f, 0.0f), castFilterGround, groundHits, 0.3f) > 0;
        if (IsAggro == true)
        {
            if (!IsFacingRight)
            {
                moveSpeed = -runSpeed;
            }
            else
            {
                moveSpeed = runSpeed;
            }
        }
        if (HasTarget)
        {
            animator.SetTrigger(AnimationStrings.Attack);
        }
        if (_isOnWall)
        {
            Flip();
            if (!IsAggro)
            {
            StartCoroutine(KnightFlip());
            }
        }
        SetFacingDirection();
    }
    private void Flip()
    {
            transform.localScale *= new Vector2(-1, 1);
        //    moveSpeed *= -1;
    }
    private void SetFacingDirection()
    {
        if (transform.localScale.x > 0)
        {
            IsFacingRight = true;
        }
        else if (transform.localScale.x < 0)
        {
            IsFacingRight = false;
        }
    }
}
