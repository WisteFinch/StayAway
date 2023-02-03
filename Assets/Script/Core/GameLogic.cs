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
            // ��ȡ���
            this._OBJPonyTransform = this.OBJPony.GetComponent<Transform>();
            this._OBJSoulTransform = this.OBJSoul.GetComponent<Transform>();
            this._renderVolume = this.OBJGlobalVolume.GetComponent<Volume>();
            this._renderVolume.profile.TryGet<ChromaticAberration>(out this._renderVolumeCHA);
            this._renderVolume.profile.TryGet<Vignette>(out this._renderVolumeVGN);
            this._renderVolume.profile.TryGet<ColorAdjustments>(out this._renderVolumeCAJ);
            this._renderVolume.profile.TryGet<Bloom>(out this._renderVolumeBM);
            this._DSTCircleScript = this.GetComponentInChildren<CircleRender>();

            this._OBJPonyAudio = this.OBJPony.GetComponentInChildren<AudioPlayer>();

            // ��ʼ��
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

        #region ����

        [Header("����")]
        /// <summary>
        /// С�����
        /// </summary>
        public GameObject OBJPony;
        /// <summary>
        /// ������
        /// </summary>
        public GameObject OBJSoul;
        /// <summary>
        /// �������
        /// </summary>
        public Camera OBJMainCamera;
        /// <summary>
        /// ����Ⱦ
        /// </summary>
        public GameObject OBJGlobalVolume;
        /// <summary>
        /// UI����
        /// </summary>
        public GameObject OBJGUI;
        /// <summary>
        /// ����Ԥ�������
        /// </summary>
        public GameObject OBJLyraPerfab;
        /// <summary>
        /// �ƶ���
        /// </summary>
        public GameObject OBJLight;

        /// <summary>
        /// С�������
        /// </summary>
        private Transform _OBJPonyTransform;
        /// <summary>
        /// С����Ƶ���
        /// </summary>
        private AudioPlayer _OBJPonyAudio;
        /// <summary>
        /// ��������
        /// </summary>
        private Transform _OBJSoulTransform;

        #endregion

        #region ����

        public struct FrameInput
        {
            /// <summary>
            /// �л���ɫ
            /// </summary>
            public bool ChangeCharacter;
            /// <summary>
            /// �л���ɫ
            /// </summary>
            public bool DisplayLight;
            /// <summary>
            /// ʹ��ħ��
            /// </summary>
            public bool UseMagic;
            /// <summary>
            /// ��ʾ�˵�
            /// </summary>
            public bool DisplayMenu;
        }

        /// <summary>
        /// ����
        /// </summary>
        private FrameInput _input;

        /// <summary>
        /// ��ȡ����
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

        #region ��ɫ

        [Header("��ɫ")]
        /// <summary>
        /// ��ɫ��С������
        /// </summary>
        public Boolean CHARPonyIsDead;
        /// <summary>
        /// ��ɫ���������
        /// </summary>
        public Boolean CHARSoulIsDead;
        /// <summary>
        /// ��ɫ��������ɫ
        /// </summary>
        public Boolean CHAREnableChangeCharacter = true;

        /// <summary>
        /// ��ɫ����ǰ��ɫ
        /// </summary>
        private Boolean _CHARCurrentCharacter;
        /// <summary>
        /// ��ɫ����ס������
        /// </summary>
        private Boolean _CHARIsControllerLocked;
        /// <summary>
        /// ��ɫ��������Ϸ
        /// </summary>
        private Boolean _CHARIsForzen;

        /// <summary>
        /// ��ɫ���л���ɫ
        /// </summary>
        /// <param name="flag">0 ��� 1 С��</param>
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
        /// ��ɫ��С������
        /// </summary>
        public void PonyDead()
        {
            CharacterDead(true, 3);
        }

        /// <summary>
        /// ��ɫ����ɫ����
        /// </summary>
        /// <param name="character">0 ��� 1 С��</param>
        /// <param name="reason">����ԭ��</param>
        public void CharacterDead(Boolean character, int reason)
        {
            // �����Ч
            ClearEffect();
            if (character) // С������
            {
                this.CHARPonyIsDead = true;
                this.OBJPony.GetComponentInChildren<Animator>().SetTrigger("Dead");
                ChangeCharacter(true);

            }
            else // �������
            {
                this.CHARSoulIsDead = true;
                ChangeCharacter(true);
                this.OBJSoul.GetComponent<SoulController>().Dead();

            }
            SetControllerLock(true);

            this.FlowGameOverEvent.Invoke(reason);
        }

        /// <summary>
        /// ��ɫ���������
        /// </summary>
        void CalcControl()
        {
            if (this._input.ChangeCharacter)
            {
                ChangeCharacter(!this._CHARCurrentCharacter);
            }
        }

        /// <summary>
        /// ��ɫ�����������
        /// </summary>
        void ControllerUnlock()
        {
            this.SetControllerLock(false);
        }

        /// <summary>
        /// ��ɫ�����ÿ�����
        /// </summary>
        /// <param name="flag">�Ƿ�����</param>
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
        /// ��ɫ�����ö���
        /// </summary>
        /// <param name="flag">�Ƿ�����</param>
        public void SetForzen(Boolean flag = false)
        {
            this._CHARIsForzen = flag;
            this.OBJPony.GetComponent<PonyController>().EnableForzen = flag;
            this.OBJSoul.GetComponent<SoulController>().EnableForzen = flag;
        }


        #endregion

        #region ��Ʒ

        [Header("��Ʒ")]
        /// <summary>
        /// ��Ʒ��ӵ�е�
        /// </summary>
        public Boolean ItemHasLight = false;
        /// <summary>
        /// ��Ʒ��ӵ����
        /// </summary>
        public Boolean ItemHasLyra = false;
        /// <summary>
        /// ��Ʒ��ʰȡ��Ʒʱ��
        /// </summary>
        public float ItemPickupTime = 2;
        /// <summary>
        /// ��Ʒ����ȡ��Ʒ��ʾƫ��
        /// </summary>
        public Vector2 ItemPickupOffset = new(0, 0.5f);
        /// <summary>
        /// ��Ʒ����ȡ��Ʒ��ͷ����
        /// </summary>
        public float ItemPickupCameraResize = -1;
        /// <summary>
        /// ��Ʒ�����õ�
        /// </summary>
        public Boolean ItemEnableLight;

        /// <summary>
        /// ��Ʒ����ʾ��
        /// </summary>
        /// <param name="enable">����</param>
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
        /// ��Ʒ����ȡ��Ʒ
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

        #region ħ��

        [Header("ħ��")]
        /// <summary>
        /// ħ������ǰӵ�е�ħ��
        /// </summary>
        public StayAwayGame.Magic MagicCurrent;
        /// <summary>
        /// ħ����ʣ��ħ����
        /// </summary>
        public int MagicLeft;
        /// <summary>
        /// ħ����ħ��ʹ�ô���
        /// </summary>
        public int MagicUsageCount = 5;
        /// <summary>
        /// ħ������ȡħ��ʱ��
        /// </summary>
        public float MagicGetTime = 2;
        /// <summary>
        /// ħ������ȡħ����ʾƫ��
        /// </summary>
        public Vector2 MagicGetOffset = new(0, 0.5f);
        /// <summary>
        /// ħ������ȡħ����ͷ����
        /// </summary>
        public float MagicGetCameraResize = -1;

        /// <summary>
        /// ħ��������ħ��
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
        /// ħ������ȡħ��
        /// </summary>
        /// <param name="magic">ħ������</param>
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

        #region ��Ⱦ

        [Header("��Ⱦ")]
        /// <summary>
        /// ��Ⱦ�����͸����
        /// </summary>
        public float RenderSoulPellucidity = 0.5f;
        /// <summary>
        /// ��Ⱦ��������Ч
        /// </summary>
        public Boolean RenderEnableEffect = true;

        /// <summary>
        /// ��Ⱦ�����ڴ���
        /// </summary>
        private Volume _renderVolume;
        /// <summary>
        /// ��Ⱦ�����ڴ���ɫ��
        /// </summary>
        private ChromaticAberration _renderVolumeCHA;
        /// <summary>
        /// ��Ⱦ�����ڽ���
        /// </summary>
        private Vignette _renderVolumeVGN;
        /// <summary>
        /// ��Ⱦ������ɫ������
        /// </summary>
        private ColorAdjustments _renderVolumeCAJ;
        /// <summary>
        /// ��Ⱦ�����ڸ߹�
        /// </summary>
        private Bloom _renderVolumeBM;

        /// <summary>
        /// ��Ⱦ�������Ч
        /// </summary>
        public void ClearEffect()
        {
            // �����Ч
            this._renderVolumeVGN.intensity.SetValue(new FloatParameter(0));
            this._renderVolumeCAJ.saturation.SetValue(new FloatParameter(0));
            this._renderVolumeCHA.intensity.SetValue(new FloatParameter(0));
            this._renderVolumeBM.intensity.SetValue(new FloatParameter(0));
            this._renderVolumeCAJ.hueShift.SetValue(new FloatParameter(0));
            // �����
            this._DSTCircleScript.SetColor(Color.clear);
            this._DSTCircleScript.DrawCircle();
            this._DSTCircleScript.Enable = false;
        }

        /// <summary>
        /// ��Ⱦ����������
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

        #region �����߼�

        [Header("�����߼�")]
        /// <summary>
        /// �����߼�����СС����������
        /// </summary>
        public float DSTMinimalPonyToSoulDistance = 1f;
        /// <summary>
        /// �����߼������С����������
        /// </summary>
        public float DSTMaximalPonyToSoulDistance = 5f;
        /// <summary>
        /// �����߼�����������ʱ��
        /// </summary>
        public float DSTTooCloseDeadTime = 2f;
        /// <summary>
        /// �����߼�����������˥��
        /// </summary>
        public float DSTTooCloseDeadDecayTime = 4f;
        /// <summary>
        /// �����߼�����Զ����ʱ��
        /// </summary>
        public float DSTTooFarDeadTime = 2f;
        /// <summary>
        /// �����߼�����Զ����˥��
        /// </summary>
        public float DSTTooFarDeadDecayTime = 4f;
        /// <summary>
        /// �����߼��������þ���Ч����
        /// </summary>
        public float DSTIllusionDistance = 1f;
        /// <summary>
        /// �����߼����þ���Чǿ��
        /// </summary>
        public float DSTIllusionWeight = 1f;
        /// <summary>
        /// �����߼���������Чǿ��
        /// </summary>
        public float DSTBlackoutWeight = 1f;
        /// <summary>
        /// �����߼�����ɫ��Ч����
        /// </summary>
        public float DSTFadingDistance = 1f;
        /// <summary>
        /// �����߼�����ɫ��Чǿ��
        /// </summary>
        public float DSTFadingWeight = 0.8f;
        /// <summary>
        /// �����߼���������Чǿ��
        /// </summary>
        public float DSTVanishWeight = 0.2f;
        /// <summary>
        /// �����߼������뻷Ĭ��ɫ��
        /// </summary>
        public Color DSTCircleDefaultColor = new(255, 255, 255);
        /// <summary>
        /// �����߼������뻷Σ��ɫ��
        /// </summary>
        public Color DSTCircleDangerColor = new(255, 100, 100);
        /// <summary>
        /// �����߼������뻷͸����
        /// </summary>
        public float DSTCirclePellucidity = 0.5f;
        /// <summary>
        /// �����߼������뻷��ʾ����
        /// </summary>
        public float DSTCircleDisplayDistance = 2;

        /// <summary>
        /// �����߼���С����������
        /// </summary>
        private float _DSTPonyToSoulDistance;
        /// <summary>
        /// �����߼���������������
        /// </summary>
        private float _DSTTooCloseDeadRatio;
        /// <summary>
        /// �����߼�����Զ��������
        /// </summary>
        private float _DSTTooFarDeadRatio;
        /// <summary>
        /// �����߼������뻷�ű�
        /// </summary>
        private CircleRender _DSTCircleScript;

        /// <summary>
        /// �����߼����������
        /// </summary>
        void CalcDistance()
        {
            // �������
            this._DSTPonyToSoulDistance = Vector2.Distance(this._OBJPonyTransform.position, this._OBJSoulTransform.position);
            // С���ӽǣ�С����������������
            if (this._DSTPonyToSoulDistance <= this.DSTMinimalPonyToSoulDistance)
            {
                if (this._DSTTooCloseDeadRatio >= 1)
                {
                    // С������
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
            SetAudioPitch(this._CHARCurrentCharacter ? 1 - this._DSTTooCloseDeadRatio : 1); // ��������
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


            // С���ӽǣ�С������������ʾ�þ�
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
            

            // ����ӽǣ�С�������Զ��������
            if (this._DSTPonyToSoulDistance >= this.DSTMaximalPonyToSoulDistance)
            {
                if (this._DSTTooFarDeadRatio >= 1)
                {
                    // �������
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

            // ����ӽǣ�С�������̫Զ����ɫ
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


            // ������뻷
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

        #region �ӽ�

        [Header("�ӽ�")]
        public float CameraSize = 3f;

        /// <summary>
        /// �ӽǣ������ӽ�
        /// </summary>
        void RestoreCameraSize()
        {
            this.OBJMainCamera.GetComponent<CameraController>().Resize();
        }

        #endregion

        #region GUI

        /// <summary>
        /// GUI���˵�����ʾ
        /// </summary>
        private bool _GUIDisplayMenu;

        #endregion

        #region ����

        [Header("����")]
        /// <summary>
        /// ���̣���һ������
        /// </summary>
        public StayAwayGame.Level FlowNextLevel = StayAwayGame.Level.End;
        /// <summary>
        /// ���̣���һ������
        /// </summary>
        public StayAwayGame.Level FlowThisLevel = StayAwayGame.Level.End;
        /// <summary>
        /// ���̣���Ϸ�����¼��б�
        /// </summary>
        public UnityEvent<int> FlowGameOverEvent = new();

        /// <summary>
        /// ���̣���Ϸ����ԭ��
        /// </summary>
        private int _flowGameOverReason;

        /// <summary>
        /// ���̣�������Ϸ��������
        /// </summary>
        /// <param name="reason">0 �ɹ� 1 С���� 2 �����</param>
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
                this.OBJGUI.GetComponent<GUIScript>().PlayText("�㱻�þ�������");
            }
            else if (this._flowGameOverReason == 2)
            {
                this.OBJGUI.GetComponent<GUIScript>().PlayText("���������ɢ��\nû�����ı���������ɭ���ｫ�粽����");
            }
            else if (this._flowGameOverReason == 3)
            {
                this.OBJGUI.GetComponent<GUIScript>().PlayText("������");
            }
            else if (this._flowGameOverReason == 4)
            {
                this.OBJGUI.GetComponent<GUIScript>().PlayText("δ�����������");
            }
        }

        /// <summary>
        /// ���̣���Ϸ���������Ч
        /// </summary>
        public void FlowGameOverClearEffect()
        {
            this.OBJGUI.GetComponent<GUIScript>().AnimationDoneStepEvent.RemoveListener(FlowGameOverClearEffect);
            this.RenderEnableEffect = false;

            ClearEffect();
        }

        /// <summary>
        /// ���̣���Ϸ�����������
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