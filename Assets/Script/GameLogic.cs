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
        }

        #region ���б���
        [Header("��Ϣ")]
        /// <summary>
        /// ӵ�е�
        /// </summary>
        public Boolean HasLight;
        /// <summary>
        /// ӵ����
        /// </summary>
        public Boolean HasLyra;
        /// <summary>
        /// С������
        /// </summary>
        public Boolean PonyIsDead;
        /// <summary>
        /// �������
        /// </summary>
        public Boolean SoulIsDead;
        /// <summary>
        /// ��ǰӵ�е�ħ��
        /// </summary>
        public StayAwayGame.Magic CurrentMagic;
        /// <summary>
        /// ʣ��ħ����
        /// </summary>
        public int MagicLeft;
        /// <summary>
        /// ��һ������
        /// </summary>
        public string NextLevelName = "level_end";
        /// <summary>
        /// ������Ч
        /// </summary>
        Boolean EnableEffect = true;


        [Header("����")]
        /// <summary>
        /// С�����
        /// </summary>
        public UnityEngine.Object Pony;
        /// <summary>
        /// ������
        /// </summary>
        public UnityEngine.Object Soul;
        /// <summary>
        /// �������
        /// </summary>
        public Camera MainCamera;
        /// <summary>
        /// ����Ⱦ
        /// </summary>
        public UnityEngine.Object GlobalVolume;
        /// <summary>
        /// UI����
        /// </summary>
        public GameObject UI;

        [Header("����")]
        /// <summary>
        /// ��СС����������
        /// </summary>
        public float MinimalPonyToSoulDistance = 1f;
        /// <summary>
        /// ���С����������
        /// </summary>
        public float MaximalPonyToSoulDistance = 5f;
        /// <summary>
        /// ��������ʱ��
        /// </summary>
        public float TooCloseDeadTime = 2f;
        /// <summary>
        /// ��������˥��
        /// </summary>
        public float TooCloseDeadDecayTime = 4f;
        /// <summary>
        /// ��Զ����ʱ��
        /// </summary>
        public float TooFarDeadTime = 2f;
        /// <summary>
        /// ��Զ����˥��
        /// </summary>
        public float TooFarDeadDecayTime = 4f;

        [Header("�ӽ�")]
        public float CameraSize = 3f;

        [Header("��Ч")]
        /// <summary>
        /// ���͸����
        /// </summary>
        public float SoulPellucidity = 0.5f;
        /// <summary>
        /// �����þ���Ч����
        /// </summary>
        public float IllusionDistance = 1f;
        /// <summary>
        /// �þ���Чǿ��
        /// </summary>
        public float IllusionWeight = 1f;
        /// <summary>
        /// ������Чǿ��
        /// </summary>
        public float BlackoutWeight = 1f;
        /// <summary>
        /// ��ɫ��Ч����
        /// </summary>
        public float FadingDistance = 1f;
        /// <summary>
        /// ��ɫ��Чǿ��
        /// </summary>
        public float FadingWeight = 0.8f;
        /// <summary>
        /// ������Чǿ��
        /// </summary>
        public float VanishWeight = 0.2f;
        /// <summary>
        /// ���뻷Ĭ��ɫ��
        /// </summary>
        public Color DistanceCircleDefaultColor = new(255, 255, 255);
        /// <summary>
        /// ���뻷Σ��ɫ��
        /// </summary>
        public Color DistanceCircleDangerColor = new(255, 100, 100);
        /// <summary>
        /// ���뻷͸����
        /// </summary>
        public float DistanceCirclePellucidity = 0.5f;
        /// <summary>
        /// ���뻷��ʾ����
        /// </summary>
        public float DistanceCircleDisplayDistance = 2;
        /// <summary>
        /// С�����ɫ
        /// </summary>
        public Color PonyLightColorStart, PonyLightColorEnd;
        /// <summary>
        /// ������ɫ
        /// </summary>
        public Color SoulLightColorStart, SoulLightColorEnd;

        [Header("��Ʒ")]
        /// <summary>
        /// �ƶ���
        /// </summary>
        public UnityEngine.Object Light;

        [Header("ħ��")]
        /// <summary>
        /// ħ��ʹ�ô���
        /// </summary>
        public int MagicUsageCount = 5;

        #endregion

        #region ˽�б���
        /// <summary>
        /// ����
        /// </summary>
        private FrameInput input;
        /// <summary>
        /// С�������
        /// </summary>
        private Transform _ponyTransform;
        /// <summary>
        /// С����Ƶ���
        /// </summary>
        private AudioPlayer _ponyAudio;
        /// <summary>
        /// ��������
        /// </summary>
        private Transform _soulTransform;
        /// <summary>
        /// С����������
        /// </summary>
        private float _ponyToSoulDistance;
        /// <summary>
        /// ���ڴ���
        /// </summary>
        private Volume _volume;
        /// <summary>
        /// ���ڴ���ɫ��
        /// </summary>
        private ChromaticAberration _volumeCHA;
        /// <summary>
        /// ���ڽ���
        /// </summary>
        private Vignette _volumeVGN;
        /// <summary>
        /// ����ɫ������
        /// </summary>
        private ColorAdjustments _volumeCAJ;
        /// <summary>
        /// ���ڸ߹�
        /// </summary>
        private Bloom _volumeBM;
        /// <summary>
        /// ����������
        /// </summary>
        private float _tooCloseDeadRatio;
        /// <summary>
        /// ��Զ������
        /// </summary>
        private float _tooFarDeadRatio;
        /// <summary>
        /// ��ǰ��ɫ
        /// </summary>
        private Boolean _currentCharacter;
        /// <summary>
        /// ���뻷�ű�
        /// </summary>
        private CircleRender _distanceCircleScript;
        /// <summary>
        /// ��ʾ��
        /// </summary>
        private Boolean _displayedLight;
        /// <summary>
        /// ��ס������
        /// </summary>
        private Boolean _isControllerLocked;
        /// <summary>
        /// ��Ϸ����ԭ��
        /// </summary>
        private int _gameOverReason;

        #endregion

        void Start()
        {
            // ��ȡ���
            this._ponyTransform = this.Pony.GetComponent<Transform>();
            this._soulTransform = this.Soul.GetComponent<Transform>();
            this._volume = this.GlobalVolume.GetComponent<Volume>();
            this._volume.profile.TryGet<ChromaticAberration>(out this._volumeCHA);
            this._volume.profile.TryGet<Vignette>(out this._volumeVGN);
            this._volume.profile.TryGet<ColorAdjustments>(out this._volumeCAJ);
            this._volume.profile.TryGet<Bloom>(out this._volumeBM);
            this._distanceCircleScript = this.GetComponentInChildren<CircleRender>();

            this._ponyAudio = this.Pony.GetComponentInChildren<AudioPlayer>();

            // ��ʼ��
            this.MainCamera.GetComponent<Camera>().orthographicSize = this.CameraSize;
            this._currentCharacter = true;
            this.Pony.GetComponent<PonyController>().EnableControl = true;
            this.Soul.GetComponent<SoulController>().EnableControl = false;
            this.MainCamera.GetComponent<CameraController>().SetTarget(this.Pony);
            this.Soul.GetComponent<SoulController>().SetAIEnable(false);

            this.HasLight = false;
            this.HasLyra = false;

            this.Light.GameObject().SetActive(false);

            this.EnableEffect = true;

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
        /// ��ȡ����
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
        /// �������
        /// </summary>
        void CalcDistance()
        {
            // �������
            this._ponyToSoulDistance = Vector2.Distance(this._ponyTransform.position, this._soulTransform.position);
            // С���ӽǣ�С����������������
            if (this._ponyToSoulDistance <= this.MinimalPonyToSoulDistance)
            {
                if (this._tooCloseDeadRatio >= 1)
                {
                    // С������
                    CharacterDead(true, 1);
                    this._ponyAudio.SetPitchOffset();
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
            SetAudioPitch(1 - this._tooCloseDeadRatio);
            if (this.EnableEffect)
            {
                if (_currentCharacter)
                {
                    this._volumeVGN.intensity.SetValue(new FloatParameter(Mathf.Clamp(this._tooCloseDeadRatio * this.BlackoutWeight, 0, 1f)));
                }
                else
                {
                    this._volumeVGN.intensity.SetValue(new FloatParameter(0));
                }
            }


            // С���ӽǣ�С������������ʾ�þ�
            if (this.EnableEffect)
            {
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
            }
            

            // ����ӽǣ�С�������Զ��������
            if (this._ponyToSoulDistance >= this.MaximalPonyToSoulDistance)
            {
                if (this._tooFarDeadRatio >= 1)
                {
                    // �������
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
            if(this.EnableEffect)
            {
                if (!_currentCharacter)
                {
                    this._volumeBM.intensity.SetValue(new FloatParameter(Mathf.Clamp(this._tooFarDeadRatio * this.VanishWeight * 100, 0, 100f)));
                }
                else
                {
                    this._volumeBM.intensity.SetValue(new FloatParameter(0));
                }
            }

            // ����ӽǣ�С�������̫Զ����ɫ
            if(this.EnableEffect)
            {
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
            }


            // ������뻷
            if (this.EnableEffect) 
            {
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
        }

        void SetAudioPitch(float pitch)
        {
            var list = GameObject.FindGameObjectsWithTag("Audio");
            foreach (var item in list)
            {
                item.GetComponent<AudioSource>().pitch = pitch;
            }
        }

        /// <summary>
        /// �������
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

        // ����ħ��
        void CalcMagic()
        {
            if(this.input.UseMagic && this.HasLyra && this.CurrentMagic!=StayAwayGame.Magic.None && this.MagicLeft > 0)
            {
                this.MagicLeft--;
                this.UI.GetComponent<GUIScript>().SetMagicCount(this.MagicLeft, this.MagicUsageCount);
                this.Soul.GetComponent<MagicEmitter>().EmitMagic(this.CurrentMagic);
            }
        }

        /// <summary>
        /// ��ʾ��
        /// </summary>
        /// <param name="enable">����</param>
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
            // �����Ч
            this._volumeCAJ.saturation.SetValue(new FloatParameter(0));
            this._volumeCHA.intensity.SetValue(new FloatParameter(0));
            this._volumeBM.intensity.SetValue(new FloatParameter(0));
            this._volumeCAJ.hueShift.SetValue(new FloatParameter(0));
            // �����
            this._distanceCircleScript.SetColor(Color.clear);
            this._distanceCircleScript.DrawCircle();
            this._distanceCircleScript.Enable = false;

            if (character)
            {
                // ======== Pony Dead ========
                this.PonyIsDead = true;

                // ��������
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

                // ���Ŀ���
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
                this.UI.GetComponent<GUIScript>().GetLyra(true);

            }
            else if(item == StayAwayGame.Item.ItemLight)
            {
                this.HasLight = true;
                this.UI.GetComponent<GUIScript>().GetLight(true);
            }
        }

        public void GetMagic(StayAwayGame.Magic magic)
        {
            this.CurrentMagic = magic;
            this.MagicLeft = this.MagicUsageCount;
            this.UI.GetComponent<GUIScript>().GetMagic(magic);
            this.UI.GetComponent<GUIScript>().SetMagicCount(this.MagicLeft, this.MagicUsageCount);
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

        #region ��Ϸ����

        /// <summary>
        /// ��Ϸ����
        /// </summary>
        /// <param name="reason">0 �ɹ� 1 С���� 2 �����</param>
        public void GameOver(int reason)
        {
            SetControllerLock(true);
            this._gameOverReason = reason;

            this.UI.GetComponent<GUIScript>().AnimationDoneEvent.AddListener(GameOverDone);
            this.UI.GetComponent<GUIScript>().AnimationDoneStepEvent.AddListener(GameOverClearEffect);

            if (this._gameOverReason == 0)
            {
                this.UI.GetComponent<GUIScript>().PlayText("��Ϸ����");
            }
            else if (this._gameOverReason == 1)
            {
                this.UI.GetComponent<GUIScript>().PlayText("�㱻�þ�������");
            }
            else if (this._gameOverReason == 2)
            {
                this.UI.GetComponent<GUIScript>().PlayText("���������ɢ��\nû�����ı���������ɭ���ｫ�粽����");
            }
            else if (this._gameOverReason == 3)
            {
                this.UI.GetComponent<GUIScript>().PlayText("������");
            }
        }

        public void GameOverClearEffect()
        {
            this.UI.GetComponent<GUIScript>().AnimationDoneStepEvent.RemoveListener(GameOverClearEffect);
            this.EnableEffect = false;

            // �����Ч
            this._volumeVGN.intensity.SetValue(new FloatParameter(0));
            this._volumeCAJ.saturation.SetValue(new FloatParameter(0));
            this._volumeCHA.intensity.SetValue(new FloatParameter(0));
            this._volumeBM.intensity.SetValue(new FloatParameter(0));
            this._volumeCAJ.hueShift.SetValue(new FloatParameter(0));
            // �����
            this._distanceCircleScript.SetColor(Color.clear);
            this._distanceCircleScript.DrawCircle();
            this._distanceCircleScript.Enable = false;
        }

        public void GameOverDone()
        {
            this.UI.GetComponent<GUIScript>().AnimationDoneEvent.RemoveListener(GameOverDone);
            SceneManager.LoadScene(this.NextLevelName);
        }

        #endregion
    }
}