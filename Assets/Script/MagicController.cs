using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        /// 最远距离
        /// </summary>
        public float MaxDistance = 10;
        /// <summary>
        /// 消失时间
        /// </summary>
        public float DisappearTime = 1;

        /// <summary>
        /// 开始追踪
        /// </summary>
        private Boolean _trackingStarted;
        private Rigidbody2D _rigid;
        private Vector2 _startForceRotation;
        private Vector2 _targetPos;
        private Vector2 _startPos;
        private Boolean _startForceUsability = false;
        private float _minDistance;
        private Boolean _hasTarget;
        private Boolean _disappearing = false;


        void Start()
        {
            this._rigid = this.GetComponent<Rigidbody2D>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!this._disappearing)
            {
                if (other.gameObject.CompareTag("Enemy"))
                {
                    other.GetComponent<EnemyController>().Dead();
                    Disappear();
                }
                else if (!(other.gameObject.CompareTag("Pony") || other.gameObject.CompareTag("Soul")))
                {
                    Disappear();
                }
            }
        }

        void FixedUpdate()
        {
            if(this._trackingStarted && !this._disappearing)
            {
                // 启始力
                if(this._startForceUsability && this.EnableStartForce)
                {
                    this._rigid.AddForce(this.Speed * this.StartForceRatio * Time.deltaTime * this._startForceRotation);
                    this._startForceUsability = false;
                }
                // 是否有目标
                if (this._hasTarget) // 有目标：飞向目标
                {
                    // 判断目标是否销毁
                    if(this.Target.gameObject == null)
                    {
                        this._hasTarget = false;
                    }
                    // 加力
                    Vector2 dir = ((Vector2)this.Target.transform.position - (Vector2)this.transform.position).normalized;
                    this._targetPos = dir;
                    Vector2 force = this.Speed * Time.deltaTime * dir;
                    this._rigid.AddForce(force);

                    // 错失判断
                    if (this.Target != null)
                    {
                        var dist = Vector2.Distance(this.transform.position, this.Target.transform.position);
                        this._minDistance = dist < this._minDistance ? dist : this._minDistance;
                        if (dist > this._minDistance + this.MissOutDestructDistance)
                        {
                            Disappear();
                        }
                    }
                }
                else // 无目标：飞行初始方向
                {
                    Vector2 force = this.Speed * Time.deltaTime * this._targetPos;
                    this._rigid.AddForce(force);
                }

                // 旋转
                float z = Vector2.SignedAngle(Vector2.right, this._rigid.velocity);
                this.transform.rotation = Quaternion.Euler(0, 0, z);

                // 超距离消失
                if (Vector2.Distance(this.transform.position, this._startPos) > this.MaxDistance)
                {
                    Disappear();
                }
            }
        }

        public void StartTracking(GameObject target, Vector2 startPos, Vector2 startForceRotation = new Vector2())
        {
            this._hasTarget = true;
            this.Target = target;
            this.transform.position = startPos;
            this._trackingStarted = true;
            this._startForceRotation = startForceRotation;
            this._startForceUsability = true;
            this._minDistance = float.MaxValue;
            this._startPos = startPos;
        }

        public void StartTracking(Vector2 targetPos, Vector2 startPos)
        {
            this._hasTarget = false;
            this.transform.position = startPos;
            this._trackingStarted = true;
            this._startForceRotation = targetPos;
            this._targetPos = targetPos;
            this._startForceUsability = true;
            this._startPos = startPos;
        }

        public void Disappear()
        {
            this._disappearing = true;
            var list = this.GetComponentsInChildren<ParticleSystem>();
            foreach(ParticleSystem p in list)
            {
                p.Stop();
            }
            Invoke(nameof(DestroyThis), this.DisappearTime);

            Destroy(this.GetComponentInChildren<TrailRenderer>());
        }

        public void DestroyThis()
        {

            Destroy(this.gameObject);
        }
    }
}