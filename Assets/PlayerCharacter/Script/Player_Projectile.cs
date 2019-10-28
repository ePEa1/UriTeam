using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Data;

public class Player_Projectile : MonoBehaviour
{
    #region Value
    private float m_Timer;      //발사후부터의 시간
    private Vector2 m_Vector;   //발사 방향
    #endregion

    #region Event
    /// <summary>
    /// 총알 초기화
    /// </summary>
    /// <param name="vec"></param>
    public void Init(Vector2 vec)
    {
        m_Timer = 0;
        m_Vector = vec;
    }

    //Unity Event
    private void Update()
    {
        //발사 방향으로 슈우우웅ㅇ
        transform.position += new Vector3(m_Vector.x, 0, m_Vector.y) * data.Projectile_Spd * Time.deltaTime;

        //수명이 끝나면 제거
        m_Timer += Time.deltaTime;
        if (data.Projectile_LifeSpan <= m_Timer)
            Destroy(gameObject);
    }
    #endregion
}
