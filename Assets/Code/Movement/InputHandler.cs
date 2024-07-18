using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private float inoutCashingTime;

    private Movable movable;
    private JumpController jumpController;
    private DashController dashController;
    private Shooter shooter;

    private Vector2 facingDirection;

    private List<InputCash> cashList;

    private void Awake()
    {
        cashList = new List<InputCash>();
        movable = GetComponent<Movable>();
        jumpController = GetComponent<JumpController>();
        dashController = GetComponent<DashController>();
        shooter = GetComponent<Shooter>();
    }

    private void Update()
    {
        float horizontalDirection = Input.GetAxis("Horizontal");
        facingDirection = horizontalDirection > 0 ? Vector2.right : Vector2.left;
        Vector2 inputVector = new Vector2(horizontalDirection, 0);
        movable.MoveInDirection(inputVector);

        if (Input.GetMouseButtonDown(0)) // Checks if the left mouse button is clicked
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 shootingDirection = new Vector2(mousePosition.x - shooter.transform.position.x, mousePosition.y - shooter.transform.position.y);

            shooter.ShootInDirection(shootingDirection); // Call shoot method on shooter
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
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            newInput = new InputCash(inoutCashingTime, KeyCode.LeftShift);
            cashList.Add(newInput);
        }
    }

    // TODO fix this stuff because unity doesn't like it. Thanks god it works... for now...
    private void executeInputs()
    {
        for (int i = cashList.Count - 1; i >= 0; i--)
        {
            InputCash cash = cashList[i];
            bool success = false;
            if (cash.getKeyCode() == KeyCode.Space)
            {
                success = jumpController.TryJump();
            }
            else
            {
                success = dashController.TryDash(facingDirection);
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