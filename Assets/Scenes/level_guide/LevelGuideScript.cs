using StayAwayGameScript;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameLevelScript
{
    public class LevelGuideScript : MonoBehaviour
    {

        public GameObject Camera;
        public float CameraTrackingRatioBeginModify = 0.25f;
        public float CameraTrackingRatioBeginModifyTime = 3;

        private float _oriCameraTrackingRatio;
        void Start()
        {
            this._oriCameraTrackingRatio = Camera.GetComponent<CameraController>().TrackingRatio;
            Camera.GetComponent<CameraController>().TrackingRatio *= this.CameraTrackingRatioBeginModify;
            Invoke(nameof(RestoreCameraTrackingRatio), this.CameraTrackingRatioBeginModifyTime);
            GameManager.Instance.InitPlayer();
            
        }

        void RestoreCameraTrackingRatio()
        {
            Camera.GetComponent<CameraController>().TrackingRatio = this._oriCameraTrackingRatio;
        }
    }
}