using StayAwayGameScript;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace StayAwayGameLevelScript
{
    public class Level01Script : MonoBehaviour
    {
        public GameObject Camera;
        public GameObject Pony;
        public GameObject Soul;
        public GameObject Crystal;
        public CircleRender CrystalCircle;
        public GameObject GUI;
        public GameObject UISkip;
        public GameObject Door;
        public GameObject DoorTriggr;
        public GameObject Boom;
        public Volume Volume;
        public Vector2 SoulOffset = new(1, 2);

        private Boolean _enableSkip = false;
        private GameLogic GameLogicScript;
        private int _gameOverReason;
        private float _closeDoorTime = -1;
        private Vector3 _oriDoorPos;
        private Vector3 _targetDoorPos;

        private float _cryCircleR = 0;
        private float _cryCircleMR = 10;
        private float _cryCircleDR = 5;
        private Boolean _cryCirclePushAwayUsability;
        private Bloom _volumeBM;
        private Boolean _enableCircle = false;
        void Start()
        {
            this._oriDoorPos = this.Door.transform.position;
            this._targetDoorPos = this._oriDoorPos + new Vector3(0, -2.5f, 0);
            this.Volume.profile.TryGet<Bloom>(out this._volumeBM);

            this.GameLogicScript = this.Pony.GetComponent<GameLogic>();
            this.CrystalCircle = this.Crystal.GetComponent<CircleRender>();
            this.Soul.GetComponent<SoulController>().EnableForzen = true;
            this.GameLogicScript.GameOverEvent.AddListener(GameOver);
            Invoke(nameof(BeginAnimationP1), 1f);
            Invoke(nameof(SetControlLock), 0.1f);

            this.CrystalCircle.Enable = false;

            var d = GameManager.Instance.GetPlayerData();
            d.Progress = 1;
            GameManager.Instance.SetPlayerData(d);

            GameManager.Instance.InitPlayer();
            MaskController(-1);
        }

        private void Update()
        {
            this.Soul.transform.position = -this.Pony.transform.position + (Vector3)this.SoulOffset;
            if(this._enableSkip)
            {
                if(GameManager.Instance.Input.InteractKeyDown)
                {
                    DisableSkip();
                }
            }
            if(_closeDoorTime >= 0 && _closeDoorTime <1)
            {
                this._closeDoorTime += Time.deltaTime;
                if(this._closeDoorTime > 1)
                {
                    CloseDoorP2();
                    return;
                }
                this.Door.transform.position = Vector3.Lerp(this._oriDoorPos, this._targetDoorPos, this._closeDoorTime);
            }
            if(this._enableCircle)
            {
                this._cryCircleR += Time.deltaTime * this._cryCircleDR;
                if(this._cryCircleR > this._cryCircleMR)
                {
                    this._cryCircleR = 0.5f;
                    this._cryCirclePushAwayUsability = true;
                    this.Crystal.GetComponentInChildren<AudioSource>().Play();
                }
                this.CrystalCircle.R = this._cryCircleR;
                this.Crystal.GetComponent<LineRenderer>().startWidth = this.Crystal.GetComponent<LineRenderer>().endWidth = Mathf.Lerp(0.5f, 0.05f, this._cryCircleR / this._cryCircleMR);

                var dist = Vector3.Distance(this.Crystal.transform.position, this.Pony.transform.position) / 2;
                if (this._cryCirclePushAwayUsability && Mathf.Abs(dist - this._cryCircleR) < 0.1f)
                {
                    this.Pony.GetComponent<PonyController>().Velocity += (Vector2)(this.Pony.transform.position - this.Crystal.transform.position).normalized * 10 * Mathf.Lerp(1.5f, 0.2f, dist / 7f);
                    this._cryCirclePushAwayUsability = false;
                }

                this._volumeBM.intensity.SetValue(new FloatParameter(Mathf.Clamp((5 - dist) * 5, 0, 100f)));
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
                this.Camera.GetComponent<Camera>().cullingMask |= (1 << LayerMask.NameToLayer("Hide01"));
                this.Camera.GetComponent<Camera>().cullingMask |= (1 << LayerMask.NameToLayer("Hide02"));
                this.Camera.GetComponent<Camera>().cullingMask |= (1 << LayerMask.NameToLayer("Hide03"));
            }
        }

        #region 关门动画
        public void CloseDoor()
        {
            this._closeDoorTime = 0;
            this.GameLogicScript.EnableChangeCharacter = false;
            this.GameLogicScript.ChangeCharacter(true);
            this.GameLogicScript.SetControllerLock(true);
            this.Door.GetComponentInChildren<AudioSource>().Play();
            Destroy(this.DoorTriggr.GetComponent<BoxCollider2D>());
        }

        public void CloseDoorP2()
        {
            this.GameLogicScript.SetControllerLock(false);
            this.Camera.GetComponent<CameraController>().SetTarget(this.Crystal);
            this.Camera.GetComponent<CameraController>().Resize(5);
            this.CrystalCircle.Enable = true;
            this._cryCirclePushAwayUsability = true;
            this.GameLogicScript.EnableEffect = false;
            this._enableCircle = true;
            this.Crystal.GetComponentInChildren<AudioSource>().Play();
        }
        #endregion

        #region 开场动画
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
            this.GUI.GetComponent<GUIScript>().PlayText("一股神秘的力量\n将你和姐姐连结", false, false);
            Invoke(nameof(BeginAnimationP4_1), 5f);
        }

        void BeginAnimationP4_1()
        {
            this.GUI.GetComponent<GUIScript>().PlayText("姐姐的灵魂被力量囚禁\n她现在的移动与你完全反向", false, false);
            Invoke(nameof(BeginAnimationP5), 5f);
        }

        void BeginAnimationP5()
        {
            this.GUI.GetComponent<GUIScript>().PlayText("前往神秘力量的源头\n或许能找到拯救姐姐的方法", false, false);
            Invoke(nameof(BeginAnimationP6), 5f);
        }
        void BeginAnimationP6()
        {
            this.GUI.GetComponent<GUIScript>().PlayText("前方机关重重\n每一步都需要慎重考虑", false, false);
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

        #region 结束动画
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

        #region 胜利
        public void Win()
        {
            Destroy(this.Crystal.GetComponent<CircleCollider2D>());

            this._enableCircle = false;
            this.GameLogicScript.SetControllerLock(true);

            this.GameLogicScript.SetAudioPitch(0.05f);

            this.Camera.GetComponent<CameraController>().Resize(0);

            Invoke(nameof(WinP2), 1f);
        }

        void WinP2()
        {
            var boom = Instantiate(this.Boom);
            boom.transform.position = this.Crystal.transform.position;
            this.Crystal.SetActive(false);
            this.CrystalCircle.Enable = false;
            Invoke(nameof(WinP3), 2f);
        }

        void WinP3()
        {
            this.GameLogicScript.GameOver(4);
        }    

        #endregion
    }
}