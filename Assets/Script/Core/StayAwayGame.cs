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

        /// <summary>
        /// 枚举关卡
        /// </summary>
        public enum Level
        {
            This = -1,
            Start = -2,
            Guide = 0,
            Level01 = 1,
            End = 1000
        }

        /// <summary>
        /// 枚举配置
        /// </summary>
        public enum Config
        {
            AudioMain,
            AudioBGM,
            AudioEffect
        }

        /// <summary>
        /// 音频类型
        /// </summary>
        public enum AudioType
        {
            BGM,
            Effect
        }

        public enum WidgetType
        {
            Slider,
            Checkbox
        }

        public static string STR_SAVES_DATA_PATH = "saves.sav";
        public static string STR_CONF_DATA_PATH = "config.json";
    }
}