using UnityEngine;

public class shardHolder : MonoBehaviour
{
    [SerializeField] private GameObject dash;
    [SerializeField] private GameObject doubleJump;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject shooting;
    [SerializeField] private GameObject abilitiesParent;  // Parent GameObject for all abilities

    [SerializeField] private AbilityManager abilityManager;

    [SerializeField] private Vector3 offset = new Vector3(1, 1, 0); // Offset for instantiation

    private bool hasDash;
    private bool hasDoubleJump;
    private bool hasShield;
    private bool hasShooting;

    private void Update()
    {
        abilityManager = FindObjectOfType<AbilityManager>();
        if (abilityManager.HasAbility("dash") && !hasDash)
        {
            hasDash = true;
            GameObject dashInstance = Instantiate(dash, transform.position + offset, Quaternion.identity);
            dashInstance.transform.SetParent(abilitiesParent.transform, false);
        }
        if (abilityManager.HasAbility("doubleJump") && !hasDoubleJump)
        {
            hasDoubleJump = true;
            GameObject doubleJumpInstance = Instantiate(doubleJump, transform.position + offset, Quaternion.identity);
            doubleJumpInstance.transform.SetParent(abilitiesParent.transform, false);
        }
        if (abilityManager.HasAbility("shield") && !hasShield)
        {
            hasShield = true;
            GameObject shieldInstance = Instantiate(shield, transform.position + offset, Quaternion.identity);
            shieldInstance.transform.SetParent(abilitiesParent.transform, false);
        }
        if (abilityManager.HasAbility("shooting") && !hasShooting)
        {
            hasShooting = true;
            GameObject shootingInstance = Instantiate(shooting, transform.position + offset, Quaternion.identity);
            shootingInstance.transform.SetParent(abilitiesParent.transform, false);
        }
    }
}