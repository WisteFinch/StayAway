using StayAwayGameScript;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StayAwayGameLevelScript
{
    public class LevelStartScript : MonoBehaviour
    {
        public float OffsetRatio = 0.5f;

        public GameObject UISky, UIBCloud, UIFCloud, UISSky, UISBCloud, UISFCloud, UIButtonStart, UIButtonExit, UIButtonBackToMain;
        public GameObject UIS1I, UIS1N, UIS1D, UIS1O, UIS1OT;
        public GameObject UIS2I, UIS2N, UIS2D, UIS2O, UIS2OT;
        public GameObject UIS3I, UIS3N, UIS3D, UIS3O, UIS3OT;
        public GameObject UISkinPreview;

        public Color UISavesOptionNewButton, UISavesOptionDeleteButton;

        public string UISavesOptionNewText = "Empty", UISavesOptionDeleteText = "Delete", UISavesNameText = "Anon", UISavesDateText = "Unknown";

        public Sprite UISavesImageDefault;

        Vector3 _UISkyPosOri, _UIBCloudPosOri, _UIFCloudPosOri, _UISSkyPosOri, _UISBCloudPosOri, _UISFCloudPosOri;
        Boolean _enableOffset;

        PlayerDataSystem.PlayerData.DataStruct _currentPlayerSave;
        int _currentPlayerIndex;

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
                UpdateOffset();
            }

        }

        public void OnExitButtonDown()
        {
            this.GetComponent<Animator>().SetTrigger("Exit");
        }

        public void OnPlayButtonDown()
        {
            LoadPlayer(1);
            LoadPlayer(2);
            LoadPlayer(3);
            this.GetComponent<Animator>().SetTrigger("MainToSaves");
        }

        public void OnBackToMainButtonDown()
        {
            this.GetComponent<Animator>().SetTrigger("SavesToMain");
        }

        public void OnBackToSavesButtonDown()
        {
            this.GetComponent<Animator>().SetTrigger("StartToSaves");
        }
        public void OnSelectButtonDown(int index)
        {
            if (GameManager.Instance.PlayerSystem.GetPlayerData(index).Enable)
            {
                this._currentPlayerIndex = index;
                this._currentPlayerSave = GameManager.Instance.PlayerSystem.GetPlayerData(index);
                this.GetComponent<Animator>().SetTrigger("Start");
            }
            else
            {
                this._currentPlayerSave = new();
                this._currentPlayerIndex = index;
                UpdateSkinPreview();
                this.GetComponent<Animator>().SetTrigger("SavesToStart");
            }
            
        }
        public void OnSavesOptionButtonDown(int index)
        {
            if (GameManager.Instance.PlayerSystem.GetPlayerData(index).Enable)
            {
                GameManager.Instance.PlayerSystem.RestorePlayerData(index);
                LoadPlayer(index);
            }
            else
            {
                OnSelectButtonDown(index);
            }
        }

        public void OnSwitchSkinPreview(bool flag)
        {
            if(flag)
            {
                this._currentPlayerSave.Skin--;
                if(this._currentPlayerSave.Skin<0)
                {
                    this._currentPlayerSave.Skin = GameManager.Instance.Skins.GetCount() - 1;
                }
            }
            else
            {
                this._currentPlayerSave.Skin++;
                if (this._currentPlayerSave.Skin >= GameManager.Instance.Skins.GetCount())
                {
                    this._currentPlayerSave.Skin = 0;
                }
            }
            UpdateSkinPreview();
        }
        
        public void OnStartButtonDown()
        {
            this.GetComponent<Animator>().SetTrigger("Start");
        }

        public void UpdateSkinPreview()
        {
            this.UISkinPreview.GetComponent<Image>().sprite = GameManager.Instance.Skins.GetIdles(this._currentPlayerSave.Skin);
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
            UIFCloud.transform.position = this._UIFCloudPosOri + (Vector3)(0.05f * this.OffsetRatio * mouseOffset * screenSize);
            UIBCloud.transform.position = this._UIBCloudPosOri + (Vector3)(0.03f * this.OffsetRatio * mouseOffset * screenSize);
            UISky.transform.position = this._UISkyPosOri + (Vector3)(0.01f * this.OffsetRatio * mouseOffset * screenSize);
            UISFCloud.transform.position = this._UISFCloudPosOri + (Vector3)(0.05f * this.OffsetRatio * mouseOffset * screenSize);
            UISBCloud.transform.position = this._UISBCloudPosOri + (Vector3)(0.03f * this.OffsetRatio * mouseOffset * screenSize);
            UISSky.transform.position = this._UISSkyPosOri + (Vector3)(0.01f * this.OffsetRatio * mouseOffset * screenSize);
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

        public void GameStart()
        {
            this._currentPlayerSave.Enable = true;
            this._currentPlayerSave.Date = System.DateTime.Now.Year.ToString() + "??" + System.DateTime.Now.Month.ToString() + "??" + System.DateTime.Now.Day.ToString() + "??" + System.DateTime.Now.Hour.ToString() + "??" + System.DateTime.Now.Minute.ToString() + "??" + System.DateTime.Now.Second.ToString() + "??";
            GameManager.Instance.SetPlayerIndex(this._currentPlayerIndex);
            GameManager.Instance.SetPlayerData(this._currentPlayerSave, _currentPlayerIndex);
            GameManager.Instance.PlayerSystem.SavePlayerData();
            GameManager.Instance.LoadLevel();
        }

        public void GameExit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        public void LoadPlayer(int i)
        {
            var data = GameManager.Instance.GetPlayerData(i);
            if (i == 1)
            {
                if (data.Enable)
                {
                    this.UIS1I.GetComponent<Image>().sprite = GameManager.Instance.Skins.GetIdles(data.Skin);
                    this.UIS1N.GetComponent<Text>().text = data.PlayerName == String.Empty ? this.UISavesNameText : data.PlayerName;
                    this.UIS1D.GetComponent<Text>().text = data.Date == String.Empty ? this.UISavesDateText : data.Date;
                    this.UIS1O.GetComponent<Image>().color = this.UISavesOptionDeleteButton;
                    this.UIS1OT.GetComponent<Text>().text = this.UISavesOptionDeleteText;
                }
                else
                {
                    this.UIS1I.GetComponent<Image>().sprite = this.UISavesImageDefault;
                    this.UIS1N.GetComponent<Text>().text = this.UISavesNameText;
                    this.UIS1D.GetComponent<Text>().text = this.UISavesDateText;
                    this.UIS1O.GetComponent<Image>().color = this.UISavesOptionNewButton;
                    this.UIS1OT.GetComponent<Text>().text = this.UISavesOptionNewText;
                }
            }
            else if (i == 2)
            {
                if (data.Enable)
                {
                    this.UIS2I.GetComponent<Image>().sprite = GameManager.Instance.Skins.GetIdles(data.Skin);
                    this.UIS2N.GetComponent<Text>().text = data.PlayerName == String.Empty ? this.UISavesNameText : data.PlayerName;
                    this.UIS2D.GetComponent<Text>().text = data.Date == String.Empty ? this.UISavesDateText : data.Date;
                    this.UIS2O.GetComponent<Image>().color = this.UISavesOptionDeleteButton;
                    this.UIS2OT.GetComponent<Text>().text = this.UISavesOptionDeleteText;
                }
                else
                {
                    this.UIS2I.GetComponent<Image>().sprite = this.UISavesImageDefault;
                    this.UIS2N.GetComponent<Text>().text = this.UISavesNameText;
                    this.UIS2D.GetComponent<Text>().text = this.UISavesDateText;
                    this.UIS2O.GetComponent<Image>().color = this.UISavesOptionNewButton;
                    this.UIS2OT.GetComponent<Text>().text = this.UISavesOptionNewText;
                }
            }
            else if (i == 3)
            {
                if (data.Enable)
                {
                    this.UIS3I.GetComponent<Image>().sprite = GameManager.Instance.Skins.GetIdles(data.Skin);
                    this.UIS3N.GetComponent<Text>().text = data.PlayerName == String.Empty ? this.UISavesNameText : data.PlayerName;
                    this.UIS3D.GetComponent<Text>().text = data.Date == String.Empty ? this.UISavesDateText : data.Date;
                    this.UIS3O.GetComponent<Image>().color = this.UISavesOptionDeleteButton;
                    this.UIS3OT.GetComponent<Text>().text = this.UISavesOptionDeleteText;
                }
                else
                {
                    this.UIS3I.GetComponent<Image>().sprite = this.UISavesImageDefault;
                    this.UIS3N.GetComponent<Text>().text = this.UISavesNameText;
                    this.UIS3D.GetComponent<Text>().text = this.UISavesDateText;
                    this.UIS3O.GetComponent<Image>().color = this.UISavesOptionNewButton;
                    this.UIS3OT.GetComponent<Text>().text = this.UISavesOptionNewText;
                }
            }
        }
    }
}