using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace CulterSystem.CommonSystem.CameraSystem
{
    public abstract class CameraChangeRotAni : MonoBehaviour
    {
        #region Inspector
        [SerializeField, TabGroup("Option"), LabelText("멀어질수록 느리게")] private bool m_IsFarSlow;
        [SerializeField, TabGroup("Option"), ShowIf("m_IsFarSlow"), LabelText("변화량 기준치")] private float m_FarSlowAngle;
        [SerializeField, TabGroup("Option"), Range(0.1f, 50.0f), HideIf("m_IsFarSlow"), LabelText("재생 속도")] private float m_AniSpeed = 3.0f;
        #endregion
        #region Get,Set
        protected Quaternion startRot
        {
            get;
            private set;
        }
        protected Quaternion targetRot
        {
            get;
            private set;
        }
        #endregion

        #region Event
        /// <summary>
        /// 카메라 회전이 시작되는 시점에 호출됩니다.
        /// </summary>
        protected abstract void OnChangeRotStart();
        /// <summary>
        /// 카메라 회전 업데이트시에 호출됩니다.
        /// </summary>
        /// <param name="normalizedTime">0.0f ~ 1.0f 까지의 normalize된 애니메이션 타임</param>
        /// <returns>이번 프레임의 카메라 줌 사이즈</returns>
        protected abstract Quaternion OnChangeRotUpdate(float normalizedTime, Quaternion _startRot, Quaternion _targetRot);
        /// <summary>
        /// 카메라 회전이 완료되는 시점에 호출됩니다.
        /// </summary>
        protected abstract void OnChangeRotEnd();
        #endregion
        #region Function
        //Public Virtual
        /// <summary>
        /// 회전 시작값 및 목표에 따른 카메라 회전 애니메이션의 속도를 가져옵니다.
        /// </summary>
        /// <param name="startRot">시작지점의 각도</param>
        /// <param name="targetRot">목표 각도</param>
        /// <returns></returns>
        public virtual float GetAnimationSpeed(Quaternion startRot, Quaternion targetRot)
        {
            if (m_IsFarSlow)
            {
                float angleDist = Quaternion.Angle(startRot, targetRot);
                float speedFactor = (angleDist == 0) ? float.MaxValue : (m_FarSlowAngle / angleDist);

                return speedFactor;
            }
            else
                return m_AniSpeed;
        }

        //Public
        /// <summary>
        /// 각도 변경을 시작합니다.
        /// </summary>
        /// <param name="_startRot">시작시점의 각도</param>
        /// <param name="_targetRot">목표 각도</param>
        public void ChangeRotStart(Quaternion _startRot, Quaternion _targetRot)
        {
            startRot = _startRot;
            targetRot = _targetRot;

            OnChangeRotStart();
        }
        /// <summary>
        /// 각도변경을 업데이트합니다.
        /// </summary>
        /// <param name="normalizedTime">normalize된 애니메이션 시간</param>
        public Quaternion ChangeRotUpdate(float normalizedTime)
        {
            return OnChangeRotUpdate(normalizedTime, startRot, targetRot);
        }
        /// <summary>
        /// 각도변경을 끝냅니다.
        /// </summary>
        public void ChangeRotEnd()
        {
            OnChangeRotEnd();
        }
        #endregion
        #region Event - Editor
#if UNITY_EDITOR
        protected virtual void Reset()
        {
            CameraChangeRotAniInitContext();
        }
#endif
        #endregion
        #region Function - Editor
#if UNITY_EDITOR
        [ContextMenu("CameraChangeRotAni Init")]
        public virtual void CameraChangeRotAniInitContext()
        {
            //자신 또는 상위에 있어야 하는 컴포넌트 추가
            GetComponentInParent<CameraManager>()?.CameraManagerInitContext();
        }
#endif
        #endregion
    }
}