using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Data;

public class Player_AttackTrigger : MonoBehaviour
{
    #region Event
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody otherRig = other.attachedRigidbody;
        if(otherRig)
        {
            EnemyController enemy = otherRig.GetComponent<EnemyController>();
            //enemy?.OnDamEvent(data;
        }
    }
    #endregion
    #region Function
    /// <summary>
    /// 공격 트리거를 켭니다.
    /// </summary>
    /// <param name="damage"></param>
    internal void Enable(float damage)
    {

    }
    /// <summary>
    /// 공격 트리거를 끕니다.
    /// </summary>
    internal void Disable()
    {

    }
    /// <summary>
    /// 공격 트리거를 cast해서 데미지를 입힙니다.
    /// </summary>
    internal void Cast()
    {

    }
    #endregion
}
