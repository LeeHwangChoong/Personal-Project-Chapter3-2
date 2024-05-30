using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class PlayerCondition : MonoBehaviour, IDamagelbe
{
    public UICondition UICondition;

    Condition health { get { return UICondition.health; } }
    Condition stamina { get { return UICondition.stamina; } }

    public event Action onTakeDamage;

    private float lastGroundedTime; // ÂøÁö ½Ã°£
    public bool isGrounded;

    private void Update()
    {
        stamina.Add(stamina.passiveValue * Time.deltaTime);
    }

    public bool IsGrounded
    {
        get => isGrounded;
        set
        {
            if (isGrounded != value)
            {
                isGrounded = value;
                if (isGrounded)
                {
                    lastGroundedTime = Time.time;
                }
            }
        }
    }

    public bool GroundedTime(float duration)
    {
        return isGrounded && (Time.time - lastGroundedTime > duration);
    }

    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }

        stamina.Subtract(amount);
        return true;
    }

    public void Heal(float amout)
    {
        health.Add(amout);
    }

    public void Drink(float amout)
    {
        stamina.Add(amout);
    }

    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
        onTakeDamage?.Invoke();
    }
}

public interface IDamagelbe
{
    void TakePhysicalDamage(int damage);
}
