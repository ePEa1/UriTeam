using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyDamBase : MonoBehaviour
{
    EnemyController manager;

    // Start is called before the first frame update
    void Awake()
    {
        manager = transform.parent.parent.GetComponent<EnemyController>();
    }

    //경직인지 피격인지 판정
    protected abstract bool IsDamOk();

    protected virtual void DamEvent(float d)
    {
        if (IsDamOk())
        {
            Damage(d);
        }
        else
        {
            manager.GetAnimator().SetTrigger("Stop");
            manager.ChangeStat(EnemyController.EStat.DAMAGE);
        }
    }

    void Damage(float d)
    {
        manager.EnemyHp -= d;

        //체력이 0 이하이면
        if (manager.EnemyHp <= 0)
        {
            //사망 이벤트 호출
            manager.OnDeadEvent();
        }
        else
        {
            if (! manager.GetSuperArmor())
            {
                manager.GetAnimator().SetTrigger("Dam");
                manager.ChangeStat(EnemyController.EStat.DAMAGE);
            }
        }
    }

    public void PlayDamEvent(float d)
    {
        if (!manager.IsNotDam())
            DamEvent(d);
    }

    protected virtual void EndDam()
    {
        manager.OnMoveEvent();
    }
}
