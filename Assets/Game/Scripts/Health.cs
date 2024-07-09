using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth;
    public int currentnHealth;

    public float CurrentHealthPercentage
    {
        get
        {
            return (float)currentnHealth / (float)maxHealth;
        }
    }

    private Character _cc;

    private void Awake()
    {
        currentnHealth = maxHealth;
        _cc = GetComponent<Character>();
    }

    public void ApplyDamage(int damage)
    {
        currentnHealth -= damage;
        //Debug.Log(gameObject.name + "took damage " + damage);
        //Debug.Log(gameObject.name + "current health" + currentnHealth);
        CheckHealth();
    }

    private void CheckHealth()
    {
        if(currentnHealth <= 0)
        {

        _cc.SwitchStateTo(Character.CharacterState.Dead);
        }
    }

    public void AddHealth(int increaseHP)
    {
        currentnHealth += increaseHP;
        if(currentnHealth > maxHealth)
        {
            currentnHealth = maxHealth;
        }
    }

}
