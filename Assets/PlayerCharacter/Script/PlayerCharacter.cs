using CulterSystem.BaseSystem.DataSystem;
using CulterSystem.CommonSystem.CameraSystem;
using CulterSystem.CommonSystem.CharacterSytem;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Data;
using static GameManager;

public class PlayerCharacter : Character
{
    #region Inspector
    [Title("Component")]
    [SerializeField] private CameraManager m_CameraManager;     //카메라 매니저
    [SerializeField] private Animator m_Animator;               //애니메이터
    [SerializeField] private PlayerCharacter_Die m_DieAction;   //사망시의 액션

    //귀찮아서 인스펙터 변수 public으로 직접 가져가서 사용하도록 만들었음, 외부에서 값 수정하면 찾아내서 없애버릴것임
    [SerializeField] public PlayerCharacter_Default DefaultAction;
    [SerializeField] public PlayerCharacter_Dash DashAction;
    [SerializeField] public PlayerCharacter_Switch SwitchAction;
    [SerializeField] public PlayerCharacter_AttackRange AttackRangeAction;
    [SerializeField] public PlayerCharacter_AttackMelee AttackMeleeAction;

    [Title("Option")]
    [SerializeField] private bool m_IsAutoInit = true;                  //자동으로 init함수를 호출할지
    [SerializeField] private float m_LookVectorChangeSpeed = 360.0f;    //캐릭터 보는방향 변경 속도
    #endregion
    #region Get,Set
    //코딩하기 편하도록 하는것들
    /// <summary>
    /// 해당 캐릭터의 맨 아랫부분 월드 위치
    /// </summary>
    public Vector3 BottomWorldPos
    {
        get
        {
            return m_Animator.transform.position;
        }
    }
    /// <summary>
    /// 플레이어의 현재 TimeScale (멈추는 중에는 Time.timeScale, 멈춘후에는 1)
    /// </summary>
    public float PlayerTimeScale
    {
        get
        {
            if (IsTimeStopped)
                return 1.0f;
            else
                return gameManager.TimeScale;
        }
    }

    //플레이어 상태정보
    /// <summary>
    /// 현재 사망했는지 여부
    /// </summary>
    public DataValue<bool> IsDied
    {
        get;
        private set;
    }
    /// <summary>
    /// 현재 원거리 무기를 사용중인지 여부
    /// </summary>
    public DataValue<bool> IsWeaponRange
    {
        get;
        private set;
    }

    /// <summary>
    /// 현재 보유중인 총알 갯수
    /// </summary>
    public DataValue<int> BulletCount
    {
        get;
        private set;
    }
    /// <summary>
    /// 총알 충전 시작까지 남은 시간
    /// </summary>
    public DataValue<float> BulletChargeTimer
    {
        get;
        private set;
    }

    /// <summary>
    /// 시간 에너지
    /// </summary>
    public DataValue<float> TimeEnergy
    {
        get;
        private set;
    }
    /// <summary>
    /// 시간정지 사용중인지
    /// </summary>
    public DataValue<bool> IsUsingTimeStop
    {
        get;
        private set;
    }
    /// <summary>
    /// 남은 시간정지 사용시간
    /// </summary>
    public DataValue<float> TimeStopTimer
    {
        get;
        private set;
    }
    /// <summary>
    /// 시간정지 쿨타임
    /// </summary>
    public DataValue<float> TimeStopCoolDown
    {
        get;
        private set;
    }
    /// <summary>
    /// 시간정지 진행상황 (0이면 정지안함, 1에 가까울수록 정지)
    /// </summary>
    public DataValue<float> TimeStopProgress
    {
        get;
        private set;
    }
    /// <summary>
    /// 현재 시간이 (거의) 완전히 멈췄는지
    /// </summary>
    public bool IsTimeStopped
    {
        get
        {
            return (0.99f < Mathf.Abs(TimeStopProgress.Value));
        }
    }
    #endregion
    #region Value
    private DataConsumTimesReporter<int> m_BulletCount_ConsumeTimesReporter;        //그냥 총알갯수 줄어드는것 추적용임
    #endregion

    #region Event
    //Unity Event
    private void Start()
    {
        if (m_IsAutoInit)
            Init();
    }

    //Character Event
    protected override void OnInit()
    {
        base.OnInit();

        //카메라 매니저 관련 초기화
        m_CameraManager.Init();
        (CurrentControl as PlayerCharacterControl).Init(m_CameraManager.CurentCamera);

        //IsDied 초기값 및 true 변경시 사망처리
        IsDied = new DataValue<bool>(false);
        IsDied.AddValueChangeEvent(() =>
        {
            if(IsDied.Value)
                SetAction(0, m_DieAction);
        }, false);

        //IsUsingRange 초기값 및 값 변경에 따른 Animator 변수 변경 처리
        IsWeaponRange = new DataValue<bool>(false);
        IsWeaponRange.AddValueChangeEvent(() =>
        {
            m_Animator.SetBool("UsingRange", IsWeaponRange.Value);
        }, true);

        //TimeEnergy 초기값 및 최대값 정의, 0이 될 경우 사망처리
        TimeEnergy = new DataValue<float>(data.TimeEnergy_Max);
        TimeEnergy.AddValueChangeEvent(() =>
        {
            if(data.TimeEnergy_Max < TimeEnergy.Value)
                TimeEnergy.SetValue(data.TimeEnergy_Max, false);

            if (TimeEnergy.Value <= 0)
                IsDied.Value = true;
        }, false);

        //TimeScale 변경에 따른 처리
        TimeStopProgress = new DataValue<float>(0);
        TimeStopProgress.AddValueChangeEvent(() =>
        {
            //0~1으로 값 Clamp 
            float clampedValue = Mathf.Clamp01(TimeStopProgress.Value);
            TimeStopProgress.SetValue(clampedValue, false);

            //시간정지에 따른 플레이어에게 가해지는 효과 설정
            m_Animator.speed = PlayerTimeScale;
            (ChildPhysic as CharacterPhysicCharacterController).SimulateScale = PlayerTimeScale;

        }, true);

        //TimeStopTimer 초기값 정의, 0아래로 줄어들면 쿨타임 적용 및 시간정지 종료
        TimeStopTimer = new DataValue<float>(0);
        TimeStopTimer.AddValueChangeEvent(() =>
        {
            if(TimeStopTimer.Value < 0)
            {
                IsUsingTimeStop.Value = false;
                TimeStopCoolDown.Value = data.TimeStop_CoolDown;
            }
        }, false);

        //BulletCount 초기값 및 최대값 정의, 줄어들때마다 BulletChargeTimer 리셋
        BulletCount = new DataValue<int>(data.Bullet_Max);
        BulletCount.AddValueChangeEvent(() =>
        {
            if (data.TimeEnergy_Max < BulletCount.Value)
                BulletCount.SetValue(data.Bullet_Max, false);
        }, false);
        m_BulletCount_ConsumeTimesReporter = new DataConsumTimesReporter<int>(BulletCount, 0);
        m_BulletCount_ConsumeTimesReporter.AddValueChangeEvent(() =>
        {
            BulletChargeTimer.Value = data.Bullet_MsBetweenRecover;         //참고 : 이 함수는 BulletCount의 값이 줄어들때만 호출됨
        }, false);

        //BulletChargeTimer 초기값 정의, 타이머가 0아래로 내려가면 총알 1개 추가 및 BulletChargeTimer 리셋
        BulletChargeTimer = new DataValue<float>(0);
        BulletChargeTimer.AddValueChangeEvent(() =>
        {
            if (BulletChargeTimer.Value < 0)
            {
                BulletCount.Value += 1;
                BulletChargeTimer.Value = data.Bullet_RecoverSpd;
            }
        });

        //기타 값들 설정
        IsUsingTimeStop = new DataValue<bool>(false);
        TimeStopCoolDown = new DataValue<float>(0);
        (ChildPhysic as CharacterPhysicCharacterController)?.MoveSpeed.SetValue(data.Char_MoveSpd, false);       //MoveSpeed
    }
    protected override void OnUpdate()
    {
        //사망중일경우(?)는 업데이트같은거 없다.
        if (GetAction(0) == m_DieAction)
        {
            TimeStopProgress.Value = 0;
            return;
        }

        base.OnUpdate();

        PlayerCharacterControl control = CurrentControl as PlayerCharacterControl;

        //탄환이 부족할 시 재충전 타이머 재생
        if (BulletCount.Value < data.Bullet_Max)
            BulletChargeTimer.Value -= Time.deltaTime;

        //시간정지
        if (TimeStopCoolDown.Value <= 0 && control.TimeStop)
        {
            //만약 이번프레임에 시간정지가 시작된경우 남은 사용시간 초기화
            if (!IsUsingTimeStop.Value)
            {
                IsUsingTimeStop.Value = true;
                TimeStopTimer.Value = data.TimeStop_MaxActiveTime;
            }

            //시간정지 타이머 흐르기 / 실제 적용
            TimeStopProgress.Value += Time.unscaledDeltaTime / data.TimeStop_MsBetweenActive;
            TimeStopTimer.Value -= Time.unscaledDeltaTime;
            TimeEnergy.Value -= data.TimeStop_EnergeUseRate * Time.unscaledDeltaTime;
        }
        else
        {
            //만약 이번프레임에 시간정지가 끝난경우 쿨타임 적용
            if (IsUsingTimeStop.Value)
            {
                IsUsingTimeStop.Value = false;
                TimeStopCoolDown.Value = data.TimeStop_CoolDown;
            }

            //쿨타임 흐르기 / 실제 적용
            TimeStopProgress.Value -= Time.unscaledDeltaTime / data.TimeStop_MsBetweenEnd;
            TimeStopCoolDown.Value -= Time.unscaledDeltaTime;
        }
    }
    #endregion
    #region Function
    //Public
    /// <summary>
    /// 캐릭터가 보는 방향을 설정합니다.
    /// </summary>
    /// <param name="look">보는 방향</param>
    public void SetLookVector(Vector2 look, bool isNow)
    {
        float now = m_Animator.transform.localEulerAngles.y;
        float target = Vector2.Angle(Vector3.up, look);
        if (look.x < 0)
            target *= -1;

        if(isNow)
            m_Animator.transform.localEulerAngles = new Vector3(0, target, 0);
        else
        {
            float dist = Mathf.Abs(Mathf.DeltaAngle(now, target));
            float change = m_LookVectorChangeSpeed * Time.unscaledDeltaTime;

            if (change < dist)
                m_Animator.transform.localEulerAngles = new Vector3(0, Mathf.LerpAngle(now, target, change / dist), 0);
            else
                m_Animator.transform.localEulerAngles = new Vector3(0, target, 0);
        }
    }
    #endregion
}

/// <summary>
/// 회피 이펙트 (대쉬에서 사용함)
/// </summary>
public class AvoidEffect : CharacterEffect
{
}