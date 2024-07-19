using UnityEngine;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }

    private JumpController jumpController;

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
        jumpController = GetComponent<JumpController>();
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