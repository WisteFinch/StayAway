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
        /// ����������
        /// </summary>
        private Animator _animator;
        private int _currentPlaying = 0; // 0 �� 1 ���� 2 ���� 3 ����
        /// <summary>
        /// ����Ļ��
        /// </summary>
        Boolean _curtainOn;
        /// <summary>
        /// ��������
        /// </summary>
        Boolean _textOn;

        void Start()
        {
            this._animator = GetComponent<Animator>();
            SetMagicCount(0, 0);
            GetMagic(StayAwayGame.Magic.None);
        }

        /// <summary>
        /// ����ħ����
        /// </summary>
        /// <param name="c">ʣ��</param>
        /// <param name="all">�ܼ�</param>
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
    }
}