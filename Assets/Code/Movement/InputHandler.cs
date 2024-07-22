using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private float inoutCashingTime;
    [SerializeField] private Animator animator;


    private Movable movable;
    private JumpController jumpController;
    private DashController dashController;
    private Shooter shooter;
    private Damageable damageable;
    private GroundChecker groundChecker;
    private PauseMenu pauseMenu;

    private Vector2 facingDirection;

    private List<InputCash> cashList;

    private void Awake()
    {
        cashList = new List<InputCash>();
        movable = GetComponent<Movable>();
        jumpController = GetComponent<JumpController>();
        dashController = GetComponent<DashController>();
        shooter = GetComponent<Shooter>();
        damageable = GetComponent<Damageable>();
        //animator = GetComponent<Animator>();
        groundChecker = GetComponent<GroundChecker>();
    }

    /*private void OnSceneLoaded(int level)
    {
        if (level != 0)
        {
            pauseMenu = FindObjectOfType<PauseMenu>();
            Debug.Log("found something");
        }
    }*/

    private void Update()
    {
        pauseMenu = FindObjectOfType<PauseMenu>();
        float horizontalDirection = Input.GetAxis("Horizontal");

        transform.localRotation = Quaternion.Euler(0, horizontalDirection > 0 ? 0 : 180, 0);
        facingDirection = horizontalDirection > 0 ? Vector2.right : Vector2.left;
        Vector2 inputVector = new Vector2(horizontalDirection, 0);
        movable.MoveInDirection(inputVector);
        animator.SetBool("isRunning", horizontalDirection != 0);
        animator.SetBool("isJumping", jumpController.hasJumped());
        animator.SetBool("isDashing", dashController.hasDashed());

        if (Input.GetMouseButtonDown(0) && AbilityManager.Instance.HasAbility("shooting")) // Checks if the left mouse button is clicked
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 shootingDirection = new Vector2(mousePosition.x - shooter.transform.position.x, mousePosition.y - shooter.transform.position.y);

            shooter.ShootInDirection(shootingDirection); // Call shoot method on shooter
        }

        if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu != null)
        {
            pauseMenu.TogglePauseMenu();
        }

        updateCashTime();
        cashInput();
        executeInputs();
    }

    private void cashInput()
    {
        InputCash newInput;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            newInput = new InputCash(inoutCashingTime, KeyCode.Space);
            cashList.Add(newInput);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && AbilityManager.Instance.HasAbility("dash"))
        {
            newInput = new InputCash(inoutCashingTime, KeyCode.LeftShift);
            cashList.Add(newInput);
        }
        if (Input.GetKeyDown(KeyCode.E) && AbilityManager.Instance.HasAbility("shield"))
        {
            newInput = new InputCash(inoutCashingTime, KeyCode.E);
            cashList.Add(newInput);
        }
    }

    //TODO fix me, it's terrible way of doing things
    private void executeInputs()
    {
        for (int i = cashList.Count - 1; i >= 0; i--)
        {
            InputCash cash = cashList[i];
            bool success = false;
            if (cash.getKeyCode() == KeyCode.Space)
            {
                success = jumpController.TryJump();
                if (success)
                    animator.SetTrigger("takeOff");
            }
            if (cash.getKeyCode() == KeyCode.LeftShift)
            {
                success = dashController.TryDash(facingDirection);
                if (success)
                    animator.SetTrigger("dashed");
            }
            if (cash.getKeyCode() == KeyCode.E)
            {
                success = damageable.TryActivateShield();
            }
            if (success)
            {
                cashList.RemoveAt(i);
            }
        }
    }

    private void updateCashTime()
    {
        for (int i = cashList.Count - 1; i >= 0; i--)
        {
            InputCash cash = cashList[i];
            cash.reduceTime(Time.deltaTime);
            if (cash.reminingTime() <= 0)
            {
                cashList.RemoveAt(i);
            }
        }
    }
}

public class InputCash
{
    private float time;
    private KeyCode keyCode;

    public InputCash(float _time, KeyCode _keyCode)
    {
        time = _time;
        keyCode = _keyCode;
    }

    public void reduceTime(float deltaTime)
    {
        time -= deltaTime;
    }

    public float reminingTime()
    {
        return time;
    }

    public KeyCode getKeyCode()
    {
        return keyCode;
    }
}