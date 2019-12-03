using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

//CSV 사용하기 전에 임시로 사용할 클래스
public class Data : MonoBehaviour
{
    #region Type
    /// <summary>
    /// 근접공격 정보 구조체
    /// </summary>
    [System.Serializable] public struct DamageTableStruct
    {
        [SerializeField, LabelText("공격판정 시작시간(sec)")] public float TriggerTime;
        [SerializeField, LabelText("공격판정 지속시간(sec)")] public float TriggerDur;
        [SerializeField, LabelText("캔슬가능 시작 시간(sec)")] public float ActiveTime;     //적용완료
        [SerializeField, LabelText("데미지")] public float Dmg;
    }
    /// <summary>
    /// 근접공격 ForceChargingTable 구조체
    /// </summary>
    [System.Serializable] public struct ForceChargingTableStruct
    {
        [SerializeField, LabelText("넉백 유지시간")] public float KnockSpeed; //몇초동안 날아가게 할건지
        [SerializeField, LabelText("넉백 세기")] public float KnockBackForce; //얼만큼 날려보낼건지

        public ForceChargingTableStruct(float _speedValue, float _knockBackForce)
        {
            KnockSpeed = _speedValue;
            KnockBackForce = _knockBackForce;
        }
    }
    /// <summary>
    /// 오브젝트 테이블 구조체
    /// </summary>
    [System.Serializable] public struct ObjectTableStruct
    {
        [SerializeField, LabelText("ID")] public string ID;
        [SerializeField, LabelText("HP")] public int HP;
        [SerializeField, LabelText("Damage")] public int Damage;
    }
    #endregion

    public static Data data
    {
        get;
        private set;
    }
    #region Inspector
    //귀찮아서 인스펙터 변수 public으로 직접 가져가서 사용하도록 만들었음, 외부에서 값 수정하면 찾아내서 없애버릴것임
    [Title("캐릭터 - 기본")]
    [SerializeField, LabelText("캐릭터 이동속도(m/s)")] public float Char_MoveSpd = 1.5f;                      //적용완료

    [Title("캐릭터 - 무기스위칭")]
    [SerializeField, LabelText("캔슬가능 시작 시간(sec)")] public float Switching_ActiveTime = 1.0f;        //적용완료

    [Title("캐릭터 - 구르기")]
    [SerializeField, LabelText("캔슬가능 시작 시간(sec)")] public float Rolling_ActiveTime = 1.0f;          //적용완료 / 없길래 추가한것임
    [SerializeField, LabelText("무적시간 전 시간(sec)")] public float Rolling_MsBetweenInvincible = 0.1f;  //적용완료
    [SerializeField, LabelText("무적 지속 시간(sec)")] public float Rolling_InvincibleActiveTime = 0.3f;      //적용완료
    [Header("캐릭터 - 구르기(애니메이팅)")]
    [SerializeField, LabelText("구르기 시간(sec)")] public float Rolling_RollTime = 0.5f;                   //적용완료 / 없길래 추가한것임 (구르기 애니메이션에서 구르기 시간이 실제 얼마일지는 모르니...)
    [SerializeField, LabelText("이동속도(m/s)")] public float Rolling_MoveSpd = 4;                          //적용완료
    [SerializeField, LabelText("구르기 이동속도 커브")] public AnimationCurve Rolling_Curve;                 //적용완료 / 없길래 추가한것임

    [Title("캐릭터 - 근접공격")]
    [SerializeField, LabelText("DamageTable")] public DamageTableStruct[] MeleeAtk;                         //콤보생각해서 합쳤음
    [SerializeField, LabelText("넉백 세기")] public ForceChargingTableStruct[] ForceCharging;              //적용완료
    [SerializeField, LabelText("넉백 커브")] public AnimationCurve forceCurve;                             //적용완료

    [Title("캐릭터 - 원거리공격")]
    [SerializeField, LabelText("최대 탄환수")] public int Bullet_Max = 10;                                   //적용완료 / 없길래 추가한것임
    [SerializeField, LabelText("발사 전 대기시간(sec)")] public float RangedAtk_MsBetweenShot = 0.1f;          //적용완료
    [SerializeField, LabelText("투사체 데미지")] public int Projectile_Dmg = 5;
    [SerializeField, LabelText("투사체 속도(m/s)")] public float Projectile_Spd = 2.0f;                      //적용완료
    [SerializeField, LabelText("투사체 수명(sec)")] public float Projectile_LifeSpan = 10.0f;                //적용완료
    [SerializeField, LabelText("탄환 재충전 딜레이(sec)")] public float Bullet_MsBetweenRecover = 2.0f;     //적용완료
    [SerializeField, LabelText("탄환당 재충전 시간(sec)")] public float Bullet_RecoverSpd = 1.0f;           //적용완료
    //RangedAtk_ActiveTime은 없앴음. 공격속도는 애니메이션 길이 또는 속도 배율으로 조절해야함

    [Title("캐릭터 - 시간정지")]
    [SerializeField, LabelText("최대 시간에너지")] public int TimeEnergy_Max = 100;                            //적용완료
    [SerializeField, LabelText("최대 사용시간(sec)")] public float TimeStop_MaxActiveTime = 100;              //적용완료
    [SerializeField, LabelText("에너지 사용량(에너지/s)")] public float TimeStop_EnergeUseRate = 1.0f;       //적용완료
    [SerializeField, LabelText("쿨타임(sec)")] public float TimeStop_CoolDown = 5.0f;                          //적용완료
    [SerializeField, LabelText("선딜레이(sec)")] public float TimeStop_MsBetweenActive = 0.5f;                  //적용완료
    [SerializeField, LabelText("후딜레이(sec)")] public float TimeStop_MsBetweenEnd = 1.0f;                     //적용완료
    [SerializeField, LabelText("선/후딜레이 커브")] public AnimationCurve TimeStop_CurveBetween;               //적용완료 / 없길래 추가한것임
    [SerializeField, LabelText("시간정지시 플레이어 시간배율")] public float TimeStop_PlayerTimeScale = 0.5f;    //적용완료

    [Title("테이블")]
    [SerializeField, LabelText("오브젝트 테이블")] public ObjectTableStruct[] ObjectTable;

    [Title("컨트롤")]
    [SerializeField, LabelText("무기 스위칭")] public KeyCode Key_SwitchWeapon = KeyCode.Q;                //적용완료
    [SerializeField, LabelText("이동(앞쪽)")] public KeyCode Key_MoveForward = KeyCode.W;                       //적용완료
    [SerializeField, LabelText("이동(왼쪽)")] public KeyCode Key_MoveLeft = KeyCode.A;                          //적용완료
    [SerializeField, LabelText("이동(오른쪽)")] public KeyCode Key_MoveRight = KeyCode.D;                        //적용완료
    [SerializeField, LabelText("이동(뒤쪽)")] public KeyCode Key_MoveBack = KeyCode.S;                          //적용완료
    [SerializeField, LabelText("대쉬")] public KeyCode Key_Dash = KeyCode.LeftShift;                          //적용완료
    [SerializeField, LabelText("공격")] public KeyCode Key_Attack = KeyCode.Mouse0;                          //적용완료
    [SerializeField, LabelText("시간정지")] public KeyCode Key_TimeStop = KeyCode.Space;                          //적용완료
    #endregion
    #region Value
    private Dictionary<string, ObjectTableStruct> m_ObjectTableDic = new Dictionary<string, ObjectTableStruct>();
    #endregion

    #region Event
    private void Awake()
    {
        if (data == null)
        {
            data = this;
            DontDestroyOnLoad(gameObject);

            //기타 초기화
            for (int i = 0; i < ObjectTable.Length; ++i)
                m_ObjectTableDic.Add(ObjectTable[i].ID, ObjectTable[i]);
        }
        else
            Destroy(gameObject);
    }
    #endregion
    #region Function
    /// <summary>
    /// 해당 ID의 오브젝트 테이블 데이터를 가져옵니다.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ObjectTableStruct GetObjectTable(string id)
    {
        if (m_ObjectTableDic.TryGetValue(id, out ObjectTableStruct value))
            return value;
        else
            return new ObjectTableStruct();
    }
    /// <summary>
    /// 공격 횟수에 따른 넉백값을 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public float GetKnockForce(int knockbackCount)
    {
        float knockback = 0;

        /*
        for (int i = 0; i < ForceCharging.Length; ++i)
        {
            if (ForceCharging[i].AtkValue <= knockbackCount)
                knockback = ForceCharging[i].KnockBackForce;
        }
        */
        return knockback;
    }
    #endregion
}