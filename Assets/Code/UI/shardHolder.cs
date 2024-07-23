using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shardHolder : MonoBehaviour
{
    [SerializeField] private GameObject dash;
    [SerializeField] private GameObject doubleJump;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject shooting;
    [SerializeField] private GameObject abilitiesParent;  // Parent GameObject for all abilities

    [SerializeField] private AbilityManager abilityManager;

    [SerializeField] private float radius = 2.0f; // Radius around the center point

    [SerializeField] private bool hasDash;
    [SerializeField] private bool hasDoubleJump;
    [SerializeField] private bool hasShield;
    [SerializeField] private bool hasShooting;

    float currentAngle = 0;

    private void Start()
    {

    }

    private void Update()
    {
        abilityManager = FindObjectOfType<AbilityManager>();
        radius = abilitiesParent.GetComponent<RectTransform>().rect.width/3;
        float angleStep = 360.0f / 4;

        if (abilityManager.HasAbility("dash") && !hasDash)
        {
            hasDash = true;
            Vector2 dashOffset = CalculatePositionOffset(currentAngle);
            GameObject dashInstance = Instantiate(dash, Vector3.zero, Quaternion.identity, abilitiesParent.transform);
            dashInstance.GetComponent<RectTransform>().anchoredPosition = dashOffset;
            Debug.Log($"placing dash prefab, offset: {dashOffset}, currentAngle: {currentAngle}");

            currentAngle += angleStep;
        }
        if (abilityManager.HasAbility("doubleJump") && !hasDoubleJump)
        {
            hasDoubleJump = true;
            Vector2 doubleJumpOffset = CalculatePositionOffset(currentAngle);
            GameObject doubleJumpInstance = Instantiate(doubleJump, Vector3.zero, Quaternion.identity, abilitiesParent.transform);
            doubleJumpInstance.GetComponent<RectTransform>().anchoredPosition = doubleJumpOffset;
            Debug.Log($"placing doubleJump prefab, offset: {doubleJumpOffset}, currentAngle: {currentAngle}");

            currentAngle += angleStep;
        }
        if (abilityManager.HasAbility("shield") && !hasShield)
        {
            hasShield = true;
            Vector2 shieldOffset = CalculatePositionOffset(currentAngle);
            GameObject shieldInstance = Instantiate(shield, Vector3.zero, Quaternion.identity, abilitiesParent.transform);
            shieldInstance.GetComponent<RectTransform>().anchoredPosition = shieldOffset;
            Debug.Log($"placing shield prefab, offset: {shieldOffset}, currentAngle: {currentAngle}");

            currentAngle += angleStep;
        }
        if (abilityManager.HasAbility("shooting") && !hasShooting)
        {
            hasShooting = true;
            Vector2 shootingOffset = CalculatePositionOffset(currentAngle);
            GameObject shootingInstance = Instantiate(shooting, Vector3.zero, Quaternion.identity, abilitiesParent.transform);
            shootingInstance.GetComponent<RectTransform>().anchoredPosition = shootingOffset;
            Debug.Log($"placing shooting prefab, offset: {shootingOffset}, currentAngle: {currentAngle}");

            currentAngle += angleStep;
        }
    }

    private Vector2 CalculatePositionOffset(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radian) * radius, Mathf.Sin(radian) * radius);
    }

    private int CountAbilities()
    {
        int count = 0;
        if (abilityManager.HasAbility("dash")) count++;
        if (abilityManager.HasAbility("doubleJump")) count++;
        if (abilityManager.HasAbility("shield")) count++;
        if (abilityManager.HasAbility("shooting")) count++;
        return count;
    }
}