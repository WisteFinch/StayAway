using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameScript
{
    public class StayAwayGame : MonoBehaviour
    {
        /// <summary>
        /// ö����Ʒ
        /// </summary>
        public enum Item
        {
            ItemLight,
            ItemLyra
        }

        /// <summary>
        /// ö��ħ��
        /// </summary>
        public enum Magic
        {
            None,
            MagicWaterBall
        }

        /// <summary>
        /// ö�ٹؿ�
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
        /// ö������
        /// </summary>
        public enum Config
        {
            AudioMain,
            AudioBGM,
            AudioEffect
        }

        /// <summary>
        /// ��Ƶ����
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