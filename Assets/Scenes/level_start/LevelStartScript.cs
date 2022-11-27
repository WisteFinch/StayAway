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

        public GameObject UISky, UIBCloud, UIFCloud, UISSky, UISBCloud, UISFCloud, UIButtonStart, UIButtonExit, UIButtonBackToMain;

        Vector3 _UISkyPosOri, _UIBCloudPosOri, _UIFCloudPosOri, _UISSkyPosOri, _UISBCloudPosOri, _UISFCloudPosOri;
        Boolean _enableOffset;

        private void Start()
        {
            this._UISkyPosOri = UISky.transform.position;
            this._UIBCloudPosOri = UIBCloud.transform.position;
            this._UIFCloudPosOri = UIFCloud.transform.position;
            this._UISSkyPosOri = UISSky.transform.position;
            this._UISBCloudPosOri = UISBCloud.transform.position;
            this._UISFCloudPosOri = UISFCloud.transform.position;
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
                UISFCloud.transform.position = this._UISFCloudPosOri + (Vector3)(mouseOffset * 0.5f);
                UISBCloud.transform.position = this._UISBCloudPosOri + (Vector3)(mouseOffset * 0.3f);
                UISSky.transform.position = this._UISSkyPosOri + (Vector3)(mouseOffset * 0.1f);
            }

        }

        public void OnExitButtonDown()
        {
            this.GetComponent<Animator>().SetTrigger("Exit");
        }

        public void OnStartButtonDown()
        {
            this.GetComponent<Animator>().SetTrigger("MainToStart");
        }

        public void OnBackToMainButtonDown()
        {
            this.GetComponent<Animator>().SetTrigger("StartToMain");
        }

        public void EnableOffset()
        {
            this._enableOffset = true;
        }

        public void DisableOffset()
        {
            this._enableOffset = false;
        }

        public void UpdateOffset()
        {
            Vector2 screenSize = new(UnityEngine.Screen.width, UnityEngine.Screen.height);
            Vector2 centre = screenSize / 2f;
            Vector2 mousePos = new(Mathf.Clamp(UnityEngine.Input.mousePosition.x, 0, screenSize.x), Mathf.Clamp(UnityEngine.Input.mousePosition.y, 0, screenSize.y));
            Vector2 mouseOffset = (mousePos - centre) / centre;
            UIFCloud.transform.position = this._UIFCloudPosOri + (Vector3)(mouseOffset * 0.5f);
            UIBCloud.transform.position = this._UIBCloudPosOri + (Vector3)(mouseOffset * 0.3f);
            UISky.transform.position = this._UISkyPosOri + (Vector3)(mouseOffset * 0.1f);
            UISFCloud.transform.position = this._UISFCloudPosOri + (Vector3)(mouseOffset * 0.5f);
            UISBCloud.transform.position = this._UISBCloudPosOri + (Vector3)(mouseOffset * 0.3f);
            UISSky.transform.position = this._UISSkyPosOri + (Vector3)(mouseOffset * 0.1f);
        }

        public void DisableButton()
        {
            this.UIButtonExit.GetComponent<Button>().enabled = false;
            this.UIButtonStart.GetComponent<Button>().enabled = false;
            this.UIButtonBackToMain.GetComponent<Button>().enabled = false;
        }    

        public void EnableButton(int scene)
        {
            if(scene == 0)
            {
                this.UIButtonExit.GetComponent<Button>().enabled = true;
                this.UIButtonStart.GetComponent<Button>().enabled = true;
            }
            else if(scene == 1)
            {
                this.UIButtonBackToMain.GetComponent<Button>().enabled = true;
            }    
        }

        void GameStart()
        {
            //SceneManager.LoadScene("level_guide");
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