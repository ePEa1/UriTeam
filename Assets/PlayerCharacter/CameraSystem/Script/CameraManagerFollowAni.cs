using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CameraSystem
{
    public abstract class CameraManagerFollowAni : MonoBehaviour
    {
        #region Inspector
        [SerializeField, TabGroup("Option"), LabelText("멀어질수록 느리게")] private bool m_IsFarSlow;
        [SerializeField, TabGroup("Option"), ShowIf("m_IsFarSlow"), LabelText("변화량 기준치")] private float m_FarSlowDist;
        [SerializeField, TabGroup("Option"), Range(0.1f, 50.0f), HideIf("m_IsFarSlow"), LabelText("재생 속도")] private float m_AniSpeed = 3.0f;
        #endregion
        #region Get,Set
        protected Vector3 startPos
        {
            get;
            private set;
        }
        protected Vector3 targetPos
        {
            get;
            private set;
        }
        #endregion

        #region Event
        /// <summary>
        /// 카메라 매니저 위치변경이 시작되는 시점에 호출됩니다.
        /// </summary>
        protected abstract void OnFollowPosStart();
        /// <summary>
        /// 카메라 매니저 위치변경 업데이트시에 호출됩니다.
        /// </summary>
        /// <param name="normalizedTime">0.0f ~ 1.0f 까지의 normalize된 애니메이션 타임</param>
        /// <returns>이번 프레임의 카메라 줌 사이즈</returns>
        protected abstract Vector3 OnFollowPosUpdate(float normalizedTime, Vector3 _startPos, Vector3 _targetPos);
        /// <summary>
        /// 카메라 매니저 위치변경이 완료되는 시점에 호출됩니다.
        /// </summary>
        protected abstract void OnFollowPosEnd();
        #endregion
        #region Function
        //Public Virtual
        /// <summary>
        /// 위치변경 시작지점 및 목표에 따른 카메라 위치변경 애니메이션의 속도를 가져옵니다.
        /// </summary>
        /// <param name="startPos">시작지점의 위치</param>
        /// <param name="targetPos">목표 위치</param>
        /// <returns></returns>
        public virtual float GetAnimationSpeed(Vector3 startPos, Vector3 targetPos)
        {
            if (m_IsFarSlow)
            {
                float distance = Vector3.Distance(startPos, targetPos);
                float speedFactor = (distance == 0) ? float.MaxValue : (m_FarSlowDist / distance);

                return speedFactor;
            }
            else
                return m_AniSpeed;
        }

        //Public
        /// <summary>
        /// 위치변경을 시작합니다.
        /// </summary>
        /// <param name="_startPos">시작시점의 위치</param>
        /// <param name="_targetPos">목표 위치</param>
        public void FollowPosStart(Vector3 _startPos, Vector3 _targetPos)
        {
            startPos = _startPos;
            targetPos = _targetPos;

            OnFollowPosStart();
        }
        /// <summary>
        /// 위치변경을 업데이트합니다.
        /// </summary>
        /// <param name="normalizedTime">normalize된 애니메이션 시간</param>
        public Vector3 FollowPosUpdate(float normalizedTime)
        {
            return OnFollowPosUpdate(normalizedTime, startPos, targetPos);
        }
        /// <summary>
        /// 위치변경을 끝냅니다.
        /// </summary>
        public void FollowPosEnd()
        {
            OnFollowPosEnd();
        }
        #endregion
        #region Event - Editor
#if UNITY_EDITOR
        protected virtual void Reset()
        {
            CameraChangePosAniInitContext();
        }
#endif
        #endregion
        #region Function - Editor
#if UNITY_EDITOR
        [ContextMenu("CameraChangePosAni Init")]
        public virtual void CameraChangePosAniInitContext()
        {
            //자신 또는 상위에 있어야 하는 컴포넌트 추가
            GetComponentInParent<CameraManager>()?.CameraManagerInitContext();
        }
#endif
        #endregion
    }
}