using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Interface for anything that can take damage
public interface IDamageable
{
    void TakeDamage(int damage);
}

public class SkeletonEnemy : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.6f;
    [SerializeField] private float edgeCheckDistance = 0.5f;
    [SerializeField] private float directionChangeDelay = 0.5f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private string deathSceneName = "DeathScene";
    [SerializeField] private GameObject attackHitbox;

    [Header("Detection")]
    [SerializeField] private float playerDetectionRange = 5f;
    [SerializeField] private float returnToPatrolTime = 5f;

    [Header("Death")]
    [SerializeField] private float deathDelay = 1f;

    // References
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private BoxCollider2D boxCollider;

    // State variables
    private int facingDirection = -1;
    private bool isAttacking = false;
    private bool canChangeDirection = true;
    private float attackTimer = 0f;
    private float lastPlayerDetectionTime;
    private bool isChasing = false;
    private Vector3 startPosition;
    private bool isDead = false;

    // Animation parameters
    private const string IsWalkingParam = "IsWalking";
    private const string AttackParam = "Attack";
    private const string IdleParam = "Idle";
    private const string DeathParam = "Death";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPosition = transform.position;

        // Configure Rigidbody2D
        if (rb != null)
        {
            rb.mass = 5f;
            rb.linearDamping = 2f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.gravityScale = 3f;
        }

        // Initialize attack hitbox
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }

        // Check if animator exists
        if (animator == null)
        {
            Debug.LogError("No Animator component found on skeleton enemy!");
        }
    }

    void Update()
    {
        // Don't do anything if dead
        if (isDead)
            return;

        // Handle attack cooldown
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // Check for player detection
        DetectPlayer();

        // Update behavior based on state
        if (!isAttacking)
        {
            if (player != null && CanAttackPlayer())
            {
                Attack();
            }
            else if (isChasing)
            {
                ChasePlayer();
            }
            else
            {
                Patrol();
            }
        }

        // Update animations
        UpdateAnimations();

        // Check if should stop chasing
        if (isChasing && Time.time - lastPlayerDetectionTime > returnToPatrolTime)
        {
            isChasing = false;
        }
    }

    void DetectPlayer()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool playerDetected = distanceToPlayer <= playerDetectionRange;

        // Start chasing when player detected
        if (playerDetected)
        {
            isChasing = true;
            lastPlayerDetectionTime = Time.time;

            // Face towards player if chasing
            if (!isAttacking && canChangeDirection)
            {
                bool isPlayerToRight = player.position.x > transform.position.x;
                facingDirection = isPlayerToRight ? 1 : -1;
                spriteRenderer.flipX = !isPlayerToRight;
            }
        }
    }

    void Patrol()
    {
        // Check for obstacles if able to change direction
        if (canChangeDirection)
        {
            // Check if there's ground ahead
            Vector2 groundCheckPos = transform.position + new Vector3(facingDirection * edgeCheckDistance, -groundCheckDistance);
            bool isGroundAhead = Physics2D.Raycast(groundCheckPos, Vector2.down, 0.5f, groundLayer);

            // Check if there's a wall ahead
            Vector2 wallCheckPos = transform.position + new Vector3(facingDirection * 0.6f, 0f);
            bool isWallAhead = Physics2D.Raycast(wallCheckPos, Vector2.right * facingDirection, 0.1f, groundLayer);

            // Turn around if needed
            if (!isGroundAhead || isWallAhead)
            {
                StartCoroutine(ChangeDirection());
            }
        }

        // Move in facing direction
        rb.linearVelocity = new Vector2(moveSpeed * facingDirection, rb.linearVelocity.y);
    }

    void ChasePlayer()
    {
        if (player == null)
            return;

        // Move toward player
        rb.linearVelocity = new Vector2(chaseSpeed * facingDirection, rb.linearVelocity.y);
    }

    IEnumerator ChangeDirection()
    {
        if (!canChangeDirection)
            yield break;

        canChangeDirection = false;
        facingDirection *= -1;
        spriteRenderer.flipX = (facingDirection < 0);

        // Wait before allowing direction change again
        yield return new WaitForSeconds(directionChangeDelay);
        canChangeDirection = true;
    }

    bool CanAttackPlayer()
    {
        if (player == null || attackTimer > 0 || isAttacking)
            return false;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        return distanceToPlayer <= attackRange;
    }

    void Attack()
    {
        if (attackTimer <= 0 && !isAttacking)
        {
            isAttacking = true;
            rb.linearVelocity = Vector2.zero; // Stop moving while attacking

            // Trigger attack animation
            if (animator != null)
            {
                animator.ResetTrigger(AttackParam);
                animator.SetTrigger(AttackParam);
            }

            // Attack coroutine will handle the attack timing based on animation events
            StartCoroutine(AttackCoroutine());

            // Reset attack cooldown
            attackTimer = attackCooldown;
        }
    }

    IEnumerator AttackCoroutine()
    {
        // Wait for the attack animation to reach the attack point
        // This should be replaced with animation events in your actual implementation
        yield return new WaitForSeconds(0.2f); // Wait for attack wind-up

        // Activate hitbox if available
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
        }

        // Calculate attack position based on facing direction
        Vector2 attackPos = transform.position + new Vector3(facingDirection * 0.8f, 0f);

        // Check for player hit
        Collider2D playerHit = Physics2D.OverlapCircle(attackPos, attackRange, playerLayer);

        if (playerHit != null)
        {
            // Try different ways to damage the player
            var damageable = playerHit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
            else
            {
                // Fallback damage method
                playerHit.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
            }

            // Wait for a moment before showing death scene
            yield return new WaitForSeconds(0.3f);

            // Load death scene
            SceneManager.LoadScene(deathSceneName);
        }
        else
        {
            // Wait for the attack animation to end
            yield return new WaitForSeconds(0.3f);
        }

        // Deactivate hitbox
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }

        // End attack state
        isAttacking = false;
    }

    void UpdateAnimations()
    {
        if (animator != null)
        {
            // Only walking if not attacking, not idle, and velocity is significant
            bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
            bool isWalking = !isAttacking && isMoving;

            // Set walking animation
            animator.SetBool(IsWalkingParam, isWalking);

            // Set idle animation when not walking and not attacking
            animator.SetBool(IdleParam, !isWalking && !isAttacking);
        }
    }

    // Animation event methods to be called from the animation clips
    public void OnAttackStart()
    {
        // This should be called from the animation event when the attack starts
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
        }
    }

    public void OnAttackEnd()
    {
        // This should be called from the animation event when the attack ends
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }
        isAttacking = false;
    }

    // Implement IDamageable interface to handle player jumping on top
    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        // Kill the skeleton
        Die();
    }

    // Handle player collision - check if player is jumping on top of skeleton
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead)
            return;

        // Check if it's the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get contact points
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Check if player is coming from above (normal points down from player perspective)
                if (contact.normal.y < -0.7f)
                {
                    // Player is on top, trigger death
                    Die();

                    // Give player a slight bounce
                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 7f);
                    }

                    break;
                }
            }
        }
    }

    private void Die()
    {
        isDead = true;

        // Stop movement
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;

        // Trigger death animation
        if (animator != null)
        {
            animator.SetTrigger(DeathParam);
        }

        // Disable attacks
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }

        // Disable collider to prevent further interactions
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        // Destroy after animation plays
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        // Wait for death animation to play
        yield return new WaitForSeconds(deathDelay);

        // Destroy the enemy
        Destroy(gameObject);
    }

    // Optional: Draw gizmos for debugging
    void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(facingDirection * 0.8f, 0), attackRange);

        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);

        // Draw ground check
        Gizmos.color = Color.green;
        Vector2 groundCheckPos = transform.position + new Vector3(facingDirection * edgeCheckDistance, -groundCheckDistance);
        Gizmos.DrawLine(groundCheckPos, groundCheckPos + Vector2.down * 0.5f);
    }
}