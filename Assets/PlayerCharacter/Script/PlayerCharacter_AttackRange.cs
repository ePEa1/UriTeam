using CulterSystem.CommonSystem.CharacterSytem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Data;

public class PlayerCharacter_AttackRange : PlayerCharacter_ActionBase
{
    #region Inspector
    [Header("Component")]
    [SerializeField] private Transform m_AttackRoot;

    [Header("Prefab")]
    [SerializeField] private GameObject m_ProjectilePrefab;
    #endregion
    #region Value
    private float m_AttackTimer;    //루프마다 진행되는 타이머
    private bool m_IsAttacked;      //이번 루프에서 공격을 했는지
    #endregion

    #region Event
    protected override void OnStartAction()
    {
        //초기값 설정
        m_AttackTimer = 0;
        m_IsAttacked = false;
    }
    protected override CharacterAction OnUpdateAction()
    {
        PlayerCharacter player = CurrentCharacter as PlayerCharacter;
        PlayerCharacterControl control = player.CurrentControl as PlayerCharacterControl;

        //원거리공격은 아무때나 캔슬 가능함!
        CharacterAction action = base.OnUpdateAction();
        if (action != this)
            return action;

        //그리고 마우스 떼면 캔슬함
        if(!control.Attack || !control.Range)
            return player.DefaultAction;

        //총알이 있는 경우만 계속 진행
        if (0 < player.BulletCount.Value)
        {
            //플레이어를 마우스방향을 보게 만들기
            player.SetLookVector(control.AttackDirection, true);

            //실제 발사 처리
            m_AttackTimer += Time.unscaledDeltaTime * player.PlayerTimeScale;
            if(!m_IsAttacked && m_AttackTimer < data.RangedAtk_MsBetweenShot)
            {
                m_IsAttacked = true;
                player.BulletCount.Value -= 1;
                Attack(control.AttackDirection);
            }
        }
        //총알없으면 기본액션으로 돌아감...
        else
            return player.DefaultAction;

        return this;
    }
    protected override void OnEndAction()
    {
        //원거리 공격 끝!!
        CurrentAni.PlayAnimation("Attack_Range_End");
    }
    protected override void OnAniEvent(string aniID, string aniName, string eventName)
    {
        if(aniName == "Attack_Range" && eventName == "complete")
        {
            //루프 관련 값 초기화
            m_AttackTimer = 0;
            m_IsAttacked = false;
        }
    }
    #endregion
    #region Function
    /// <summary>
    /// 발사합니다!
    /// </summary>
    private void Attack(Vector2 vec)
    {
        Instantiate(m_ProjectilePrefab, m_AttackRoot.position, Quaternion.identity).GetComponent<Player_Projectile>().Init(vec);
    }
    #endregion
}
