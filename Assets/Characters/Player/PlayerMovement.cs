using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    public float walkSpeed = 16f;
    public float jumpPower = 20f;
    private bool isFacingRight = true;
    
    private Animator animator;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundcheck;
    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (horizontal != 0)
            animator.SetBool("isRunning", true);
        else
            animator.SetBool("isRunning", false);

        if (Input.GetButtonDown("Jump") && IsGrounded())
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpPower);

        if (Input.GetButtonUp("Jump") && rb.linearVelocityY > 0)
            rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * .5f);

        Flip();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * walkSpeed, rb.linearVelocityY);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundcheck.position, .2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localscale = transform.localScale;
            localscale.x *= -1f;
            transform.localScale = localscale;
        }
    }
}
