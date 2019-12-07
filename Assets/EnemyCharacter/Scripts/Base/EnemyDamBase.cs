using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class EnemyDamBase : MonoBehaviour
{
    EnemyController manager;

    [SerializeField, LabelText("넉백 수치")] float kn = 0.0f;

    // Start is called before the first frame update
    void Awake()
    {
        manager = transform.parent.parent.GetComponent<EnemyController>();
    }

    //공격이 들어가는지 체크
    protected virtual bool IsDamOk()
    {
        return true;
    }

    protected virtual void DamEvent(int atkNum, Vector3 nor)
    {
        if (IsDamOk()) //피격판정
        {
            Damage(atkNum, nor); //피격 이벤트
        }
        else
        {
            manager.GetAnimator().SetTrigger("Stop"); //경직 애니메이션 재생
            //manager.ChangeStat(EnemyController.EStat.DAMAGE); //피격 상태로 변경
        }
    }

    void Damage(int d, Vector3 nor)
    {
        //체력이 남아있을 경우에만 피격 판정
        if (manager.EnemyHp>0)
        {
            manager.EnemyHp -= Data.data.MeleeAtk[d].Dmg; //데미지만큼 체력 차감
            Debug.Log("[" + manager.name +  "] current hp : " + manager.EnemyHp);

            //피격 가능 상태일 경우(막타이거나, 슈퍼아머가 아닐 경우)
            if (manager.EnemyHp <= 0 || !manager.GetSuperArmor())
            {
                manager.GetAnimator().SetTrigger("Dam"); //피격 애니메이션 재생
                manager.ChangeStat(EnemyController.EStat.DAMAGE); //피격 상태로 변경
                manager.OnKnockEvent(nor, d); //피격 시 넉백
            }
        }
    }

    public void PlayDamEvent(int atkNum, Vector3 nor)
    {
        if (!manager.IsNotDam()) //무적상태가 아닐경우
            DamEvent(atkNum, nor); //데미지 이벤트 실행
    }
}
