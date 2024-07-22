using UnityEngine;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private bool godMode;
    public static AbilityManager Instance { get; private set; }

    private string[] tutorial =  new string[4] { "", "", "", "" };
    private string[] level_1  =  new string[4] { "shooting", "", "", "" };
    private string[] level_2  =  new string[4] { "shooting", "dash", "", "" };
    private string[] level_3  =  new string[4] { "shooting", "dash", "doubleJump", "" };

    [SerializeField] private HashSet<string> abilities = new HashSet<string>();

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
        GrantAbility("doubleJump");
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

    public void GrantAbilities(string[] abilityNames)
    {
        for (int i = 0; i < 3; i++)
        {
            if (abilities.Count < 4 && abilityNames[i] != "")
            {
                //player = GameObject.FindGameObjectWithTag("Player");
                //jumpController = player.GetComponent<InputHandler>();
                abilities.Add(abilityNames[i]);
                Debug.Log($"Ability granted: {abilityNames[i]}");
            }
            else
            {
                Debug.Log("Maximum number of abilities reached.");
            }
        }
    }

    public void ReloadAbilities(int levelIndex)
    {
        abilities.Clear();
        Debug.Log("level index: "+ levelIndex);
        switch (levelIndex)
        {
            case 1:
                GrantAbilities(tutorial);
                break;
            case 2:
                GrantAbilities(level_1);
                break;
            case 3:
                GrantAbilities(level_2);
                break;
            case 4:
                GrantAbilities(level_3);
                break;
            default:
                GrantAbilities(tutorial);
                break;
        }

    }

    public bool HasAbility(string abilityName)
    {
        return abilities.Contains(abilityName);
    }
}