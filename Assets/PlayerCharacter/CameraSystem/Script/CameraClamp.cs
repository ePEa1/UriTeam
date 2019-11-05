using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CameraSystem
{
    public abstract class CameraClamp : MonoBehaviour
    {
        #region Get,Set
        /// <summary>
        /// 해당 CameraClamp를 소유하고있는 카메라매니저
        /// </summary>
        public CameraManager ParentCameraManager
        {
            get;
            private set;
        }
        #endregion

        #region Event
        public void Init(CameraManager cameraManager)
        {
            ParentCameraManager = cameraManager;
        }
        #endregion
        #region Function
        /// <summary>
        /// 변경된 위치에 제한을 적용합니다.
        /// </summary>
        /// <param name="isPosChange">위치가 변경되었는지</param>
        /// <param name="isRotChange">회전값이 변경되었는지</param>
        /// <param name="isZoomChange">줌값이 변경되었는지</param>
        /// <param name="pos">(변경된) 위치</param>
        /// <param name="rot">(변경된) 회전값</param>
        /// <param name="zoom">(변경된) 줌값</param>
        public abstract void Clamp(bool isPosChange, bool isRotChange, bool isZoomChange, ref Vector3 pos, ref Quaternion rot, ref float zoom);
        #endregion
    }
}