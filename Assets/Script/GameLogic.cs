using StayAwayGameScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

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
            /// <summary>
            /// 切换角色
            /// </summary>
            public bool DisplayLight;
            /// <summary>
            /// 使用魔法
            /// </summary>
            public bool UseMagic;
        }

        #region 公有变量
        [Header("信息")]
        /// <summary>
        /// 拥有灯
        /// </summary>
        public Boolean HasLight;
        /// <summary>
        /// 拥有琴
        /// </summary>
        public Boolean HasLyra;
        /// <summary>
        /// 小马死亡
        /// </summary>
        public Boolean PonyIsDead;
        /// <summary>
        /// 灵魂死亡
        /// </summary>
        public Boolean SoulIsDead;
        /// <summary>
        /// 当前拥有的魔法
        /// </summary>
        public StayAwayGame.Magic CurrentMagic;
        /// <summary>
        /// 剩余魔法量
        /// </summary>
        public int MagicLeft;
        /// <summary>
        /// 下一关名字
        /// </summary>
        public string NextLevelName = "level_end";


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
        /// <summary>
        /// UI界面
        /// </summary>
        public GameObject UI;

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
        public float CameraSize = 3f;

        [Header("特效")]
        /// <summary>
        /// 灵魂透明的
        /// </summary>
        public float SoulPellucidity = 0.5f;
        /// <summary>
        /// 产生幻觉特效距离
        /// </summary>
        public float IllusionDistance = 1f;
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
        public float FadingDistance = 1f;
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
        /// <summary>
        /// 小马灯颜色
        /// </summary>
        public Color PonyLightColorStart, PonyLightColorEnd;
        /// <summary>
        /// 灵魂灯颜色
        /// </summary>
        public Color SoulLightColorStart, SoulLightColorEnd;

        [Header("物品")]
        /// <summary>
        /// 灯对象
        /// </summary>
        public UnityEngine.Object Light;

        [Header("魔法")]
        /// <summary>
        /// 魔法使用次数
        /// </summary>
        public int MagicUsageCount = 5;

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
        /// <summary>
        /// 显示灯
        /// </summary>
        private Boolean _displayedLight;
        /// <summary>
        /// 锁住控制器
        /// </summary>
        private Boolean _isControllerLocked;
        /// <summary>
        /// 游戏结束原因
        /// </summary>
        private int _gameOverReason;

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
            this.MainCamera.GetComponent<Camera>().orthographicSize = this.CameraSize;
            this._currentCharacter = true;
            this.Pony.GetComponent<PonyController>().EnableControl = true;
            this.Soul.GetComponent<SoulController>().EnableControl = false;
            this.MainCamera.GetComponent<CameraController>().SetTarget(this.Pony);
            this.Soul.GetComponent<SoulController>().SetAIEnable(false);

            this.HasLight = false;
            this.HasLyra = false;

            this.Light.GameObject().SetActive(false);

            // Debug
            Color c = Color.white;
            c.a = this.SoulPellucidity;
            this.Soul.GetComponentInChildren<SpriteRenderer>().color = c;
        }

        void Update()
        {
            GatherInput();

            if(this.input.DisplayLight)
            {
                DisplayLight(!this._displayedLight);
            }

            if (!(this.SoulIsDead || this.PonyIsDead || this._isControllerLocked))
            {
                CalcDistance();
                CalcControl();
                CalcMagic();
            }
        }

        /// <summary>
        /// 获取输入
        /// </summary>
        void GatherInput()
        {
            this.input = new FrameInput
            {
                ChangeCharacter = UnityEngine.Input.GetKeyDown(KeyCode.R),
                DisplayLight = UnityEngine.Input.GetKeyDown(KeyCode.L),
                UseMagic = UnityEngine.Input.GetKeyDown(KeyCode.E)
            };
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
                    // 小马死亡
                    CharacterDead(true, 1);
                    return;
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
                else if (this._tooCloseDeadRatio < 0)
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
                    // 灵魂死亡
                    CharacterDead(false, 2);
                    return;
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
            if (this._ponyToSoulDistance <= this.MinimalPonyToSoulDistance + this.DistanceCircleDisplayDistance)
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
        /// 计算控制
        /// </summary>
        void CalcControl()
        {
            if (this.input.ChangeCharacter)
            {
                ChangeCharacter(!this._currentCharacter);
            }
        }

        void ChangeCharacter(Boolean flag)
        {
            if (!flag)
            {
                this.Pony.GetComponent<PonyController>().EnableControl = false;
                this.Soul.GetComponent<SoulController>().EnableControl = true;
                this.Soul.GetComponent<SoulController>().SetAIEnable(false);
                this.MainCamera.GetComponent<CameraController>().SetTarget(this.Soul);
                this.Soul.GetComponentInChildren<SpriteRenderer>().color = Color.white;
                this._volumeCAJ.hueShift.SetValue(new FloatParameter(180));
                this._currentCharacter = false;

            }
            else
            {
                this.Pony.GetComponent<PonyController>().EnableControl = true;
                this.Soul.GetComponent<SoulController>().EnableControl = false;
                this.Soul.GetComponent<SoulController>().SetAIEnable(!this.SoulIsDead && this.HasLight && this._displayedLight);
                this.MainCamera.GetComponent<CameraController>().SetTarget(this.Pony);
                Color c = Color.white;
                c.a = this.SoulPellucidity;
                this.Soul.GetComponentInChildren<SpriteRenderer>().color = c;
                this._volumeCAJ.hueShift.SetValue(new FloatParameter(0));
                this._currentCharacter = true;
            }
        }

        // 计算魔法
        void CalcMagic()
        {
            if(this.input.UseMagic && this.HasLyra && this.CurrentMagic!=StayAwayGame.Magic.None && this.MagicLeft > 0)
            {
                this.MagicLeft--;
                this.UI.GetComponent<UIScript>().SetMagicCount(this.MagicLeft, this.MagicUsageCount);
                this.Soul.GetComponent<MagicEmitter>().EmitMagic(this.CurrentMagic);
            }
        }

        /// <summary>
        /// 显示灯
        /// </summary>
        /// <param name="enable">启用</param>
        void DisplayLight(Boolean enable)
        {
            if(enable && this.HasLight)
            {
                this._displayedLight = true;
                this.Soul.GetComponent<SoulController>().SetAIEnable(!this.SoulIsDead);
                this.Light.GameObject().SetActive(true);
            }
            else
            {
                this._displayedLight = false;
                this.Soul.GetComponent<SoulController>().SetAIEnable(false);
                this.Light.GameObject().SetActive(false);
            }
        }

        public void CharacterDead(Boolean character, int reason)
        {
            // 清除特效
            this._volumeCAJ.saturation.SetValue(new FloatParameter(0));
            this._volumeCHA.intensity.SetValue(new FloatParameter(0));
            this._volumeBM.intensity.SetValue(new FloatParameter(0));
            this._volumeCAJ.hueShift.SetValue(new FloatParameter(0));
            // 清除环
            this._distanceCircleScript.SetColor(Color.clear);
            this._distanceCircleScript.DrawCircle();
            this._distanceCircleScript.Enable = false;

            if (character)
            {
                // ======== Pony Dead ========
                this.PonyIsDead = true;

                // 死亡动画
                this.Pony.GetComponentInChildren<Animator>().SetTrigger("Dead");

                this.Pony.GetComponent<PonyController>().EnableControl = false;
                this.Soul.GetComponent<SoulController>().EnableControl = false;
                this.Soul.GetComponent<SoulController>().SetAIEnable(!this.SoulIsDead && this.HasLight && this._displayedLight);
                this.MainCamera.GetComponent<CameraController>().SetTarget(this.Pony);
                Color c = Color.white;
                c.a = this.SoulPellucidity;
                this.Soul.GetComponentInChildren<SpriteRenderer>().color = c;
                this._currentCharacter = true;

            }
            else
            {
                this.SoulIsDead = true;

                // 更改控制
                this.Pony.GetComponent<PonyController>().EnableControl = true;
                this.Soul.GetComponent<SoulController>().EnableControl = false;
                this.Soul.GetComponent<SoulController>().SetAIEnable(false);
                this.MainCamera.GetComponent<CameraController>().SetTarget(this.Pony);
                Color c = Color.white;
                c.a = this.SoulPellucidity;
                this.Soul.GetComponentInChildren<SpriteRenderer>().color = c;
                this._currentCharacter = true;

                this._volumeVGN.intensity.SetValue(new FloatParameter(0));

                this.Soul.GetComponent<SoulController>().Dead();

            }

            GameOver(reason);
        }

        public void GetItem(StayAwayGame.Item item)
        {
            if(item == StayAwayGame.Item.ItemLyra)
            {
                this.HasLyra = true;
                this.UI.GetComponent<UIScript>().GetLyra(true);

            }
            else if(item == StayAwayGame.Item.ItemLight)
            {
                this.HasLight = true;
                this.UI.GetComponent<UIScript>().GetLight(true);
            }
        }

        public void GetMagic(StayAwayGame.Magic magic)
        {
            this.CurrentMagic = magic;
            this.MagicLeft = this.MagicUsageCount;
            this.UI.GetComponent<UIScript>().GetMagic(magic);
            this.UI.GetComponent<UIScript>().SetMagicCount(this.MagicLeft, this.MagicUsageCount);
        }

        public void SetControllerLock(Boolean flag = false)
        {
            this._isControllerLocked = flag;
            this.Pony.GetComponent<PonyController>().EnableControl = false;
            this.Soul.GetComponent<SoulController>().EnableControl = false;
            if (!flag)
            {
                ChangeCharacter(this._currentCharacter);
            }
        }

        #region 游戏结束

        /// <summary>
        /// 游戏结束
        /// </summary>
        /// <param name="reason">0 成功 1 小马死 2 姐姐死</param>
        public void GameOver(int reason)
        {
            SetControllerLock(true);
            this._gameOverReason = reason;
            Invoke(nameof(GameOver_P0), 2);
        }

        public void GameOver_P0()
        {
            this.UI.GetComponent<UIScript>().PlayCurtain(true);
            Invoke(nameof(GameOver_P1), 1.5f);
        }

        void GameOver_P1()
        {
            if (this.UI.GetComponent<UIScript>().IsPlaying)
            {
                Invoke(nameof(GameOver_P1), 1.5f);
                return;
            }

            // 清除特效
            this._volumeCAJ.saturation.SetValue(new FloatParameter(0));
            this._volumeCHA.intensity.SetValue(new FloatParameter(0));
            this._volumeBM.intensity.SetValue(new FloatParameter(0));
            this._volumeCAJ.hueShift.SetValue(new FloatParameter(0));
            // 清除环
            this._distanceCircleScript.SetColor(Color.clear);
            this._distanceCircleScript.DrawCircle();
            this._distanceCircleScript.Enable = false;

            if (this._gameOverReason == 0)
            {
                this.UI.GetComponent<UIScript>().PlayText("游戏结束");
                Invoke(nameof(GameOver_P2), 5f);
            }
            else if (this._gameOverReason == 1)
            {
                this.UI.GetComponent<UIScript>().PlayText("你被幻觉击晕了");
                Invoke(nameof(GameOver_P2), 5f);
            }
            else if (this._gameOverReason == 2)
            {
                this.UI.GetComponent<UIScript>().PlayText("姐姐的灵魂消散了\n没有她的保护，你在森林里将寸步难行");
                Invoke(nameof(GameOver_P2), 5f);
            }
            else if (this._gameOverReason == 3)
            {
                this.UI.GetComponent<UIScript>().PlayText("你死了");
                Invoke(nameof(GameOver_P2), 5f);
            }

        }

        void GameOver_P2()
        {
            SceneManager.LoadScene(this.NextLevelName);
        }

        #endregion
    }
}