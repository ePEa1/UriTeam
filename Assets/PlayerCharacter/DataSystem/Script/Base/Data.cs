using System;

namespace CulterSystem.BaseSystem.DataSystem
{
    /// <summary>
    /// 데이터 추적을 위한 기본 기능이 들어있는 클래스
    /// </summary>
    /// <typeparam name="T">데이터 타입</typeparam>
    public abstract class Data<T>
    {
        #region Get,Set
        /// <summary>
        /// 해당 데이터입니다.
        /// 참고 : get또는 set을 실제로 구현하지 않는 타입이 있을 수 있음
        /// </summary>
        public abstract T Value
        {
            get;
            set;
        }
        /// <summary>
        /// 해당 데이터가 변경된 적이 있는지 여부를 나타내는 Flag
        /// </summary>
        public bool IsChanged
        {
            get;
            private set;
        }
        #endregion
        #region Value
        protected T m_Value;                        //실제 데이터
        protected Action m_OnValueChanged;       //실제 데이터 변경시의 이벤트 델리게이트
        #endregion

        #region Event
        /// <summary>
        /// 초기화합니다.
        /// </summary>
        /// <param name="value">초기값</param>
        public Data(T value)
        {
            m_Value = value;
        }
        #endregion
        #region Function
        //Public
        /// <summary>
        /// 변수를 다양한 옵션으로 설정합니다.
        /// </summary>
        /// <param name="callValueChanged">변수 변경 이벤트를 호출할지</param>
        public void SetValue(T value, bool callValueChanged)
        {
            if (!callValueChanged)
                m_Value = value;
            else
                Value = value;
        }
        /// <summary>
        /// 변수 변경시 호출될 이벤트를 추가합니다.
        /// </summary>
        /// <param name="action">호출될 이벤트</param>
        /// <param name="isCallNow">해당 이벤트를 즉시 한번 수행할지 여부</param>
        public void AddValueChangeEvent(Action action, bool isCallNow = true)
        {
            m_OnValueChanged += action;

            if (isCallNow)
                action?.Invoke();
        }
        /// <summary>
        /// 변수 변경시 호출되던 이벤트를 제거합니다.
        /// </summary>
        /// <param name="action"></param>
        public void RemoveValueChangeEvent(Action action)
        {
            m_OnValueChanged -= action;
        }
        /// <summary>
        /// IsChanged Flag를 false로 초기화합니다.
        /// </summary>
        public void ClearChangedFlag()
        {
            IsChanged = false;
        }

        //Public Override
        public override string ToString()
        {
            return m_Value.ToString();
        }
        #endregion
    }
}