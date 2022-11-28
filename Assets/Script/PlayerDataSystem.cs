using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace StayAwayGameScript
{
    public class PlayerDataSystem
    {

        const string PLAYER_DATA_PATH = "saves.sav";

        public class PlayerData
        {
            [Serializable]
            public struct DataStruct
            {
                public string PlayerName;
                public int Skin;
                public int Progress;
                public string Date;
                public bool Enable;
            }
            public DataStruct Save1 = new(), Save2 = new(), Save3 = new() ;
        }

        public PlayerData PlayerSaves = new();

        public void SavePlayerData()
        {
            SaveSystem.Save(Path.Combine(Application.persistentDataPath, PLAYER_DATA_PATH), this.PlayerSaves);
        }

        public void LoadPlayerData()
        {
            FileInfo file = new(Path.Combine(Application.persistentDataPath, PLAYER_DATA_PATH));
            if (file.Exists)
            {
                PlayerData data = SaveSystem.Load<PlayerData>(Path.Combine(Application.persistentDataPath, PLAYER_DATA_PATH));
                this.PlayerSaves = data;
            }
            else
            {
                SaveSystem.Save(Path.Combine(Application.persistentDataPath, PLAYER_DATA_PATH), this.PlayerSaves);
            }
        }


        /// <summary>
        /// 获取玩家信息
        /// </summary>
        /// <param name="i">存档序号</param>
        /// <returns></returns>
        public PlayerData.DataStruct GetPlayerData(int i)
        {
            if(i == 1)
            {
                return this.PlayerSaves.Save1;
            }
            else if(i == 2)
            {
                return this.PlayerSaves.Save2;
            }
            else if (i == 3)
            {
                return this.PlayerSaves.Save3;
            }
            return this.PlayerSaves.Save1;
        }

        /// <summary>
        /// 设置玩家信息
        /// </summary>
        /// <param name="d">信息</param>
        /// <param name="i">存档序号</param>
        public void SetPlayerData(PlayerData.DataStruct d, int i)
        {
            if (i == 1)
            {
                this.PlayerSaves.Save1 = d;
            }
            else if (i == 2)
            {
                this.PlayerSaves.Save2 = d;
            }
            else if (i == 3)
            {
                this.PlayerSaves.Save3 = d;
            }
            SavePlayerData();
        }

        /// <summary>
        /// 重置玩家信息
        /// </summary>
        /// <param name="i">存档序号</param>
        public void RestorePlayerData(int i)
        {
            if (i == 1)
            {
                this.PlayerSaves.Save1 = new();
            }
            else if (i == 2)
            {
                this.PlayerSaves.Save2 = new();
            }
            else if (i == 3)
            {
                this.PlayerSaves.Save3 = new();
            }
            SavePlayerData();
        }
    }
}