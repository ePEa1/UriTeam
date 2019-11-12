using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMoveBase : MonoBehaviour
{
    EnemyController manager;

    void Awake()
    {
        manager = transform.parent.parent.GetComponent<EnemyController>();
    }

    protected virtual void MoveEvent()
    {
        manager.ChangeStat(EnemyController.EStat.MOVE);
        manager.GetAnimator().SetBool("Moving", true);
    }

    public void PlayMoveEvent()
    {
        MoveEvent();
    }

    public abstract void Moving(float rushSpeed, float rushRad, float runSpeed, float runRad);
}
