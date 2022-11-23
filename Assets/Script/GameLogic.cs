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
            /// �л���ɫ
            /// </summary>
            public bool ChangeCharacter;
        }

        #region ���б���
        [Header("����")]
        /// <summary>
        /// С������
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
        public float CameraSize = 30f;

        [Header("��Ч")]
        /// <summary>
        /// �����þ���Ч����
        /// </summary>
        public float IllusionDistance = 2f;
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
        public float FadingDistance = 2f;
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


        #endregion

        #region ˽�б���
        /// <summary>
        /// ����
        /// </summary>
        private FrameInput input;
        /// <summary>
        /// С��������
        /// </summary>
        private Transform _ponyTransform;
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

            // ��ʼ��
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
        /// ��ȡ����
        /// </summary>
        void GatherInput()
        {
            this.input.ChangeCharacter = UnityEngine.Input.GetKeyDown(KeyCode.R);
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

            // С���ӽǣ�С������������ʾ�þ�
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

            // ����ӽǣ�С�������Զ��������
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

            // ����ӽǣ�С�������̫Զ����ɫ
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

            // ������뻷
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
        /// �����ӽ�
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