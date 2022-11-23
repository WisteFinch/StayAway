using StayAwayGameScript;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameScript
{
    public class SoulAnimationHandler : MonoBehaviour
    {
        /// <summary>
        /// �Ƿ�ת
        /// </summary>
        public Boolean IsFlip = false;

        /// <summary>
        /// ��������
        /// </summary>
        private SoulController _controller;
        /// <summary>
        /// ����������
        /// </summary>
        private Animator _animator;
        /// <summary>
        /// ��Ⱦ��
        /// </summary>
        private SpriteRenderer _renderer;

        void Start()
        {
            this._controller = GetComponent<SoulController>();
            this._animator = this.GetComponentInChildren<Animator>();
            this._renderer = this.GetComponentInChildren<SpriteRenderer>();
            this.IsFlip = false;
            this._animator.SetBool("IsFlying", true);
        }

        void Update()
        {
            if (this._controller.Velocity.x > 0)
            {
                this.IsFlip = true;
            }
            else if (this._controller.Velocity.x < 0)
            {
                this.IsFlip = false;
            }
            this._renderer.flipX = this.IsFlip;
        }
    }
}
