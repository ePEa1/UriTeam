using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace CulterSystem.CommonSystem.CameraSystem
{
    public class CameraManager : MonoBehaviour
    {
        #region Inspector
        [SerializeField, TabGroup("Component"), LabelText("카메라")] private Camera m_Camera;
        [SerializeField, TabGroup("Component"), LabelText("카메라 회전 기준")] private Transform m_CameraRotRoot;
        [SerializeField, TabGroup("Component"), LabelText("카메라 위치 기준")] private Transform m_CameraPosRoot;

        [SerializeField, TabGroup("Component"), LabelText("이동 애니메이션")] private CameraChangePosAni m_CameraChangePosAni;
        [SerializeField, TabGroup("Component"), LabelText("회전 애니메이션")] private CameraChangeRotAni m_CameraChangeRotAni;
        [SerializeField, TabGroup("Component"), LabelText("줌 애니메이션")] private CameraChangeZoomAni m_CameraChangeZoomAni;

        [SerializeField, TabGroup("Option"), MinMaxSlider(0.1f, 179.9f, true), LabelText("줌 범위(Perspective)")] private Vector2 m_FOVRange = new Vector2(30.0f, 90.0f);
        [SerializeField, TabGroup("Option"), MinMaxSlider(0.1f, 1000.0f, true), LabelText("줌 범위(Orthographic)")] private Vector2 m_OrthographicSizeRange = new Vector2(3.0f, 10.0f);
        #endregion
        #region Get,Set
        /// <summary>
        /// 카메라를 가져옵니다. (왜부에서 수정하지는 마세요)
        /// </summary>
        public Camera useCamera
        {
            get
            {
                return m_Camera;
            }
        }
        /// <summary>
        /// 카메라 회전 기준 Transform을 가져옵니다. (트랜스폼 외부에서 수정하지는 마세요)
        /// </summary>
        public Transform cameraRotRoot
        {
            get
            {
                return m_CameraRotRoot;
            }
        }
        /// <summary>
        /// 카메라 위치 기준 Transform을 가져옵니다. (트랜스폼 외부에서 수정하지는 마세요)
        /// </summary>
        public Transform cameraPosRoot
        {
            get
            {
                return m_CameraPosRoot;
            }
        }
        /// <summary>
        /// 카메라의 투영 타입이 Orthographic인지 여부 (false일 경우 Perspective)
        /// </summary>
        public bool isOrthographic
        {
            get
            {
                return m_Camera.orthographic;
            }
        }
        /// <summary>
        /// Perspective 뷰에서의 FOV 최소값
        /// </summary>
        public float fovMin
        {
            get
            {
                return m_FOVRange.x;
            }
            set
            {
                m_FOVRange.x = value;
            }
        }
        /// <summary>
        /// Perspective 뷰에서의 FOV 최댓값
        /// </summary>
        public float fovMax
        {
            get
            {
                return m_FOVRange.y;
            }
            set
            {
                m_FOVRange.y = value;
            }
        }
        /// <summary>
        /// Orthographic 뷰에서의 Size 최솟값
        /// </summary>
        public float orthographicSizeMin
        {
            get
            {
                return m_OrthographicSizeRange.x;
            }
            set
            {
                m_OrthographicSizeRange.x = value;
            }
        }
        /// <summary>
        /// Orthographic 뷰에서의 Size 최댓값
        /// </summary>
        public float orthographicSizeMax
        {
            get
            {
                return m_OrthographicSizeRange.y;
            }
            set
            {
                m_OrthographicSizeRange.y = value;
            }
        }
        /// <summary>
        /// 목표 카메라 회전 기준점의 각도
        /// </summary>
        public Quaternion targetRootRot
        {
            get
            {
                if (m_TargetRootRot.HasValue)
                    return m_TargetRootRot.Value;
                else
                    return m_CameraRotRoot.rotation;
            }
        }
        /// <summary>
        /// 현재 상태의 카메라 회전 기준점의 각도
        /// </summary>
        public Quaternion currentRootRot
        {
            get
            {
                return m_CameraRotRoot.rotation;
            }
        }
        /// <summary>
        /// 목표 카메라 위치 기준점의 위치
        /// </summary>
        public Vector3 targetRootPos
        {
            get
            {
                if (m_TargetRootPos.HasValue)
                    return m_TargetRootPos.Value;
                else
                    return currentRootPos;
            }
        }
        /// <summary>
        /// 현재 상태의 카메라 위치 기준점의 위치
        /// </summary>
        public Vector3 currentRootPos
        {
            get
            {
                return m_CameraPosRoot.localPosition;
            }
        }
        /// <summary>
        /// 목표 카메라 줌 값
        /// </summary>
        public float targetZoom
        {
            get
            {
                if (m_TargetZoom.HasValue)
                    return m_TargetZoom.Value;
                else
                    return currentZoom;
            }
        }
        /// <summary>
        /// 현재 상태의 카메라 줌 값
        /// </summary>
        public float currentZoom
        {
            get
            {
                return m_Camera.orthographic ? m_Camera.orthographicSize : m_Camera.fieldOfView;
            }
        }
        #endregion
        #region Value
        private Quaternion? m_TargetRootRot;
        private float m_TargetRootRotSpeed;
        private float m_TargetRootRotTimer;

        private Vector3? m_TargetRootPos;
        private float m_TargetRootPosSpeed;
        private float m_TargetRootPosTimer;

        private float? m_TargetZoom;
        private float m_TargetZoomSpeed;
        private float m_TargetZoomTimer;
        #endregion

        #region Event
        public virtual void Init()
        {
        }
        private void Update()
        {
            //TargetRootRot 회전 진행 및 진행 완료 처리
            if(m_TargetRootRot.HasValue)
            {
                m_TargetRootRotTimer += Time.deltaTime * m_TargetRootRotSpeed;
                m_CameraRotRoot.localRotation = m_CameraChangeRotAni.ChangeRotUpdate(Mathf.Clamp(m_TargetRootRotTimer, 0.0f, 1.0f));
                if(1.0f <= m_TargetRootRotTimer)
                {
                    m_CameraRotRoot.localRotation = m_TargetRootRot.Value;
                    m_TargetRootRot = null;
                    m_CameraChangeRotAni.ChangeRotEnd();
                }
            }

            //TargetRootPos 이동 진행 및 진행 완료 처리
            if (m_TargetRootPos.HasValue)
            {
                m_TargetRootPosTimer += Time.deltaTime * m_TargetRootPosSpeed;
                m_CameraPosRoot.localPosition = m_CameraChangePosAni.ChangePosUpdate(Mathf.Clamp(m_TargetRootPosTimer, 0.0f, 1.0f));
                if (1.0f <= m_TargetRootPosTimer)
                {
                    m_CameraPosRoot.localPosition = m_TargetRootPos.Value;
                    m_TargetRootPos = null;
                    m_CameraChangePosAni.ChangePosEnd();
                }
            }

            //TargetRootZoom 줌인아웃 진행 및 진행 완료 처리
            if (m_TargetZoom.HasValue)
            {
                m_TargetZoomTimer += Time.deltaTime * m_TargetZoomSpeed;
                if (m_Camera.orthographic)
                    m_Camera.orthographicSize = m_CameraChangeZoomAni.ChangeZoomUpdate(Mathf.Clamp(m_TargetZoomTimer, 0.0f, 1.0f));
                else
                    m_Camera.fieldOfView = m_CameraChangeZoomAni.ChangeZoomUpdate(Mathf.Clamp(m_TargetZoomTimer, 0.0f, 1.0f));

                if (1.0f <= m_TargetZoomTimer)
                {
                    if (m_Camera.orthographic)
                        m_Camera.orthographicSize = m_TargetZoom.Value;
                    else
                        m_Camera.fieldOfView = m_TargetZoom.Value;
                    m_TargetZoom = null;
                    m_CameraChangeZoomAni.ChangeZoomEnd();
                }
            }

        }
        #endregion
        #region Function
        //Public
        /// <summary>
        /// 카메라 회전 기준점의 회전값을 설정합니다.
        /// </summary>
        /// <param name="rot">쿼터니언 각도</param>
        public void SetRootRot(Quaternion rot)
        {
            m_TargetRootRot = null;
            m_CameraRotRoot.localRotation = rot;
        }
        /// <summary>
        /// 카메라 회전 기준점의 회전값 목표를 설정합니다. (애니메이션되면서 해당 수치까지 변경됩니다.
        /// </summary>
        /// <param name="rot">쿼터니언 각도</param>
        public void ChangeRootRot(Quaternion rot)
        {
            if (m_CameraChangeRotAni)
            {
                m_TargetRootRot = rot;
                m_TargetRootRotTimer = 0.0f;
                m_TargetRootRotSpeed = m_CameraChangeRotAni.GetAnimationSpeed(currentRootRot, rot);
                m_CameraChangeRotAni.ChangeRotStart(currentRootRot, rot);
            }
            else
                SetRootRot(rot);
        }
        /// <summary>
        /// 카메라 위치 기준점의 위치값을 설정합니다.
        /// </summary>
        /// <param name="pos">위치</param>
        public void SetRootPos(Vector3 pos)
        {
            m_TargetRootPos = null;
            m_CameraPosRoot.localPosition = pos;
        }
        /// <summary>
        /// 카메라 위치 기준점의 위치값 목표를 설정합니다. (애니메이션되면서 해당 수치까지 변경됩니다.)
        /// </summary>
        /// <param name="pos">위치</param>
        public void ChangeRootPos(Vector3 pos)
        {
            if (m_CameraChangePosAni)
            {
                m_TargetRootPos = pos;
                m_TargetRootPosTimer = 0.0f;
                m_TargetRootPosSpeed = m_CameraChangePosAni.GetAnimationSpeed(currentRootPos, pos);
                m_CameraChangePosAni.ChangePosStart(currentRootPos, pos);
            }
            else
                SetRootPos(pos);
        }
        /// <summary>
        /// 카메라 줌(Perspective FOV, Orthographic Size)을 설정합니다.
        /// </summary>
        /// <param name="zoom">카메라 줌 값</param>
        public void SetZoom(float _zoom)
        {
            if (m_Camera.orthographic)
                m_Camera.orthographicSize = GetClampedZoom(_zoom);
            else
                m_Camera.fieldOfView = GetClampedZoom(_zoom);
        }
        /// <summary>
        /// 카메라 줌(Perspective FOV, Orthographic Size)을 설정합니다. (애니메이션되면서 해당 수치까지 변경됩니다.)
        /// </summary>
        /// <param name="zoom">카메라 줌 값</param>
        public void ChangeZoom(float _zoom)
        {
            if (m_CameraChangeZoomAni)
            {
                _zoom = GetClampedZoom(_zoom);
                m_TargetZoom = _zoom;
                m_TargetZoomTimer = 0.0f;
                m_TargetZoomSpeed = m_CameraChangeZoomAni.GetAnimationSpeed(currentZoom, _zoom);
                m_CameraChangeZoomAni.ChangeZoomStart(currentZoom, _zoom, m_Camera.orthographic);
            }
            else
                SetZoom(_zoom);
        }

        //Private
        /// <summary>
        /// 줌을 범위내로 Clamp하여 가져옵니다.
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        private float GetClampedZoom(float zoom)
        {
            if (m_Camera.orthographic)
                return Mathf.Clamp(zoom, m_OrthographicSizeRange.x, m_OrthographicSizeRange.y);
            else
                return Mathf.Clamp(zoom, m_FOVRange.x, m_FOVRange.y);
        }
        #endregion
        #region Event - Editor
#if UNITY_EDITOR
        protected virtual void Reset()
        {
            CameraManagerInitContext();
        }
#endif
        #endregion
        #region Function - Editor
#if UNITY_EDITOR
        [ContextMenu("CameraManager Init")]
        public virtual void CameraManagerInitContext()
        {
            Undo.RecordObject(gameObject, $"PopupManager Init {gameObject.name}");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            //자신 또는 하위에 있어야 하는 컴포넌트 추가
            m_Camera = GetComponentInChildren<Camera>();
            m_CameraPosRoot = m_Camera?.transform.parent;
            m_CameraRotRoot = m_CameraPosRoot?.transform.parent;
            m_CameraChangePosAni = GetComponentInChildren<CameraChangePosAni>();
            m_CameraChangeRotAni = GetComponentInChildren<CameraChangeRotAni>();
            m_CameraChangeZoomAni = GetComponentInChildren<CameraChangeZoomAni>();
        }
#endif
        #endregion
    }
}
