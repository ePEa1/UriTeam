using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CulterSystem.CommonSystem.CharacterSytem;

using static Data;

public class PlayerCharacterControl : CharacterControl
{
    #region Get,Set
    /// <summary>
    /// 무기 스위칭 (Q - down)
    /// </summary>
    public bool SwitchWeapon
    {
        get
        {
            return false;
            //return Input.GetKeyDown(data.Key_SwitchWeapon);
        }
    }
    /// <summary>
    /// 이동 (WASD)
    /// </summary>
    public Vector2 Move
    {
        get
        {
            Vector2 move = Vector2.zero;
            if (Input.GetKey(data.Key_MoveForward))
                move.y = 1.0f;
            else if (Input.GetKey(data.Key_MoveBack))
                move.y = -1.0f;
            if (Input.GetKey(data.Key_MoveRight))
                move.x = 1.0f;
            else if (Input.GetKey(data.Key_MoveLeft))
                move.x = -1.0f;

            return move.normalized;
        }
    }
    /// <summary>
    /// 대쉬 (Shift - down)
    /// </summary>
    public bool Dash
    {
        get
        {
            return Input.GetKeyDown(data.Key_Dash);
        }
    }
    /// <summary>
    /// 공격 (마우스왼쪽버튼)
    /// </summary>
    public bool Attack
    {
        get
        {
            return Input.GetKey(data.Key_Attack);
        }
    }
    /// <summary>
    /// 원거리 (마우스 오른쪽버튼)
    /// </summary>
    public bool Range
    {
        get
        {
            return false;
            //return Input.GetKey(data.Key_Range);
        }
    }
    /// <summary>
    /// 공격 방향 (플레이어에 상대적인 커서 위치)
    /// </summary>
    public Vector2 AttackDirection
    {
        get;
        private set;
    }
    /// <summary>
    /// 시간정지 (Space)
    /// </summary>
    public bool TimeStop
    {
        get
        {
            //return Input.GetKey(data.Key_TimeStop);
            return false;
        }
    }
    #endregion
    #region Value
    private Camera m_MainCamera;
    #endregion

    #region Event
    /// <summary>
    /// 초기화합니다.
    /// </summary>
    /// <param name="mainCamera"></param>
    public void Init(Camera mainCamera)
    {
        m_MainCamera = mainCamera;
    }

    public override void OnUpdateControl()
    {
        //AttackDirection
        PlayerCharacter player = CurrentCharacter as PlayerCharacter;
        if (player)
        {
            Ray ray = m_MainCamera.ScreenPointToRay(Input.mousePosition);
            Plane characterPlane = new Plane(Vector3.up, player.BottomWorldPos);
            if(characterPlane.Raycast(ray, out float dist))
            {
                Vector3 mouseVec = ray.GetPoint(dist) - player.BottomWorldPos;
                AttackDirection = new Vector2(mouseVec.x, mouseVec.z);
                AttackDirection.Normalize();
            }
        }
    }
    #endregion
}
