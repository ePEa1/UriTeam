﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CulterSystem.CommonSystem.CharacterSytem;

public class PlayerCharacterControl : CharacterControl
{
    #region Inspector
    [SerializeField] private Camera m_MainCamera;
    #endregion
    #region Get,Set
    /// <summary>
    /// 무기 스위칭 (Q - down)
    /// </summary>
    public bool SwitchWeapon
    {
        get
        {
            return Input.GetKeyDown(KeyCode.Q);
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
            if (Input.GetKey(KeyCode.W))
                move.y = 1.0f;
            else if (Input.GetKey(KeyCode.S))
                move.y = -1.0f;
            if (Input.GetKey(KeyCode.D))
                move.x = 1.0f;
            else if (Input.GetKey(KeyCode.A))
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
            return Input.GetKeyDown(KeyCode.LeftShift);
        }
    }
    /// <summary>
    /// 공격 (마우스왼쪽버튼)
    /// </summary>
    public bool Attack
    {
        get
        {
            return Input.GetMouseButton(0);
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
            return Input.GetKey(KeyCode.Space);
        }
    }
    #endregion

    #region Event
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
