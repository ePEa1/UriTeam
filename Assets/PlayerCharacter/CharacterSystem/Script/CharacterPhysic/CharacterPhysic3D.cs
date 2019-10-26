using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CharacterSytem
{
    public class CharacterPhysic3D : CharacterPhysic
    {
        #region Inspector
        [SerializeField, TabGroup("Component"), LabelText("Rigidbody")] private Rigidbody m_Rigidbody;
        [SerializeField, TabGroup("Component"), LabelText("캐릭터 캡슐")] private CapsuleCollider m_CharacterCapsule;

        [SerializeField, TabGroup("Physics"), LabelText("기본 머테리얼")] private PhysicMaterial m_DefaultPhysicsMaterial;
        [SerializeField, TabGroup("Physics"), LabelText("이동시 머테리얼")] private PhysicMaterial m_MovePhysicsMaterial;

        [SerializeField, TabGroup("Option"), LabelText("최대 이동속도")] private float m_MaxMoveSpeed;
        [SerializeField, TabGroup("Option"), LabelText("0->최대이속 시간")] private float m_MoveMaxSec;
        [SerializeField, TabGroup("Option"), LabelText("최대이속->0 시간")] private float m_MoveMinSec;
        [SerializeField, TabGroup("Option"), LabelText("점프 세기")] private float m_JumpPower;
        [SerializeField, TabGroup("Option"), LabelText("점프시 이동시간 배율")] private float m_JumpMoveChangeFactor;
        #endregion

        #region Get,Set
        public FlyStateEnum FlyState
        {
            get
            {
                if (0 < m_GroundCollider.Count)
                    return FlyStateEnum.Ground;
                else
                    return m_FlyState;
            }
        }

        //Action
        /// <summary>
        /// FlyState가 변경되었을 시 호출되는 이벤트입니다.
        /// </summary>
        public Action<FlyStateEnum> OnChangeFlyState;
        #endregion
        #region Value
        private List<Collider> m_GroundCollider = new List<Collider>();        //현재 인접한 바닥 콜라이더
        private FlyStateEnum m_FlyState;            //(바닥에 있지 않은경우만 사용) 현재 어떤이유로 인해 떠있는지
        private int m_CurrentJumpCount;             //현재 몇단점프중인지(0일 경우 점프하지 않은 상태)
        protected bool m_IsMovedThisFrame;          //이번 프레임에 이동 버튼을 눌렀는지 여부
        protected Vector2 m_MoveVelocity;           //목표 이동방향
        protected float m_MoveEndTimer;             //이 시간 이상 지나면 강제로 멈추도록 함
        #endregion

        #region Event
        protected override void OnInit()
        {
            base.OnInit();

            //변수 초기화
            m_FlyState = FlyStateEnum.Float;                            //기본적으론 떠있음
        }

        //Unity Event
        protected void FixedUpdate()
        {
            //이동 물리 업데이트
            //이번 프레임에 이동했을 경우 가속
            if (m_IsMovedThisFrame)
            {
                float changeSec = m_MoveMaxSec * ((FlyState == FlyStateEnum.Jump) ? m_JumpMoveChangeFactor : 1);    //몇초에 걸쳐 이동속도를 변경할지 구한다. (Jump중인 경우 다르다)
                SetMoveVelocityLerp(changeSec, m_MoveVelocity * m_MaxMoveSpeed);                                    //실제로 변경시킨다.
                m_CharacterCapsule.sharedMaterial = m_MovePhysicsMaterial;                                          //미끄러지는 재질로 변경한다.
            }
            //이동하지 않은 경우
            else
            {
                //떠있지 않은 경우
                if (FlyState == FlyStateEnum.Ground)
                {
                    //정지하고나서 충분한 시간이 흐르기 전엔 서서히 이동속도를 줄인다.
                    if (0 < m_MoveEndTimer)
                    {
                        SetMoveVelocityLerp(m_MoveMinSec, Vector2.zero);   //이동속도를 줄인다.
                        m_MoveEndTimer -= Time.deltaTime;       //흐른 시간을 기록한다.
                    }
                    //정지하고나서 충분한 시간이 흐르면 이동을 멈춘다.
                    else
                    {
                        m_Rigidbody.velocity = new Vector3(0, m_Rigidbody.velocity.y, 0);          //이동속도를 0으로 만든다.
                        m_CharacterCapsule.sharedMaterial = m_DefaultPhysicsMaterial;                    //미끄러지지 않는 재질로 교체한다.
                    }
                }
                //떠있는 경우 기본 재질(미끄러지는)으로 변경한다.
                else
                    m_CharacterCapsule.sharedMaterial = m_MovePhysicsMaterial;
            }

            //마무리
            m_IsMovedThisFrame = false;
        }
        private void OnCollisionEnter(Collision collision)
        {
            //충돌 위치의 평균을 구한다.
            Vector3 averConPos = Vector3.zero;
            for (int i = 0; i < collision.contactCount; ++i)
                averConPos += collision.contacts[i].point;
            averConPos /= collision.contactCount;

            //충돌 위치로 보았을 때 바닥인 경우 바닥 추가
            if (averConPos.y < transform.position.y)
                AddGroundCollider(collision.collider);
        }
        private void OnCollisionStay(Collision collision)
        {
            //점프상태에서는 Stay2D는 오차때문에 사용하지 않는다.
            if (FlyState != FlyStateEnum.Jump)
            {
                //충돌 위치의 평균을 구한다.
                Vector3 averageConPos = Vector3.zero;
                for (int i = 0; i < collision.contactCount; ++i)
                    averageConPos += collision.contacts[i].point;
                averageConPos /= collision.contactCount;

                //충돌 위치로 보았을 때 바닥인 경우 바닥 추가, 아닐 시 바닥 제거
                if (averageConPos.y < transform.position.y)
                    AddGroundCollider(collision.collider);
                else
                    RemoveGroundCollider(collision.collider);
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            //바닥이었을수있으니 제거
            RemoveGroundCollider(collision.collider);
        }
        #endregion
        #region Function
        //Public
        /// <summary>
        /// 해당 방향으로 이동합니다.
        /// </summary>
        /// <param name="vec">이동할 속도</param>
        public void Move(Vector2 vec)
        {
            m_IsMovedThisFrame = true;                  //이번 프레임에 이동했다고 한다.
            m_MoveVelocity = m_MaxMoveSpeed * vec;      //이동 방향/세기를 기록한다.
            m_MoveEndTimer = m_MoveMinSec;              //이동 타이머를 초기화한다.
        }
        /// <summary>
        /// 점프합니다.
        /// </summary>
        /// <param name="power">점프 세기</param>
        public void Jump(float power = 1.0f)
        {
            //점프처리를 한다.
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower * power, m_Rigidbody.velocity.z);    //velocity를 설정한다.
            ++m_CurrentJumpCount;                                                                   //점프 카운트를 변경한다.

            //FlyState변경
            m_FlyState = FlyStateEnum.Jump;
            RemoveAllGroundCollider();
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
                float velocityX = Mathf.Lerp(m_Rigidbody.velocity.x, move.x, (m_MaxMoveSpeed / Mathf.Abs(m_Rigidbody.velocity.x - move.x)) * Time.deltaTime / changeSec);
                float velocityY = Mathf.Lerp(m_Rigidbody.velocity.z, move.y, (m_MaxMoveSpeed / Mathf.Abs(m_Rigidbody.velocity.z - move.y)) * Time.deltaTime / changeSec);
                m_Rigidbody.velocity = new Vector3(velocityX, m_Rigidbody.velocity.y, velocityY);
            }
            //0인 경우는 즉시 변경한다.
            else
                m_Rigidbody.velocity = new Vector3(move.x, m_Rigidbody.velocity.y, move.y);
        }
        /// <summary>
        /// 밟고있는 바닥을 추가합니다.
        /// </summary>
        /// <param name="col">콜라이더</param>
        private void AddGroundCollider(Collider col)
        {
            //바닥 추가
            if (!m_GroundCollider.Contains(col))
                m_GroundCollider.Add(col);

            //GroundState 초기화
            m_FlyState = FlyStateEnum.Float;
        }
        /// <summary>
        /// 밟고 있었던 바닥을 제거합니다.
        /// </summary>
        /// <param name="col"></param>
        private void RemoveGroundCollider(Collider col)
        {
            //원래 FlyState를 저장한다.
            FlyStateEnum originalState = FlyState;

            //바닥을 제거합니다.
            m_GroundCollider.Remove(col);

            //FlyState가 변경된 경우 호출합니다.
            if (originalState != FlyState)
                OnChangeFlyState?.Invoke(FlyState);
        }
        /// <summary>
        /// 현재 밟고있는 것으로 등록된 바닥을 전부 제거합니다.
        /// </summary>
        private void RemoveAllGroundCollider()
        {
            //원래 FlyState를 저장한다.
            FlyStateEnum originalState = FlyState;

            //바닥을 제거합니다.
            m_GroundCollider.Clear();

            //FlyState가 변경된 경우 호출합니다.
            if (originalState != FlyState)
                OnChangeFlyState?.Invoke(FlyState);
        }
        #endregion
    }
}
