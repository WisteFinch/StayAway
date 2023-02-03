using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameScript
{
    public class AudioController : MonoBehaviour
    {
        public StayAwayGame.AudioType AudioType;

        private void Start()
        {
            GameManager.Instance.ConfigAudioChangeEvent += OnConfigChange;
            OnConfigChange();
        }

        public void OnConfigChange()
        {
            if (this.AudioType == StayAwayGame.AudioType.Effect)
                this.GetComponent<AudioSource>().volume = (float)GameManager.Instance.GetConfigData(StayAwayGame.Config.AudioMain) * (float)GameManager.Instance.GetConfigData(StayAwayGame.Config.AudioEffect);
            else
                this.GetComponent<AudioSource>().volume = (float)GameManager.Instance.GetConfigData(StayAwayGame.Config.AudioMain) * (float)GameManager.Instance.GetConfigData(StayAwayGame.Config.AudioBGM);
        }

        void OnDestroy()
        {
            GameManager.Instance.ConfigAudioChangeEvent -= OnConfigChange;
        }
    }
}