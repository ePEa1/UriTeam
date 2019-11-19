using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Data;
using static GameManager;

public class Player_AttackTrigger : MonoBehaviour
{
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

    }
    /// <summary>
    /// 공격 트리거를 끕니다.
    /// </summary>
    public void Disable()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponentInParent<EnemyController>();
        enemy?.OnDamEvent(m_Damage);
        //if (gameManager.IsTimeStopped)
            //enemy?.OnKnockEvent((enemy.transform.position - transform.position).normalized);
    }
    #endregion
    #region Function
    /// <summary>
    /// 공격 트리거를 cast해서 데미지를 입힙니다.
    /// </summary>
    internal void Cast()
    {

    }
    #endregion
}
