using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using static Data;
using static GameManager;

public class AttackTriggerUtil
{
    //Public
    public static void GetDamageComponent(Collider other, out MonoBehaviour damObject, out Transform damTrans, out IDamage iDamage)
    {
        //
        damObject = null;
        damTrans = null;
        iDamage = null;

        //EnemyController
        EnemyController enemyController = other.GetComponentInParent<EnemyController>();
        iDamage = enemyController as IDamage;
        if (iDamage != null)
        {
            damObject = enemyController;
            damTrans = enemyController.transform;
            return;
        }
    }
}

public class Player_AttackTrigger : MonoBehaviour
{
    int atkIndex = 0;

    #region Get,Set
    public Action<Transform, MonoBehaviour, IDamage, int> onTargetTriggered;
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

    public void SetAtkIndex(int ind)
    {
        atkIndex = ind;
    }

    private void OnTriggerEnter(Collider other)
    {
        AttackTriggerUtil.GetDamageComponent(other, out MonoBehaviour damObject, out Transform damTrans, out IDamage iDamage);
        onTargetTriggered.Invoke(damTrans, damObject, iDamage, atkIndex);
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
    #endregion
}
