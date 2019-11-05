using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace CulterSystem.CommonSystem.CameraSystem
{
    public abstract class CameraChangeZoomAni : MonoBehaviour
    {
        #region Inspector
        [SerializeField, TabGroup("Option"), LabelText("멀어질수록 느리게")] private bool m_IsFarSlow;
        [SerializeField, TabGroup("Option"), ShowIf("m_IsFarSlow"), LabelText("변화량 기준치")] private float m_FarSlowZoom;
        [SerializeField, TabGroup("Option"), Range(0.1f, 50.0f), HideIf("m_IsFarSlow"), LabelText("재생 속도")] private float m_AniSpeed = 3.0f;
        #endregion
        #region Get,Set
        protected float startZoom
        {
            get;
            private set;
        }
        protected float targetZoom
        {
            get;
            private set;
        }
        protected bool isOrthographic
        {
            get;
            private set;
        }
        #endregion

        #region Event
        /// <summary>
        /// 카메라 줌 사이즈 변경이 시작되는 시점에 호출됩니다.
        /// </summary>
        protected abstract void OnChangeZoomStart();
        /// <summary>
        /// 카메라 줌 사이즈 변경 업데이트시에 호출됩니다.
        /// </summary>
        /// <param name="normalizedTime">0.0f ~ 1.0f 까지의 normalize된 애니메이션 타임</param>
        /// <param name="startZoom">현재 시점의 줌 사이즈</param>
        /// <param name="targetZoom">줌 사이즈 타겟</param>
        /// <param name="isOrthographic">투영 방식</param>
        /// <returns>이번 프레임의 카메라 줌 사이즈</returns>
        protected abstract float OnChangeZoomUpdate(float normalizedTime, float _startZoom, float _targetZoom, bool _isOrthographic);
        /// <summary>
        /// 카메라 줌 사이즈 변경이 완료되는 시점에 호출됩니다.
        /// </summary>
        protected abstract void OnChangeZoomEnd();
        #endregion
        #region Function
        //Public Virtual
        /// <summary>
        /// 줌 시작지점 및 목표에 따른 카메라 줌 애니메이션의 속도를 가져옵니다.
        /// </summary>
        /// <param name="startZoom">줌 시작지점</param>
        /// <param name="targetZoom">줌 목표점</param>
        /// <returns></returns>
        public virtual float GetAnimationSpeed(float startZoom, float targetZoom)
        {
            if (m_IsFarSlow)
            {
                float distance = Mathf.Abs(startZoom - targetZoom);
                float speedFactor = (distance == 0) ? float.MaxValue : (m_FarSlowZoom / distance);

                return speedFactor;
            }
            else
                return m_AniSpeed;
        }

        //Public
        /// <summary>
        /// 줌을 시작합니다.
        /// </summary>
        /// <param name="_startZoom">시작시점의 줌</param>
        /// <param name="_targetZoom">목표 줌</param>
        /// <param name="_isOrthographic">투영 방식</param>
        public void ChangeZoomStart(float _startZoom, float _targetZoom, bool _isOrthographic)
        {
            startZoom = _startZoom;
            targetZoom = _targetZoom;
            isOrthographic = _isOrthographic;

            OnChangeZoomStart();
        }
        /// <summary>
        /// 줌을 업데이트합니다.
        /// </summary>
        /// <param name="normalizedTime">normalize된 애니메이션 시간</param>
        public float ChangeZoomUpdate(float normalizedTime)
        {
            return OnChangeZoomUpdate(normalizedTime, startZoom, targetZoom, isOrthographic);
        }
        /// <summary>
        /// 줌을 끝냅니다.
        /// </summary>
        public void ChangeZoomEnd()
        {
            OnChangeZoomEnd();
        }
        #endregion
        #region Event - Editor
#if UNITY_EDITOR
        protected virtual void Reset()
        {
            CameraChangeZoomAniInitContext();
        }
#endif
        #endregion
        #region Function - Editor
#if UNITY_EDITOR
        [ContextMenu("CameraChangeZoomAni Init")]
        public virtual void CameraChangeZoomAniInitContext()
        {
            //자신 또는 상위에 있어야 하는 컴포넌트 추가
            GetComponentInParent<CameraManager>()?.CameraManagerInitContext();
        }
#endif
        #endregion
    }
}