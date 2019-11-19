using CulterSystem.BaseSystem.DataSystem;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackableObject : MonoBehaviour, IDamage
{
    #region Inspector
    [SerializeField, LabelText("오브젝트 최대 HP")] private int m_MaxHP;
    #endregion
    #region Get,Set
    /// <summary>
    /// 오브젝트 HP
    /// </summary>
    public DataValue<float> HP
    {
        get;
        private set;
    }
    #endregion
    #region Value
    private Vector3 m_KnockBackDir;
    private int m_KnockBackCount;
    #endregion

    #region Event
    //Unity Event
    private void Awake()
    {
        HP = new DataValue<float>(m_MaxHP);
        HP.AddValueChangeEvent(() =>
        {

        }, true);
    }
    private void Update()
    {

    }

    //IDamage Event
    public void OnDamEvent(float d)
    {
    }
    public void OnKnockEvent(Vector3 nor)
    {
    }
    #endregion
    #region Function
    private void Die()
    {

    }
    #endregion
}
