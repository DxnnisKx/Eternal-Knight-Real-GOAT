using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Interface for anything that can take damage
public interface IDamageable
{
    void TakeDamage(int damage);
}

public class SkeletonEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.6f;
    [SerializeField] private float edgeCheckDistance = 0.5f;
    [SerializeField] private float directionChangeDelay = 0.5f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f; // Increased attack range for better detection
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private string deathSceneName = "DeathScene";
    [SerializeField] private GameObject attackHitbox; // Optional: reference to a child object representing the sword hitbox

    [Header("Detection")]
    [SerializeField] private float playerDetectionRange = 5f;
    [SerializeField] private float playerChaseSpeed = 3f;
    [SerializeField] private float returnToPatrolTime = 5f;

    [Header("Physics Properties")]
    [SerializeField] private float mass = 5f; // Make skeleton heavier
    [SerializeField] private float linearDrag = 2f; // Add some drag to prevent sliding
    [SerializeField] private bool freezeRotation = true; // Prevent rotation

    [Header("Debugging")]
    [SerializeField] private bool debugSpriteDirection = false;
    [SerializeField] private bool showAttackDebug = true;

    // References
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // State variables
    private int facingDirection = -1;
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private bool playerDetected = false;
    private Transform player;
    private Vector3 startPosition;
    private bool canChangeDirection = true;
    private float lastPlayerDetectionTime;
    private bool isChasing = false;
    private bool playerInContactWithEnemy = false;
    private bool hasDealtDamage = false; // Track if we've already dealt damage in this attack

    // Animation parameters
    private const string WALK_ANIMATION = "IsWalking";
    private const string ATTACK_ANIMATION = "Attack";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;

        // Find the player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Configure Rigidbody2D for proper physics behavior
        if (rb != null)
        {
            rb.mass = mass;
            rb.linearDamping = linearDrag;
            rb.freezeRotation = freezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Better collision detection
            rb.gravityScale = 3f; // Keep affected by gravity but heavier
        }

        // Initialize attack hitbox if defined
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }
    }

    void Update()
    {
        // Handle attack cooldown
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        // Check for player detection
        DetectPlayer();

        // Try to attack if player detected and in range
        if (playerDetected && CanAttackPlayer() && !isAttacking)
        {
            Attack();
        }
        // Chase player if detected but not in attack range
        else if (isChasing && !isAttacking)
        {
            ChasePlayer();
        }
        // Otherwise patrol
        else if (!isAttacking)
        {
            Patrol();
        }

        // Update animations
        UpdateAnimations();

        // Check if should stop chasing
        if (isChasing && Time.time - lastPlayerDetectionTime > returnToPatrolTime)
        {
            isChasing = false;
        }

        // Debug sprite direction if enabled
        if (debugSpriteDirection)
        {
            Debug.Log($"Facing Direction: {facingDirection}, Sprite FlipX: {spriteRenderer.flipX}, " +
                      $"Player Position: {(player != null ? player.position.x.ToString() : "null")}, " +
                      $"Enemy Position: {transform.position.x}, Is Attacking: {isAttacking}");
        }

        // Debug attack range
        if (showAttackDebug && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= playerDetectionRange)
            {
                Debug.Log($"Distance to player: {distanceToPlayer}, Attack Range: {attackRange}, " +
                          $"Can Attack: {distanceToPlayer <= attackRange && attackTimer <= 0}");
            }
        }
    }

    void DetectPlayer()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool wasDetected = playerDetected;
        playerDetected = distanceToPlayer <= playerDetectionRange;

        // Start chasing when player first detected
        if (playerDetected && !wasDetected)
        {
            isChasing = true;
            if (showAttackDebug)
                Debug.Log("Player detected! Starting to chase.");
        }

        // Update last detection time while player is in range
        if (playerDetected)
        {
            lastPlayerDetectionTime = Time.time;

            // If chasing, face towards player
            if (isChasing && !isAttacking && canChangeDirection)
            {
                bool isPlayerToRight = player.position.x > transform.position.x;
                facingDirection = isPlayerToRight ? 1 : -1;

                // Assuming sprite faces RIGHT by default when not flipped
                spriteRenderer.flipX = !isPlayerToRight;
            }
        }
    }

    void Patrol()
    {
        // Only check for edge/wall if we're allowed to change direction
        if (canChangeDirection)
        {
            // Check if there's ground ahead before moving
            Vector2 groundCheckPos = transform.position + new Vector3(facingDirection * edgeCheckDistance, -groundCheckDistance);
            bool isGroundAhead = Physics2D.Raycast(groundCheckPos, Vector2.down, 0.5f, groundLayer);

            // Draw the raycast in Scene view for debugging
            Debug.DrawRay(groundCheckPos, Vector2.down * 0.5f, isGroundAhead ? Color.green : Color.red);

            // Check if there's a wall ahead
            Vector2 wallCheckPos = transform.position + new Vector3(facingDirection * 0.6f, 0f);
            bool isWallAhead = Physics2D.Raycast(wallCheckPos, Vector2.right * facingDirection, 0.1f, groundLayer);

            Debug.DrawRay(wallCheckPos, Vector2.right * facingDirection * 0.1f, isWallAhead ? Color.red : Color.green);

            // If no ground ahead or wall ahead, turn around
            if (!isGroundAhead || isWallAhead)
            {
                StartCoroutine(ChangeDirection());
            }
        }

        // Move in facing direction at patrol speed
        rb.linearVelocity = new Vector2(moveSpeed * facingDirection, rb.linearVelocity.y);
    }

    void ChasePlayer()
    {
        if (player == null)
            return;

        // Use increased speed when chasing player
        float speed = playerChaseSpeed;

        // Move toward player
        rb.linearVelocity = new Vector2(speed * facingDirection, rb.linearVelocity.y);

        // Check if we're getting close enough to attack
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange && attackTimer <= 0)
        {
            if (showAttackDebug)
                Debug.Log("In attack range! Preparing to attack...");
        }
    }

    IEnumerator ChangeDirection()
    {
        if (!canChangeDirection)
            yield break;

        canChangeDirection = false;
        facingDirection *= -1;

        // Assuming sprite faces RIGHT by default when not flipped
        spriteRenderer.flipX = (facingDirection < 0);

        // Wait before allowing direction change again
        yield return new WaitForSeconds(directionChangeDelay);
        canChangeDirection = true;
    }

    bool CanAttackPlayer()
    {
        if (player == null || attackTimer > 0)
            return false;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        return distanceToPlayer <= attackRange;
    }

    void Attack()
    {
        if (attackTimer <= 0 && !isAttacking)
        {
            isAttacking = true;
            hasDealtDamage = false;
            rb.linearVelocity = Vector2.zero; // Stop moving while attacking

            if (showAttackDebug)
                Debug.Log("?? Starting attack animation!");

            // Trigger attack animation
            if (animator != null)
            {
                animator.SetTrigger(ATTACK_ANIMATION);
            }
            else
            {
                Debug.LogError("No Animator component found on skeleton enemy!");
            }

            // Apply damage to player through coroutine
            StartCoroutine(AttackCoroutine());

            // Reset attack cooldown
            attackTimer = attackCooldown;
        }
    }

    IEnumerator AttackCoroutine()
    {
        // Wait for the animation to reach the attack point (may need adjustment based on your actual animation)
        yield return new WaitForSeconds(0.3f);

        // Activate hitbox if available
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
        }

        // Calculate attack position based on facing direction
        Vector2 attackPos = transform.position + new Vector3(facingDirection * 0.8f, 0f);

        // Draw debug sphere to show attack area
        if (showAttackDebug)
        {
            Debug.DrawRay(transform.position, new Vector3(facingDirection * 0.8f, 0f), Color.red, 0.5f);
            Debug.Log($"Checking for player at position {attackPos} with radius {attackRange}");
        }

        // Deal damage to player if in range
        Collider2D playerHit = Physics2D.OverlapCircle(
            attackPos,
            attackRange,
            playerLayer
        );

        if (playerHit != null && !hasDealtDamage)
        {
            hasDealtDamage = true;
            Debug.Log("Player hit by skeleton's attack!");

            // Try different ways to damage the player
            // Option 1: Try IDamageable interface
            var damageable = playerHit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
                Debug.Log("Damaged player via IDamageable interface");
            }

            // Option 2: Try to find a method called TakeDamage via SendMessage
            playerHit.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);

            // Load the death scene when player is hit by the attack
            Debug.Log("Skeleton successfully attacked player. Loading death scene: " + deathSceneName);
            SceneManager.LoadScene(deathSceneName);
        }

        // Deactivate hitbox after a short time
        yield return new WaitForSeconds(0.2f);
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }

        // End attack after the full animation
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;

        if (showAttackDebug)
            Debug.Log("Attack finished!");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if it's the player
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInContactWithEnemy = true;
            // We don't kill the player immediately on contact anymore
            // Only the attack animation will trigger death
            Debug.Log("Player made contact with skeleton, but not dying immediately");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInContactWithEnemy = false;
        }
    }

    void UpdateAnimations()
    {
        if (animator != null)
        {
            // Set walking animation - only walking if not attacking and moving
            animator.SetBool(WALK_ANIMATION, !isAttacking && Mathf.Abs(rb.linearVelocity.x) > 0.1f);
        }
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