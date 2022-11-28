using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameScript
{
    public class GameManager : MonoBehaviour
    {
        #region ��幫�б���

        #endregion

        /// <summary>
        /// ��ȡ����
        /// </summary>
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// �浵����
        /// </summary>
        public PlayerDataSystem PlayerSystem = new();

        /// <summary>
        /// Ƥ������
        /// </summary>
        public SkinSystem Skins;

        /// <summary>
        /// ��ǰ������
        /// </summary>
        private int _currentPlayerIndex;

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
            this.PlayerSystem.LoadPlayerData();
        }

        /// <summary>
        /// ��ȡ�����Ϣ
        /// </summary>
        /// <param name="i">�浵���</param>
        /// <returns></returns>
        public PlayerDataSystem.PlayerData.DataStruct GetPlayerData(int index)
        {
            return this.PlayerSystem.GetPlayerData(index);
        }

        /// <summary>
        /// ���õ�ǰ������
        /// </summary>
        /// <param name="index"></param>
        public void SetPlayerIndex(int index)
        {
            this._currentPlayerIndex = index;
        }

        /// <summary>
        /// ���������Ϣ
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        public void SetPlayerData(PlayerDataSystem.PlayerData.DataStruct data, int index)
        {
            this.PlayerSystem.SetPlayerData(data, index);
        }

        /// <summary>
        /// ���������Ϣ
        /// </summary>
        public void SavePlayerData()
        {
            this.PlayerSystem.SavePlayerData();
        }

        /// <summary>
        /// �ؿ���ʼ����ʼ����ɫ
        /// </summary>
        public void InitPlayer()
        {
            // ����С��Ƥ��
            GameObject.FindGameObjectWithTag("Pony").GetComponentInChildren<Animator>().runtimeAnimatorController = this.Skins.GetAnimator(this.PlayerSystem.GetPlayerData(this._currentPlayerIndex).Skin);
        }
    }
}