using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace CulterSystem.CommonSystem.CameraSystem
{
    public class CameraChangeRotNormal : CameraChangeRotAni
    {
        #region Inspector
        [SerializeField, TabGroup("Animation"), LabelText("회전 커브")] private AnimationCurve m_Curve;
        #endregion

        #region Event
        protected override void OnChangeRotStart()
        {
        }
        protected override Quaternion OnChangeRotUpdate(float normalizedTime, Quaternion _startRot, Quaternion _targetRot)
        {
            Quaternion rot = Quaternion.Lerp(_startRot, _targetRot, m_Curve.Evaluate(normalizedTime));

            return rot;
        }
        protected override void OnChangeRotEnd()
        {
        }
        #endregion
    }
}