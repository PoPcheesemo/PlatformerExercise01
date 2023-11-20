using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Knight : MonoBehaviour
{
    public ContactFilter2D castFilterGround;

    public int countdownTime;
    public float moveSlowRate;
    public float moveFastRate;
    public float moveSpeed;
    public float walkSpeed = 1.75f;
    public float runSpeed = 5f;
    public float rayLength = 3.4f;
    public float attackRate = 10f;

    [SerializeField] private bool _isAggro;
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
            moveSpeed = Mathf.Lerp(0f, -walkSpeed, moveFastRate);
        }
        else
        {
            moveSpeed = Mathf.Lerp(0f, walkSpeed, moveFastRate);
        }
    }
    public IEnumerator KnightAggro()
    {
        Debug.Log("AGGRO!");
        if (!IsFacingRight)
        {
            moveSpeed = Mathf.Lerp(0f, -runSpeed, moveFastRate);
        }
        else
        {
            moveSpeed = Mathf.Lerp(0f, runSpeed, moveFastRate);
        }
         for (int i = countdownTime * 5; i > 0; i--)
        {
            yield return new WaitForSeconds(1f / 5);
            Debug.Log("COUNTDOWN: " + ((float)i / 5));
            IsAggro = true;
        }

        if (!IsFacingRight)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, -walkSpeed, moveSlowRate);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, moveSlowRate);
        }
        Debug.Log("AGGRO OVER!!!");
        IsAggro = false;
    }


    CapsuleCollider2D capsuleCollider;
    CircleCollider2D circleCollider;

    public DetectionZone attackZone;

    Rigidbody2D rb;
    public Animator animator;

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
    public bool CanMove { get { return animator.GetBool(AnimationStrings.canMove); } }
    public bool IsAttacking { get { return animator.GetBool(AnimationStrings.IsAttacking); } }
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
        rb.velocity = new Vector2(moveSpeed * Vector2.right.x, rb.velocity.y);
    }
    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;
        animator.SetFloat(AnimationStrings.rbVelocity, Mathf.Abs(rb.velocityX));
        IsGrounded = circleCollider.Cast(new Vector2(0f, -1.0f), castFilterGround, groundHits, 0.1f) > 0;

        IsOnWall = circleCollider.Cast(new Vector2(1f, 0.0f), castFilterGround, groundHits, 0.3f) > 0 ||
            circleCollider.Cast(new Vector2(-1f, 0.0f), castFilterGround, groundHits, 0.3f) > 0;

        
        
    }
    private void FixedUpdate()
    {
        if (CanMove && !IsAttacking)
        {
             StartCoroutine(WalkCooldown());
        }
        else
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
        if (HasTarget && !IsAttacking)
        {
            StartCoroutine(AttackCooldown(attackRate));
        }
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
        } else
        {
            if (!IsFacingRight)
            {
                moveSpeed = Mathf.Lerp(0f, -walkSpeed, moveFastRate);
            }
            else
            {
                moveSpeed = Mathf.Lerp(0f, walkSpeed, moveFastRate);
            }
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
    IEnumerator WalkCooldown()
    {
        yield return new WaitForSeconds(0.1f);

        rb.velocity = new Vector2(moveSpeed * Vector2.right.x, rb.velocity.y);
    }
    IEnumerator AttackCooldown(float attackRate)
    {
        Debug.LogWarning("! ! ! ATTACK STARTED ! ! !");
        animator.SetBool(AnimationStrings.IsAttacking, true);
        animator.SetBool(AnimationStrings.canMove, false);
        yield return new WaitForSeconds(0.2f);

      attackRate = 20 / attackRate;
        animator.SetTrigger(AnimationStrings.Attack);
        yield return new WaitForSeconds(0.1f + attackRate);
        Debug.LogWarning("! ! ! ATTACK ENDED ! ! !");
        animator.SetBool(AnimationStrings.IsAttacking, false);
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
