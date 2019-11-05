using CulterSystem.BaseSystem.DataSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager
    {
        get;
        private set;
    }
    #region Inspector
    [SerializeField] private PlayerCharacter m_Player;
    #endregion
    #region Get,Set
    //신경쓰면됨
    /// <summary>
    /// 현재 게임의 Timescale
    /// </summary>
    public float TimeScale
    {
        get
        {
            return 1.0f - m_Player.TimeStopProgress.Value;
        }
    }
    #endregion

    #region Event
    //Unity
    private void Awake()
    {
        gameManager = this;
    }
    #endregion
}
