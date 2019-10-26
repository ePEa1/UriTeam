using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CharacterSytem.Demo
{
    public class CharacterSystemDemoScript : MonoBehaviour
    {
        #region Inspector
        [SerializeField] private CharacterSystemDemoCharacter m_Character;
        #endregion

        #region Event
        private void Start()
        {
            m_Character.Init();
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
                m_Character.AddEffect(new CharacterPoisonEffect(10), new CharacterEffectList.EffectStateStruct(5));
            else if (Input.GetKeyDown(KeyCode.E))
                m_Character.AddEffect(new CharacterStunEffect(), new CharacterEffectList.EffectStateStruct(3));
        }
        #endregion
    }
}