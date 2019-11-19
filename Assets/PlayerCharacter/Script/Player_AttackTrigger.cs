using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using static Data;
using static GameManager;

public class Player_AttackTrigger : MonoBehaviour
{
    #region Get,Set
    public Action<Transform, MonoBehaviour, IDamage, float> onTargetTriggered;
    #endregion
    #region Value
    private float m_Damage;
    #endregion

    #region Event
    /// <summary>
    /// 공격 트리거를 켭니다.
    /// </summary>
    /// <param name="damage"></param>
    public void Enable(float damage)
    {
        m_Damage = damage;
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 공격 트리거를 끕니다.
    /// </summary>
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        GetDamageComponent(other, out MonoBehaviour damObject, out Transform damTrans, out IDamage iDamage);
        onTargetTriggered.Invoke(damTrans, damObject, iDamage, m_Damage);
    }
    #endregion
    #region Function
    //Public
    /// <summary>
    /// 공격 트리거를 cast해서 데미지를 입힙니다.
    /// </summary>
    internal void Cast()
    {
        //TODO : 필요해지면 그떄 구현
    }
    
    //Private
    private void GetDamageComponent(Collider other, out MonoBehaviour damObject, out Transform damTrans, out IDamage iDamage)
    {
        //
        damObject = null;
        damTrans = null;
        iDamage = null;

        //EnemyController
        EnemyController enemyController = other.GetComponentInParent<EnemyController>();
        iDamage = enemyController as IDamage;
        if(iDamage != null)
        {
            damObject = enemyController;
            damTrans = enemyController.transform;
            return;
        }

        //AttackableObject
        AttackableObject attackableObj = other.GetComponentInParent<AttackableObject>();
        iDamage = attackableObj as IDamage;
        if (iDamage != null)
        {
            damObject = attackableObj;
            damTrans = attackableObj.transform;
            return;
        }
    }
    #endregion
}
