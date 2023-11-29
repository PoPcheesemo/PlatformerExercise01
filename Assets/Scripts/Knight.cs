using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class Knight : MonoBehaviour
{
    public ContactFilter2D castFilterGround;

    private int deathCounter = 0;
    public int countdownTime;
    public float moveSlowRate;
    public float moveFastRate;
    public float moveSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float rayLength;
    public float attackRate;

    CapsuleCollider2D capsuleCollider;
    CircleCollider2D circleCollider;
    IDamageable damageable;
    Rigidbody2D rb;
    SpriteRenderer spriteRend;

    public Sprite deathSprite;
    public DetectionZone attackZone;
    public Animator animator;

    [SerializeField] private bool _isAggro;
    public bool IsAggro { get { return _isAggro; }
        set
        {
            _isAggro = value;
        }
    }
    public enum EnemyDirection {Left, Right}
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
        IsAggro = false;
    }

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
    public bool _hasTarget = false;

    public bool HasTarget { get {  return _hasTarget; }
    private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.HasTarget, value);
        }
    }
    private bool _canMove;
    public bool CanMove { get { return animator.GetBool(AnimationStrings.canMove); }
        set 
        {
            _canMove = value;
        }
    }
    public bool IsAttacking { get { return animator.GetBool(AnimationStrings.IsAttacking); } }
    [SerializeField] private bool _isEdge;
    public bool IsEdge { get { return _isEdge; }
        private set 
        {
            _isEdge = value;
        }
    }
    [SerializeField] private bool _isOnWall;

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
    [SerializeField] private bool _isHurt;

    public bool IsHurt
    {
        get
        {
            return _isHurt;
        }
        private set
        {
            _isHurt = value;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = rb.GetComponent<CapsuleCollider2D>();
        circleCollider = rb.GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        rb.velocity = new Vector2(moveSpeed * Vector2.right.x, rb.velocity.y);
        damageable = GetComponent<IDamageable>();
        spriteRend = GetComponent<SpriteRenderer>();
        
    }
    void Update()
    {
        if (!damageable.IsAlive)
        {
            if (spriteRend.sprite != deathSprite)
            {
                spriteRend.sprite = deathSprite;
            }
        }
        if (damageable.IsAlive) 
        {
            HasTarget = attackZone.detectedColliders.Count > 0;
            animator.SetFloat(AnimationStrings.rbVelocity, Mathf.Abs(rb.velocityX));
            IsGrounded = capsuleCollider.Cast(new Vector2(0f, -1.0f), castFilterGround, groundHits, 0.1f) > 0;
            IsEdge = circleCollider.Cast(new Vector2(0f, -1.0f), castFilterGround, groundHits, 0.1f) <= 0;
            IsOnWall = circleCollider.Cast(new Vector2(1f, 0.0f), castFilterGround, groundHits, 0.3f) > 0 ||
            circleCollider.Cast(new Vector2(-1f, 0.0f), castFilterGround, groundHits, 0.3f) > 0;
        }
        if (!damageable.IsAlive)
        {
            while (deathCounter == 0)
            {
                Debug.LogWarning("KNIGHT DIED");
                deathCounter++;
                StartCoroutine(Death());
            }
        }
    }
    IEnumerator Death()
    {
        damageable.IsAlive = false;
        animator.SetBool(AnimationStrings.IsBlocking, false);
      //  animator.SetTrigger(AnimationStrings.Death);
        yield return new WaitForSeconds(0.5f);
        Debug.LogWarning("KNIGHT DIED 2");
        spriteRend.sprite = deathSprite;
    }
    private void FixedUpdate()
    {
        if (damageable.IsAlive)
        {
            if (IsHurt || damageable.IsBlocking)
            {
                moveSpeed = 0f;
            }
            else if (CanMove && !IsAttacking)
            {
                StartCoroutine(WalkCooldown());
            }
            else
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
            if (rb.velocity.y >= 0)
            {
                rb.gravityScale = 2.5f;
                rb.drag = 10f;
            }
            else
            {
                rb.gravityScale = 10f;
                rb.drag = 2f;
            }
            if (HasTarget && !IsAttacking)
            {
                StartCoroutine(AttackCooldown(attackRate));
            //    StartCoroutine(Blocking());
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
            }
            else
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

            if (IsOnWall || (IsEdge && IsGrounded))
            {
                Flip();
                if (!IsAggro)
                {
                    StartCoroutine(KnightFlip());
                }
            }
            SetFacingDirection();
        }
    }
    IEnumerator WalkCooldown()
    {
        yield return new WaitForSeconds(0.1f);

        rb.velocity = new Vector2(moveSpeed * Vector2.right.x, rb.velocity.y);
    }
    IEnumerator AttackCooldown(float attackRate)
    {
        animator.SetBool(AnimationStrings.IsAttacking, true);
        animator.SetBool(AnimationStrings.canMove, false);
        yield return new WaitForSeconds(0.2f);

        attackRate = 20 / attackRate;
        animator.SetTrigger(AnimationStrings.Attack);
        yield return new WaitForSeconds(0.1f + attackRate);
        animator.SetBool(AnimationStrings.IsAttacking, false);
        
    }
    private void Flip()
    {
            transform.localScale *= new Vector2(-1, 1);
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
    public void OnHit(int damage, Vector2 knockback, float faceRight)
    {
        IsHurt = true;
        StartCoroutine(KnockbackDelay(knockback, faceRight));
    }
    IEnumerator KnockbackDelay(Vector2 knockback, float faceRight)
    {
        float tempSpeed = moveSpeed;
        moveSpeed = 0;
        if (faceRight < 0)
        {
            rb.AddForce(new Vector2(-knockback.x, knockback.y));
        }
        else if (faceRight > 0)
        {
            rb.AddForce(knockback);
        }
        yield return new WaitForSeconds(0.02f);
        IsHurt = false;
        moveSpeed = tempSpeed;
    }
    public void OnBlock()
    {
        StartCoroutine(Blocking());
    }
    IEnumerator Blocking()
    {
        damageable.IsBlocking = true;
        animator.SetBool(AnimationStrings.IsBlocking, true);
        yield return new WaitForSeconds(0.5f);
        damageable.IsBlocking = false;
        animator.SetBool(AnimationStrings.IsBlocking, false);
    }
}
