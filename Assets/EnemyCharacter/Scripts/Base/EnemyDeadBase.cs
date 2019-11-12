using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadBase : MonoBehaviour
{
    EnemyController manager;

    // Start is called before the first frame update
    void Awake()
    {
        manager = transform.parent.parent.GetComponent<EnemyController>();
    }

    protected virtual void DeadEvent()
    {
        manager.ChangeStat(EnemyController.EStat.DEAD);
        manager.GetAnimator().SetTrigger("Dead");
    }

    protected virtual void EndDead()
    {
        Destroy(transform.parent.parent.gameObject);
    }

    public void PlayDeadEvent()
    {
        DeadEvent();
    }
}
