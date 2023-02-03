using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StayAwayGameScript
{
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// 获取单例
        /// </summary>
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// 输入管理
        /// </summary>
        public InputManager Input;

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
            // 读取存档
            this.SavesSystem.LoadSavesData();
            // 读取配置
            this.ConfigSystem.LoadConfigData();
            // 设置输入管理
            this.Input = this.gameObject.GetComponent<InputManager>();
        }

        #region 存档

        /// <summary>
        /// 存档功能
        /// </summary>
        private readonly SavesDataSystem SavesSystem = new();

        /// <summary>
        /// 当前存档序号
        /// </summary>
        private int _currentSavesIndex;

        /// <summary>
        /// 获取存档数据
        /// </summary>
        /// <param name="i">存档序号</param>
        /// <returns>存档数据</returns>
        public SavesDataSystem.SavesData.DataStruct GetSavesData(int index = -1)
        {
            return index == -1 ? this.SavesSystem.GetSavesData(this._currentSavesIndex) : this.SavesSystem.GetSavesData(index);
        }

        /// <summary>
        /// 设置当前存档序号
        /// </summary>
        /// <param name="index">存档序号</param>
        public void SetSavesIndex(int index)
        {
            this._currentSavesIndex = index;
        }

        /// <summary>
        /// 设置存档数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="index">存档序号</param>
        public void SetSavesData(SavesDataSystem.SavesData.DataStruct data, int index = -1)
        {
            if (index == -1)
                this.SavesSystem.SetSavesData(data, this._currentSavesIndex);
            else
                this.SavesSystem.SetSavesData(data, index);
        }

        /// <summary>
        /// 保存存档数据
        /// </summary>
        public void StoreSavesData()
        {
            this.SavesSystem.StoreSavesData();
        }

        /// <summary>
        /// 重置存档数据
        /// </summary>
        /// <param name="index">存档序号</param>
        public void RestoreSavesData(int i)
        {
            this.SavesSystem.RestoreSavesData(i);
        }

        #endregion

        #region 配置

        /// <summary>
        /// 配置功能
        /// </summary>
        private readonly ConfigSystem ConfigSystem = new();

        /// <summary>
        /// 获取配置数据
        /// </summary>
        /// <param name="c">配置类型</param>
        /// <returns>配置数据</returns>
        public object GetConfigData(StayAwayGame.Config c)
        {
            return this.ConfigSystem.GetConfigData(c);
        }

        /// <summary>
        /// 存储配置数据
        /// </summary>
        public void StoreConfigData()
        {
            this.ConfigSystem.StoreConfigData();
        }

        /// <summary>
        /// 获取配置数据
        /// </summary>
        /// <param name="c">配置类型</param>
        /// <param name="">数值</param>
        /// <returns></returns>
        public void SetConfigData(StayAwayGame.Config c, object v)
        {
            this.ConfigSystem.SetConfigData(c, v);
            ConfigAudioChangeEvent();
        }

        /// <summary>
        /// 音频配置值更改委托
        /// </summary>
        public Action ConfigAudioChangeEvent;

        #endregion

        #region 皮肤

        /// <summary>
        /// 皮肤功能
        /// </summary>
        public SkinSystem Skins;

        /// <summary>
        /// 关卡开始：初始化角色
        /// </summary>
        public void InitPlayer()
        {
            print($"Current Saves Index: {this._currentSavesIndex}");
            // 设置小马皮肤
            GameObject.FindGameObjectWithTag("Pony").GetComponentInChildren<Animator>().runtimeAnimatorController = this.Skins.GetAnimator(this.SavesSystem.GetSavesData(this._currentSavesIndex).Skin);
        }

        #endregion

        #region GUI

        /// <summary>
        /// 当前GUI
        /// </summary>
        private GameObject _gui;

        public void SetGUI(GameObject obj)
        {
            this._gui = obj;
        }

        public void AddTips(string str, bool urgent = false)
        {
            if (this._gui != null)
            {
                if (urgent)
                    this._gui.GetComponent<GUIScript>().TipsList.Insert(this._gui.GetComponent<GUIScript>().TipsList.Count == 0 ? 0 : 1, str);
                else
                    this._gui.GetComponent<GUIScript>().TipsList.Add(str);
            }
        }

        #endregion

        #region 逻辑脚本

        /// <summary>
        /// 当前游戏逻辑
        /// </summary>
        private GameLogic _logic;

        public void SetLogic(GameLogic l)
        {
            this._logic = l;
        }

        public GameLogic GetLogic()
        {
            return this._logic;
        }

        #endregion

        #region 工具

        public void LoadLevel(StayAwayGame.Level level = StayAwayGame.Level.This)
        {
            var p = level == StayAwayGame.Level.This ? (StayAwayGame.Level)this.SavesSystem.GetSavesData(this._currentSavesIndex).Level : level;
            if (p == StayAwayGame.Level.Start)
                SceneManager.LoadScene("level_start");
            else if (p == StayAwayGame.Level.Guide)
                SceneManager.LoadScene("level_guide");
            else if(p == StayAwayGame.Level.Level01)
                SceneManager.LoadScene("level_01");
            else if(p == StayAwayGame.Level.End)
                SceneManager.LoadScene("level_end");
            else
                SceneManager.LoadScene("level_guide");
        }

        #endregion
    }
}