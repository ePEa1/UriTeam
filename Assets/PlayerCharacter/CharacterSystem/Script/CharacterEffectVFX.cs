using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CharacterSytem
{
    /// <summary>
    /// 캐릭터의 효과에 따른 비주얼이펙트입니다.
    /// </summary>
    public class CharacterEffectVFX : MonoBehaviour
    {
        #region Inspector
        [SerializeField, TabGroup("Component"), LabelText("AudioSource")] private AudioSource m_AudioSource;
        [SerializeField, TabGroup("Effect"), LabelText("파티클 이펙트")] private ParticleSystem m_Particle;
        [SerializeField, TabGroup("Effect"), LabelText("기본 지속 사운드")] private AudioClip m_DefaultLoopSound;
        [SerializeField, TabGroup("Option"), LabelText("사운드 종료 시간")] private float m_SoundOffTimer;
        [SerializeField, TabGroup("Option"), LabelText("사운드 종료 커브")] private AnimationCurve m_SoundOffCurve;
        #endregion
        #region Value
        private bool m_IsOpened;        //재생중인 상태인지
        private bool m_IsClosing;       //종료중인지 여부
        private bool m_IsDestroy;       //완벽하게 종료된 후에 아얘 Destroy시킬지
        private Coroutine m_CompleteCoroutine;  //완벽하게 종료하기 위한 코루틴
        #endregion

        #region Event
        //CharacterEffectVFX Event
        /// <summary>
        /// 비주얼이펙트를 시작할 시 호출되는 이벤트입니다.
        /// </summary>
        protected virtual void OnOpenVFX()
        {
        }
        /// <summary>
        /// 비주얼이펙트 재생을 종료시킬 시 호출되는 이벤트입니다.
        /// </summary>
        protected virtual void OnCloseVFX()
        {
        }
        /// <summary>
        /// 비주얼이펙트 종료가 취소되었을 시 호출되는 이벤트입니다.
        /// </summary>
        protected virtual void OnCloseVFXCancel()
        {
        }
        #endregion
        #region Function
        //Public
        /// <summary>
        /// 비주얼이펙트 재생을 시작합니다.
        /// </summary>
        /// <param name="isDestroy">재생 완료 후 삭제할지, false일 경우 SetActive(false)만 실행한다.</param>
        /// <param name="duration">이펙트 길이</param>
        public void OpenVFX()
        {
            //이미 재생중일때는 무시
            if (m_IsOpened)
                return;

            //만약 종료중이었다면 캔슬시킨다.
            if(m_IsClosing)
            {
                if (m_CompleteCoroutine != null)
                    StopCoroutine(m_CompleteCoroutine);

                OnCloseVFXCancel();
            }

            //켜기
            gameObject.SetActive(true);

            //파티클 이펙트
            m_Particle?.Play();
            
            //기본 지속 사운드
            if(m_AudioSource && m_DefaultLoopSound)
            {
                m_AudioSource.clip = m_DefaultLoopSound;
                m_AudioSource.Play();
            }

            //이벤트
            OnOpenVFX();
        }
        /// <summary>
        /// 비주얼 이펙트 재생을 종료합니다.
        /// </summary>
        /// <param name="isDestory">완벽하게 종료된 후에 아얘 Destroy시킬지</param>
        public void CloseVFX(bool isDestory)
        {
            m_IsClosing = true;
            m_IsDestroy = isDestory;

            if (m_CompleteCoroutine == null)
                m_CompleteCoroutine = StartCoroutine(CompleteCoroutine());

            OnCloseVFX();
        }

        //Protected
        /// <summary>
        /// 가능할 경우 비주얼이펙트를 완전히 끝냅니다.
        /// </summary>
        protected void CompleteVFX()
        {
            m_IsOpened = true;

            //끄거나 삭제
            if (m_IsDestroy)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }

        //Protected Virtual
        /// <summary>
        /// 비주얼이펙트를 완전히 끝낼 수 있는지 여부를 가져옵니다.
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetCompleteEnable()
        {
            return (m_CompleteCoroutine == null);
        }

        //Private Coroutine
        /// <summary>
        /// 비주얼이펙트를 완전히 끝내기 위한 코루틴
        /// </summary>
        /// <returns></returns>
        private IEnumerator CompleteCoroutine()
        {
            while (true)
            {
                if (GetCompleteEnable())
                    CompleteVFX();
            }

        }
        #endregion
    }
}