using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

using static CulterSystem.CommonSystem.CharacterSytem.CharacterEffectList;

namespace CulterSystem.CommonSystem.CharacterSytem
{
    public class Character : MonoBehaviour
    {
        #region Inspector
        [SerializeField, TabGroup("Component"), LabelText("기본 컨트롤")] private CharacterControl m_DefaultControl;
        [SerializeField, TabGroup("Component"), LabelText("캐릭터 물리")] private CharacterPhysic m_CharacterPhysic;
        [SerializeField, TabGroup("Component"), LabelText("캐릭터 애니")] private CharacterAni[] m_CharacterAni;         //해당 갯수만큼 액션 사용 가능
        [SerializeField, TabGroup("Component"), LabelText("기본 액션")] private CharacterAction[] m_DefaultAction;      //생성된 액션의 기본값 (m_CharacterAni 갯수 이상은 무시)
        #endregion
        #region Get,Set
        /// <summary>
        /// 캐릭터 물리 관리 클래스를 가져옵니다.
        /// </summary>
        public CharacterPhysic ChildPhysic
        {
            get
            {
                return m_CharacterPhysic;
            }
        }
        /// <summary>
        /// 현재 해당 캐릭터의 컨트롤을 가져옵니다.
        /// </summary>
        public CharacterControl CurrentControl
        {
            get;
            private set;
        }
        #endregion
        #region Value
        private CharacterEffectList m_EffectList = new CharacterEffectList();
        private CharacterAction[] m_CurrentAction;
        #endregion

        #region Event
        /// <summary>
        /// 초기화
        /// </summary>
        public void Init()
        {
            //기본 초기화
            m_EffectList.Init(this);

            CurrentControl = m_DefaultControl;
            CurrentControl?.Init(this);
            m_CharacterPhysic.Init(this);

            m_CurrentAction = new CharacterAction[m_CharacterAni.Length];
            int layerStart = 0;
            for (int i = 0; i < m_CharacterAni.Length; ++i)
            {
                m_CharacterAni[i].Init(this, layerStart);

                int index = i;
                m_CharacterAni[i].OnAniEvent += (string aniID, string aniName, string eventName) =>
                {
                    m_CurrentAction[index]?.PostAniEvent(aniID, aniName, eventName);
                };
                
                if (i < m_DefaultAction.Length)
                {
                    m_CurrentAction[i] = m_DefaultAction[i];
                    m_CurrentAction[i]?.StartAction(this,  m_CharacterAni[i]);
                }

                layerStart += m_CharacterAni[i].LayerCount;
            }

            //초기화 이벤트 발생
            OnInit();
        }

        //Unity Event
        private void Update()
        {
            //Control 업데이트
            CurrentControl?.UpdateControl();

            //Action 업데이트
            for(int i=0;i<m_CurrentAction.Length;++i)
            {
                if (m_CurrentAction[i] != null)
                {
                    CharacterAction next = m_CurrentAction[i].UpdateAction();
                    if (next != m_CurrentAction[i])
                        SetAction(i, next);
                }
            }

            //EffectList 업데이트
            m_EffectList.UpdateEffect();
            OnUpdateEffect(m_EffectList.GetEffect_All());

            //캐릭터 자체 업데이트
            OnUpdate();
        }

        //Character Event
        /// <summary>
        /// 초기화될 때 호출됩니다.
        /// </summary>
        protected virtual void OnInit()
        {
        }
        /// <summary>
        /// 업데이트될 때 호출됩니다.
        /// </summary>
        protected virtual void OnUpdate()
        {
        }
        /// <summary>
        /// 캐릭터 효과가 추가되었을 떄 호출됩니다.
        /// </summary>
        /// <param name="effect"></param>
        protected virtual void OnAddEffect(CharacterEffect effect)
        {
        }
        /// <summary>
        /// 캐릭터 효과가 업데이트될 때 호출됩니다.
        /// </summary>
        /// <param name="effect"></param>
        protected virtual void OnUpdateEffect(CharacterEffect[] effect)
        {
        }
        /// <summary>
        /// 캐릭터 효과가 제거될 때 호출됩니다.
        /// </summary>
        /// <param name="effect"></param>
        protected virtual void OnRemoveEffect(CharacterEffect effect)
        {
        }
        #endregion
        #region Function
        //Protected - 캐릭터 Effect 관련 (CharacterEffectList에서 플레이어가 사용해야하는 함수가 많지는 않기때문에... 많아지면 직접접근해서 쓰도록 변경)
        /// <summary>
        /// 특정 타입의 캐릭터 효과에 대한 비주얼이펙트를 지정합니다.
        /// 반드시 이미 생성되어있는 비주얼이펙트를 사용해야합니다.
        /// 일단 한번 정한 타입의 비주얼 이펙트를 재교체하는것은 현재 지원하지 않습니다.
        /// </summary>
        /// <param name="type">효과 타입</param>
        /// <param name="vfx">비주얼이펙트</param>
        protected void SetEffectVFX(Type type, CharacterEffectVFX vfx)
        {
            m_EffectList.SetDefaultVFX(type, vfx);
        }
        /// <summary>
        /// 캐릭터 효과를 추가합니다.
        /// </summary>
        /// <param name="effect">추가할 효과</param>
        public void AddEffect(CharacterEffect effect, EffectStateStruct state)
        {
            m_EffectList.AddEffect(effect, state);
            OnAddEffect(effect);
        }
        /// <summary>
        /// 캐릭터 효과를 제거합니다.
        /// </summary>
        /// <param name="effect"></param>
        public void RemoveEffect(CharacterEffect effect)
        {
            m_EffectList.RemoveEffect(effect);
            OnRemoveEffect(effect);
        }
        /// <summary>
        /// 해당 효과의 상태를 가져옵니다.
        /// </summary>
        /// <param name="effect">효과</param>
        /// <returns></returns>
        public EffectStateStruct? GetEffectState(CharacterEffect effect)
        {
            return m_EffectList.GetEffectState(effect);
        }
        /// <summary>
        /// 해당 효과의 상태를 설정합니다.
        /// </summary>
        /// <param name="effect">효과</param>
        /// <param name="newState">변경할 상태</param>
        public void SetEffectState(CharacterEffect effect, EffectStateStruct newState)
        {
            m_EffectList.SetEffectState(effect, newState);
        }

        //Protected - 캐릭터 Action 관련
        /// <summary>
        /// 해당 액션으로 액션을 설정합니다.
        /// </summary>
        /// <param name="action"></param>
        protected void SetAction(int layerIndex, CharacterAction action)
        {
            if (0 <= layerIndex && layerIndex < m_CurrentAction.Length)
            {
                if (m_CurrentAction[layerIndex] != action)
                {
                    m_CurrentAction[layerIndex]?.EndAction();

                    m_CurrentAction[layerIndex] = action;
                    m_CurrentAction[layerIndex].StartAction(this, m_CharacterAni[layerIndex]);
                }
            }
        }
        /// <summary>
        /// 해당 레이어의 액션을 가져옵니다.
        /// </summary>
        /// <param name="layerIndex">레이어</param>
        /// <returns></returns>
        protected CharacterAction GetAction(int layerIndex)
        {
            if (0 <= layerIndex && layerIndex < m_CurrentAction.Length)
                return m_CurrentAction[layerIndex];
            else
                return null;
        }
        #endregion
    }

    /// <summary>
    /// 캐릭터에 적용된 효과 리스트, Character에서 사용되며, 신경안써도 됨
    /// </summary>
    public class CharacterEffectList
    {
        #region Type
        /// <summary>
        /// 각 효과에 따른 비주얼이펙트
        /// </summary>
        protected struct EffectVFXStateStruct
        {
            public CharacterEffectVFX vfx;      //대상 비주얼이펙트

            public int effectCount;             //활성화된 해당 비주얼이펙트를 활성화시키는 효과의 갯수
        }
        /// <summary>
        /// 효과의 상태를 저장하는 구조체
        /// </summary>
        public struct EffectStateStruct
        {
            public float? removeTimer;         //삭제까지 남은 시간

            public EffectStateStruct(float? _removeTimer)
            {
                removeTimer = _removeTimer;
            }

            /// <summary>
            /// 더 강력한쪽으로 교환합니다.
            /// </summary>
            /// <param name="replaceState">교환할것</param>
            public void Replace(EffectStateStruct replaceState)
            {
                //removeTimer
                if (!replaceState.removeTimer.HasValue)
                    removeTimer = replaceState.removeTimer;
                else if(removeTimer.HasValue)
                {
                    if (removeTimer.Value < replaceState.removeTimer.Value)
                        removeTimer = replaceState.removeTimer;
                }
            }
        }
        #endregion

        #region Get,Set
        /// <summary>
        /// 해당 효과리스트를 소유하고 있는 캐릭터
        /// </summary>
        public Character ParentCharacter
        {
            get;
            private set;
        }
        #endregion
        #region Value
        private Dictionary<Type, EffectVFXStateStruct> m_DefaultVFX = new Dictionary<Type, EffectVFXStateStruct>();              //효과 타입로 활성화되는 비주얼이펙트
        private Dictionary<CharacterEffect, EffectStateStruct> m_Effect = new Dictionary<CharacterEffect, EffectStateStruct>(); //현재 활성화된 효과
        #endregion

        #region Event
        /// <summary>
        /// 초기화합니다.
        /// </summary>
        /// <param name="parentCharacter">해당 효과리스트를 소유하고 있는 캐릭터</param>
        public void Init(Character parentCharacter)
        {
            ParentCharacter = parentCharacter; 
        }
        #endregion
        #region Function
        //Public
        /// <summary>
        /// 효과를 추가합니다.
        /// </summary>
        public void AddEffect(CharacterEffect effect, EffectStateStruct state)
        {
            if(m_Effect.TryGetValue(effect, out EffectStateStruct originalState))
            {
                originalState.Replace(state);
                m_Effect[effect] = originalState;
            }
            else
            {
                m_Effect.Add(effect, state);

                //초기화
                effect.Init(this);

                //DefaultVFX에서 효과 갯수를 추가한다.
                LoopDefaultVFX(effect, (Type effectType, EffectVFXStateStruct vfxState) =>
                {
                    vfxState.effectCount++;
                    m_DefaultVFX[effectType] = vfxState;

                    //효과가 추가된 기본 VFX 재생
                    vfxState.vfx.OpenVFX();
                });
            }
        }
        /// <summary>
        /// 효과를 업데이트합니다.
        /// 참고 : Character
        /// </summary>
        public void UpdateEffect()
        {
            CharacterEffect[] effect = GetEffect_All();
            for (int i = 0; i < effect.Length; ++i)
            {
                EffectStateStruct state = m_Effect[effect[i]];

                //removeTimer 처리
                bool isRemove = false;
                if (state.removeTimer.HasValue)
                {
                    state.removeTimer -= Time.deltaTime;
                    if (state.removeTimer <= 0)
                        isRemove = true;
                }

                //CharacterEffect.OnUpdateEffect 호출
                effect[i].OnUpdateEffect();

                //제거해야 할 경우 제거, 아닌경우 업데이트
                if (isRemove)
                    RemoveEffect(effect[i]);
                else
                    m_Effect[effect[i]] = state;
            }
        }
        /// <summary>
        /// 효과를 제거합니다.
        /// </summary>
        public void RemoveEffect(CharacterEffect effect)
        {
            if (m_Effect.Remove(effect))
            {
                //DefaultVFX에서 효과 갯수를 제거한다.
                LoopDefaultVFX(effect, (Type effectType, EffectVFXStateStruct vfxState) =>
                {
                    vfxState.effectCount--;
                    m_DefaultVFX[effectType] = vfxState;

                    //효과로 제거된 기본 VFX 재생 중지
                    if (vfxState.effectCount < 0)
                        vfxState.vfx.CloseVFX(false);
                });
            }
        }
        /// <summary>
        /// 특정 타입의 캐릭터 효과에 대한 비주얼이펙트를 지정합니다.
        /// 반드시 이미 생성되어있는 비주얼이펙트를 사용해야합니다.
        /// 반드시 맨 처음에 등록하고 시작해야합니다. (아직 예외처리같은것은 구현 안함)
        /// </summary>
        /// <param name="type">효과 타입</param>
        /// <param name="vfx">비주얼이펙트</param>
        public void SetDefaultVFX(Type type, CharacterEffectVFX vfx)
        {
            if(!m_DefaultVFX.ContainsKey(type))
            {
                EffectVFXStateStruct effect = m_DefaultVFX[type];
                vfx.gameObject.SetActive(effect.vfx.gameObject.activeSelf);
                effect.vfx.gameObject.SetActive(false);
                effect.vfx = vfx;
                m_DefaultVFX[type] = effect;
            }
            //TODO : 중간중간 비주얼이펙트를 바꿀 수 있도록 하는 기능 추가
        }
        /// <summary>
        /// 현재 적용된 모든 이펙트를 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public CharacterEffect[] GetEffect_All()
        {
            CharacterEffect[] effect = new CharacterEffect[m_Effect.Keys.Count];
            m_Effect.Keys.CopyTo(effect, 0);

            return effect;
        }
        /// <summary>
        /// 해당 효과의 상태를 가져옵니다.
        /// </summary>
        /// <param name="effect">효과</param>
        /// <returns></returns>
        public EffectStateStruct? GetEffectState(CharacterEffect effect)
        {
            if (m_Effect.TryGetValue(effect, out EffectStateStruct state))
                return state;
            else
                return null;
        }
        /// <summary>
        /// 해당 효과의 상태를 설정합니다.
        /// </summary>
        /// <param name="effect">효과</param>
        /// <param name="newState">변경할 상태</param>
        public void SetEffectState(CharacterEffect effect, EffectStateStruct newState)
        {
            if(m_Effect.ContainsKey(effect))
                m_Effect[effect] = newState;
        }

        //Private
        /// <summary>
        /// 해당 이펙트 타입에 해당하는 DefaultVFX를 전부 탐색합니다.
        /// </summary>
        /// <param name="effect"></param>
        private void LoopDefaultVFX(CharacterEffect effect, Action<Type, EffectVFXStateStruct> call)
        {
            Type effectType = effect.GetType();
            while (effectType.BaseType != null)
            {
                if (m_DefaultVFX.TryGetValue(effectType, out EffectVFXStateStruct vfxState))
                {
                    call(effectType, vfxState);
                }
                effectType = effectType.BaseType;
            }
        }
        #endregion
    }

    /// <summary>
    /// 캐릭터에 적용될 수 있는 효과
    /// 상속해서 클래스 타입으로 필터링해서 캐릭터에서 혹은 해당 클래스 자체에서 효과 처리가 일어나도록 하면 된다.
    /// </summary>
    public abstract class CharacterEffect
    {
        #region Get,Set
        /// <summary>
        /// 해당 효과를 소유하고 있는 효과리스트
        /// </summary>
        public CharacterEffectList ParentEffectList
        {
            get;
            private set;
        }
        #endregion

        #region Event
        /// <summary>
        /// 초기화합니다.
        /// </summary>
        /// <param name="parentEffectList">해당 이펙트를 소유하고 있는 이펙트리스트</param>
        public void Init(CharacterEffectList parentEffectList)
        {
            ParentEffectList = parentEffectList;

            OnInit();
        }

        /// <summary>
        /// 초기화시 호출되는 이벤트입니다.
        /// </summary>
        protected virtual void OnInit()
        {
        }
        /// <summary>
        /// 효과 시작시 호출되는 이벤트입니다.
        /// </summary>
        public virtual void OnStartEffect()
        {
        }
        /// <summary>
        /// 효과 진행중 프레임마다 호출되는 이벤트입니다.
        /// </summary>
        public virtual void OnUpdateEffect()
        {
        }
        /// <summary>
        /// 효과 종료시 호출되는 이벤트입니다.
        /// </summary>
        public virtual void OnEndEffect()
        {
        }
        #endregion
    }
}
