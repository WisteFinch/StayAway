using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameScript
{
    public class GameManager : MonoBehaviour
    {
        #region 面板公有变量

        #endregion

        /// <summary>
        /// 获取单例
        /// </summary>
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// 存档功能
        /// </summary>
        public PlayerDataSystem PlayerSystem = new();

        /// <summary>
        /// 皮肤功能
        /// </summary>
        public SkinSystem Skins;

        /// <summary>
        /// 当前玩家序号
        /// </summary>
        private int _currentPlayerIndex;

        private void Awake()
        {
            // 创建单例
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
            this.PlayerSystem.LoadPlayerData();
        }

        /// <summary>
        /// 获取玩家信息
        /// </summary>
        /// <param name="i">存档序号</param>
        /// <returns></returns>
        public PlayerDataSystem.PlayerData.DataStruct GetPlayerData(int index)
        {
            return this.PlayerSystem.GetPlayerData(index);
        }

        /// <summary>
        /// 设置当前玩家序号
        /// </summary>
        /// <param name="index"></param>
        public void SetPlayerIndex(int index)
        {
            this._currentPlayerIndex = index;
        }

        /// <summary>
        /// 设置玩家信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        public void SetPlayerData(PlayerDataSystem.PlayerData.DataStruct data, int index)
        {
            this.PlayerSystem.SetPlayerData(data, index);
        }

        /// <summary>
        /// 保存玩家信息
        /// </summary>
        public void SavePlayerData()
        {
            this.PlayerSystem.SavePlayerData();
        }

        /// <summary>
        /// 关卡开始：初始化角色
        /// </summary>
        public void InitPlayer()
        {
            // 设置小马皮肤
            GameObject.FindGameObjectWithTag("Pony").GetComponentInChildren<Animator>().runtimeAnimatorController = this.Skins.GetAnimator(this.PlayerSystem.GetPlayerData(this._currentPlayerIndex).Skin);
        }
    }
}