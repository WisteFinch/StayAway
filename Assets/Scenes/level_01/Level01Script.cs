using StayAwayGameScript;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace StayAwayGameLevelScript
{
    public class Level01Script : MonoBehaviour
    {
        public GameObject Camera;
        public GameObject Pony;
        public GameObject Soul;
        public GameObject Crystal;
        public GameObject GUI;
        public GameObject UISkip;
        public Vector2 SoulOffset = new(1, 2);

        private Boolean _enableSkip = false;
        private GameLogic GameLogicScript;
        private int _gameOverReason;
        void Start()
        {
            this.GameLogicScript = this.Pony.GetComponent<GameLogic>();
            this.Soul.GetComponent<SoulController>().EnableForzen = true;
            this.GameLogicScript.GameOverEvent.AddListener(GameOver);
            Invoke(nameof(BeginAnimationP1), 1f);
            Invoke(nameof(SetControlLock), 0.1f);
            GameManager.Instance.InitPlayer();
        }

        private void Update()
        {
            this.Soul.transform.position = -this.Pony.transform.position + (Vector3)this.SoulOffset;
            if(this._enableSkip)
            {
                if(UnityEngine.Input.GetKeyDown(KeyCode.E))
                {
                    DisableSkip();
                }
            }
        }

        public void MaskController(int area)
        {
            print($"enter {area}");
            if(area == 0)
            {
                this.Camera.GetComponent<Camera>().cullingMask |= (1 << LayerMask.NameToLayer("Hide01"));
                this.Camera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Hide02"));
                this.Camera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Hide03"));
            }
            else if (area == 1)
            {
                this.Camera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Hide01"));
                this.Camera.GetComponent<Camera>().cullingMask |= (1 << LayerMask.NameToLayer("Hide02"));
                this.Camera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Hide03"));
            }
            else if (area == 2)
            {
                this.Camera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Hide01"));
                this.Camera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Hide02"));
                this.Camera.GetComponent<Camera>().cullingMask |= (1 << LayerMask.NameToLayer("Hide03"));
            }
            else
            {
                this.Camera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Hide01"));
                this.Camera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Hide02"));
                this.Camera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Hide03"));
            }
        }

        #region ��������
        void BeginAnimationP1()
        {
            this.UISkip.SetActive(true);
            this._enableSkip = true;
            this.Pony.GetComponent<PonyAnimationHandler>().ShakeHead();
            Invoke(nameof(BeginAnimationP2), 2f);
        }
        void BeginAnimationP2()
        {
            this.Camera.GetComponent<CameraController>().SetTarget(this.Soul);
            Invoke(nameof(BeginAnimationP3), 3f);
        }

        void BeginAnimationP3()
        {
            this.Camera.GetComponent<CameraController>().SetTarget(this.Crystal);
            Invoke(nameof(BeginAnimationP4), 1f);
        }

        void BeginAnimationP4()
        {
            this.GUI.GetComponent<GUIScript>().PlayText("һ�����ص�����\n����ͽ������", false, false);
            Invoke(nameof(BeginAnimationP4_1), 5f);
        }

        void BeginAnimationP4_1()
        {
            this.GUI.GetComponent<GUIScript>().PlayText("������걻��������\n�����ڵ��ƶ�������ȫ����", false, false);
            Invoke(nameof(BeginAnimationP5), 5f);
        }

        void BeginAnimationP5()
        {
            this.GUI.GetComponent<GUIScript>().PlayText("ǰ������������Դͷ\n�������ҵ����Ƚ��ķ���", false, false);
            Invoke(nameof(BeginAnimationP6), 5f);
        }
        void BeginAnimationP6()
        {
            this.GUI.GetComponent<GUIScript>().PlayText("ǰ����������\nÿһ������Ҫ���ؿ���", false, false);
            this.Camera.GetComponent<CameraController>().Resize(11);
            Invoke(nameof(BeginAnimationP7), 5f);
        }

        void BeginAnimationP7()
        {
            this.Camera.GetComponent<CameraController>().Resize(2);
            this.Camera.GetComponent<CameraController>().SetTarget(this.Pony);
            this.Pony.GetComponent<GameLogic>().SetControllerLock(false);
            DisableSkip();
        }

        void DisableSkip()
        {
            this._enableSkip = false;
            CancelInvoke();
            this.Camera.GetComponent<CameraController>().Resize(2);
            this.Camera.GetComponent<CameraController>().SetTarget(this.Pony);
            this.Pony.GetComponent<GameLogic>().SetControllerLock(false);
            this.Pony.GetComponentInChildren<TrailRenderer>().Clear();
            this.Soul.GetComponentInChildren<TrailRenderer>().Clear();
            Destroy(this.UISkip);
        }

        void SetControlLock()
        {
            this.GameLogicScript.SetControllerLock(true);
            this.GameLogicScript.ClearEffect();
        }

        #endregion

        #region ��������
        void GameOver(int flag)
        {
            this.GameLogicScript.GameOverEvent.RemoveListener(GameOver);
            this._gameOverReason = flag;

            if(flag == 2)
            {
                this.Camera.GetComponent<CameraController>().SetTarget(this.Soul);
            }
            else
            {
                this.Camera.GetComponent<CameraController>().SetTarget(this.Pony);
            }

            Invoke(nameof(GameOverP2), 2);
        }

        void GameOverP2()
        {
            this.Camera.GetComponent<CameraController>().SetTarget(this.Crystal);
            this.Camera.GetComponent<CameraController>().Resize(11);
            this.Camera.GetComponent<Camera>().cullingMask |= (1 << LayerMask.NameToLayer("Hide01")) + (1 << LayerMask.NameToLayer("Hide02")) + (1 << LayerMask.NameToLayer("Hide03")) + (1 << LayerMask.NameToLayer("Hide04"));
            Invoke(nameof(GameOverP3), 5);
        }

        void GameOverP3()
        {
            this.Camera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Hide04"));
            this.GameLogicScript.GameOver(this._gameOverReason);
        }
        #endregion
    }
}