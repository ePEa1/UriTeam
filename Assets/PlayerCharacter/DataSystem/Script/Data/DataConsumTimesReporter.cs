using System;

namespace CulterSystem.BaseSystem.DataSystem
{
    /// <summary>
    /// 데이터 소비(-) 횟수를 추적하여 기록하는 클래스
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataConsumTimesReporter<T> : DataReporter<int, T>
        where T : unmanaged, IComparable
    {
        #region Event
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="targetData">추적하여 기록할 타겟</param>
        /// <param name="originalReportData">기본값으로 사용할 기록값</param>
        public DataConsumTimesReporter(Data<T> targetData, int originalReportData) : base(targetData, originalReportData)
        {
        }

        //DataReporter Event
        protected override int OnNeedUpdateReport(T data, T lastedData)
        {
            //전보다 작아졌으면 +1을 한다.
            return (data.CompareTo(lastedData) < 0) ? Value + 1 : Value;
        }
        #endregion
    }
}