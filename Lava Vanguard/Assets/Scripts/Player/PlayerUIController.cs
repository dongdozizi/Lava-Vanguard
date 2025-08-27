using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [Header("UI Components")]
    public Image hpBarFill;
    public Image expBarFill;

    [Header("Player Stats")]
    private float maxHP = 100f;
    private float currentHP;

    private float maxEXP = 100f;
    private float currentEXP;

    void Start()
    {
        currentHP = maxHP;
        currentEXP = 0;
        UpdateUI();
    }
    
    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateUI();
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateUI();
    }
    public void GainEXP(float amount)
    {
        currentEXP += amount;
        currentEXP = Mathf.Clamp(currentEXP, 0, maxEXP);
        UpdateUI();
    }
    private void CheckLevelUp()
    {
        if (currentEXP >= maxEXP)
        {
            currentEXP = 0;
            maxEXP *= 1.2f;
            maxHP += 10f; 
            currentHP = maxHP; 
        }
    }

    private void UpdateUI()
    {
        if (hpBarFill)
            hpBarFill.fillAmount = currentHP / maxHP;

        if (expBarFill)
            expBarFill.fillAmount = currentEXP / maxEXP;
    }
}
