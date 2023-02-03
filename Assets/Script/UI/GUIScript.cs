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
        /// 枚举动画目标
        /// </summary>
        public enum AnimationTartgetEnum
        {
            CurtainOn,
            CurtainOff,
            Text
        }
        /// <summary>
        /// 动画目标列表
        /// </summary>
        public List<AnimationTartgetEnum> AnimationTartget = new();

        /// <summary>
        /// 动画完成事件
        /// </summary>
        public UnityEvent AnimationDoneEvent = new();

        /// <summary>
        /// 动画完成步骤事件
        /// </summary>
        public UnityEvent AnimationDoneStepEvent = new();

        /// <summary>
        /// 提示列表
        /// </summary>
        public List<String> TipsList = new();

        /// <summary>
        /// 提示持续时间
        /// </summary>
        public float TipsDuration = 5f;

        /// <summary>
        /// 动画控制器
        /// </summary>
        private Animator _animator;

        //private int _currentPlaying = 0; // 0 无 1 渐黑 2 渐白 3 文字

        /// <summary>
        /// 开启幕布
        /// </summary>
        Boolean _curtainOn;

        /// <summary>
        /// 开启文字
        /// </summary>
        // Boolean _textOn;

        /// <summary>
        /// 提示剩余时间
        /// </summary>
        float _tipsTimeRemaining;

        /// <summary>
        /// 提示状态 -1进入 0显示 1退出 2允许切换
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
                // 显示阶段
                if(this._tipsStatus == 0)
                {
                    this._tipsTimeRemaining -= Time.deltaTime;
                    // 跳过或时间结束
                    if (GameManager.Instance.Input.InteractKeyDown || this._tipsTimeRemaining < 0)
                    {
                        this._tipsStatus = 1;
                        this._animator.SetTrigger("TipsOff");
                        this.TipsList.RemoveAt(0);
                    }
                }
                // 进入新提示
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
        /// 提示进入结束
        /// </summary>
        void TipsEnterFinish()
        {
            this._tipsStatus = 0;
        }

        /// <summary>
        /// 提示退出结束
        /// </summary>
        void TipsQuitFinish()
        {
            this._tipsStatus = 2;
        }

        /// <summary>
        /// 设置魔法量
        /// </summary>
        /// <param name="c">剩余</param>
        /// <param name="all">总计</param>
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
        /// 更新动画目标
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