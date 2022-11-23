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
        /// ÊÇ·ñ·­×ª
        /// </summary>
        public Boolean IsFlip = false;

        /// <summary>
        /// Áé»ê¿ØÖÆÆ÷
        /// </summary>
        private SoulController _controller;
        /// <summary>
        /// ¶¯»­¿ØÖÆÆ÷
        /// </summary>
        private Animator _animator;
        /// <summary>
        /// äÖÈ¾Æ÷
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
