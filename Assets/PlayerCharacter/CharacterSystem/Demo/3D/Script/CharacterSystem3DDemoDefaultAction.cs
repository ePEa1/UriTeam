using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CharacterSytem.Demo
{
    public class CharacterSystem3DDemoDefaultAction : CharacterAction
    {
        #region Inspector
        [SerializeField] private CharacterAction m_AttackAction;
        #endregion
        private bool isJumped;

        #region Event
        protected override void OnStartAction()
        {
        }
        protected override CharacterAction OnUpdateAction()
        {
            CharacterSystemDemoUserControl control = CurrentCharacter.CurrentControl as CharacterSystemDemoUserControl;
            CharacterPhysic3D physic = CurrentCharacter.ChildPhysic as CharacterPhysic3D;

            //공격시 공격액션으로 넘어감
            //if (physic.FlyState == CharacterPhysic2DSide.FlyStateEnum.Ground && control.IsAttack)
            //    return m_AttackAction;

            //행동 업데이트
            if (control.IsJump)
            {
                physic.Jump();
                CurrentAni.PlayAnimation("Jump");
                isJumped = true;
            }
            else if (0.1f < control.MoveVec.magnitude)
            {
                if (isJumped)
                    isJumped = false;

                physic.Move(control.MoveVec);

                if (physic.FlyState == CharacterPhysic.FlyStateEnum.Ground)
                    CurrentAni.PlayAnimation("Move");
            }
            else
            {
                if (isJumped)
                {
                    Debug.Log("asdf");
                    isJumped = false;
                }

                if (physic.FlyState == CharacterPhysic.FlyStateEnum.Ground)
                    CurrentAni.PlayAnimation("Idle");
            }

            return this;
        }
        protected override void OnEndAction()
        {
        }
        protected override void OnAniEvent(string aniID, string aniName, string eventName)
        {
        }
        #endregion
    }
}