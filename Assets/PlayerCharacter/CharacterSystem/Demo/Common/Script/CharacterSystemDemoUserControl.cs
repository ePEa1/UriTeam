using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CharacterSytem.Demo
{
    public class CharacterSystemDemoUserControl : CharacterControl
    {
        #region Get,Set
        /// <summary>
        /// 점프 명령을 했는지
        /// </summary>
        public bool IsJump
        {
            get;
            private set;
        }
        /// <summary>
        /// 이동 방향
        /// </summary>
        public Vector2 MoveVec
        {
            get;
            private set;
        }
        /// <summary>
        /// 공격 명령을 했는지
        /// </summary>
        public bool IsAttack
        {
            get;
            private set;
        }
        #endregion

        #region Event
        public override void OnUpdateControl()
        {
            IsJump = Input.GetKeyDown(KeyCode.Space);

            Vector2 move = Vector2.zero;
            if (Input.GetKey(KeyCode.A))
                move.x = -1;
            else if (Input.GetKey(KeyCode.D))
                move.x = 1;
            if (Input.GetKey(KeyCode.S))
                move.y = -1;
            else if (Input.GetKey(KeyCode.W))
                move.y = 1;
            MoveVec = move.normalized;

            IsAttack = Input.GetMouseButton(0);
        }
        #endregion
    }
}