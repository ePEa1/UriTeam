using CulterSystem.CommonSystem.CharacterSytem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using ePEaCostomFunction;
using static Data;

public class PlayerCharacter_Rush : PlayerCharacter_ActionBase
{
    #region Value
    [SerializeField, LabelText("추격타 종료시간")] float endTime;

    float nowTime = 0.0f;

    PlayerCharacter player;

    EnemyController enemy;

    #endregion

    #region Event
    protected override void OnStartAction()
    {
        player = CurrentCharacter as PlayerCharacter;
        enemy = player.target;
        nowTime = 0.0f;

        Vector3 nor = CostomFunctions.PointNormalize(player.transform.position, enemy.transform.position);

        player.SetLookVector(new Vector2(nor.x, nor.z), true);
        player.transform.position = enemy.transform.position - nor * 1.0f;
        enemy.OnDamEvent(2, nor);
        player.rushReady = false;
    }

    protected override CharacterAction OnUpdateAction()
    {
        nowTime += Time.deltaTime;

        if (nowTime < endTime)
            return this;
        else
        {
            nowTime = 0.0f;
            return base.OnUpdateAction();
        }
            

    }
    protected override void OnEndAction()
    {
    }
    protected override void OnAniEvent(string aniID, string aniName, string eventName)
    {
    }
    #endregion
}
