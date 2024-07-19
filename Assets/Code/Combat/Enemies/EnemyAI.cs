using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float shootingRange = 10f;
    [SerializeField] private bool eligibleToShoot = true;
    [SerializeField] private State startState = State.Idle;

    private GameObject player;
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
            SetState(eligibleToShoot && IsPlayerInRange(shootingRange) ? State.Shooting : State.Chasing);
        }
    }

    private void HandleWandering()
    {
        if (Time.time >= nextDirectionChangeTime)
        {
            ChangeDirectionRandomly();
        }

        movable.MoveInDirection(currentDirection);

        if (IsPlayerInRange(detectionRange))
        {
            SetState(eligibleToShoot && IsPlayerInRange(shootingRange) ? State.Shooting : State.Chasing);
        }
    }

    private void HandleChasing()
    {
        currentDirection = (player.transform.position - transform.position).normalized;
        movable.MoveInDirection(currentDirection);

        if (eligibleToShoot && IsPlayerInRange(shootingRange))
        {
            SetState(State.Shooting);
        }
    }

    private void HandleShooting()
    {
        currentDirection = (player.transform.position - transform.position).normalized;
        shooter.ShootInDirection(currentDirection);

        // Transition back to chasing if player moves out of shooting range or enemy becomes ineligible to shoot
        if (!IsPlayerInRange(shootingRange) || !eligibleToShoot)
        {
            SetState(State.Chasing);
        }
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector2.Distance(player.transform.position, transform.position) < range;
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
            ChangeDirectionRandomly(); // Ensure a direction is set when starting wandering
        }
    }
}