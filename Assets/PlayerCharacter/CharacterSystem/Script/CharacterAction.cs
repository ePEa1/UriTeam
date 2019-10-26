using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CharacterSytem
{
    /// <summary>
    /// 캐릭터의 행동을 정의하는 클래스
    /// 상속해서 해당 액션의 실제 행동을 정의한다. 해야할 것은 아래와 같다.
    /// - 애니메이션 재생 (CurrentAni)
    /// - 애니메이션 이벤트 처리 (OnAniEvent)
    /// - 쓸데없는것이 남지 않도록 액션 마무리 (OnEndAction)
    /// - 컨트롤에 따른 처리 (OnUpdateAction에서 CurrentControl 사용)
    /// </summary>
    public abstract class CharacterAction : MonoBehaviour
    {
        #region Inspector
        [SerializeField, TabGroup("Option"), LabelText("기본 애니메이션")] private string m_DefaultAniID;
        #endregion
        #region Get,Set
        /// <summary>
        /// 해당 액션을 사용하고 있는 캐릭터
        /// </summary>
        public Character CurrentCharacter
        {
            get;
            private set;
        }
        /// <summary>
        /// 해당 액션을 사용하고 있는 캐릭터의 애니메이션
        /// (여러개가 있어서 그중에서 해당 액션에서 사용할 수 있는것만)
        /// </summary>
        protected CharacterAni CurrentAni
        {
            get;
            private set;
        }
        #endregion

        #region Event
        //CharacterAction Event
        /// <summary>
        /// 액션이 시작되었을 때 호출됩니다.
        /// </summary>
        protected abstract void OnStartAction();
        /// <summary>
        /// 액션이 업데이트 될 때 호출됩니다.
        /// </summary>
        /// <returns>다음 재생될 액션(기본 : 자기자신 리턴)</returns>
        protected abstract CharacterAction OnUpdateAction();
        /// <summary>
        /// 액션이 끝났을 때 호출됩니다.
        /// </summary>
        protected abstract void OnEndAction();
        /// <summary>
        /// 애니메이션 이벤트가 있을 떄 호출됩니다.
        /// </summary>
        /// <param name="aniID">애니메이션 ID</param>
        /// <param name="aniName">애니메이션 조각 이름</param>
        /// <param name="eventName">이벤트 이름</param>
        protected abstract void OnAniEvent(string aniID, string aniName, string eventName);
        #endregion
        #region Function
        //Internal
        /// <summary>
        /// 액션을 시작합니다.
        /// </summary>
        /// <param name="character">해당 액션을 시작한 캐릭터</param>
        internal void StartAction(Character character, CharacterAni ani)
        {
            //대입
            CurrentCharacter = character;
            CurrentAni = ani;

            //기본 애니메이션 재생
            if (m_DefaultAniID != "")
                CurrentAni.PlayAnimation(m_DefaultAniID);

            //이벤트 호출
            OnStartAction();
        }
        /// <summary>
        /// 액션을 업데이트합니다.
        /// </summary>
        /// <returns>다음 재생될 액션(기본 : 자기자신 리턴)</returns>
        internal CharacterAction UpdateAction()
        {
            return OnUpdateAction();
        }
        /// <summary>
        /// 액션을 끝냅니다.
        /// </summary>
        internal void EndAction()
        {
            CurrentCharacter = null;
            CurrentAni = null;

            //이벤트 호출
            OnEndAction();
        }
        /// <summary>
        /// 애니메이션 이벤트를 호출합니다.
        /// </summary>
        /// <param name="aniID">애니메이션 ID</param>
        /// <param name="aniName">애니메이션 조각 이름</param>
        /// <param name="eventName">이벤트 이름</param>
        internal void PostAniEvent(string aniID, string aniName, string eventName)
        {
            OnAniEvent(aniID, aniName, eventName);
        }
        #endregion
    }
}