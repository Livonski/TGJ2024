using UnityEngine;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private bool godMode;
    public static AbilityManager Instance { get; private set; }

    private JumpController jumpController;
    private GameObject player;

    private HashSet<string> abilities = new HashSet<string>();

    private void Awake()
    {
        if (Instance != this)
        {
            if (Instance != null)
                Destroy(gameObject);
            else
                Instance = this;
        }

        DontDestroyOnLoad(gameObject);
        player = GameObject.FindGameObjectWithTag("Player");
        jumpController = player.GetComponent<JumpController>();
        if (godMode)
            enableGodMode();
    }

    private void enableGodMode()
    {
        GrantAbility("dash");
        //GrantAbility("doubleJump");
        GrantAbility("shield");
        GrantAbility("shooting");
    }

    public void GrantAbility(string abilityName)
    {
        if (abilities.Count < 4)
        {
            abilities.Add(abilityName);
            if (abilityName == "doubleJump")
                jumpController.setMaxJumpCharges(2);
            Debug.Log($"Ability granted: {abilityName}");
        }
        else
        {
            Debug.Log("Maximum number of abilities reached.");
        }
    }

    public bool HasAbility(string abilityName)
    {
        return abilities.Contains(abilityName);
    }
}