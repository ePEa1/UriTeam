using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CameraSystem
{
    public class CameraSystemDemoManager : MonoBehaviour
    {
        #region Inspector
        [SerializeField] private CameraManager m_CameraManager;
        #endregion

        #region Event
        private void Update()
        {
            if (Input.GetKey(KeyCode.W))
                m_CameraManager.ChangeRootPos(m_CameraManager.targetRootPos + new Vector3(0, 0, 3 * Time.deltaTime));
            else if(Input.GetKey(KeyCode.S))
                m_CameraManager.ChangeRootPos(m_CameraManager.targetRootPos + new Vector3(0, 0, -3 * Time.deltaTime));

            if (Input.GetKey(KeyCode.A))
                m_CameraManager.ChangeRootRot(m_CameraManager.targetRootRot * Quaternion.Euler(0, 30 * Time.deltaTime, 0));
            else if (Input.GetKey(KeyCode.D))
                m_CameraManager.ChangeRootRot(m_CameraManager.targetRootRot * Quaternion.Euler(0, -30 * Time.deltaTime, 0));

            if (0.01f < Mathf.Abs(Input.mouseScrollDelta.sqrMagnitude))
                m_CameraManager.ChangeZoom(m_CameraManager.targetZoom - 3 * Input.mouseScrollDelta.y);
        }
        #endregion
    }
}