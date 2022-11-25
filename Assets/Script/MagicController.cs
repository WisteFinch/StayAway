using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameScript
{
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class MagicController : MonoBehaviour
    {
        /// <summary>
        /// 目标
        /// </summary>
        public GameObject Target;
        /// <summary>
        /// 速度
        /// </summary>
        public float Speed = 300;
        /// <summary>
        /// 允许初始方向力
        /// </summary>
        public Boolean EnableStartForce = true;
        /// <summary>
        /// 初始力系数
        /// </summary>
        public float StartForceRatio = 50;
        /// <summary>
        /// 错过目标自毁距离
        /// </summary>
        public float MissOutDestructDistance = 2;

        /// <summary>
        /// 开始追踪
        /// </summary>
        private Boolean _trackingStarted;
        private Rigidbody2D _rigid;
        private Vector2 _startForceRotation;
        private Boolean _startForceEnable = false;
        private float _minDistance;

        void Start()
        {
            this._rigid = this.GetComponent<Rigidbody2D>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject == this.Target)
            {
                Target.GetComponent<EnemyController>().Dead();
                Destroy(this.gameObject);
            }
        }

        void FixedUpdate()
        {
            if(this._trackingStarted)
            {
                // 启始力
                if(this._startForceEnable && this.EnableStartForce)
                {
                    this._rigid.AddForce(this.Speed * this.StartForceRatio * Time.deltaTime * this._startForceRotation);
                    this._startForceEnable = false;
                }

                // 加力
                Vector2 dir = ((Vector2)this.Target.transform.position - (Vector2)this.transform.position).normalized;
                Vector2 force = this.Speed * Time.deltaTime * dir;
                this._rigid.AddForce(force);

                // 旋转
                float z = Vector2.SignedAngle(Vector2.right, this._rigid.velocity);
                this.transform.rotation = Quaternion.Euler(0, 0, z);

                // 错失判断
                var dist = Vector2.Distance(this.transform.position, this.Target.transform.position);
                this._minDistance = dist < this._minDistance ? dist : this._minDistance;
                if(dist > this._minDistance + this.MissOutDestructDistance)
                {
                    Destroy(this.gameObject);
                }
            }
        }

        public void StartTracking(GameObject target, Vector2 startPos, Vector2 startForceRotation = new Vector2())
        {
            this.Target = target;
            this.transform.position = startPos;
            this._trackingStarted = true;
            this._startForceRotation = startForceRotation;
            this._startForceEnable = true;
            this._minDistance = float.MaxValue;
        }
    }
}