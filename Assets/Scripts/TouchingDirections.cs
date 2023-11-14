using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D castFilter;
    public float groundDistance = 0.05f;

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

        } }
    RaycastHit2D[] groundHits = new RaycastHit2D[8];
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
    }
}
