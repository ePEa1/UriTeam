using CulterSystem.CommonSystem.CharacterSytem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using static Data;

public class PlayerCharacter_Parry : PlayerCharacter_ActionBase
{
    #region Value
    [Title("능력치")]
    [SerializeField, LabelText("패링 판정 시간")] float parryTime;
    [SerializeField, LabelText("패링 종료 시간")] float endTime;

    [Title("이펙트")]
    [SerializeField, LabelText("패링 이펙트")] GameObject PEff;

    PlayerCharacter player;
    float nowTime = 0.0f;

    #endregion

    #region Event
    protected override void OnStartAction()
    {
        player = CurrentCharacter as PlayerCharacter;

        nowTime = 0.0f;
        player.coolParry = Data.data.Cool_Parry; //패링 쿨타임
        PEff.SetActive(true); //패링 이펙트
        PEff.GetComponent<ParticleSystem>().Play();
        player.nowParry = true;
    }

    protected override CharacterAction OnUpdateAction()
    {
        nowTime += Time.deltaTime;

        if (nowTime >= parryTime)
        {
            player.nowParry = false;
            Debug.Log("Parry end");
        }
            

        if (nowTime >= endTime)
            return base.OnUpdateAction();
        else
            return this;
    }
    protected override void OnEndAction()
    {
    }
    protected override void OnAniEvent(string aniID, string aniName, string eventName)
    {
    }
    #endregion
}
