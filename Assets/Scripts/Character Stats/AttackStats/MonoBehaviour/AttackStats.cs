using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStats : MonoBehaviour
{
    public AttackData_SO templateData;
    private AttackData_SO attackData;
    private void Awake()
    {
        if (templateData != null)
        {
            attackData = Instantiate(templateData);
        }
    }
    #region Read from Data_SO
    public float AttackRange
    {
        get { return attackData == null ? 0 : attackData.attackRange; }
        set { attackData.attackRange = Mathf.Max(value, 0); }
    }
    public float SkillRange
    {
        get { return attackData == null ? 0 : attackData.skillRange; }
        set { attackData.skillRange = Mathf.Max(value, 0); }
    }
    public float CoolDown
    {
        get { return attackData == null ? 0 : attackData.coolDown; }
        set { attackData.coolDown = Mathf.Max(value, 0); }
    }
    public int MinDamage
    {
        get { return attackData == null ? 0 : attackData.minDamage; }
        set { attackData.minDamage = Mathf.Max(value, 0); }
    }
    public int MaxDamage
    {
        get { return attackData == null ? 0 : attackData.maxDamage; }
        set { attackData.maxDamage = Mathf.Max(value, 0); }
    }
    public float CriticalMultiplier
    {
        get { return attackData == null ? 0 : attackData.criticalMultiplier; }
        set { attackData.criticalMultiplier = Mathf.Max(value, 0); }
    }
    public float CriticalRate
    {
        get { return attackData == null ? 0 : attackData.criticalRate; }
        set { attackData.criticalRate = Mathf.Max(value, 0); }
    }
    public bool IsCritical
    {
        get { return attackData == null ? false : attackData.isCritical; }
        set { attackData.isCritical = value; }
    }

    #endregion
}
