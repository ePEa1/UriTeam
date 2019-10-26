using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CulterSystem.CommonSystem.CharacterSytem
{
    public abstract class CharacterPhysic : MonoBehaviour
    {
        #region Type
        /// <summary>
        /// 캐릭터의 발이 어디에 있는지를 저장하기 위한 Enum
        /// </summary>
        public enum FlyStateEnum
        {
            Ground,         //지상
            Float,          //일반적인 이유로 공중에 떠있는 중
            Fly,            //날아다니는중
            Jump,           //점프해서 공중에 떠있는 중
        }
        #endregion

        #region Get,Set
        /// <summary>
        /// 해당 물리를 소유하고있는 캐릭터를 가져옵니다.
        /// </summary>
        public Character ParentCharacter
        {
            get;
            private set;
        }
        #endregion

        #region Event
        /// <summary>
        /// 캐릭터 물리를 초기화합니다.
        /// </summary>
        /// <param name="parentCharacter"></param>
        public void Init(Character parentCharacter)
        {
            ParentCharacter = parentCharacter;

            OnInit();
        }

        /// <summary>
        /// 캐릭터 물리 초기화시 호출됩니다.
        /// </summary>
        protected virtual void OnInit()
        {
        }
        #endregion
    }

    //TODO : 언제한번 이런저런 타입의 캐릭터 물리를 생각하면서 2D랑 3D랑 구조한번 손봐야함
}