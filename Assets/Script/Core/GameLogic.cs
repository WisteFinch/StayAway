using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace StayAwayGameScript
{
    public class GameLogic : MonoBehaviour
    {
        void Start()
        {
            // 获取组件
            this._OBJPonyTransform = this.OBJPony.GetComponent<Transform>();
            this._OBJSoulTransform = this.OBJSoul.GetComponent<Transform>();
            this._renderVolume = this.OBJGlobalVolume.GetComponent<Volume>();
            this._renderVolume.profile.TryGet<ChromaticAberration>(out this._renderVolumeCHA);
            this._renderVolume.profile.TryGet<Vignette>(out this._renderVolumeVGN);
            this._renderVolume.profile.TryGet<ColorAdjustments>(out this._renderVolumeCAJ);
            this._renderVolume.profile.TryGet<Bloom>(out this._renderVolumeBM);
            this._DSTCircleScript = this.GetComponentInChildren<CircleRender>();

            this._OBJPonyAudio = this.OBJPony.GetComponentInChildren<AudioPlayer>();

            // 初始化
            this._CHARCurrentCharacter = true;
            this.OBJPony.GetComponent<PonyController>().EnableControl = true;
            this.OBJSoul.GetComponent<SoulController>().EnableControl = false;
            this.OBJMainCamera.GetComponent<CameraController>().SetTarget(this.OBJPony);
            this.OBJMainCamera.GetComponent<CameraController>().SetSize(this.CameraSize);
            this.OBJSoul.GetComponent<SoulController>().SetAIEnable(false);

            Color c = Color.white;
            c.a = this.RenderSoulPellucidity;
            this.OBJSoul.GetComponentInChildren<SpriteRenderer>().color = c;
        }

        void Update()
        {
            if (!this._CHARIsControllerLocked)
            {
                GatherInput();

                if(this._input.DisplayMenu)
                {
                    SetControllerLock(true);
                    SetForzen(true);
                    this.OBJGUI.GetComponent<GUIScript>().ShowMenu();
                }
                else
                {
                    if (this._input.DisplayLight)
                    {
                        DisplayLight(!this.ItemEnableLight);
                    }
                }
                CalcDistance();
                CalcControl();
                CalcMagic();
            }
        }

        #region 对象

        [Header("对象")]
        /// <summary>
        /// 小马对象
        /// </summary>
        public GameObject OBJPony;
        /// <summary>
        /// 灵魂对象
        /// </summary>
        public GameObject OBJSoul;
        /// <summary>
        /// 主摄像机
        /// </summary>
        public Camera OBJMainCamera;
        /// <summary>
        /// 主渲染
        /// </summary>
        public GameObject OBJGlobalVolume;
        /// <summary>
        /// UI界面
        /// </summary>
        public GameObject OBJGUI;
        /// <summary>
        /// 竖琴预制体对象
        /// </summary>
        public GameObject OBJLyraPerfab;
        /// <summary>
        /// 灯对象
        /// </summary>
        public GameObject OBJLight;

        /// <summary>
        /// 小马变形器
        /// </summary>
        private Transform _OBJPonyTransform;
        /// <summary>
        /// 小马音频组件
        /// </summary>
        private AudioPlayer _OBJPonyAudio;
        /// <summary>
        /// 灵魂变形器
        /// </summary>
        private Transform _OBJSoulTransform;

        #endregion

        #region 输入

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
            /// <summary>
            /// 显示菜单
            /// </summary>
            public bool DisplayMenu;
        }

        /// <summary>
        /// 输入
        /// </summary>
        private FrameInput _input;

        /// <summary>
        /// 获取输入
        /// </summary>
        void GatherInput()
        {
            this._input = new FrameInput
            {
                ChangeCharacter = GameManager.Instance.Input.ChangeCharacterKeyDown && this.CHAREnableChangeCharacter,
                DisplayLight = GameManager.Instance.Input.LightKeyDown,
                UseMagic = GameManager.Instance.Input.FireKeyDown,
                DisplayMenu = GameManager.Instance.Input.EscapeKeyDown
            };
        }

        #endregion

        #region 角色

        [Header("角色")]
        /// <summary>
        /// 角色：小马死亡
        /// </summary>
        public Boolean CHARPonyIsDead;
        /// <summary>
        /// 角色：灵魂死亡
        /// </summary>
        public Boolean CHARSoulIsDead;
        /// <summary>
        /// 角色：允许换角色
        /// </summary>
        public Boolean CHAREnableChangeCharacter = true;

        /// <summary>
        /// 角色：当前角色
        /// </summary>
        private Boolean _CHARCurrentCharacter;
        /// <summary>
        /// 角色：锁住控制器
        /// </summary>
        private Boolean _CHARIsControllerLocked;
        /// <summary>
        /// 角色：冻结游戏
        /// </summary>
        private Boolean _CHARIsForzen;

        /// <summary>
        /// 角色：切换角色
        /// </summary>
        /// <param name="flag">0 灵魂 1 小马</param>
        public void ChangeCharacter(Boolean flag)
        {
            this.OBJPony.GetComponent<PonyController>().EnableControl = flag;
            this.OBJSoul.GetComponent<SoulController>().EnableControl = !flag;
            this.OBJSoul.GetComponent<SoulController>().SetAIEnable(flag && !this.CHARSoulIsDead && this.ItemHasLight && this.ItemEnableLight);
            this.OBJMainCamera.GetComponent<CameraController>().SetTarget(flag ? this.OBJPony : this.OBJSoul);
            this._CHARCurrentCharacter = flag;
            this._renderVolumeCAJ.hueShift.SetValue(new FloatParameter(flag ? 0 : 180));
            if (flag)
            {
                Color c = Color.white;
                c.a = this.RenderSoulPellucidity;
                this.OBJSoul.GetComponentInChildren<SpriteRenderer>().color = c;
            }
            else
            {
                this.OBJSoul.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }
        }

        /// <summary>
        /// 角色：小马死亡
        /// </summary>
        public void PonyDead()
        {
            CharacterDead(true, 3);
        }

        /// <summary>
        /// 角色：角色死亡
        /// </summary>
        /// <param name="character">0 灵魂 1 小马</param>
        /// <param name="reason">死亡原因</param>
        public void CharacterDead(Boolean character, int reason)
        {
            // 清除特效
            ClearEffect();
            if (character) // 小马死亡
            {
                this.CHARPonyIsDead = true;
                this.OBJPony.GetComponentInChildren<Animator>().SetTrigger("Dead");
                ChangeCharacter(true);

            }
            else // 灵魂死亡
            {
                this.CHARSoulIsDead = true;
                ChangeCharacter(true);
                this.OBJSoul.GetComponent<SoulController>().Dead();

            }
            SetControllerLock(true);

            this.FlowGameOverEvent.Invoke(reason);
        }

        /// <summary>
        /// 角色：计算控制
        /// </summary>
        void CalcControl()
        {
            if (this._input.ChangeCharacter)
            {
                ChangeCharacter(!this._CHARCurrentCharacter);
            }
        }

        /// <summary>
        /// 角色：解除控制锁
        /// </summary>
        void ControllerUnlock()
        {
            this.SetControllerLock(false);
        }

        /// <summary>
        /// 角色：设置控制锁
        /// </summary>
        /// <param name="flag">是否启用</param>
        public void SetControllerLock(Boolean flag = false)
        {
            this._CHARIsControllerLocked = flag;
            this.OBJPony.GetComponent<PonyController>().EnableControl = false;
            this.OBJSoul.GetComponent<SoulController>().EnableControl = false;
            this.OBJSoul.GetComponent<SoulController>().SetAIEnable(false);
            if (!flag)
            {
                ChangeCharacter(this._CHARCurrentCharacter);
            }
        }

        /// <summary>
        /// 角色：设置冻结
        /// </summary>
        /// <param name="flag">是否启用</param>
        public void SetForzen(Boolean flag = false)
        {
            this._CHARIsForzen = flag;
            this.OBJPony.GetComponent<PonyController>().EnableForzen = flag;
            this.OBJSoul.GetComponent<SoulController>().EnableForzen = flag;
        }


        #endregion

        #region 物品

        [Header("物品")]
        /// <summary>
        /// 物品：拥有灯
        /// </summary>
        public Boolean ItemHasLight = false;
        /// <summary>
        /// 物品：拥有琴
        /// </summary>
        public Boolean ItemHasLyra = false;
        /// <summary>
        /// 物品：拾取物品时间
        /// </summary>
        public float ItemPickupTime = 2;
        /// <summary>
        /// 物品：获取物品显示偏移
        /// </summary>
        public Vector2 ItemPickupOffset = new(0, 0.5f);
        /// <summary>
        /// 物品：获取物品镜头缩放
        /// </summary>
        public float ItemPickupCameraResize = -1;
        /// <summary>
        /// 物品：启用灯
        /// </summary>
        public Boolean ItemEnableLight;

        /// <summary>
        /// 物品：显示灯
        /// </summary>
        /// <param name="enable">启用</param>
        void DisplayLight(Boolean enable)
        {
            if (enable && this.ItemHasLight)
            {
                this.ItemEnableLight = true;
                this.OBJSoul.GetComponent<SoulController>().SetAIEnable(!this.CHARSoulIsDead);
                this.OBJLight.GameObject().SetActive(true);
            }
            else
            {
                this.ItemEnableLight = false;
                this.OBJSoul.GetComponent<SoulController>().SetAIEnable(false);
                this.OBJLight.GameObject().SetActive(false);
            }
            this.OBJGUI.GetComponent<GUIScript>().SetLightStatus(this.ItemEnableLight);
        }

        /// <summary>
        /// 物品：获取物品
        /// </summary>
        /// <param name="item"></param>
        public void GetItem(StayAwayGame.Item item, GameObject itemObj)
        {
            this.SetControllerLock(true);

            this.OBJPony.GetComponent<PonyAnimationHandler>().Nod();
            itemObj.transform.parent = this.transform;
            itemObj.transform.position = this.transform.position + (Vector3)this.ItemPickupOffset;
            this.OBJMainCamera.GetComponent<CameraController>().Resize(this.ItemPickupCameraResize);

            if (item == StayAwayGame.Item.ItemLyra)
            {
                this.ItemHasLyra = true;
                this.OBJGUI.GetComponent<GUIScript>().SetItemUsable(StayAwayGame.Item.ItemLyra, true);

            }
            else if (item == StayAwayGame.Item.ItemLight)
            {
                this.ItemHasLight = true;
                this.OBJGUI.GetComponent<GUIScript>().SetItemUsable(StayAwayGame.Item.ItemLight, true);
            }

            itemObj.GetComponent<ItemPickup>().DestryThisLater(this.ItemPickupTime);
            Invoke(nameof(ControllerUnlock), this.ItemPickupTime);
            Invoke(nameof(RestoreCameraSize), this.ItemPickupTime);
        }

        #endregion

        #region 魔法

        [Header("魔法")]
        /// <summary>
        /// 魔法：当前拥有的魔法
        /// </summary>
        public StayAwayGame.Magic MagicCurrent;
        /// <summary>
        /// 魔法：剩余魔法量
        /// </summary>
        public int MagicLeft;
        /// <summary>
        /// 魔法：魔法使用次数
        /// </summary>
        public int MagicUsageCount = 5;
        /// <summary>
        /// 魔法：获取魔法时间
        /// </summary>
        public float MagicGetTime = 2;
        /// <summary>
        /// 魔法：获取魔法显示偏移
        /// </summary>
        public Vector2 MagicGetOffset = new(0, 0.5f);
        /// <summary>
        /// 魔法：获取魔法镜头缩放
        /// </summary>
        public float MagicGetCameraResize = -1;

        /// <summary>
        /// 魔法：计算魔法
        /// </summary>
        void CalcMagic()
        {
            if (this._input.UseMagic && this.ItemHasLyra && this.MagicCurrent != StayAwayGame.Magic.None && this.MagicLeft > 0)
            {
                this.MagicLeft--;
                this.OBJGUI.GetComponent<GUIScript>().SetMagicCount(this.MagicLeft, this.MagicUsageCount);
                this.OBJSoul.GetComponent<MagicEmitter>().EmitMagic(this.MagicCurrent);
            }
        }

        /// <summary>
        /// 魔法：获取魔法
        /// </summary>
        /// <param name="magic">魔法类型</param>
        public void GetMagic(StayAwayGame.Magic magic)
        {
            this.SetControllerLock(true);

            this.MagicCurrent = magic;
            this.MagicLeft = this.MagicUsageCount;
            this.OBJGUI.GetComponent<GUIScript>().GetMagic(magic);
            this.OBJGUI.GetComponent<GUIScript>().SetMagicCount(this.MagicLeft, this.MagicUsageCount);

            this.OBJPony.GetComponent<PonyAnimationHandler>().Nod();
            this.OBJMainCamera.GetComponent<CameraController>().Resize(this.MagicGetCameraResize);

            var obj = Instantiate(this.OBJLyraPerfab);
            Destroy(obj.GetComponent<CircleCollider2D>());
            obj.transform.parent = this.transform;
            obj.transform.position = this.transform.position + (Vector3)this.MagicGetOffset;
            obj.GetComponent<ItemPickup>().DestryThisLater(this.MagicGetTime);
            Invoke(nameof(ControllerUnlock), this.MagicGetTime);
            Invoke(nameof(RestoreCameraSize), this.MagicGetTime);
        }

        #endregion

        #region 渲染

        [Header("渲染")]
        /// <summary>
        /// 渲染：灵魂透明度
        /// </summary>
        public float RenderSoulPellucidity = 0.5f;
        /// <summary>
        /// 渲染：允许特效
        /// </summary>
        public Boolean RenderEnableEffect = true;

        /// <summary>
        /// 渲染：后期处理
        /// </summary>
        private Volume _renderVolume;
        /// <summary>
        /// 渲染：后期处理色差
        /// </summary>
        private ChromaticAberration _renderVolumeCHA;
        /// <summary>
        /// 渲染：后期渐晕
        /// </summary>
        private Vignette _renderVolumeVGN;
        /// <summary>
        /// 渲染：后期色彩修正
        /// </summary>
        private ColorAdjustments _renderVolumeCAJ;
        /// <summary>
        /// 渲染：后期高光
        /// </summary>
        private Bloom _renderVolumeBM;

        /// <summary>
        /// 渲染：清除特效
        /// </summary>
        public void ClearEffect()
        {
            // 清除特效
            this._renderVolumeVGN.intensity.SetValue(new FloatParameter(0));
            this._renderVolumeCAJ.saturation.SetValue(new FloatParameter(0));
            this._renderVolumeCHA.intensity.SetValue(new FloatParameter(0));
            this._renderVolumeBM.intensity.SetValue(new FloatParameter(0));
            this._renderVolumeCAJ.hueShift.SetValue(new FloatParameter(0));
            // 清除环
            this._DSTCircleScript.SetColor(Color.clear);
            this._DSTCircleScript.DrawCircle();
            this._DSTCircleScript.Enable = false;
        }

        /// <summary>
        /// 渲染：设置音高
        /// </summary>
        /// <param name="pitch"></param>
        public void SetAudioPitch(float pitch)
        {
            var list = GameObject.FindGameObjectsWithTag("Audio");
            foreach (var item in list)
            {
                item.GetComponent<AudioSource>().pitch = pitch;
            }
        }

        #endregion

        #region 距离逻辑

        [Header("距离逻辑")]
        /// <summary>
        /// 距离逻辑：最小小马至灵魂距离
        /// </summary>
        public float DSTMinimalPonyToSoulDistance = 1f;
        /// <summary>
        /// 距离逻辑：最大小马至灵魂距离
        /// </summary>
        public float DSTMaximalPonyToSoulDistance = 5f;
        /// <summary>
        /// 距离逻辑：过近死亡时间
        /// </summary>
        public float DSTTooCloseDeadTime = 2f;
        /// <summary>
        /// 距离逻辑：过近死亡衰减
        /// </summary>
        public float DSTTooCloseDeadDecayTime = 4f;
        /// <summary>
        /// 距离逻辑：过远死亡时间
        /// </summary>
        public float DSTTooFarDeadTime = 2f;
        /// <summary>
        /// 距离逻辑：过远死亡衰减
        /// </summary>
        public float DSTTooFarDeadDecayTime = 4f;
        /// <summary>
        /// 距离逻辑：产生幻觉特效距离
        /// </summary>
        public float DSTIllusionDistance = 1f;
        /// <summary>
        /// 距离逻辑：幻觉特效强度
        /// </summary>
        public float DSTIllusionWeight = 1f;
        /// <summary>
        /// 距离逻辑：黑视特效强度
        /// </summary>
        public float DSTBlackoutWeight = 1f;
        /// <summary>
        /// 距离逻辑：褪色特效距离
        /// </summary>
        public float DSTFadingDistance = 1f;
        /// <summary>
        /// 距离逻辑：褪色特效强度
        /// </summary>
        public float DSTFadingWeight = 0.8f;
        /// <summary>
        /// 距离逻辑：消逝特效强度
        /// </summary>
        public float DSTVanishWeight = 0.2f;
        /// <summary>
        /// 距离逻辑：距离环默认色彩
        /// </summary>
        public Color DSTCircleDefaultColor = new(255, 255, 255);
        /// <summary>
        /// 距离逻辑：距离环危险色彩
        /// </summary>
        public Color DSTCircleDangerColor = new(255, 100, 100);
        /// <summary>
        /// 距离逻辑：距离环透明度
        /// </summary>
        public float DSTCirclePellucidity = 0.5f;
        /// <summary>
        /// 距离逻辑：距离环显示距离
        /// </summary>
        public float DSTCircleDisplayDistance = 2;

        /// <summary>
        /// 距离逻辑：小马至灵魂距离
        /// </summary>
        private float _DSTPonyToSoulDistance;
        /// <summary>
        /// 距离逻辑：过近死亡进度
        /// </summary>
        private float _DSTTooCloseDeadRatio;
        /// <summary>
        /// 距离逻辑：过远死亡进度
        /// </summary>
        private float _DSTTooFarDeadRatio;
        /// <summary>
        /// 距离逻辑：距离环脚本
        /// </summary>
        private CircleRender _DSTCircleScript;

        /// <summary>
        /// 距离逻辑：距离计算
        /// </summary>
        void CalcDistance()
        {
            // 计算距离
            this._DSTPonyToSoulDistance = Vector2.Distance(this._OBJPonyTransform.position, this._OBJSoulTransform.position);
            // 小马视角：小马离灵魂过近，黑视
            if (this._DSTPonyToSoulDistance <= this.DSTMinimalPonyToSoulDistance)
            {
                if (this._DSTTooCloseDeadRatio >= 1)
                {
                    // 小马死亡
                    CharacterDead(true, 1);
                    this._OBJPonyAudio.SetPitchOffset();
                    return;
                }
                else
                {
                    this._DSTTooCloseDeadRatio += Time.deltaTime / this.DSTTooCloseDeadTime;
                    
                }
            }
            else
            {
                if (this._DSTTooCloseDeadRatio > 0)
                {
                    this._DSTTooCloseDeadRatio -= Time.deltaTime / this.DSTTooCloseDeadDecayTime;
                }
                else if (this._DSTTooCloseDeadRatio < 0)
                {
                    this._DSTTooCloseDeadRatio = 0;
                }
            }
            SetAudioPitch(this._CHARCurrentCharacter ? 1 - this._DSTTooCloseDeadRatio : 1); // 调整音高
            if (this.RenderEnableEffect)
            {
                if (_CHARCurrentCharacter)
                {
                    this._renderVolumeVGN.intensity.SetValue(new FloatParameter(Mathf.Clamp(this._DSTTooCloseDeadRatio * this.DSTBlackoutWeight, 0, 1f)));
                }
                else
                {
                    this._renderVolumeVGN.intensity.SetValue(new FloatParameter(0));
                }
            }


            // 小马视角：小马离灵魂近，显示幻觉
            if (this.RenderEnableEffect)
            {
                if (this._CHARCurrentCharacter)
                {
                    if (this._DSTPonyToSoulDistance <= this.DSTMinimalPonyToSoulDistance + this.DSTIllusionDistance)
                    {
                        this._renderVolumeCHA.intensity.SetValue(new FloatParameter(Mathf.Clamp((this.DSTMinimalPonyToSoulDistance + this.DSTIllusionDistance - this._DSTPonyToSoulDistance) * this.DSTIllusionWeight / this.DSTIllusionDistance, 0, 1f)));
                    }
                }
                else
                {
                    this._renderVolumeCHA.intensity.SetValue(new FloatParameter(0));
                }
            }
            

            // 灵魂视角：小马离灵魂远近，消逝
            if (this._DSTPonyToSoulDistance >= this.DSTMaximalPonyToSoulDistance)
            {
                if (this._DSTTooFarDeadRatio >= 1)
                {
                    // 灵魂死亡
                    CharacterDead(false, 2);
                    return;
                }
                else
                {
                    this._DSTTooFarDeadRatio += Time.deltaTime / this.DSTTooFarDeadTime;
                }
            }
            else
            {
                if (this._DSTTooFarDeadRatio > 0)
                {
                    this._DSTTooFarDeadRatio -= Time.deltaTime / this.DSTTooFarDeadDecayTime;
                }
                else if (this._DSTTooFarDeadRatio < 0)
                {
                    this._DSTTooFarDeadRatio = 0;
                }
            }
            if(this.RenderEnableEffect)
            {
                if (!_CHARCurrentCharacter)
                {
                    this._renderVolumeBM.intensity.SetValue(new FloatParameter(Mathf.Clamp(this._DSTTooFarDeadRatio * this.DSTVanishWeight * 100, 0, 100f)));
                }
                else
                {
                    this._renderVolumeBM.intensity.SetValue(new FloatParameter(0));
                }
            }

            // 灵魂视角：小马离灵魂太远，褪色
            if(this.RenderEnableEffect)
            {
                if (!this._CHARCurrentCharacter)
                {
                    if (this._DSTPonyToSoulDistance >= this.DSTMaximalPonyToSoulDistance - this.DSTFadingDistance)
                    {
                        this._renderVolumeCAJ.saturation.SetValue(new FloatParameter(Mathf.Clamp((this._DSTPonyToSoulDistance - this.DSTMaximalPonyToSoulDistance + this.DSTFadingDistance) * this.DSTFadingWeight * -100 / this.DSTFadingDistance, -100f, 0)));
                    }
                }
                else
                {
                    this._renderVolumeCAJ.saturation.SetValue(new FloatParameter(0));
                }
            }


            // 计算距离环
            if (this.RenderEnableEffect) 
            {
                if (this._DSTPonyToSoulDistance <= this.DSTMinimalPonyToSoulDistance + this.DSTCircleDisplayDistance)
                {
                    this._DSTCircleScript.Enable = true;
                    this._DSTCircleScript.R = this.DSTMinimalPonyToSoulDistance;
                    var pell = Mathf.Clamp((this.DSTMinimalPonyToSoulDistance + this.DSTCircleDisplayDistance - this._DSTPonyToSoulDistance) / this.DSTCircleDisplayDistance * this.DSTCirclePellucidity, 0, this.DSTCirclePellucidity);
                    var c = this._DSTPonyToSoulDistance <= this.DSTMinimalPonyToSoulDistance ? this.DSTCircleDangerColor : this.DSTCircleDefaultColor;
                    c.a = pell;
                    this._DSTCircleScript.SetColor(c);
                }
                else if (this._DSTPonyToSoulDistance >= this.DSTMaximalPonyToSoulDistance - this.DSTCircleDisplayDistance)
                {
                    this._DSTCircleScript.Enable = true;
                    this._DSTCircleScript.R = this.DSTMaximalPonyToSoulDistance;
                    var pell = Mathf.Clamp((this._DSTPonyToSoulDistance - this.DSTMaximalPonyToSoulDistance + this.DSTCircleDisplayDistance) / this.DSTCircleDisplayDistance * this.DSTCirclePellucidity, 0, this.DSTCirclePellucidity);
                    var c = this._DSTPonyToSoulDistance >= this.DSTMaximalPonyToSoulDistance ? this.DSTCircleDangerColor : this.DSTCircleDefaultColor;
                    c.a = pell;
                    this._DSTCircleScript.SetColor(c);
                }
                else
                {
                    this._DSTCircleScript.SetColor(Color.clear);
                    this._DSTCircleScript.DrawCircle();
                    this._DSTCircleScript.Enable = false;
                }
            }
        }

        #endregion

        #region 视角

        [Header("视角")]
        public float CameraSize = 3f;

        /// <summary>
        /// 视角：重置视角
        /// </summary>
        void RestoreCameraSize()
        {
            this.OBJMainCamera.GetComponent<CameraController>().Resize();
        }

        #endregion

        #region GUI

        /// <summary>
        /// GUI：菜单已显示
        /// </summary>
        private bool _GUIDisplayMenu;

        #endregion

        #region 流程

        [Header("流程")]
        /// <summary>
        /// 流程：下一关名字
        /// </summary>
        public StayAwayGame.Level FlowNextLevel = StayAwayGame.Level.End;
        /// <summary>
        /// 流程：下一关名字
        /// </summary>
        public StayAwayGame.Level FlowThisLevel = StayAwayGame.Level.End;
        /// <summary>
        /// 流程：游戏结束事件列表
        /// </summary>
        public UnityEvent<int> FlowGameOverEvent = new();

        /// <summary>
        /// 流程：游戏结束原因
        /// </summary>
        private int _flowGameOverReason;

        /// <summary>
        /// 流程：启动游戏结束流程
        /// </summary>
        /// <param name="reason">0 成功 1 小马死 2 姐姐死</param>
        public void FlowGameOver(int reason)
        {
            SetControllerLock(true);
            this._flowGameOverReason = reason;

            this.OBJGUI.GetComponent<GUIScript>().AnimationDoneEvent.AddListener(FlowGameOverDone);
            this.OBJGUI.GetComponent<GUIScript>().AnimationDoneStepEvent.AddListener(FlowGameOverClearEffect);

            if (this._flowGameOverReason == 0)
            {
                this.OBJGUI.GetComponent<GUIScript>().PlayCurtain(true);
            }
            else if (this._flowGameOverReason == 1)
            {
                this.OBJGUI.GetComponent<GUIScript>().PlayText("你被幻觉击晕了");
            }
            else if (this._flowGameOverReason == 2)
            {
                this.OBJGUI.GetComponent<GUIScript>().PlayText("姐姐的灵魂消散了\n没有她的保护，你在森林里将寸步难行");
            }
            else if (this._flowGameOverReason == 3)
            {
                this.OBJGUI.GetComponent<GUIScript>().PlayText("你死了");
            }
            else if (this._flowGameOverReason == 4)
            {
                this.OBJGUI.GetComponent<GUIScript>().PlayText("未完待续···");
            }
        }

        /// <summary>
        /// 流程：游戏结束清除特效
        /// </summary>
        public void FlowGameOverClearEffect()
        {
            this.OBJGUI.GetComponent<GUIScript>().AnimationDoneStepEvent.RemoveListener(FlowGameOverClearEffect);
            this.RenderEnableEffect = false;

            ClearEffect();
        }

        /// <summary>
        /// 流程：游戏结束流程完成
        /// </summary>
        public void FlowGameOverDone()
        {
            if (this._flowGameOverReason == 0 || this._flowGameOverReason == 4)
            {
                this.OBJGUI.GetComponent<GUIScript>().AnimationDoneEvent.RemoveListener(FlowGameOverDone);
                GameManager.Instance.LoadLevel(this.FlowNextLevel);
            }
            else
            {
                this.OBJGUI.GetComponent<GUIScript>().AnimationDoneEvent.RemoveListener(FlowGameOverDone);
                GameManager.Instance.LoadLevel(this.FlowThisLevel);
            }
        }

        #endregion

    }
}