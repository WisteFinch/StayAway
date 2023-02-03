using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StayAwayGameScript
{
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// ��ȡ����
        /// </summary>
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// �������
        /// </summary>
        public InputManager Input;

        private void Awake()
        {
            // ��������
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
            // ��ȡ�浵
            this.SavesSystem.LoadSavesData();
            // ��ȡ����
            this.ConfigSystem.LoadConfigData();
            // �����������
            this.Input = this.gameObject.GetComponent<InputManager>();
        }

        #region �浵

        /// <summary>
        /// �浵����
        /// </summary>
        private readonly SavesDataSystem SavesSystem = new();

        /// <summary>
        /// ��ǰ�浵���
        /// </summary>
        private int _currentSavesIndex;

        /// <summary>
        /// ��ȡ�浵����
        /// </summary>
        /// <param name="i">�浵���</param>
        /// <returns>�浵����</returns>
        public SavesDataSystem.SavesData.DataStruct GetSavesData(int index = -1)
        {
            return index == -1 ? this.SavesSystem.GetSavesData(this._currentSavesIndex) : this.SavesSystem.GetSavesData(index);
        }

        /// <summary>
        /// ���õ�ǰ�浵���
        /// </summary>
        /// <param name="index">�浵���</param>
        public void SetSavesIndex(int index)
        {
            this._currentSavesIndex = index;
        }

        /// <summary>
        /// ���ô浵����
        /// </summary>
        /// <param name="data">����</param>
        /// <param name="index">�浵���</param>
        public void SetSavesData(SavesDataSystem.SavesData.DataStruct data, int index = -1)
        {
            if (index == -1)
                this.SavesSystem.SetSavesData(data, this._currentSavesIndex);
            else
                this.SavesSystem.SetSavesData(data, index);
        }

        /// <summary>
        /// ����浵����
        /// </summary>
        public void StoreSavesData()
        {
            this.SavesSystem.StoreSavesData();
        }

        /// <summary>
        /// ���ô浵����
        /// </summary>
        /// <param name="index">�浵���</param>
        public void RestoreSavesData(int i)
        {
            this.SavesSystem.RestoreSavesData(i);
        }

        #endregion

        #region ����

        /// <summary>
        /// ���ù���
        /// </summary>
        private readonly ConfigSystem ConfigSystem = new();

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="c">��������</param>
        /// <returns>��������</returns>
        public object GetConfigData(StayAwayGame.Config c)
        {
            return this.ConfigSystem.GetConfigData(c);
        }

        /// <summary>
        /// �洢��������
        /// </summary>
        public void StoreConfigData()
        {
            this.ConfigSystem.StoreConfigData();
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="c">��������</param>
        /// <param name="">��ֵ</param>
        /// <returns></returns>
        public void SetConfigData(StayAwayGame.Config c, object v)
        {
            this.ConfigSystem.SetConfigData(c, v);
            ConfigAudioChangeEvent();
        }

        /// <summary>
        /// ��Ƶ����ֵ����ί��
        /// </summary>
        public Action ConfigAudioChangeEvent;

        #endregion

        #region Ƥ��

        /// <summary>
        /// Ƥ������
        /// </summary>
        public SkinSystem Skins;

        /// <summary>
        /// �ؿ���ʼ����ʼ����ɫ
        /// </summary>
        public void InitPlayer()
        {
            print($"Current Saves Index: {this._currentSavesIndex}");
            // ����С��Ƥ��
            GameObject.FindGameObjectWithTag("Pony").GetComponentInChildren<Animator>().runtimeAnimatorController = this.Skins.GetAnimator(this.SavesSystem.GetSavesData(this._currentSavesIndex).Skin);
        }

        #endregion

        #region GUI

        /// <summary>
        /// ��ǰGUI
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

        #region �߼��ű�

        /// <summary>
        /// ��ǰ��Ϸ�߼�
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

        #region ����

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