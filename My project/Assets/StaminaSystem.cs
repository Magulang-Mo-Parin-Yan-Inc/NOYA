using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    public CharacterStats stats;

    [Header("Stamina Settings")]
    public float staminaRegenRate = 10f;
    public float staminaRegenDelay = 2f;

    private float lastStaminaUseTime;
    private bool isConsumingStamina = false;

    void Update()
    {
        RegenerateStamina();
        // Reset isConsumingStamina here AFTER regeneration check so it reflects current frame correctly
        isConsumingStamina = false;
    }

    public bool UseStamina(float amount)
    {
        if (stats.currentStamina >= amount)
        {
            stats.currentStamina -= amount;
            lastStaminaUseTime = Time.time;
            isConsumingStamina = true;

            Debug.Log($"Used stamina: {amount:F2}, remaining stamina: {stats.currentStamina:F2}");
            return true;
        }
        else
        {
            Debug.Log($"Not enough stamina for {amount:F2}, current stamina: {stats.currentStamina:F2}");
            return false;
        }
    }

    private void RegenerateStamina()
    {
        if (!isConsumingStamina && Time.time - lastStaminaUseTime > staminaRegenDelay && stats.currentStamina < stats.maxStamina)
        {
            stats.currentStamina += staminaRegenRate * Time.deltaTime;
            if (stats.currentStamina > stats.maxStamina)
                stats.currentStamina = stats.maxStamina;

            Debug.Log($"Regenerating stamina: {stats.currentStamina:F2}");
        }
    }
}