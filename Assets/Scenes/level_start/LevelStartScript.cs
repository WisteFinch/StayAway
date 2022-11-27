using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StayAwayGameLevelStart
{
    public class LevelStartScript : MonoBehaviour
    {

        public GameObject UISky, UIBCloud, UIFCloud, UIButtonStart, UIButtonExit;

        Vector3 _UISkyPosOri, _UIBCloudPosOri, _UIFCloudPosOri;
        Boolean _enableOffset;

        private void Start()
        {
            this._UISkyPosOri = UISky.transform.position;
            this._UIBCloudPosOri = UIBCloud.transform.position;
            this._UIFCloudPosOri = UIFCloud.transform.position;
            this._enableOffset = true;
        }

        void Update()
        {
            if (this._enableOffset)
            {
                Vector2 screenSize = new(UnityEngine.Screen.width, UnityEngine.Screen.height);
                Vector2 centre = screenSize / 2f;
                Vector2 mousePos = new(Mathf.Clamp(UnityEngine.Input.mousePosition.x, 0, screenSize.x), Mathf.Clamp(UnityEngine.Input.mousePosition.y, 0, screenSize.y));
                Vector2 mouseOffset = (mousePos - centre) / centre;

                UIFCloud.transform.position = this._UIFCloudPosOri + (Vector3)(mouseOffset * 0.5f);
                UIBCloud.transform.position = this._UIBCloudPosOri + (Vector3)(mouseOffset * 0.3f);
                UISky.transform.position = this._UISkyPosOri + (Vector3)(mouseOffset * 0.1f);
            }
        }

        public void OnExitButtonDown()
        {
            this._enableOffset = false;
            this.UIButtonExit.GetComponent<Button>().enabled = false;
            this.UIButtonStart.GetComponent<Button>().enabled = false;
            this.GetComponent<Animator>().SetTrigger("Exit");
            Invoke(nameof(GameExit), 2f);
        }

        public void OnStartButtonDown()
        {
            this._enableOffset = false;
            this.UIButtonExit.GetComponent<Button>().enabled = false;
            this.UIButtonStart.GetComponent<Button>().enabled = false;
            this.GetComponent<Animator>().SetTrigger("Exit");
            Invoke(nameof(GameStart), 2f);
        }

        void GameStart()
        {
            SceneManager.LoadScene("level_guide");
        }

        void GameExit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}