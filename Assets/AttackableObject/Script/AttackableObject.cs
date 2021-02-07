using CulterSystem.BaseSystem.DataSystem;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Data;
using static GameManager;

public class AttackableObject : MonoBehaviour, IDamage
{
    #region Inspector
    [SerializeField] private Rigidbody m_Rigidbody;
    [SerializeField, LabelText("오브젝트 ID")] private string m_ID;
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
    private void Start()
    {
        ObjectTableStruct objectTable = data.GetObjectTable(m_ID);

        HP = new DataValue<float>((float)objectTable.HP);
    }
    private void Update()
    {
        if (!gameManager.IsTimeStopped)
        {
            if(0 < m_KnockBackCount)
            {
                m_Rigidbody.velocity = m_KnockBackDir * data.GetKnockForce(m_KnockBackCount);
                m_KnockBackCount = 0;
            }

            if (HP.Value <= 0 && m_Rigidbody.velocity.sqrMagnitude < 0.01f)
            {
                Die();
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        AttackTriggerUtil.GetDamageComponent(collision.collider, out MonoBehaviour damObject, out Transform damTrans, out IDamage iDamage);
        if (iDamage != null)
        {
            ObjectTableStruct objectTable = data.GetObjectTable(m_ID);
            iDamage.OnDamEvent(objectTable.Damage, new Vector3(0,0,0));
            Die();
        }
    }

    //IDamage Event
    public void OnDamEvent(float d)
    {
        HP.Value -= d;
    }
    public void OnKnockEvent(Vector3 nor)
    {
        ++m_KnockBackCount;
        m_KnockBackDir += nor / m_KnockBackCount;
        m_KnockBackDir.Normalize();
    }
    #endregion
    #region Function
    private void Die()
    {
        Destroy(gameObject);
    }

    public void OnDamEvent(int atkNum, Vector3 nor)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
