using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace StayAwayGameScript
{
    public class GUIScript : MonoBehaviour
    {
        public Sprite None, NoItem, ItemLight, ItemLyra, MagicWater;
        public GameObject UIItemLight, UIItemLyra, UIMagicCount, UIMagicType, UIText;

        public float TextTime = 3f;
        public bool IsCurtainOn = false;
        public Boolean IsPlaying;

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
        /// 动画控制器
        /// </summary>
        private Animator _animator;
        private int _currentPlaying = 0; // 0 无 1 渐黑 2 渐白 3 文字
        /// <summary>
        /// 开启幕布
        /// </summary>
        Boolean _curtainOn;
        /// <summary>
        /// 开启文字
        /// </summary>
        Boolean _textOn;

        void Start()
        {
            this._animator = GetComponent<Animator>();
            SetMagicCount(0, 0);
            GetMagic(StayAwayGame.Magic.None);
        }

        /// <summary>
        /// 设置魔法量
        /// </summary>
        /// <param name="c">剩余</param>
        /// <param name="all">总计</param>
        public void SetMagicCount(int c, int all)
        {
            this.UIMagicCount.GetComponent<Text>().text = c.ToString() + "/" + all.ToString();
        }

        public void GetLight(Boolean b)
        {
            if (b)
            {
                this.UIItemLight.GetComponent<Image>().sprite = this.ItemLight;
            }
            else
            {
                this.UIItemLight.GetComponent<Image>().sprite = this.NoItem;
            }
        }

        public void GetLyra(Boolean b)
        {
            if (b)
            {
                this.UIItemLyra.GetComponent<Image>().sprite = this.ItemLyra;
            }
            else
            {
                this.UIItemLyra.GetComponent<Image>().sprite = this.NoItem;
            }
        }

        public void GetMagic(StayAwayGame.Magic m)
        {
            if (m == StayAwayGame.Magic.MagicWaterBall)
            {
                this.UIMagicType.GetComponent<Image>().sprite = this.MagicWater;
            }
            else
            {
                this.UIMagicType.GetComponent<Image>().sprite = this.None;
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
    }
}