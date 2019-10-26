using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CharacterSytem.Demo
{
    public class CharacterSystem3DDemoAttackAction : CharacterAction
    {
        #region Inspector
        [SerializeField] private CharacterAction m_DefaultAction;
        [SerializeField] private string[] m_AttackAni;
        #endregion
        #region Value
        private bool m_IsAttacking;
        #endregion

        #region Event
        protected override void OnStartAction()
        {
            m_IsAttacking = true;
        }
        protected override CharacterAction OnUpdateAction()
        {
            CharacterSystemDemoUserControl control = CurrentCharacter.CurrentControl as CharacterSystemDemoUserControl;

            if (!m_IsAttacking)
            {
                //공격버튼 눌려있는 경우
                if (control.IsAttack)
                {
                    m_IsAttacking = true;
                    CurrentAni.PlayAnimation(m_AttackAni[Random.Range(0, m_AttackAni.Length)], true);
                }
                //안눌린 경우
                else
                    return m_DefaultAction;
            }

            return this;
        }
        protected override void OnEndAction()
        {
        }
        protected override void OnAniEvent(string aniID, string aniName, string eventName)
        {
            if (aniID == "Attack1" || aniID == "Attack2")
            {
                if (eventName == "complete")
                    m_IsAttacking = false;
            }
        }
        #endregion
    }
}