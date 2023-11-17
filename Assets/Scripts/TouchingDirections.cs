using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D castFilter;
    public float groundDistance = 0.05f;
    public float wallDistance = 0.2f;
    public float ceilingDistance = 0.05f;
    

    CapsuleCollider2D touchCol;
    Rigidbody2D rb;
    Animator animator;

    [SerializeField] private bool _isGrounded;
        public bool IsGrounded { get
            {
                return _isGrounded;
            } private set
            {
                _isGrounded = value;
                animator.SetBool(AnimationStrings.IsGrounded, value);

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

    [SerializeField] private bool _isCeiling;
    public bool IsCeiling
    {
        get
        {
            return _isCeiling;
        }
        private set
        {
            _isCeiling = value;
            animator.SetBool(AnimationStrings.IsCeiling, value);

        }
    }

    RaycastHit2D[] groundHits = new RaycastHit2D[8];
    RaycastHit2D[] wallHits = new RaycastHit2D[8];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[8];
    // Start is called before the first frame update
    private void Awake()
    {
        touchCol = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
    }
    void Start()
    {
        
    }
    void FixedUpdate()
    {
     IsGrounded = touchCol.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
     IsOnWall = touchCol.Cast(Vector2.left, castFilter, wallHits, wallDistance) > 0 || 
            touchCol.Cast(Vector2.right, castFilter, wallHits, wallDistance) > 0;
     IsCeiling = touchCol.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
    }
}
