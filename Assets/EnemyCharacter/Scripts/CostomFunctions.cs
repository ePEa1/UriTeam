using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ePEaCostomFunction
{
    public static class CostomFunctions
    {
        /// <summary>
        /// 최대값 반환
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static float Max(float n1, float n2)
        {
            if (n1 > n2)
                return n1;
            else return n2;
        }

        /// <summary>
        /// 최소값 반환
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static float Min(float n1, float n2)
        {
            if (n1 > n2)
                return n2;
            else return n1;
        }

        /// <summary>
        /// 두 벡터 사이의 쿼터니언 회전값 반환
        /// </summary>
        /// <param name="center"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Quaternion PointDirection(Vector3 center, Vector3 target)
        {
            Vector3 vecDir =(target - center).normalized;

            vecDir.y = 0;

            return Quaternion.LookRotation(vecDir);
        }

        /// <summary>
        /// 두 벡터 사이 노멀값 반환
        /// </summary>
        /// <param name="center"></param>
        /// <param name="target"></param>
        /// <param name="isInY"></param>
        /// <returns></returns>
        public static Vector3 PointNormalize(Vector3 center, Vector3 target, bool isInY)
        {
            Vector3 nor = (target - center).normalized;
            if (!isInY) nor.y = 0;

            return nor;
        }

        public static Vector3 PointNormalize(Vector3 center, Vector3 target)
        {
            Vector3 nor = (target - center).normalized;
            nor.y = 0;
            return nor;
        }
    }
}

