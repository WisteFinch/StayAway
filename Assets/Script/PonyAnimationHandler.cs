using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameController
{
    public class PonyAnimationHandler : MonoBehaviour
    {

        /// <summary>
        /// �Ƿ�ת
        /// </summary>
        public Boolean IsFlip = false;

        /// <summary>
        /// С�������
        /// </summary>
        private PonyController _controller;
        /// <summary>
        /// ����������
        /// </summary>
        private Animator _animator;
        /// <summary>
        /// ��Ⱦ��
        /// </summary>
        private SpriteRenderer _renderer;
        /// <summary>
        /// �Ƿ�ת����
        /// </summary>
        public Boolean _isFlipCache = false;

        // Start is called before the first frame update
        void Start()
        {
            this._controller = GetComponent<PonyController>();
            this._animator = this.GetComponentInChildren<Animator>();
            this._renderer = this.GetComponentInChildren<SpriteRenderer>();
            this._isFlipCache = false;
            this.IsFlip = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (this._controller.Velocity.x > 0)
            {
                this._isFlipCache = false; 
            }
            else if (this._controller.Velocity.x < 0)
            {
                this._isFlipCache = true;
            }
            if(this.IsFlip != this._isFlipCache)
            {
                this.IsFlip = this._isFlipCache;
                this._renderer.flipX = !this.IsFlip;
            }
            this._animator.SetBool("IsTroting", this._controller.Velocity.x != 0);
            this._animator.SetBool("IsFlying", this._controller.IsGliding);
        }
    }
}
