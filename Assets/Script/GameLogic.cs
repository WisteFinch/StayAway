using StayAwayGameController;
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

        [Header("视角")]
        public float CameraSize = 3f;

        [Header("特效")]
        /// <summary>
        /// 产生幻觉距离
        /// </summary>
        public float IllusionDistance = 1f;
        /// <summary>
        /// 幻觉强度
        /// </summary>
        public float IllusionWeight = 1f;

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
        private ChromaticAberration _volumeCA;
        /// <summary>
        /// 后期伽马亮度
        /// </summary>
        private LiftGammaGain _volumeLGG;
        /// <summary>
        /// 上次过近时间
        /// </summary>
        private float _lastTooCloseTime;
        /// <summary>
        /// 当前角色
        /// </summary>
        private Boolean _currentCharacter;
        #endregion

        void Start()
        {
            // 获取组件
            this._ponyTransform = this.Pony.GetComponent<Transform>();
            this._soulTransform = this.Soul.GetComponent<Transform>();
            this._volume = this.GlobalVolume.GetComponent<Volume>();
            this._volume.profile.TryGet<ChromaticAberration>(out this._volumeCA);
            this._volume.profile.TryGet<LiftGammaGain>(out this._volumeLGG);

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
            this._ponyToSoulDistance = Vector2.Distance(this._ponyTransform.position, this._soulTransform.position);
            // 离灵魂过近，死亡
            if (this._ponyToSoulDistance <= this.MinimalPonyToSoulDistance)
            {
                //if(this._lastTooCloseTime == -1)
                //{
                //    this._lastTooCloseTime = Time.time;
                //}
                //else if(this._lastTooCloseTime + this.TooCloseDeadTime >= Time.time)
                //{
                //    // ==================== 死亡 =======================
                //}
                //else
                //{
                //    print(true);
                //    var v = Mathf.Clamp((this._lastTooCloseTime + this.TooCloseDeadTime - Time.time) / this.TooCloseDeadTime, 0, 1f);
                //    this._volumeLGG.gain.SetValue(new FloatParameter(v));
                //}
                //return;
            }
            else if (this._lastTooCloseTime != -1)
            {
                this._lastTooCloseTime = -1;
            }
            // 离灵魂太近，显示幻觉
            if (this._ponyToSoulDistance <= this.MinimalPonyToSoulDistance + this.IllusionDistance)
            {
                this._volumeCA.intensity.SetValue(new FloatParameter(Mathf.Clamp((this.MinimalPonyToSoulDistance + this.IllusionDistance - this._ponyToSoulDistance) * this.IllusionDistance / this.IllusionDistance, 0, 1f)));
            }
        }


        /// <summary>
        /// 计算视角
        /// </summary>
        void CalcView()
        {
            if(this.input.ChangeCharacter)
            {
                if(this._currentCharacter)
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
            }
            this._currentCharacter = !this._currentCharacter;
        }
    }
}