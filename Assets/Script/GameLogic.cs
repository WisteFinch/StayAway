using StayAwayGameScript;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace StayAwayGameScript
{
    public class GameLogic : MonoBehaviour
    {
        public struct FrameInput
        {
            /// <summary>
            /// 切换角色
            /// </summary>
            public bool ChangeCharacter;
        }

        #region 公有变量
        [Header("对象")]
        /// <summary>
        /// 小马对象
        /// </summary>
        public UnityEngine.Object Pony;
        /// <summary>
        /// 灵魂对象
        /// </summary>
        public UnityEngine.Object Soul;
        /// <summary>
        /// 主摄像机
        /// </summary>
        public Camera MainCamera;
        /// <summary>
        /// 主渲染
        /// </summary>
        public UnityEngine.Object GlobalVolume;

        [Header("参数")]
        /// <summary>
        /// 最小小马至灵魂距离
        /// </summary>
        public float MinimalPonyToSoulDistance = 1f;
        /// <summary>
        /// 最大小马至灵魂距离
        /// </summary>
        public float MaximalPonyToSoulDistance = 5f;
        /// <summary>
        /// 过近死亡时间
        /// </summary>
        public float TooCloseDeadTime = 2f;
        /// <summary>
        /// 过近死亡衰减
        /// </summary>
        public float TooCloseDeadDecayTime = 4f;
        /// <summary>
        /// 过远死亡时间
        /// </summary>
        public float TooFarDeadTime = 2f;
        /// <summary>
        /// 过远死亡衰减
        /// </summary>
        public float TooFarDeadDecayTime = 4f;

        [Header("视角")]
        public float CameraSize = 30f;

        [Header("特效")]
        /// <summary>
        /// 产生幻觉特效距离
        /// </summary>
        public float IllusionDistance = 2f;
        /// <summary>
        /// 幻觉特效强度
        /// </summary>
        public float IllusionWeight = 1f;
        /// <summary>
        /// 黑视特效强度
        /// </summary>
        public float BlackoutWeight = 1f;
        /// <summary>
        /// 褪色特效距离
        /// </summary>
        public float FadingDistance = 2f;
        /// <summary>
        /// 褪色特效强度
        /// </summary>
        public float FadingWeight = 0.8f;
        /// <summary>
        /// 消逝特效强度
        /// </summary>
        public float VanishWeight = 0.2f;
        /// <summary>
        /// 距离环默认色彩
        /// </summary>
        public Color DistanceCircleDefaultColor = new(255, 255, 255);
        /// <summary>
        /// 距离环危险色彩
        /// </summary>
        public Color DistanceCircleDangerColor = new(255, 100, 100);
        /// <summary>
        /// 距离环透明度
        /// </summary>
        public float DistanceCirclePellucidity = 0.5f;
        /// <summary>
        /// 距离环显示距离
        /// </summary>
        public float DistanceCircleDisplayDistance = 2;


        #endregion

        #region 私有变量
        /// <summary>
        /// 输入
        /// </summary>
        private FrameInput input;
        /// <summary>
        /// 小马变形器
        /// </summary>
        private Transform _ponyTransform;
        /// <summary>
        /// 灵魂变形器
        /// </summary>
        private Transform _soulTransform;
        /// <summary>
        /// 小马至灵魂距离
        /// </summary>
        private float _ponyToSoulDistance;
        /// <summary>
        /// 后期处理
        /// </summary>
        private Volume _volume;
        /// <summary>
        /// 后期处理色差
        /// </summary>
        private ChromaticAberration _volumeCHA;
        /// <summary>
        /// 后期渐晕
        /// </summary>
        private Vignette _volumeVGN;
        /// <summary>
        /// 后期色彩修正
        /// </summary>
        private ColorAdjustments _volumeCAJ;
        /// <summary>
        /// 后期高光
        /// </summary>
        private Bloom _volumeBM;
        /// <summary>
        /// 过近死亡度
        /// </summary>
        private float _tooCloseDeadRatio;
        /// <summary>
        /// 过远死亡度
        /// </summary>
        private float _tooFarDeadRatio;
        /// <summary>
        /// 当前角色
        /// </summary>
        private Boolean _currentCharacter;
        /// <summary>
        /// 距离环脚本
        /// </summary>
        private CircleRender _distanceCircleScript;
        #endregion

        void Start()
        {
            // 获取组件
            this._ponyTransform = this.Pony.GetComponent<Transform>();
            this._soulTransform = this.Soul.GetComponent<Transform>();
            this._volume = this.GlobalVolume.GetComponent<Volume>();
            this._volume.profile.TryGet<ChromaticAberration>(out this._volumeCHA);
            this._volume.profile.TryGet<Vignette>(out this._volumeVGN);
            this._volume.profile.TryGet<ColorAdjustments>(out this._volumeCAJ);
            this._volume.profile.TryGet<Bloom>(out this._volumeBM);
            this._distanceCircleScript = this.GetComponentInChildren<CircleRender>();

            // 初始化
            this.MainCamera.GetComponent<Camera>().fieldOfView = this.CameraSize;
            this._currentCharacter = true;
            this.Pony.GetComponent<PonyController>().EnableControl = true;
            this.Soul.GetComponent<SoulController>().EnableControl = false;
            this.MainCamera.GetComponent<CameraController>().SetTarget(this.Pony);
        }

        void Update()
        {
            GatherInput();
            CalcDistance();
            CalcView();
        }

        /// <summary>
        /// 获取输入
        /// </summary>
        void GatherInput()
        {
            this.input.ChangeCharacter = UnityEngine.Input.GetKeyDown(KeyCode.R);
        }

        /// <summary>
        /// 距离计算
        /// </summary>
        void CalcDistance()
        {
            // 计算距离
            this._ponyToSoulDistance = Vector2.Distance(this._ponyTransform.position, this._soulTransform.position);
            // 小马视角：小马离灵魂过近，黑视
            if (this._ponyToSoulDistance <= this.MinimalPonyToSoulDistance)
            {
                if (this._tooCloseDeadRatio >= 1)
                {
                    // ================= Pony Dead =================
                }
                else
                {
                    this._tooCloseDeadRatio += Time.deltaTime / this.TooCloseDeadTime;
                }
            }
            else
            {
                if (this._tooCloseDeadRatio > 0)
                {
                    this._tooCloseDeadRatio -= Time.deltaTime / this.TooCloseDeadDecayTime;
                } 
                else if(this._tooCloseDeadRatio < 0)
                {
                    this._tooCloseDeadRatio = 0;
                }
            }
            if (_currentCharacter)
            {
                this._volumeVGN.intensity.SetValue(new FloatParameter(Mathf.Clamp(this._tooCloseDeadRatio * this.BlackoutWeight, 0, 1f)));
            }
            else
            {
                this._volumeVGN.intensity.SetValue(new FloatParameter(0));
            }

            // 小马视角：小马离灵魂近，显示幻觉
            if (this._currentCharacter)
            {
                if (this._ponyToSoulDistance <= this.MinimalPonyToSoulDistance + this.IllusionDistance)
                {
                    this._volumeCHA.intensity.SetValue(new FloatParameter(Mathf.Clamp((this.MinimalPonyToSoulDistance + this.IllusionDistance - this._ponyToSoulDistance) * this.IllusionWeight / this.IllusionDistance, 0, 1f)));
                }
            }
            else
            {
                this._volumeCHA.intensity.SetValue(new FloatParameter(0));
            }

            // 灵魂视角：小马离灵魂远近，消逝
            if (this._ponyToSoulDistance >= this.MaximalPonyToSoulDistance)
            {
                if (this._tooFarDeadRatio >= 1)
                {
                    // ================= Soul Dead =================
                }
                else
                {
                    this._tooFarDeadRatio += Time.deltaTime / this.TooFarDeadTime;
                }
            }
            else
            {
                if (this._tooFarDeadRatio > 0)
                {
                    this._tooFarDeadRatio -= Time.deltaTime / this.TooFarDeadDecayTime;
                }
                else if (this._tooFarDeadRatio < 0)
                {
                    this._tooFarDeadRatio = 0;
                }
            }
            if (!_currentCharacter)
            {
                this._volumeBM.intensity.SetValue(new FloatParameter(Mathf.Clamp(this._tooFarDeadRatio * this.VanishWeight * 100, 0, 100f)));
            }
            else
            {
                this._volumeBM.intensity.SetValue(new FloatParameter(0));
            }

            // 灵魂视角：小马离灵魂太远，褪色
            if (!this._currentCharacter)
            {
                if (this._ponyToSoulDistance >= this.MaximalPonyToSoulDistance - this.FadingDistance)
                {
                    this._volumeCAJ.saturation.SetValue(new FloatParameter(Mathf.Clamp((this._ponyToSoulDistance - this.MaximalPonyToSoulDistance + this.FadingDistance) * this.FadingWeight * -100 / this.FadingDistance, -100f, 0)));
                }
            }
            else
            {
                this._volumeCAJ.saturation.SetValue(new FloatParameter(0));
            }

            // 计算距离环
            if(this._ponyToSoulDistance <= this.MinimalPonyToSoulDistance + this.DistanceCircleDisplayDistance)
            {
                this._distanceCircleScript.Enable = true;
                this._distanceCircleScript.R = this.MinimalPonyToSoulDistance;
                var pell = Mathf.Clamp((this.MinimalPonyToSoulDistance + this.DistanceCircleDisplayDistance - this._ponyToSoulDistance) / this.DistanceCircleDisplayDistance * this.DistanceCirclePellucidity, 0, this.DistanceCirclePellucidity);
                var c = this._ponyToSoulDistance <= this.MinimalPonyToSoulDistance ? this.DistanceCircleDangerColor : this.DistanceCircleDefaultColor;
                c.a = pell;
                this._distanceCircleScript.SetColor(c);
            }
            else if (this._ponyToSoulDistance >= this.MaximalPonyToSoulDistance - this.DistanceCircleDisplayDistance)
            {
                this._distanceCircleScript.Enable = true;
                this._distanceCircleScript.R = this.MaximalPonyToSoulDistance;
                var pell = Mathf.Clamp((this._ponyToSoulDistance - this.MaximalPonyToSoulDistance + this.DistanceCircleDisplayDistance) / this.DistanceCircleDisplayDistance * this.DistanceCirclePellucidity, 0, this.DistanceCirclePellucidity);
                var c = this._ponyToSoulDistance >= this.MaximalPonyToSoulDistance ? this.DistanceCircleDangerColor : this.DistanceCircleDefaultColor;
                c.a = pell;
                this._distanceCircleScript.SetColor(c);
            }
            else
            {
                this._distanceCircleScript.SetColor(Color.clear);
                this._distanceCircleScript.DrawCircle();
                this._distanceCircleScript.Enable = false;
            }
        }


        /// <summary>
        /// 计算视角
        /// </summary>
        void CalcView()
        {
            if (this.input.ChangeCharacter)
            {
                if (this._currentCharacter)
                {
                    this.Pony.GetComponent<PonyController>().EnableControl = false;
                    this.Soul.GetComponent<SoulController>().EnableControl = true;
                    this.MainCamera.GetComponent<CameraController>().SetTarget(this.Soul);
                }
                else
                {
                    this.Pony.GetComponent<PonyController>().EnableControl = true;
                    this.Soul.GetComponent<SoulController>().EnableControl = false;
                    this.MainCamera.GetComponent<CameraController>().SetTarget(this.Pony);
                }
                this._currentCharacter = !this._currentCharacter;
            }
        }
    }
}