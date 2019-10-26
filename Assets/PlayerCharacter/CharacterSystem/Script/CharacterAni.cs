using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace CulterSystem.CommonSystem.CharacterSytem
{
    public abstract class CharacterAni : MonoBehaviour
    {
        #region Inspector
        [SerializeField, TabGroup("Option"), LabelText("레이어 크기")] private int m_LayerCount = 1;
        #endregion
        #region Get,Set
        /// <summary>
        /// 해당 애니메이션의 부모 캐릭터
        /// </summary>
        public Character ParentCharacter
        {
            get;
            private set;
        }
        /// <summary>
        /// 애니메이션 레이어 시작지점
        /// </summary>
        public int LayerStart
        {
            get;
            private set;
        }
        /// <summary>
        /// 애니메이션시 사용할 수 있는 레이어의 갯수
        /// </summary>
        public int LayerCount
        {
            get
            {
                return m_LayerCount;
            }
        }
        /// <summary>
        /// 현재 재생중인 애니메이션 ID
        /// </summary>
        public string CurrentAniID
        {
            get;
            private set;
        }

        //Action
        /// <summary>
        /// 애니메이션 이벤트 발생시 호출되는 이벤트
        /// (애니메이션 ID, 애니메이션 조각 이름(기본적으로 지원하지 않음, 방식에 따라 지원할 수도 있음), 이벤트 이름)
        /// </summary>
        public Action<string, string, string> OnAniEvent;
        #endregion

        #region Event
        /// <summary>
        /// 초기화합니다.
        /// </summary>
        /// <param name="parentCharacter">해당 애니메이션의 부모 캐릭터</param>
        public void Init(Character parentCharacter, int layerStart)
        {
            ParentCharacter = parentCharacter;
            CurrentAniID = "";
            LayerStart = layerStart;

            OnInit();
        }

        //CharacterAni
        /// <summary>
        /// 초기화 시 호출됩니다.
        /// </summary>
        protected virtual void OnInit()
        {
        }
        /// <summary>
        /// 애니메이션 재생 요청시 호출됩니다.
        /// 여기에서 애니메이션을 재생해야하며, 애니메이션의 속도 및 특수한 기능들도 리셋해줘야합니다.
        /// </summary>
        /// <param name="aniID">애니메이션 ID</param>
        protected abstract void OnNeedPlayAnimation(string aniID);
        #endregion
        #region Function
        //Public
        /// <summary>
        /// 해당 ID의 애니메이션을 재생합니다.
        /// </summary>
        /// <param name="aniID">애니메이션 ID</param>
        /// <param name="isForce">이미 해당ID로 재생중이어도 재생할것인지</param>
        public void PlayAnimation(string aniID, bool isForce = false)
        {
            if (aniID == CurrentAniID && !isForce)
                return;

            //실제 재생처리
            OnNeedPlayAnimation(aniID);
            CurrentAniID = aniID;
        }

        //Protected
        /// <summary>
        /// 애니메이션 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="layerIndex">레이어 인덱스</param>
        /// <param name="aniName">이벤트를 발생시킨 애니메이션 조각 이름 (지원안할경우 ""으로 처리)</param>
        /// <param name="eventName">이벤트 이름</param>
        protected void PostAniEvent(string aniID, string aniName, string eventName)
        {
            OnAniEvent?.Invoke(aniID, aniName, eventName);
        }
        #endregion
    }

    //참고 : 이미 다른애니메이션으로 교체한 애니메이션의 이벤트가 발생할 수도 있는데 일단은 신경 안썼음
}