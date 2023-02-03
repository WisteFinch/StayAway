using StayAwayGameScript;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameLevelScript
{
    public class LevelGuideScript : MonoBehaviour
    {

        public GameObject Camera;
        public GameObject Pony;
        public GameObject GUI;
        public float CameraTrackingRatioBeginModify = 0.25f;
        public float CameraTrackingRatioBeginModifyTime = 0;

        private float _oriCameraTrackingRatio;
        private int _gameOverReason;
        private GameLogic GameLogicScript;
        void Start()
        {
            
            this.GameLogicScript = this.Pony.GetComponent<GameLogic>();
            //this._oriCameraTrackingRatio = Camera.GetComponent<CameraController>().TrackingRatio;
            //Camera.GetComponent<CameraController>().TrackingRatio *= this.CameraTrackingRatioBeginModify;
            //Invoke(nameof(RestoreCameraTrackingRatio), this.CameraTrackingRatioBeginModifyTime);

            this.GameLogicScript.FlowGameOverEvent.AddListener(GameOver);

            GameManager.Instance.InitPlayer();

            GameManager.Instance.SetGUI(this.GUI);

            GameManager.Instance.SetLogic(this.GameLogicScript);
        }

        void RestoreCameraTrackingRatio()
        {
            Camera.GetComponent<CameraController>().TrackingRatio = this._oriCameraTrackingRatio;
        }

        #region ½áÊø¶¯»­
        void GameOver(int flag)
        {
            this.GameLogicScript.FlowGameOverEvent.RemoveListener(GameOver);
            this._gameOverReason = flag;
            Invoke(nameof(GameOverP2), 1);
        }

        void GameOverP2()
        {
            this.GameLogicScript.FlowGameOverEvent.RemoveListener(GameOver);

            this.GameLogicScript.FlowGameOver(this._gameOverReason);
        }

        #endregion
    }
}