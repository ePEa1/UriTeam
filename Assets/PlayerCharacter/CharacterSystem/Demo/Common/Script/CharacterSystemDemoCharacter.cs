using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CharacterSytem.Demo
{
    public class CharacterSystemDemoCharacter : Character
    {
        #region Event
        protected override void OnUpdateEffect(CharacterEffect[] effect)
        {
            base.OnUpdateEffect(effect);

            //효과들 활성화 정보
            bool isStern = false;       //스턴 활성화 여부

            //현재 적용된 효과를 순회한다.
            for (int i = 0; i < effect.Length; ++i)
            {
                if (effect[i] as CharacterStunEffect != null)
                    isStern = true;
            }

            //효과들에 따른 실제 처리
            if (isStern)
                Debug.Log("스턴상태");
        }
        #endregion
        #region Function
        /// <summary>
        /// 독데미지를 입습니다.
        /// </summary>
        /// <param name="damage">데미지 양</param>
        public void PoisonDamage(int damage)
        {
            Debug.Log($"{damage}의 독뎀을 입었습니다.");
        }
        #endregion
    }

    /// <summary>
    /// 독데미지 - 해당 클래스에서 실제 효과 처리
    /// </summary>
    public class CharacterPoisonEffect : CharacterEffect
    {
        #region Get,Set
        /// <summary>
        /// 초당 독데미지
        /// </summary>
        public int DamagePerSec
        {
            get;
            private set;
        }
        #endregion
        #region Value
        private float m_PoisonTimer;        //독뎀 타이머
        #endregion

        #region Event
        /// <summary>
        /// 이펙트 생성
        /// </summary>
        /// <param name="damagePerSec">해당 효과에 걸렸을 때 1초당 입는 데미지</param>
        public CharacterPoisonEffect(int damagePerSec)
        {
            DamagePerSec = damagePerSec;
        }

        public override void OnUpdateEffect()
        {
            base.OnUpdateEffect();

            //1초마다 데미지
            m_PoisonTimer += Time.deltaTime;
            if(1.0f <= m_PoisonTimer)
            {
                m_PoisonTimer = 0;
                CharacterSystemDemoCharacter character = ParentEffectList.ParentCharacter as CharacterSystemDemoCharacter;
                character?.PoisonDamage(DamagePerSec);
            }
        }
        #endregion
    }
    /// <summary>
    /// 스턴 - 캐릭터클래스에서 실제 효과 처리
    /// </summary>
    public class CharacterStunEffect : CharacterEffect
    {
    }
}