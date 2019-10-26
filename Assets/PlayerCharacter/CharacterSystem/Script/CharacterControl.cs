using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CharacterSytem
{
    /// <summary>
    /// 캐릭터의 컨트롤 상태를 저장하고 있어야 하는 클래스
    /// 상속해서 컨트롤 정보를 저장해놓는다. 해야할 것은 다음과 같다.
    /// - 플레이어 인풋 혹은 AI 처리 (OnUpdateControl)
    /// - 처리된 컨트롤 정보 저장 (상속해서 구현)
    /// </summary>
    public abstract class CharacterControl : MonoBehaviour
    {
        #region Get,Set
        /// <summary>
        /// 해당 컨트롤을 소유하고있는 캐릭터를 가져옵니다.
        /// </summary>
        public Character ParentCharacter
        {
            get;
            private set;
        }
        #endregion

        #region Event
        /// <summary>
        /// 캐릭터 컨트롤을 초기화합니다.
        /// </summary>
        /// <param name="parentCharacter"></param>
        public void Init(Character parentCharacter)
        {
            ParentCharacter = parentCharacter;
        }

        //CharacterControl
        /// <summary>
        /// 캐릭터 컨트롤이 업데이트될 때 호출됩니다.
        /// </summary>
        public abstract void OnUpdateControl();
        #endregion
        #region Function
        //Internal
        /// <summary>
        /// 캐릭터 컨트롤을 업데이트합니다.
        /// </summary>
        internal void UpdateControl()
        {
            OnUpdateControl();
        }
        #endregion
    }
}