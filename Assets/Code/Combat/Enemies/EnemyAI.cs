using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float shootingRange = 10f;
    [SerializeField] private bool eligibleToShoot = true;
    [SerializeField] private State startState = State.Idle;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private float avoidanceStrength = 1.5f;
    [SerializeField] private float enemyDetectionRadius = 2f;


    [SerializeField] private GameObject player;
    private Movable movable;
    private Shooter shooter;
    private Vector2 currentDirection;
    private float directionChangeInterval = 3f;
    private float nextDirectionChangeTime;

    private enum State
    {
        Idle,
        Wandering,
        Chasing,
        Shooting
    }

    [SerializeField] private State currentState;

    private void Awake()
    {
        movable = GetComponent<Movable>();
        shooter = GetComponent<Shooter>();
        if (movable == null)
        {
            Debug.LogError("Movable component not found on " + gameObject.name);
        }
        if (shooter == null && eligibleToShoot)
        {
            Debug.LogError("Shooter component not found on " + gameObject.name);
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found in the scene.");
        }
        SetState(startState);
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Wandering:
                HandleWandering();
                break;
            case State.Chasing:
                HandleChasing();
                break;
            case State.Shooting:
                HandleShooting();
                break;
        }
    }

    private void OnDrawGizmos()
    {
        // Draw detection range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw shooting range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }

    private void HandleIdle()
    {
        if (IsPlayerInRange(detectionRange))
        {
            SetState(eligibleToShoot && IsPlayerInRange(shootingRange) && CanHitPlayer() ? State.Shooting : State.Chasing);
        }
    }

    private void HandleWandering()
    {
        if (Time.time >= nextDirectionChangeTime)
        {
            ChangeDirectionRandomly();
        }

        currentDirection += AvoidOtherEnemies();
        currentDirection.Normalize();

        movable.MoveInDirection(currentDirection);

        if (IsPlayerInRange(detectionRange))
        {
            SetState(eligibleToShoot && IsPlayerInRange(shootingRange) && CanHitPlayer() ? State.Shooting : State.Chasing);
        }
    }

    private void HandleChasing()
    {
        currentDirection = (player.transform.position - transform.position).normalized;
        currentDirection += AvoidOtherEnemies();
        currentDirection.Normalize();

        movable.MoveInDirection(currentDirection);

        if (eligibleToShoot && IsPlayerInRange(shootingRange) && CanHitPlayer())
        {
            SetState(State.Shooting);
        }
    }

    private void HandleShooting()
    {
        currentDirection = (player.transform.position - transform.position).normalized;
        shooter.ShootInDirection(currentDirection);

        // Transition back to chasing if player moves out of shooting range or enemy becomes ineligible to shoot
        if ((!IsPlayerInRange(shootingRange) || !eligibleToShoot) && CanHitPlayer())
        {
            currentDirection = (player.transform.position - transform.position).normalized;
            shooter.ShootInDirection(currentDirection);
        }
        else
        {
            SetState(State.Chasing);
        }
    }

    private Vector2 AvoidOtherEnemies()
    {
        Vector2 avoidanceVector = Vector2.zero;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, enemyDetectionRadius, LayerMask.GetMask("Enemy"));

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject != gameObject) // Ensure the enemy does not consider itself
            {
                Vector2 fleeDirection = (transform.position - hit.transform.position).normalized;
                avoidanceVector += fleeDirection;
            }
        }

        return avoidanceVector * avoidanceStrength;
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector2.Distance(player.transform.position, transform.position) < range;
    }

    private bool CanHitPlayer()
    {
        float offsetX = player.transform.position.x > transform.position.x ? 0.6f : -0.6f;
        float offsetY = player.transform.position.y > transform.position.y ? 0.6f : -0.6f;
        Vector3 offset = new Vector3(offsetX, offsetY, 0);
        RaycastHit2D hit = Physics2D.Raycast(transform.position + offset, player.transform.position - (transform.position + offset), shootingRange, obstacleMask);
        Debug.DrawLine(transform.position + offset, player.transform.position);

        if (hit.collider != null)
        {
            return hit.collider.gameObject.tag == "Player";
        }
        else
        {
            return false; 
        }
    }

    private void ChangeDirectionRandomly()
    {
        currentDirection = new Vector2(Random.Range(-1f, 1f), 0).normalized;
        nextDirectionChangeTime = Time.time + directionChangeInterval;
    }

    private void SetState(State newState)
    {
        currentState = newState;
        if (newState == State.Wandering)
        {
            ChangeDirectionRandomly();
        }
    }
}