using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CameraSystem
{
    public class CameraManagerFollowNormal : CameraManagerFollowAni
    {
        #region Inspector
        [SerializeField, TabGroup("Animation"), LabelText("위치변경 커브")] private AnimationCurve m_Curve;
        #endregion

        #region Event
        protected override void OnFollowPosStart()
        {
        }
        protected override Vector3 OnFollowPosUpdate(float normalizedTime, Vector3 _startPos, Vector3 _targetPos)
        {
            Vector3 pos = Vector3.LerpUnclamped(_startPos, _targetPos, m_Curve.Evaluate(normalizedTime));

            return pos;
        }
        protected override void OnFollowPosEnd()
        {
        }
        #endregion
    }
}