using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Knight : MonoBehaviour
{
    public ContactFilter2D castFilterGround;

    public enum EnemyDirection { Left, Right}

    private EnemyDirection _walkDirection;

    public EnemyDirection WalkDirection
    {
        get { return _walkDirection; }
        set { 
            if (_walkDirection != value)
            {
                Flip();
            }
            
            _walkDirection = value; }
    }


    public float walkSpeed = 3f;
    public float rayLength = 3.4f;

    CapsuleCollider2D capsuleCollider;
    CircleCollider2D circleCollider;

    public DetectionZone attackZone;

    Rigidbody2D rb;
    Animator animator;

    RaycastHit2D[] groundHits = new RaycastHit2D[1];
    RaycastHit2D[] wallHits = new RaycastHit2D[4];

    [SerializeField] private bool _isGrounded = true;

    public bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            _isFacingRight = value;
        }
    }
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
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(walkSpeed * Vector2.right.x, rb.velocity.y);

        IsGrounded = circleCollider.Cast(new Vector2(0f, -1.0f), castFilterGround, groundHits, 0.3f) > 0;

        IsOnWall = circleCollider.Cast(new Vector2(1f, 0.0f), castFilterGround, groundHits, 0.3f) > 0 ||
            circleCollider.Cast(new Vector2(-1f, 0.0f), castFilterGround, groundHits, 0.3f) > 0;

        if (_isOnWall)
        {
            Flip();
        }

        SetFacingDirection(walkSpeed);



    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Flip()
    {
            transform.localScale *= new Vector2(-1, 1);
            walkSpeed *= -1;
    }
    private void SetFacingDirection(float walkSpeed)
    {
        if (walkSpeed > 0)
        {
            IsFacingRight = true;
        }
        else if (walkSpeed < 0)
        {
            IsFacingRight = false;
        }
    }
}
