using UnityEngine;
using UnityEngine.SceneManagement;

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
        /// �������
        /// </summary>
        public InputManager Input;

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

            this.Input = this.gameObject.GetComponent<InputManager>();
        }

        /// <summary>
        /// ��ȡ�����Ϣ
        /// </summary>
        /// <param name="i">�浵���</param>
        /// <returns></returns>
        public PlayerDataSystem.PlayerData.DataStruct GetPlayerData(int index = -1)
        {
            return index == -1 ? this.PlayerSystem.GetPlayerData(this._currentPlayerIndex) : this.PlayerSystem.GetPlayerData(index);
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
        public void SetPlayerData(PlayerDataSystem.PlayerData.DataStruct data, int index = -1)
        {
            if(index == -1)
                this.PlayerSystem.SetPlayerData(data, this._currentPlayerIndex);
            else
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
            print(_currentPlayerIndex);
            // ����С��Ƥ��
            GameObject.FindGameObjectWithTag("Pony").GetComponentInChildren<Animator>().runtimeAnimatorController = this.Skins.GetAnimator(this.PlayerSystem.GetPlayerData(this._currentPlayerIndex).Skin);
        }

        public void LoadLevel(int l = -1)
        {
            var p = l == -1 ? this.PlayerSystem.GetPlayerData(this._currentPlayerIndex).Progress : l;
            if (p == 0)
                SceneManager.LoadScene("level_guide");
            else if(p == 1)
                SceneManager.LoadScene("level_01");
            else if(p == 2)
                SceneManager.LoadScene("level_end");
            else
                SceneManager.LoadScene("level_guide");
        }
    }
}