using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private float inoutCashingTime;

    private Movable movable;
    private JumpController jumpController;
    private DashController dashController;

    private Vector2 facingDirection;

    private List<InputCash> cashList;

    private void Awake()
    {
        cashList = new List<InputCash>();
        movable = GetComponent<Movable>();
        jumpController = GetComponent<JumpController>();
        dashController = GetComponent<DashController>();
    }

    private void Update()
    {
        float horizontalDirection = Input.GetAxis("Horizontal");
        facingDirection = horizontalDirection > 0 ? Vector2.right : Vector2.left;
        Vector2 inputVector = new Vector2(horizontalDirection, 0);
        movable.MoveInDirection(inputVector);

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
        foreach (InputCash cash in cashList)
        {
            bool succes = false;
            if(cash.getKeyCode() == KeyCode.Space)
            {
                succes = jumpController.TryJump();
            }
            else
            {
                succes = dashController.TryDash(facingDirection);
            }
            if (succes)
                cashList.Remove(cash);
        }
    }

    private void updateCashTime()
    {
        foreach (InputCash cash in cashList)
        {
            cash.reduceTime(Time.deltaTime);
            if (cash.reminingTime() <= 0)
            {
                cashList.Remove(cash);
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