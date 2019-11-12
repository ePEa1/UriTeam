using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAtkBase : MonoBehaviour
{
    EnemyController manager;

    void Awake()
    {
        manager = transform.parent.parent.GetComponent<EnemyController>();
    }

    void Start()
    {

    }

    protected virtual void AtkEvent()
    {
        manager.ChangeStat(EnemyController.EStat.ATK);
        manager.GetAnimator().SetTrigger("Atk");
    }

    public void PlayAtkEvent()
    {
        AtkEvent();
    }

    protected abstract void RealAtk();

    protected virtual void EndAtk()
    {
        manager.OnMoveEvent();
    }
}
