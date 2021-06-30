using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBaseStats : MonoBehaviour
{
    public CharacterBaseData_SO templateData;
    private CharacterBaseData_SO characterData;
    private void Awake()
    {
        if(templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }
    #region Read from CharacterDataSO
    public int MaxHealth
    {
        get { return characterData == null ? 0 : characterData.maxHealth; }
        set { characterData.maxHealth = Mathf.Max(value, 0); }
    }
    public int CurrentHealth
    {
        get { return characterData == null ? 0 : characterData.currentHealth; }
        set { characterData.currentHealth = Mathf.Max(value, 0); }
    }
    public int BaseDefence
    {
        get { return characterData == null ? 0 : characterData.baseDefence; }
        set { characterData.baseDefence = Mathf.Max(value, 0); }
    }
    public int CurrentDefence
    {
        get { return characterData == null ? 0 : characterData.currentDefence; }
        set { characterData.currentDefence = Mathf.Max(value, 0); }
    }
    #endregion
    public void Hurt(AttackStats attackerAttackStats)
    {
        int damage = Mathf.Max(GetRealDamage(attackerAttackStats) - CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        if (attackerAttackStats.IsCritical)
        {
            GetComponent<Animator>().SetTrigger("Hit");
            print(attackerAttackStats.gameObject + "暴击！" + damage);
        }
        else
        {
            print(attackerAttackStats.gameObject + "未暴击！" + damage);
        }
        //TODO:UI变化
    }
    private int GetRealDamage(AttackStats attacker)
    {
        float realDamage = Random.Range(attacker.MinDamage, attacker.MaxDamage);
        if (attacker.IsCritical)
        {
            realDamage *= attacker.CriticalMultiplier;
        }
        return (int)realDamage;
    }
}
