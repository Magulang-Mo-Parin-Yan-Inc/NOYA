using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Stats")]
    public float endurance = 10f;      // Affects stamina regeneration speed
    public float baseDamage = 10f;     // Used for damage calculation in sword attacks
}

