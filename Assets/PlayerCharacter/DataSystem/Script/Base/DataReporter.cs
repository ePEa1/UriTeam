using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.BaseSystem.DataSystem
{
    /// <summary>
    /// 데이터를 추적하여 특정 값을 기록하기 위한 클래스의 기본형
    /// </summary>
    /// <typeparam name="T">기록의 형식</typeparam>
    /// <typeparam name="U">추적할 데이터의 형식</typeparam>
    public abstract class DataReporter<T, U> : Data<T>
        where T : unmanaged
    {
        #region Value
        private U m_LastedTargetValue;                  //이전의 데이터 값
        private Data<U> m_TargetDataWrapper;            //추적하여 기록할 타겟
        #endregion
        #region Get,Set
        public override T Value
        {
            get
            {
                return m_Value;
            }
            set => throw new System.NotImplementedException();
        }
        #endregion

        #region Event
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="targetData">추적하여 기록할 타겟</param>
        /// <param name="originalReportData">기본값으로 사용할 기록값</param>
        public DataReporter(Data<U> targetData, T originalReportData) : base(originalReportData)
        {
            m_Value = originalReportData;
            m_LastedTargetValue = targetData.Value;
            m_TargetDataWrapper = targetData;
            targetData.AddValueChangeEvent(OnTargetDataChanged, true);
        }

        //DataWrapper Event
        private void OnTargetDataChanged()
        {
            //기록을 업데이트한다. 변경되었을 경우 대입 및 이벤트 호출
            T t = OnNeedUpdateReport(m_TargetDataWrapper.Value, m_LastedTargetValue);
            if (!t.Equals(Value))
            {
                m_Value = t;
                m_OnValueChanged?.Invoke();
            }

            //다음 호출을 위해 이전의 데이터 값 업데이트
            m_LastedTargetValue = m_TargetDataWrapper.Value;
        }

        //DataReporter Event
        /// <summary>
        /// 기록을 업데이트합니다. 타겟 데이터 변경시마다 호출됩니다.
        /// </summary>
        /// <param name="data">변경된 데이터</param>
        /// <param name="lastedData">변경 전의 데이터</param>
        /// <returns>변경할 기록</returns>
        protected abstract T OnNeedUpdateReport(U data, U lastedData);
        #endregion
    }
}