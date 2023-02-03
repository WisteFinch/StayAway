using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace StayAwayGameScript
{
    public class SavesDataSystem
    {
        /// <summary>
        /// 存档数据
        /// </summary>
        public class SavesData
        {
            [Serializable]
            public struct DataStruct
            {
                public string PlayerName;
                public int Skin;
                public StayAwayGame.Level Level;
                public string Date;
                public bool Enable;
            }
            public DataStruct Save1 = new(), Save2 = new(), Save3 = new() ;
        }

        public SavesData Saves = new();

        /// <summary>
        /// 存储存档数据
        /// </summary>
        public void StoreSavesData()
        {
            FileSystem.Save(Path.Combine(Application.persistentDataPath, StayAwayGame.STR_SAVES_DATA_PATH), this.Saves);
        }

        /// <summary>
        /// 读取存档数据
        /// </summary>
        public void LoadSavesData()
        {
            FileInfo file = new(Path.Combine(Application.persistentDataPath, StayAwayGame.STR_SAVES_DATA_PATH));
            if (file.Exists)
            {
                SavesData data = FileSystem.Load<SavesData>(Path.Combine(Application.persistentDataPath, StayAwayGame.STR_SAVES_DATA_PATH));
                this.Saves = data;
            }
            else
            {
                FileSystem.Save(Path.Combine(Application.persistentDataPath, StayAwayGame.STR_SAVES_DATA_PATH), this.Saves);
            }
        }


        /// <summary>
        /// 获取玩家数据
        /// </summary>
        /// <param name="i">存档序号</param>
        /// <returns>数据</returns>
        public SavesData.DataStruct GetSavesData(int i)
        {
            if(i == 1)
            {
                return this.Saves.Save1;
            }
            else if(i == 2)
            {
                return this.Saves.Save2;
            }
            else if (i == 3)
            {
                return this.Saves.Save3;
            }
            return this.Saves.Save1;
        }

        /// <summary>
        /// 设置玩家数据
        /// </summary>
        /// <param name="d">数据</param>
        /// <param name="i">存档序号</param>
        public void SetSavesData(SavesData.DataStruct d, int i)
        {
            if (i == 1)
            {
                this.Saves.Save1 = d;
            }
            else if (i == 2)
            {
                this.Saves.Save2 = d;
            }
            else if (i == 3)
            {
                this.Saves.Save3 = d;
            }
            StoreSavesData();
        }

        /// <summary>
        /// 重置玩家数据
        /// </summary>
        /// <param name="i">存档序号</param>
        public void RestoreSavesData(int i)
        {
            if (i == 1)
            {
                this.Saves.Save1 = new();
            }
            else if (i == 2)
            {
                this.Saves.Save2 = new();
            }
            else if (i == 3)
            {
                this.Saves.Save3 = new();
            }
            StoreSavesData();
        }
    }
}