using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace StayAwayGameScript
{
    public class ConfigSystem
    {
        /// <summary>
        /// ��������
        /// </summary>
        [Serializable]
        public struct ConfigDataStruct
        {
            public float AudioMain;
            public float AudioBGM;
            public float AudioEffect;

            public ConfigDataStruct(float v)
            {
                this.AudioMain = v;
                this.AudioBGM = v;
                this.AudioEffect = v;
            }
        }

        public ConfigDataStruct Data = new(1f);

        /// <summary>
        /// �洢��������
        /// </summary>
        public void StoreConfigData()
        {
            FileSystem.Save(Path.Combine(Application.persistentDataPath, StayAwayGame.STR_CONF_DATA_PATH), this.Data);
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        public void LoadConfigData()
        {
            FileInfo file = new(Path.Combine(Application.persistentDataPath, StayAwayGame.STR_CONF_DATA_PATH));
            if (file.Exists)
            {
                ConfigDataStruct data = FileSystem.Load<ConfigDataStruct>(Path.Combine(Application.persistentDataPath, StayAwayGame.STR_CONF_DATA_PATH));
                this.Data = data;
            }
            else
            {
                FileSystem.Save(Path.Combine(Application.persistentDataPath, StayAwayGame.STR_CONF_DATA_PATH), this.Data);
            }
        }


        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="t">����</param>
        /// <returns>����</returns>
        public object GetConfigData(StayAwayGame.Config t)
        {
            return t switch
            {
                StayAwayGame.Config.AudioMain => Mathf.Clamp(this.Data.AudioMain, 0f, 1f),
                StayAwayGame.Config.AudioBGM => Mathf.Clamp(this.Data.AudioBGM, 0f, 1f),
                StayAwayGame.Config.AudioEffect => Mathf.Clamp(this.Data.AudioEffect, 0f, 1f),
                _ => (object)0,
            };
        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="t">����</param>
        /// <param name="v">��ֵ</param>
        public void SetConfigData(StayAwayGame.Config t, object v)
        {
            switch (t)
            {
                case StayAwayGame.Config.AudioMain: this.Data.AudioMain = Mathf.Clamp((float)v, 0f, 1f); break;
                case StayAwayGame.Config.AudioBGM: this.Data.AudioBGM = Mathf.Clamp((float)v, 0f, 1f); break;
                case StayAwayGame.Config.AudioEffect: this.Data.AudioEffect = Mathf.Clamp((float)v, 0f, 1f); break;
            }
            StoreConfigData();
        }

        /// <summary>
        /// ������������
        /// </summary>
        public void RestoreConfigData()
        {
            this.Data = new(1f);
            StoreConfigData();
        }
    }
}