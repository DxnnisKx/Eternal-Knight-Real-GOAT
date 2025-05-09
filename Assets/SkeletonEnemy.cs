using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float directionChangeDelay = 0.5f; // Prevent rapid direction changes

    [Header("Attack")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Detection")]
    [SerializeField] private float playerDetectionRange = 5f;
    [SerializeField] private float playerChaseSpeed = 3f; // Faster when chasing player
    [SerializeField] private float returnToPatrolTime = 5f; // Time before resuming patrol if player escapes

    // References
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // State variables
    private int facingDirection = -1; // -1 left, 1 right
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private bool playerDetected = false;
    private Transform player;
    private Vector3 startPosition;
    private bool canChangeDirection = true;
    private float lastPlayerDetectionTime;
    private bool isChasing = false;

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
    }

    void Update()
    {
        // Handle attack cooldown
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        // Check for player detection
        DetectPlayer();

        // Try to attack if player detected and in range
        if (playerDetected && CanAttackPlayer())
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
        }

        // Update last detection time while player is in range
        if (playerDetected)
        {
            lastPlayerDetectionTime = Time.time;

            // If chasing, face towards player
            if (isChasing && !isAttacking && canChangeDirection)
            {
                facingDirection = (player.position.x > transform.position.x) ? 1 : -1;
                spriteRenderer.flipX = facingDirection > 0;
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
    }

    IEnumerator ChangeDirection()
    {
        if (!canChangeDirection)
            yield break;

        canChangeDirection = false;
        facingDirection *= -1;
        spriteRenderer.flipX = facingDirection > 0;

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
        if (attackTimer <= 0)
        {
            isAttacking = true;
            rb.linearVelocity = Vector2.zero; // Stop moving while attacking

            // Trigger attack animation
            if (animator != null)
                animator.SetTrigger(ATTACK_ANIMATION);

            // Apply damage to player
            StartCoroutine(AttackCoroutine());

            // Reset attack cooldown
            attackTimer = attackCooldown;
        }
    }

    IEnumerator AttackCoroutine()
    {
        // Wait for the animation to reach the attack point (around halfway)
        yield return new WaitForSeconds(0.3f);

        // Deal damage to player if in range
        Collider2D playerHit = Physics2D.OverlapCircle(
            transform.position + new Vector3(facingDirection * 0.5f, 0f),
            attackRange,
            playerLayer
        );

        if (playerHit != null)
        {
            // Try different ways to damage the player
            // Option 1: Try IDamageable interface
            var damageable = playerHit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }

            // Option 2: Try to find a method called TakeDamage via SendMessage
            playerHit.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);

            // Option 3: Try to find a method called Damage via SendMessage
            playerHit.SendMessage("Damage", attackDamage, SendMessageOptions.DontRequireReceiver);

            // Debug log when attacking player
            Debug.Log("Skeleton attacked player for " + attackDamage + " damage");
        }

        // End attack after a short delay (full animation)
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    void UpdateAnimations()
    {
        if (animator != null)
        {
            // Set walking animation
            animator.SetBool(WALK_ANIMATION, !isAttacking && rb.linearVelocity.x != 0);
        }
    }

    // Optional: Draw gizmos for debugging
    void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(facingDirection * 0.5f, 0), attackRange);

        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);

        // Draw ground check
        Gizmos.color = Color.green;
        Vector2 groundCheckPos = transform.position + new Vector3(facingDirection * edgeCheckDistance, -groundCheckDistance);
        Gizmos.DrawLine(groundCheckPos, groundCheckPos + Vector2.down * 0.5f);
    }
}