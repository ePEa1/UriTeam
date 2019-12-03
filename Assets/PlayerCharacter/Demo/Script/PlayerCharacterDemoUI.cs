using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static Data;
using static GameManager;

public class PlayerCharacterDemoUI : MonoBehaviour
{
    #region Inspector
    [Header("Target")]
    [SerializeField] private PlayerCharacter m_PlayerCharacter;

    [Header("Component")]
    [SerializeField] private Image m_TimeStopScale;
    [SerializeField] private GameObject m_TimeStopFrame;

    [SerializeField] private Image m_TimeEnergyBar;

    [SerializeField] private GameObject m_WeaponRange;
    [SerializeField] private GameObject m_WeaponMelee;

    [SerializeField] private GameObject[] m_Bullet;
    #endregion

    #region Event
    private IEnumerator Start()
    {
        yield return null;

        //시간정지 정도(?)
        m_PlayerCharacter.TimeStopProgress.AddValueChangeEvent(() =>
        {
            Color color = m_TimeStopScale.color;
            color.a = Mathf.Lerp(0.0f, 0.15f, m_PlayerCharacter.TimeStopProgress.Value);
            m_TimeStopScale.color = color;

            m_TimeStopFrame.SetActive(gameManager.IsTimeStopped);
        });

        //시간에너지
        m_PlayerCharacter.TimeEnergy.AddValueChangeEvent(() =>
        {
            m_TimeEnergyBar.fillAmount = m_PlayerCharacter.TimeEnergy.Value / data.TimeEnergy_Max;
        });

        //현재 무기
        m_PlayerCharacter.IsWeaponRange.AddValueChangeEvent(() =>
        {
            m_WeaponRange.SetActive(m_PlayerCharacter.IsWeaponRange.Value);
            m_WeaponMelee.SetActive(!m_PlayerCharacter.IsWeaponRange.Value);
        });

        //현재 총알
        m_PlayerCharacter.BulletCount.AddValueChangeEvent(() =>
        {
            for (int i = 0; i < m_Bullet.Length; ++i)
            {
                m_Bullet[i].SetActive(i < m_PlayerCharacter.BulletCount.Value);
            }
        });
    }
    #endregion
}
