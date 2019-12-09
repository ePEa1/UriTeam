﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using ePEaCostomFunction;

public class EShieldAtk : EnemyAtkBase
{
    bool isAtking = false;

    [Title("방패병")]
    [SerializeField, LabelText("공격 유지시간")] float atkTime;

    float nowAtkTime = 0.0f;

    public override void RealAtk()
    {
        nowAtkTime = atkTime; //공격 유지시간 초기화
        isAtking = true; //공격 콜라이더 활성화
    }

    protected override void Update()
    {
        base.Update();

        if (isAtking) //만약 공격 콜라이더가 활성화된 상태면
        {
            nowAtkTime -= Time.deltaTime; //공격 유지시간 흐르게 하기
            if (nowAtkTime <= 0) //다 끝났으면
            {
                isAtking = false; //공격 콜라이더 비활성화
                nowAtkTime = 0.0f;
            }
                
        }
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject col = other.gameObject;

        if (isAtking && col.tag == "Player") //공격콜라이더 활성화 상태에서 플레이어한테 닿으면
        {
            PlayerCharacter pc = col.GetComponent<PlayerCharacter>();
            Debug.Log(pc.nowParry);
            if (pc.nowParry)
            {
                manager.isTarget = true;
                manager.nowTargetTime = manager.targetTime;
                manager.nowGrogiTime = manager.grogiTime;
                manager.SetSuperArmor(false);
                manager.OnKnockEvent(CostomFunctions.PointNormalize(manager.transform.position, target.transform.position), 0);
            }
            else
            {
                //player 피격 이벤트 실행
                Debug.Log("[" + manager.name + "] is hitting");
                pc.OnDamEvent(20, Vector3.zero);
            }

            isAtking = false;
        }
    }
}
