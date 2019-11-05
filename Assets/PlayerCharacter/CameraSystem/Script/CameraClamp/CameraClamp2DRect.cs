using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CameraSystem
{
    public class CameraClamp2DRect : CameraClamp
    {
        #region Inspector
        [SerializeField, TabGroup("Option"), LabelText("위치 제한 사용")] private bool m_IsPosClamp = true;
        [SerializeField, TabGroup("Option"), LabelText("회전 제한 사용")] private bool m_IsRotClamp = true;
        [SerializeField, TabGroup("Option"), LabelText("줌 제한 사용")] private bool m_IsZoomClamp = true;

        [SerializeField, TabGroup("Option"), LabelText("Rect 왼쪽 아래")] private Vector2 m_RectLeftBot;
        [SerializeField, TabGroup("Option"), LabelText("Rect 오른쪽 위")] private Vector2 m_RectRightTop;

        [SerializeField, TabGroup("Option"), MinMaxSlider(0.1f, 179.9f, true), LabelText("줌 범위(Perspective)")] private Vector2 m_FOVRange = new Vector2(30.0f, 90.0f);
        [SerializeField, TabGroup("Option"), MinMaxSlider(0.1f, 1000.0f, true), LabelText("줌 범위(Orthographic)")] private Vector2 m_OrthographicSizeRange = new Vector2(3.0f, 10.0f);
        [SerializeField, TabGroup("Option"), LabelText("화면 기준 거리")] private float m_ScreenDistance = 10;
        #endregion

        #region Function
        //Public Override
        public override void Clamp(bool isPosChange, bool isRotChange, bool isZoomChange, ref Vector3 pos, ref Quaternion rot, ref float zoom)
        {
            //해당 방식에서는 회전은 일단 지원하지 않는다.
            if (m_IsRotClamp)
            {
                //TODO : 심심할떄 회전 가능하도록 변경
                rot = Quaternion.identity;
            }

            //위치 변경
            Vector2 halfSize = ParentCameraManager.GetLocalScreenPos(new Vector2(Screen.width, Screen.height), m_ScreenDistance);
            pos.x = Mathf.Clamp(pos.x, m_RectLeftBot.x + halfSize.x, m_RectRightTop.x - halfSize.x);
            pos.y = Mathf.Clamp(pos.y, m_RectLeftBot.y + halfSize.y, m_RectRightTop.y - halfSize.y);

            //일정 수준의 줌까지만 지원한다.
            if(m_IsZoomClamp)
            {
                //TODO : 심심할때 위치에 따른 줌 제한 지원하도록 변경
                if (ParentCameraManager.IsOrthographic)
                    zoom = Mathf.Clamp(zoom, m_OrthographicSizeRange.x, m_OrthographicSizeRange.y);
                else
                    zoom = Mathf.Clamp(zoom, m_FOVRange.x, m_FOVRange.y);
            }
        }

        //Public
        /// <summary>
        /// 카메라를 Clamp할 범위값을 정의합니다.
        /// </summary>
        /// <param name="leftBot">왼쪽 아래</param>
        /// <param name="rightTop">오른쪽 위</param>
        public void SetClampRect(Vector2 leftBot, Vector2 rightTop)
        {
            m_RectLeftBot = leftBot;
            m_RectRightTop = rightTop;
        }

        //Private
        Vector3 ClampPos(Vector3 pos)
        {
            pos.x = Mathf.Clamp(pos.x, m_RectLeftBot.x, m_RectRightTop.x);
            pos.y = Mathf.Clamp(pos.y, m_RectLeftBot.y, m_RectRightTop.y);

            return pos;
        }
        float ClampZoom(float zoom)
        {
            if (ParentCameraManager.IsOrthographic)
                return Mathf.Clamp(zoom, m_OrthographicSizeRange.x, m_OrthographicSizeRange.y);
            else
                return Mathf.Clamp(zoom, m_FOVRange.x, m_FOVRange.y);
        }
        #endregion
    }
}