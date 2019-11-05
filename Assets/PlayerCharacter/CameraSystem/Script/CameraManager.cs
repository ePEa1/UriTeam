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
        [SerializeField, TabGroup("Component"), LabelText("카메라 제한")] private CameraClamp m_CameraClamp;
        [SerializeField, TabGroup("Component"), LabelText("따라갈 트랜스폼")] private Transform m_FollowTransform;
        [SerializeField, TabGroup("Component"), LabelText("따라가기 애니메이션")] private CameraManagerFollowAni m_CameraFollowAni;
        [SerializeField, TabGroup("Component"), LabelText("이동 애니메이션")] private CameraChangePosAni m_CameraChangePosAni;
        [SerializeField, TabGroup("Component"), LabelText("회전 애니메이션")] private CameraChangeRotAni m_CameraChangeRotAni;
        [SerializeField, TabGroup("Component"), LabelText("줌 애니메이션")] private CameraChangeZoomAni m_CameraChangeZoomAni;
        #endregion
        #region Get,Set
        //어쩔수 없이 필요한 경우를 위한 내부 구성요소들
        /// <summary>
        /// 카메라를 가져옵니다. (왠만하면 외부에서 수정하지는 마세요)
        /// </summary>
        public Camera CurentCamera
        {
            get
            {
                return m_Camera;
            }
        }
        /// <summary>
        /// 카메라 회전 기준 Transform을 가져옵니다. (왠만하면 외부에서 수정하지는 마세요)
        /// </summary>
        public Transform RotationRoot
        {
            get
            {
                return m_CameraRotRoot;
            }
        }
        /// <summary>
        /// 카메라 위치 기준 Transform을 가져옵니다. (왠만하면 외부에서 수정하지는 마세요)
        /// </summary>
        public Transform PositionRoot
        {
            get
            {
                return m_CameraPosRoot;
            }
        }

        //기타 구성요소
        /// <summary>
        /// 현재의 카메라 제한
        /// </summary>
        public CameraClamp CurrentClamp
        {
            get
            {
                return m_CameraClamp;
            }
        }

        //현재 옵션
        /// <summary>
        /// 카메라의 투영 타입이 Orthographic인지 여부 (false일 경우 Perspective)
        /// </summary>
        public bool IsOrthographic
        {
            get
            {
                return m_Camera.orthographic;
            }
        }

        //현재 카메라 변환값
        /// <summary>
        /// 목표 카메라 위치 기준점의 위치
        /// </summary>
        public Vector3 TargetPosition
        {
            get
            {
                if (m_TargetPos.HasValue)
                    return m_TargetPos.Value;
                else
                    return CurrentPosition;
            }
        }
        /// <summary>
        /// 목표 카메라 회전 기준점의 각도
        /// </summary>
        public Quaternion TargetRotation
        {
            get
            {
                if (m_TargetRot.HasValue)
                    return m_TargetRot.Value;
                else
                    return CurrentRotation;
            }
        }
        /// <summary>
        /// 목표 카메라 줌 값
        /// </summary>
        public float TargetZoom
        {
            get
            {
                if (m_TargetZoom.HasValue)
                    return m_TargetZoom.Value;
                else
                    return CurrentZoom;
            }
        }

        /// <summary>
        /// 현재 상태의 카메라 위치 기준점의 위치
        /// </summary>
        public Vector3 CurrentPosition
        {
            get
            {
                return m_CameraPosRoot.localPosition;
            }
            private set
            {
                m_CameraPosRoot.localPosition = value;
            }
        }
        /// <summary>
        /// 현재 상태의 카메라 회전 기준점의 각도
        /// </summary>
        public Quaternion CurrentRotation
        {
            get
            {
                return m_CameraRotRoot.rotation;
            }
            private set
            {
                m_CameraRotRoot.rotation = value;
            }
        }
        /// <summary>
        /// 현재 상태의 카메라 줌 값
        /// </summary>
        public float CurrentZoom
        {
            get
            {
                return m_Camera.orthographic ? m_Camera.orthographicSize : m_Camera.fieldOfView;
            }
            private set
            {
                if (m_Camera.orthographic)
                    m_Camera.orthographicSize = value;
                else
                    m_Camera.fieldOfView = value;
            }
        }
        #endregion
        #region Value
        private Vector3 m_TargetFollowPos;
        private float m_FollowSpeed;
        private float m_FollowTimer;

        private Quaternion? m_TargetRot;
        private float m_TargetRotSpeed;
        private float m_TargetRotTimer;

        private Vector3? m_TargetPos;
        private float m_TargetPosSpeed;
        private float m_TargetPosTimer;

        private float? m_TargetZoom;
        private float m_TargetZoomSpeed;
        private float m_TargetZoomTimer;
        #endregion

        #region Event
        public virtual void Init()
        {
            m_CameraClamp?.Init(this);

            //기본값 설정
            if (m_FollowTransform)
            {
                transform.position = m_FollowTransform.position;
                m_TargetFollowPos = transform.position;
                m_FollowTimer = 1.0f;
            }
        }
        private void Update()
        {
            //따라가기 진행처리
            if (m_FollowTransform)
            {
                if (0.005f <= (m_FollowTransform.position - m_TargetFollowPos).sqrMagnitude)
                {
                    m_FollowSpeed = m_CameraFollowAni.GetAnimationSpeed(transform.position, m_FollowTransform.position);
                    m_TargetFollowPos = m_FollowTransform.position;
                    m_FollowTimer = 0;
                    m_CameraFollowAni.FollowPosStart(transform.position, m_TargetFollowPos);
                }
                if (m_FollowTimer < 1.0f)
                {
                    m_FollowTimer += Time.deltaTime * m_FollowSpeed;
                    transform.position = m_CameraFollowAni.FollowPosUpdate(Mathf.Clamp(m_FollowTimer, 0.0f, 1.0f));
                    //if (1.0f <= m_FollowTimer)
                    //    transform.position = m_TargetFollowPos;
                }
            }

            //이동 진행처리
            if (m_TargetPos.HasValue)
            {
                m_TargetPosTimer += Time.deltaTime * m_TargetPosSpeed;
                CurrentPosition = m_CameraChangePosAni.ChangePosUpdate(Mathf.Clamp(m_TargetPosTimer, 0.0f, 1.0f));
                if (1.0f <= m_TargetPosTimer)
                    CurrentPosition = m_TargetPos.Value;
            }

            //회전 진행처리
            if (m_TargetRot.HasValue)
            {
                m_TargetRotTimer += Time.deltaTime * m_TargetRotSpeed;
                CurrentRotation = m_CameraChangeRotAni.ChangeRotUpdate(Mathf.Clamp(m_TargetRotTimer, 0.0f, 1.0f));
                if(1.0f <= m_TargetRotTimer)
                    CurrentRotation = m_TargetRot.Value;
            }

            //줌인아웃 진행처리
            if (m_TargetZoom.HasValue)
            {
                m_TargetZoomTimer += Time.deltaTime * m_TargetZoomSpeed;
                CurrentZoom = m_CameraChangeZoomAni.ChangeZoomUpdate(Mathf.Clamp(m_TargetZoomTimer, 0.0f, 1.0f));
                if (1.0f <= m_TargetZoomTimer)
                    CurrentZoom = m_TargetZoom.Value;
            }

            //따라가기 / 이동 / 회전 / 줌 완료처리
            if (m_FollowTransform && 1.0f <= m_FollowTimer)
                m_CameraFollowAni.FollowPosEnd();
            if (1.0f <= m_TargetPosTimer)
            {
                m_TargetPos = null;
                m_CameraChangePosAni.ChangePosEnd();
            }
            if (1.0f <= m_TargetRotTimer)
            {
                m_TargetRot = null;
                m_CameraChangeRotAni.ChangeRotEnd();
            }
            if (1.0f <= m_TargetZoomTimer)
            {
                m_TargetZoom = null;
                m_CameraChangeZoomAni.ChangeZoomEnd();
            }
        }
        #endregion
        #region Function
        //Public
        /// <summary>
        /// 해당 스크린 위치의 월드기준 위치를 가져옵니다.
        /// </summary>
        /// <param name="screenPos">스크린 기준 위치</param>
        /// <param name="screenDist">기준 거리(raycast에 쓰이는 plane의 거리로 사용)</param>
        /// <returns></returns>
        public Vector3 GetWorldScreenPos(Vector2 screenPos, float screenDist)
        {
            Vector3 camPos = m_Camera.transform.position;
            Vector3 camForward = m_Camera.transform.forward;
            Plane plane = new Plane(camForward * -1, camPos + camForward * screenDist);
            Ray ray = m_Camera.ScreenPointToRay(screenPos);

            if (plane.Raycast(ray, out float enter))
                return ray.GetPoint(enter);
            else
                return camPos;
        }
        /// <summary>
        /// 해당 스크린 위치의 카메라 트랜스폼 기준 로컬 위치를 가져옵니다.
        /// </summary>
        /// <param name="screenPos">스크린 기준 위치</param>
        /// <param name="screenDist">기준 거리(raycast에 쓰이는 plane의 거리로 사용)</param>
        /// <returns></returns>
        public Vector3 GetLocalScreenPos(Vector2 screenPos, float screenDist)
        {
            Vector3 worldScreenPos = GetWorldScreenPos(screenPos, screenDist);
            return m_Camera.transform.InverseTransformPoint(worldScreenPos);
        }

        /// <summary>
        /// 카메라 위치 기준점의 위치값을 설정합니다.
        /// </summary>
        /// <param name="pos">위치</param>
        public void SetPosition(Vector3 pos)
        {
            m_TargetPos = null;
            CurrentPosition = pos;

            ClampCameraCurrentPosRotZoom(true, false, false);
        }
        /// <summary>
        /// 카메라 회전 기준점의 회전값을 설정합니다.
        /// </summary>
        /// <param name="cameraRot">쿼터니언 각도</param>
        public void SetRotation(Quaternion rot)
        {
            m_TargetRot = null;
            CurrentRotation = rot;

            ClampCameraCurrentPosRotZoom(false, true, false);
        }
        /// <summary>
        /// 카메라 줌(Perspective FOV, Orthographic Size)을 설정합니다.
        /// </summary>
        /// <param name="zoom">카메라 줌 값</param>
        public void SetZoom(float zoom)
        {
            m_TargetZoom = null;
            CurrentZoom = zoom;

            ClampCameraCurrentPosRotZoom(false, false, true);
        }

        /// <summary>
        /// 카메라 위치 기준점의 위치값 목표를 설정합니다. (애니메이션되면서 해당 수치까지 변경됩니다.)
        /// </summary>
        /// <param name="pos">위치</param>
        public void ChangePosition(Vector3 pos)
        {
            if (m_CameraChangePosAni)
            {
                m_TargetPos = pos;
                ClampCameraTargetPosRotZoom(true, false, false);

                m_TargetPosTimer = 0.0f;
                m_TargetPosSpeed = m_CameraChangePosAni.GetAnimationSpeed(CurrentPosition, pos);
                m_CameraChangePosAni.ChangePosStart(CurrentPosition, m_TargetPos.Value);
            }
            else
                SetPosition(pos);
        }
        /// <summary>
        /// 카메라 회전 기준점의 회전값 목표를 설정합니다. (애니메이션되면서 해당 수치까지 변경됩니다.
        /// </summary>
        /// <param name="rot">쿼터니언 각도</param>
        public void ChangeRotation(Quaternion rot)
        {
            if (m_CameraChangeRotAni)
            {
                m_TargetRot = rot;
                ClampCameraTargetPosRotZoom(false, true, false);

                m_TargetRotTimer = 0.0f;
                m_TargetRotSpeed = m_CameraChangeRotAni.GetAnimationSpeed(CurrentRotation, rot);
                m_CameraChangeRotAni.ChangeRotStart(CurrentRotation, m_TargetRot.Value);
            }
            else
                SetRotation(rot);
        }
        /// <summary>
        /// 카메라 줌(Perspective FOV, Orthographic Size)을 설정합니다. (애니메이션되면서 해당 수치까지 변경됩니다.)
        /// </summary>
        /// <param name="zoom">카메라 줌 값</param>
        public void ChangeZoom(float zoom)
        {
            if (m_CameraChangeZoomAni)
            {
                m_TargetZoom = zoom;
                ClampCameraTargetPosRotZoom(false, false, true);

                m_TargetZoomTimer = 0.0f;
                m_TargetZoomSpeed = m_CameraChangeZoomAni.GetAnimationSpeed(CurrentZoom, zoom);
                m_CameraChangeZoomAni.ChangeZoomStart(CurrentZoom, m_TargetZoom.Value, m_Camera.orthographic);
            }
            else
                SetZoom(zoom);
        }

        //Private
        /// <summary>
        /// 카메라의 타겟 위치/회전/줌값을 CameraClamp를 사용해 제한합니다.
        /// </summary>
        /// <param name="isPosChange"></param>
        /// <param name="isRotChange"></param>
        /// <param name="isZoomChange"></param>
        private void ClampCameraTargetPosRotZoom(bool isPosChange, bool isRotChange, bool isZoomChange)
        {
            Vector3 cameraPos = TargetPosition;
            Quaternion cameraRot = TargetRotation;
            float cameraZoom = TargetZoom;

            m_CameraClamp?.Clamp(isPosChange, isRotChange, isZoomChange, ref cameraPos, ref cameraRot, ref cameraZoom);

            if (m_TargetPos.HasValue)
                m_TargetPos = cameraPos;
            else
                CurrentPosition = cameraPos;

            if (m_TargetRot.HasValue)
                m_TargetRot = cameraRot;
            else
                CurrentRotation = cameraRot;

            if (m_TargetZoom.HasValue)
                m_TargetZoom = cameraZoom;
            else
                CurrentZoom = cameraZoom;
        }
        /// <summary>
        /// 카메라의 현재 위치/회전/줌값을 CameraClamp를 사용해 제한합니다.
        /// </summary>
        /// <param name="isPosChange"></param>
        /// <param name="isRotChange"></param>
        /// <param name="isZoomChange"></param>
        private void ClampCameraCurrentPosRotZoom(bool isPosChange, bool isRotChange, bool isZoomChange)
        {
            Vector3 cameraPos = CurrentPosition;
            Quaternion cameraRot = CurrentRotation;
            float cameraZoom = CurrentZoom;

            m_CameraClamp?.Clamp(isPosChange, isRotChange, isZoomChange, ref cameraPos, ref cameraRot, ref cameraZoom);

            CurrentPosition = cameraPos;
            CurrentRotation = cameraRot;
            CurrentZoom = cameraZoom;
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
