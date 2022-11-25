using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StayAwayGameScript
{
    public class UIScript : MonoBehaviour
    {
        public Sprite None, NoItem, ItemLight, ItemLyra, MagicWater;
        public GameObject UIItemLight, UIItemLyra, UIMagicCount, UIMagicType, UIText, UICurtain, UIE, UIL;

        public float CurtainTime = 1f, TextTime = 3f;
        public bool IsCurtainOn = false;
        public Boolean IsPlaying;

        public float PlayingTimeLeft;
        private int _currentPlaying = 0; // 0 ÎÞ 1 ½¥ºÚ 2 ½¥°× 3 ÎÄ×Ö

        void Start()
        {
            SetMagicCount(0, 0);
            GetMagic(StayAwayGame.Magic.None);
            this.UICurtain.GetComponent<SpriteRenderer>().color = Color.clear;
            this.UICurtain.SetActive(false);
        }

        private void Update()
        {
            if(this.IsPlaying)
            {
                if(this._currentPlaying == 1)
                {
                    this.PlayingTimeLeft -= Time.deltaTime;
                    Color c = Color.black;
                    c.a = (this.CurtainTime - this.PlayingTimeLeft) / this.CurtainTime;
                    if(this.PlayingTimeLeft < 0)
                    {
                        this.IsPlaying = false;
                        SetCurtain(true);
                        return;
                    }
                    this.UICurtain.GetComponent<SpriteRenderer>().color = c;
                }
                else if(this._currentPlaying == 2)
                {
                    this.PlayingTimeLeft -= Time.deltaTime;
                    Color c = Color.black;
                    c.a = this.PlayingTimeLeft / this.CurtainTime;
                    if (this.PlayingTimeLeft < 0)
                    {
                        this.IsPlaying = false;
                        SetCurtain(false);
                        return;
                    }
                    this.UICurtain.GetComponent<SpriteRenderer>().color = c;
                }
                else if (this._currentPlaying == 3)
                {
                    this.PlayingTimeLeft -= Time.deltaTime;
                    if(this.PlayingTimeLeft <= 0)
                    {
                        this.UIText.GetComponent<Text>().text = "";
                        this.IsPlaying = false;
                        return;
                    }
                }
                else
                {
                    this.IsPlaying = false;
                }
            }
        }

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
            if(m == StayAwayGame.Magic.MagicWaterBall)
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
            if(flag)
            {
                if(this.IsCurtainOn)
                {
                    return;
                }
                this.IsPlaying = true;
                this._currentPlaying = 1;
                this.PlayingTimeLeft = this.CurtainTime;
                this.UICurtain.SetActive(true);

                this.UIItemLight.SetActive(false);
                this.UIItemLyra.SetActive(false);
                this.UIMagicType.SetActive(false);
                this.UIMagicCount.SetActive(false);
                this.UIE.SetActive(false);
                this.UIL.SetActive(false);
            }
            else
            {
                if (!this.IsCurtainOn)
                {
                    return;
                }
                this.IsPlaying = true;
                this._currentPlaying = 2;
                this.PlayingTimeLeft = this.CurtainTime;
                this.UICurtain.SetActive(true);

                this.UIItemLight.SetActive(true);
                this.UIItemLyra.SetActive(true);
                this.UIMagicType.SetActive(true);
                this.UIMagicCount.SetActive(true);
                this.UIE.SetActive(true);
                this.UIL.SetActive(true);
            }
        }

        public void SetCurtain(Boolean flag)
        {
            if(flag)
            {
                this.UICurtain.SetActive(true);

                this.UIItemLight.SetActive(false);
                this.UIItemLyra.SetActive(false);
                this.UIMagicType.SetActive(false);
                this.UIMagicCount.SetActive(false);
                this.UIE.SetActive(false);
                this.UIL.SetActive(false);
                this.IsCurtainOn = true;
                this.UICurtain.GetComponent<SpriteRenderer>().color = Color.black;
            }
            else
            {
                this.IsCurtainOn = false;
                this.UICurtain.GetComponent<SpriteRenderer>().color = Color.clear;
                this.UICurtain.SetActive(false);

                this.UIItemLight.SetActive(true);
                this.UIItemLyra.SetActive(true);
                this.UIMagicType.SetActive(true);
                this.UIMagicCount.SetActive(true);
                this.UIE.SetActive(true);
                this.UIL.SetActive(true);
            }
        }

        public void PlayText(string str)
        {
            this.IsPlaying = true;
            this._currentPlaying = 3;
            this.PlayingTimeLeft = this.TextTime;
            this.UIText.GetComponent<Text>().text = str;
        }
    }
}