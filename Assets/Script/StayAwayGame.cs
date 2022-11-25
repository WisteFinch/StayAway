using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameScript
{
    public class StayAwayGame : MonoBehaviour
    {
        /// <summary>
        /// 枚举物品
        /// </summary>
        public enum Item
        {
            ItemLight,
            ItemLyra
        }

        /// <summary>
        /// 枚举魔法
        /// </summary>
        public enum Magic
        {
            None,
            MagicWaterBall
        }
    }
}