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
        private Dictionary<string, AnimatorAniDataStruct> m_AniDataDic = new Dictionary<string, AnimatorAniDataStruct>();     //애니메이션 정보들을 사용하기 편하도록 하는 Dictionary
        #endregion

        #region Event
        protected override void OnInit()
        {
            base.OnInit();

            for (int i = 0; i < m_AniData.Length; ++i)
                m_AniDataDic.Add(m_AniData[i].aniID, m_AniData[i]);
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
    }

    //애니메이터 설계시 trigger, bool 값으로만 애니메이션간에 변경을 처리하고
    //int, float값으로 상세 수치(이동방향 등)를 조절하는것을 권장한다.

    //또한, 파라미터를 사용하여
}