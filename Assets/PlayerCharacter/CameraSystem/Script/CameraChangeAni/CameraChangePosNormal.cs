using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace CulterSystem.CommonSystem.CameraSystem
{
    public class CameraChangePosNormal : CameraChangePosAni
    {
        #region Inspector
        [SerializeField, TabGroup("Animation"), LabelText("위치변경 커브")] private AnimationCurve m_Curve;
        #endregion

        #region Event
        protected override void OnChangePosStart()
        {
        }
        protected override Vector3 OnChangePosUpdate(float normalizedTime, Vector3 _startPos, Vector3 _targetPos)
        {
            Vector3 pos = Vector3.Lerp(_startPos, _targetPos, m_Curve.Evaluate(normalizedTime));

            return pos;
        }
        protected override void OnChangePosEnd()
        {
        }
        #endregion
    }
}