using UnityEngine;

public class SwordCombatSystem : MonoBehaviour
{
    [Header("Combat Settings")]
    public float comboResetTime = 1.2f;
    public float staminaPerAttack = 20f;

    // Single fixed delay between all combo attacks
    public float attackDelay = 0.5f;

    [Header("References")]
    public StaminaSystem staminaSystem;
    public CharacterStats stats;
    public Animator animator;

    [Header("Attack Pattern")]
    public Vector3[] slashAngles = {
        new Vector3(0, 0, 0),     // Horizontal slash
        new Vector3(-45, 0, 0),   // Diagonal slash
        new Vector3(0, 90, 0)     // Finishing slash
    };

    [Header("Optional Sword Transform")]
    public Transform swordTransform;

    private int comboStep = 0;
    private float lastAttackTime;
    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryAttack();
        }

        if (Time.time - lastAttackTime > comboResetTime && isAttacking)
        {
            ResetCombo();
        }

        // Debug current animation state (optional)
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("Attack1"))
                Debug.Log("Currently playing: Attack1 (Horizontal Slash)");
            else if (stateInfo.IsName("Attack2"))
                Debug.Log("Currently playing: Attack2 (Diagonal Slash)");
            else if (stateInfo.IsName("Attack3"))
                Debug.Log("Currently playing: Attack3 (Finishing Slash)");
        }
    }

    void TryAttack()
    {
        // Block input if attack delay isn't finished yet
        if (Time.time < nextAttackTime)
        {
            return;
        }

        if (staminaSystem != null && stats != null)
        {
            staminaSystem.UseStamina(staminaPerAttack);
            PerformAttack();

            // Set next allowed attack time to now + fixed attackDelay
            nextAttackTime = Time.time + attackDelay;
        }
        else
        {
            Debug.LogWarning("Missing staminaSystem or stats reference.");
        }
    }

    void PerformAttack()
    {
        lastAttackTime = Time.time;
        isAttacking = true;

        float staminaMultiplier = staminaSystem.GetStaminaMultiplier();
        float damage = stats.baseDamage * staminaMultiplier;

        Vector3 attackAngle = slashAngles[comboStep];
        Debug.Log($"Slash {comboStep + 1}: Damage = {damage:F2}, Angle = {attackAngle}");

        if (swordTransform != null)
        {
            swordTransform.localRotation = Quaternion.Euler(attackAngle);
        }

        if (animator != null)
        {
            string triggerName = $"Attack{comboStep + 1}";
            Debug.Log("Setting animation trigger: " + triggerName);
            animator.SetTrigger(triggerName);
        }

        comboStep = (comboStep + 1) % slashAngles.Length;
    }

    void ResetCombo()
    {
        comboStep = 0;
        isAttacking = false;
        Debug.Log("Combo reset.");
    }
}
