using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CharacterSytem
{
    public class CharacterAniAnimator : CharacterAni
    {
        #region Type
        /// <summary>
        /// 애니메이터의 Bool값 변경값을 저장하기 위한 구조체
        /// </summary>
        [System.Serializable] public struct AnimatorBoolStruct
        {
            public string id;
            public bool value;
        }
        /// <summary>
        /// 메카님 파라미터 변경 정보를 저장하는 구조체
        /// </summary>
        [System.Serializable] public struct AnimatorAniDataStruct
        {
            public string aniID;                        //해당 애니의 ID

            public bool isUseParam;                     //파라미터 사용 여부

            [HideIf("isUseParam")] public string[] aniName;                    //(파라미터 미사용시) 해당 애니의 실제 애니메이션 이름

            [ShowIf("isUseParam")] public string[] triggerParamID;             //(파라미터 사용시) 활성화할 트리거
            [ShowIf("isUseParam")] public AnimatorBoolStruct[] boolParam;      //(파라미터 사용시) 값을 설정할 Bool
        }
        #endregion

        #region Inspector
        [SerializeField, TabGroup("Component"), LabelText("Animator")] private Animator m_Animator;
        [SerializeField, TabGroup("Option"), LabelText("애니메이션 리스트")] private AnimatorAniDataStruct[] m_AniData;    //애니메이션 정보
        #endregion
        #region Value
        private Dictionary<string, AnimatorAniDataStruct> m_AniDataDic = new Dictionary<string, AnimatorAniDataStruct>();   //애니메이션 정보들을 사용하기 편하도록 하는 Dictionary
        private string[] m_ClipName;                                                                                        //현재 재생중인 애니메이션 클립 이름들
        private float[] m_ClipTimer;                                                                                        //현재 재생중인 애니메이션 클립의 최근 시간
        #endregion

        #region Event
        //Unity Event
        private void LateUpdate()
        {
            for (int i = 0; i < LayerCount; ++i)
            {
                int layerIndex = LayerStart + i;

                //기존에 기록해놓은 클립과 다른 클립이 재생되고 있는지로 애니메이션 변경 감지
                AnimatorClipInfo[] clipInfo = m_Animator.GetCurrentAnimatorClipInfo(layerIndex);
                if (0 < clipInfo.Length)
                {
                    if(m_ClipName[i] != clipInfo[0].clip.name)
                    {
                        if (m_ClipName[i] != null)
                            PostAniEvent(CurrentAniID, m_ClipName[i], "end");

                        m_ClipName[i] = clipInfo[0].clip.name;
                        m_ClipTimer[i] = 0;
                        PostAniEvent(CurrentAniID, m_ClipName[i], "start");
                    }
                }

                //재생시간 초기화로 애니메이션 루프 완료 감지
                AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(layerIndex);
                float normalizedTime = stateInfo.normalizedTime % 1.0f;
                if (normalizedTime < m_ClipTimer[i])
                    PostAniEvent(CurrentAniID, m_ClipName[i], "complete");
                m_ClipTimer[i] = normalizedTime;
            }
        }

        //CharacterAni Event
        protected override void OnInit()
        {
            base.OnInit();

            for (int i = 0; i < m_AniData.Length; ++i)
                m_AniDataDic.Add(m_AniData[i].aniID, m_AniData[i]);

            m_ClipName = new string[LayerCount];
            m_ClipTimer = new float[LayerCount];
        }
        protected override void OnNeedPlayAnimation(string aniID)
        {
            if (m_AniDataDic.TryGetValue(aniID, out AnimatorAniDataStruct aniData))
            {
                //파라미터 사용시 파라미터값 변경
                if (aniData.isUseParam)
                {
                    //Trigger
                    for (int i = 0; i < aniData.triggerParamID.Length; ++i)
                        m_Animator.SetTrigger(aniData.triggerParamID[i]);

                    //Bool
                    for (int i = 0; i < aniData.boolParam.Length; ++i)
                        m_Animator.SetBool(aniData.boolParam[i].id, aniData.boolParam[i].value);


                }
                //파라미터 미사용시 이름으로 즉시 재생
                else
                {
                    for (int i = 0; i < LayerCount; ++i)
                    {
                        if (i < aniData.aniName.Length)
                            m_Animator.Play(aniData.aniName[i], LayerStart + i);
                    }
                }
            }
            else
                Debug.LogError($"{aniID} not contains");
        }
        #endregion
        #region Function
        /// <summary>
        /// 이벤트를 호출합니다. 애니메이터에서 쓰라고 만들었습니다.
        /// </summary>
        /// <param name="eventName">이벤트명</param>
        public void PostEvent(string eventName)
        {
            PostAniEvent(CurrentAniID, "", eventName);
        }
        #endregion
    }

    //애니메이터 설계시 trigger, bool 값으로만 애니메이션간에 변경을 처리하고
    //int, float값으로 상세 수치(이동방향 등)를 조절하는것을 권장한다.

    //start : 애니메이션 시작 시점에서 호출됨
    //complete : 애니메이션이 한번 재생이 완료되었을 때 (루프일 경우만 루프마다 호출됨)
    //end : 애니메이션이 완전히 완료되었을 때 호출됨 (다음 애니메이션으로 넘어갈때)
    //모든 애니메이션 이벤트의 타이밍이 부정확할 수 있다. (이전 애니메이션 조각의 이벤트가 호출될 수 있음)
    //그리고 이번 애니메이션과 다음 애니메이션의 클립명이 같으면 end와 start가 호출안된다 끔찍하다.
}