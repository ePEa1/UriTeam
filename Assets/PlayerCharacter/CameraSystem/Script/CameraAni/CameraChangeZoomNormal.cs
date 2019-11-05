using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace CulterSystem.CommonSystem.CameraSystem
{
    public class CameraChangeZoomNormal : CameraChangeZoomAni
    {
        #region Inspector
        [SerializeField, TabGroup("Animation"), LabelText("줌 커브")] private AnimationCurve m_Curve;
        #endregion

        #region Event
        protected override void OnChangeZoomStart()
        {
        }
        protected override float OnChangeZoomUpdate(float normalizedTime, float _startZoom, float _targetZoom, bool _isOrthographic)
        {
            float zoom = Mathf.LerpUnclamped(_startZoom, _targetZoom, m_Curve.Evaluate(normalizedTime));

            return zoom;
        }
        protected override void OnChangeZoomEnd()
        {
        }
        #endregion
    }
}