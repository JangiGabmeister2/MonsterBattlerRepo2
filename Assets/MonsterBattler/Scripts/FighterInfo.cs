using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FighterInfo : MonoBehaviour
{
    [Header("Health Information")]
    public int health;
    public int maxHealth;

    [Header("Stamina Information")]
    public int stamina;
    public int maxStamina;

    [Header("Is Fighter Blocking?")]
    public bool isBlocking = false;

    [Header("Health and Stamina Texts")]
    public Text healthText;
    public Text staminaText;

    public void Damage(int damageAmount)
    {
        if (health - damageAmount < 0)
        {
            health = 0;
        }
        else
        {
            health -= damageAmount;
        }
    }

    public void Heal(int healAmount)
    {
        if (health + healAmount > maxHealth)
        {
            health = maxHealth;

        }
        else
        {
            health += healAmount;
        }
    }

    public void Update()
    {
        healthText.text = $"Health: {health} / {maxHealth}";

        if (stamina > maxStamina)
        {
            stamina = maxStamina;
        }

        if (stamina < 0)
        {
            stamina = 0;
        }

        staminaText.text = $"Stamina: {stamina} / {maxStamina}";
    }
}
