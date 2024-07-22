using UnityEngine;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private bool godMode;
    public static AbilityManager Instance { get; private set; }

    //[SerializeField] private InputHandler jumpController;
    //[SerializeField] private GameObject player;

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
        //player = GameObject.FindGameObjectWithTag("Player");
        //jumpController = GameObject.FindGameObjectWithTag("Player").GetComponent<InputHandler>();
        if (godMode)
            enableGodMode();
    }

    private void enableGodMode()
    {
        GrantAbility("dash");
        //GrantAbility("doubleJump");
        //GrantAbility("shield");
        GrantAbility("shooting");
    }

    public void GrantAbility(string abilityName)
    {
        if (abilities.Count < 4)
        {
            //player = GameObject.FindGameObjectWithTag("Player");
            //jumpController = player.GetComponent<InputHandler>();
            abilities.Add(abilityName);
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