using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    public CharacterStats stats;

    [Header("Stamina Settings")]
    public float baseRegenRate = 5f; // Base regen before scaling with endurance
    public float staminaRegenDelay = 2f;
    private float lastStaminaUseTime;
    private bool isConsumingStamina = false;

    void Update()
    {
        RegenerateStamina();
        isConsumingStamina = false;
    }

    public bool UseStamina(float amount)
    {
        if (stats.currentStamina >= amount)
        {
            stats.currentStamina -= amount;
            lastStaminaUseTime = Time.time;
            isConsumingStamina = true;

            return true;
        }
        else
        {
            // Still consume a bit for penalty behavior if you want
            return false;
        }
    }

    private void RegenerateStamina()
    {
        if (!isConsumingStamina && Time.time - lastStaminaUseTime > staminaRegenDelay && stats.currentStamina < stats.maxStamina)
        {
            float regenAmount = (baseRegenRate + stats.endurance) * Time.deltaTime;
            stats.currentStamina += regenAmount;

            if (stats.currentStamina > stats.maxStamina)
                stats.currentStamina = stats.maxStamina;
        }
    }

    public float GetStaminaMultiplier()
    {
        float staminaPercent = stats.currentStamina / stats.maxStamina;

        if (staminaPercent > 0.3f)
            return 1f; // Full strength
        else
            return 0.5f; // Weakened attacks below 30% stamina
    }
}
