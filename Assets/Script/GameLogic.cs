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
            /// �л���ɫ
            /// </summary>
            public bool ChangeCharacter;
        }

        #region ���б���
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

        [Header("�ӽ�")]
        public float CameraSize = 3f;

        [Header("��Ч")]
        /// <summary>
        /// �����þ�����
        /// </summary>
        public float IllusionDistance = 1f;
        /// <summary>
        /// �þ�ǿ��
        /// </summary>
        public float IllusionWeight = 1f;

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
        private ChromaticAberration _volumeCA;
        /// <summary>
        /// ����٤������
        /// </summary>
        private LiftGammaGain _volumeLGG;
        /// <summary>
        /// �ϴι���ʱ��
        /// </summary>
        private float _lastTooCloseTime;
        /// <summary>
        /// ��ǰ��ɫ
        /// </summary>
        private Boolean _currentCharacter;
        #endregion

        void Start()
        {
            // ��ȡ���
            this._ponyTransform = this.Pony.GetComponent<Transform>();
            this._soulTransform = this.Soul.GetComponent<Transform>();
            this._volume = this.GlobalVolume.GetComponent<Volume>();
            this._volume.profile.TryGet<ChromaticAberration>(out this._volumeCA);
            this._volume.profile.TryGet<LiftGammaGain>(out this._volumeLGG);

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
            this._ponyToSoulDistance = Vector2.Distance(this._ponyTransform.position, this._soulTransform.position);
            // ��������������
            if (this._ponyToSoulDistance <= this.MinimalPonyToSoulDistance)
            {
                //if(this._lastTooCloseTime == -1)
                //{
                //    this._lastTooCloseTime = Time.time;
                //}
                //else if(this._lastTooCloseTime + this.TooCloseDeadTime >= Time.time)
                //{
                //    // ==================== ���� =======================
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
            // �����̫������ʾ�þ�
            if (this._ponyToSoulDistance <= this.MinimalPonyToSoulDistance + this.IllusionDistance)
            {
                this._volumeCA.intensity.SetValue(new FloatParameter(Mathf.Clamp((this.MinimalPonyToSoulDistance + this.IllusionDistance - this._ponyToSoulDistance) * this.IllusionDistance / this.IllusionDistance, 0, 1f)));
            }
        }


        /// <summary>
        /// �����ӽ�
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