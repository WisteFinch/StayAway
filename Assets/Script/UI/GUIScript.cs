using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace StayAwayGameScript
{
    public class GUIScript : MonoBehaviour
    {
        public Sprite ItemLightOn, ItemLightOff, ItemLightDisable, ItemLyra, ItemLyraDisable;
        public GameObject UIItemLight, UIItemLyra, UIMagic, UIText, UITipsText, UITipsPB;

        public float TextTime = 3f;
        public bool IsCurtainOn = false;
        public Boolean IsPlaying;
        public Boolean HasLight, HasLyra, LightState;
        public Color MagicNone, MagicWater;

        /// <summary>
        /// ö�ٶ���Ŀ��
        /// </summary>
        public enum AnimationTartgetEnum
        {
            CurtainOn,
            CurtainOff,
            Text
        }
        /// <summary>
        /// ����Ŀ���б�
        /// </summary>
        public List<AnimationTartgetEnum> AnimationTartget = new();

        /// <summary>
        /// ��������¼�
        /// </summary>
        public UnityEvent AnimationDoneEvent = new();

        /// <summary>
        /// ������ɲ����¼�
        /// </summary>
        public UnityEvent AnimationDoneStepEvent = new();

        /// <summary>
        /// ��ʾ�б�
        /// </summary>
        public List<String> TipsList = new();

        /// <summary>
        /// ��ʾ����ʱ��
        /// </summary>
        public float TipsDuration = 5f;

        /// <summary>
        /// ����������
        /// </summary>
        private Animator _animator;

        //private int _currentPlaying = 0; // 0 �� 1 ���� 2 ���� 3 ����

        /// <summary>
        /// ����Ļ��
        /// </summary>
        Boolean _curtainOn;

        /// <summary>
        /// ��������
        /// </summary>
        // Boolean _textOn;

        /// <summary>
        /// ��ʾʣ��ʱ��
        /// </summary>
        float _tipsTimeRemaining;

        /// <summary>
        /// ��ʾ״̬ -1���� 0��ʾ 1�˳� 2�����л�
        /// </summary>
        int _tipsStatus = 0;

        Text _tipsText;
        Image _tipsPB;

        void Start()
        {
            this._animator = GetComponent<Animator>();
            SetMagicCount(0, 0);
            GetMagic(StayAwayGame.Magic.None);

            this._tipsText = this.UITipsText.GetComponent<Text>();
            this._tipsPB = this.UITipsPB.GetComponent<Image>();
            _tipsStatus = 2;
        }

        private void Update()
        {
            this._tipsPB.fillAmount = Mathf.Clamp01(this._tipsTimeRemaining / this.TipsDuration);
            if(this.TipsList.Count > 0)
            {
                // ��ʾ�׶�
                if(this._tipsStatus == 0)
                {
                    this._tipsTimeRemaining -= Time.deltaTime;
                    // ������ʱ�����
                    if (GameManager.Instance.Input.InteractKeyDown || this._tipsTimeRemaining < 0)
                    {
                        this._tipsStatus = 1;
                        this._animator.SetTrigger("TipsOff");
                        this.TipsList.RemoveAt(0);
                    }
                }
                // ��������ʾ
                if(this._tipsStatus == 2)
                {
                    this._tipsStatus = -1;
                    this._tipsText.text = this.TipsList[0];
                    this._tipsTimeRemaining = this.TipsDuration;
                    this._animator.SetTrigger("TipsOn");
                }
            }
        }

        /// <summary>
        /// ��ʾ�������
        /// </summary>
        void TipsEnterFinish()
        {
            this._tipsStatus = 0;
        }

        /// <summary>
        /// ��ʾ�˳�����
        /// </summary>
        void TipsQuitFinish()
        {
            this._tipsStatus = 2;
        }

        /// <summary>
        /// ����ħ����
        /// </summary>
        /// <param name="c">ʣ��</param>
        /// <param name="all">�ܼ�</param>
        public void SetMagicCount(int c, int all)
        {
            this.UIMagic.GetComponent<Image>().fillAmount = this.HasLyra ? (float)c / (float)all : 0;
        }

        public void SetItemUsable(StayAwayGame.Item item, bool flag)
        {
            switch(item)
            {
                case StayAwayGame.Item.ItemLight: this.HasLight = flag; this.UIItemLight.GetComponent<Image>().sprite = this.HasLight ? this.LightState ? this.ItemLightOn : this.ItemLightOff : this.ItemLightDisable; break;
                case StayAwayGame.Item.ItemLyra: this.HasLyra = flag; this.UIItemLyra.GetComponent<Image>().sprite = this.HasLyra ? this.ItemLyra : this.ItemLyraDisable; break;
            }
        }

        public void SetLightStatus(bool flag)
        {
            this.LightState = flag;
            this.UIItemLight.GetComponent<Image>().sprite = this.HasLight ? this.LightState ? this.ItemLightOn : this.ItemLightOff : this.ItemLightDisable;
        }

        public void GetMagic(StayAwayGame.Magic m)
        {
            if (m == StayAwayGame.Magic.MagicWaterBall)
            {
                this.UIMagic.GetComponent<Image>().color = this.MagicWater;
            }
            else
            {
                this.UIMagic.GetComponent<Image>().color = this.MagicNone;
            }
        }

        public void PlayCurtain(Boolean flag)
        {
            if(flag != this._curtainOn)
            {
                if (flag)
                {
                    this._animator.SetTrigger("CurtainOn");
                }
                else
                {
                    this._animator.SetTrigger("CurtainOff");
                }
                this._curtainOn = !this._curtainOn;
            }
        }

        public void PlayText(string str, Boolean enableCurtainBefore = true, Boolean enableCurtainAfter = true)
        {
            this.UIText.GetComponent<Text>().text = str;
            if(enableCurtainBefore)
            {
                this.AnimationTartget.Add(AnimationTartgetEnum.CurtainOn);
            }
            this.AnimationTartget.Add(AnimationTartgetEnum.Text);
            if (!enableCurtainAfter)
            {
                this.AnimationTartget.Add(AnimationTartgetEnum.CurtainOff);
            }
            UpdateAnimationTarget();
        }

        /// <summary>
        /// ���¶���Ŀ��
        /// </summary>
        public void UpdateAnimationTarget()
        {
            if (this.AnimationTartget.Count != 0)
            {
                if (this.AnimationTartget[0] == AnimationTartgetEnum.CurtainOn)
                {
                    if (this._curtainOn)
                    {
                        AnimationTargetDone();
                    }
                    else
                    {
                        this._animator.SetTrigger("CurtainOn");
                    }
                }
                else if (this.AnimationTartget[0] == AnimationTartgetEnum.CurtainOff)
                {
                    if (!this._curtainOn)
                    {
                        AnimationTargetDone();
                    }
                    else
                    {
                        this._animator.SetTrigger("CurtainOff");
                    }
                }
                else if (this.AnimationTartget[0] == AnimationTartgetEnum.Text)
                {
                    this._animator.SetTrigger("TextOn");
                }
            }
            else
            {
                AnimationTargetDone();
            }
        }

        public void AnimationTargetDone()
        {
            this.AnimationDoneStepEvent.Invoke();
            if (this.AnimationTartget.Count != 0)
            {
                this.AnimationTartget.RemoveAt(0);
                UpdateAnimationTarget();
            }
            else
            {
                this.AnimationDoneEvent.Invoke();
                return;
            }
        }

        void OnAnimationTextOnFinished()
        {
            Invoke(nameof(TextOff), this.TextTime);
        }

        void TextOff()
        {
            this._animator.SetTrigger("TextOff");
        }

        public void OnMenuBack()
        {
            this._animator.SetTrigger("MenuOff");
            GameManager.Instance.GetLogic().SetControllerLock(false);
            GameManager.Instance.GetLogic().SetForzen(false);
        }

        public void OnMenuReload()
        {
            GameManager.Instance.LoadLevel();
        }

        public void OnMenuExit()
        {
            GameManager.Instance.LoadLevel(StayAwayGame.Level.Start);
        }

        public void ShowMenu()
        {
            this._animator.SetTrigger("MenuOn");
        }
    }
}