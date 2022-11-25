using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace StayAwayGameScript
{
    public class PonyAnimationHandler : MonoBehaviour
    {

        /// <summary>
        /// 是否翻转
        /// </summary>
        public Boolean IsFlip = false;
        /// <summary>
        /// 眨眼间隔
        /// </summary>
        public float EyesBlinkInterval = 5f;
        /// <summary>
        /// 眨眼随机范围
        /// </summary>
        public float EyesBlinkRandomRange = 1f;

        /// <summary>
        /// 小马控制器
        /// </summary>
        private PonyController _controller;
        /// <summary>
        /// 动画控制器
        /// </summary>
        private Animator _animator;
        /// <summary>
        /// 渲染器
        /// </summary>
        private SpriteRenderer _renderer;
        /// <summary>
        /// 上次眨眼时间
        /// </summary>
        public float _lastEyesBlinkTime;
        /// <summary>
        /// 下次眨眼间隔
        /// </summary>
        public float _nextEyesBlinkInterval;

        void Start()
        {
            this._controller = GetComponent<PonyController>();
            this._animator = this.GetComponentInChildren<Animator>();
            this._renderer = this.GetComponentInChildren<SpriteRenderer>();
            this.IsFlip = true;
            this._lastEyesBlinkTime = Time.time;
            this._nextEyesBlinkInterval = this.EyesBlinkInterval + UnityEngine.Random.Range(-this.EyesBlinkRandomRange, this.EyesBlinkRandomRange);
        }

        void Update()
        { 
            if(this._lastEyesBlinkTime + this._nextEyesBlinkInterval <= Time.time)
            {
                this._animator.SetTrigger("EyesBlink");
                this._lastEyesBlinkTime = Time.time;
                this._nextEyesBlinkInterval = this.EyesBlinkInterval + UnityEngine.Random.Range(-this.EyesBlinkRandomRange, this.EyesBlinkRandomRange);
            }
            if (this._controller.Velocity.x > 0)
            {
                this.IsFlip = true;
            }
            else if (this._controller.Velocity.x < 0)
            {
                this.IsFlip = false;
            }
            this._renderer.flipX = this.IsFlip;
            this._animator.SetBool("IsTroting", this._controller.Velocity.x != 0);
            this._animator.SetBool("IsFlying", this._controller.IsGliding);
        }
    }
}
