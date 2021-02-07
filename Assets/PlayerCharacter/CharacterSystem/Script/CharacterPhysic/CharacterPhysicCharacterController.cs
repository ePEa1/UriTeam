using CulterSystem.BaseSystem.DataSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CharacterSytem
{
    public class CharacterPhysicCharacterController : CharacterPhysic
    {
        //TODO : 점프 구현해야함, 프로젝트에서 사용안해서 구현 안했음 - 어차피 내가 선호하는 방식은 rigidbody로 만드는거니까 다음에 CharacterPhysic 통합할때 생각하는걸로

        #region Inspector
        [SerializeField, TabGroup("Component"), LabelText("CharacterController")] private CharacterController m_CharacterController;

        [SerializeField, TabGroup("Option"), LabelText("시뮬레이션 배율")] private float m_SimulateScale = 1.0f;
        [SerializeField, TabGroup("Option"), LabelText("최대 이동속도")] private float m_MoveSpeed;
        [SerializeField, TabGroup("Option"), LabelText("0->최대이속 시간")] private float m_MoveMaxSec;
        [SerializeField, TabGroup("Option"), LabelText("최대이속->0 시간")] private float m_MoveMinSec;

        [SerializeField, TabGroup("Physics"), LabelText("중력가속도")] private float m_Gravity = -9.81f;
        #endregion
        #region Get,Set
        /// <summary>
        /// 시뮬레이션 시의 배율
        /// </summary>
        public float SimulateScale
        {
            get
            {
                return m_SimulateScale;
            }
            set
            {
                m_SimulateScale = value;
            }
        }
        /// <summary>
        /// 이동속도
        /// </summary>
        public DataValue<float> MoveSpeed
        {
            get;
            set;
        }
        #endregion
        #region Value
        protected bool m_IsMovedThisFrame;          //이번 프레임에 이동 버튼을 눌렀는지 여부
        protected Vector2 m_MoveVelocity;           //목표 이동방향
        protected float m_MoveEndTimer;             //이 시간 이상 지나면 강제로 멈추도록 함
        protected Vector2 m_Velocity;               //현재의 velocity
        protected float m_UpDown;                   //위/아래방향 velocity
        #endregion

        #region Event
        protected override void OnInit()
        {
            base.OnInit();

            //변수 초기화
            MoveSpeed = new DataValue<float>(m_MoveSpeed);
        }

        //Unity Event
        protected void Update()
        {
            //중력
            m_UpDown += m_Gravity * Time.unscaledDeltaTime * m_SimulateScale;

            //값 업데이트
            //이번 프레임에 이동했을 경우 가속
            if (m_IsMovedThisFrame)
                SetMoveVelocityLerp(m_MoveMaxSec, m_MoveVelocity * MoveSpeed.Value);
            //이동하지 않은 경우
            else
            {
                //정지하고나서 충분한 시간이 흐르기 전엔 서서히 이동속도를 줄인다.
                if (0 < m_MoveEndTimer)
                {
                    SetMoveVelocityLerp(m_MoveMinSec, Vector2.zero);   //이동속도를 줄인다.
                    m_MoveEndTimer -= Time.deltaTime;       //흐른 시간을 기록한다.
                }
                //정지하고나서 충분한 시간이 흐르면 이동을 멈춘다.
                else
                    m_Velocity = Vector2.zero;          //이동속도를 0으로 만든다.
            }

            //값 적용
            CollisionFlags flag = m_CharacterController.Move(new Vector3(m_Velocity.x, m_UpDown, m_Velocity.y) * Time.deltaTime * SimulateScale);       //이동
            if ((flag & CollisionFlags.Below) != 0)
                m_UpDown = 0;


            //마무리
            m_IsMovedThisFrame = false;
        }
        #endregion
        #region Function
        //Public
        /// <summary>
        /// 해당 방향으로 이동합니다.
        /// </summary>
        /// <param name="vec">이동할 속도</param>
        /// <param name="isNow">즉시 해당 속도로 변할지</param>
        public void Move(Vector2 vec, bool isNow = false)
        {
            m_IsMovedThisFrame = true;                  //이번 프레임에 이동했다고 한다.
            m_MoveVelocity = MoveSpeed.Value * vec;      //이동 방향/세기를 기록한다.
            m_MoveEndTimer = m_MoveMinSec;              //이동 타이머를 초기화한다.

            if (isNow)
                m_Velocity = vec;
        }

        //Private
        /// <summary>
        /// 이동 속도를 changeSec에 걸쳐 변경합니다.
        /// </summary>
        /// <param name="changeSec">변경에 걸리는 시간</param>
        /// <param name="move">목표 이동방향/속도</param>
        private void SetMoveVelocityLerp(float changeSec, Vector2 move)
        {
            //변경에 걸리는 시간이 0이 아닌 경우 changeSec에 걸쳐 변경한다.
            if (changeSec != 0)
            {
                float velocityX = Mathf.Lerp(m_Velocity.x, move.x, (MoveSpeed.Value / Mathf.Abs(m_Velocity.x - move.x)) * Time.deltaTime / changeSec);
                float velocityY = Mathf.Lerp(m_Velocity.y, move.y, (MoveSpeed.Value / Mathf.Abs(m_Velocity.y - move.y)) * Time.deltaTime / changeSec);
                m_Velocity = new Vector2(velocityX, velocityY);
            }
            //0인 경우는 즉시 변경한다.
            else
                m_Velocity = new Vector3(move.x, move.y);
        }
        #endregion
    }
}
