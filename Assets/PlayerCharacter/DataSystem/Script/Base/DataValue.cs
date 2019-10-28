using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.BaseSystem.DataSystem
{
    /// <summary>
    /// 데이터
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataValue<T> : Data<T>
    {
        #region Get,Set
        /// <summary>
        /// 실제 데이터의 Getter, Setter
        /// </summary>
        public override T Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
                m_OnValueChanged?.Invoke();
            }
        }
        #endregion

        #region Event
        /// <summary>
        /// 초기화합니다.
        /// </summary>
        /// <param name="value">초기값</param>
        public DataValue(T value) : base(value)
        {
        }
        #endregion
    }
}