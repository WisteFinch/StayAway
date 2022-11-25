using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameScript
{
    public class EnemyController : MonoBehaviour
    {
        public GameObject Target;

        public float TrackingDistance = 5f;

        public Boolean EnableAI = true;

        public float DeadTime = 1f;

        [Header("AI寻路")]
        /// <summary>
        /// AI速度
        /// </summary>
        public float Speed = 200f;
        /// <summary>
        /// 切换下一导航点距离
        /// </summary>
        public float AINextWayPointDistance = 1f;
        /// <summary>
        /// AI更新寻路间隔
        /// </summary>
        public float AIUpdatePathInterval = 0.5f;
        /// <summary>
        /// 停止距离
        /// </summary>
        public float AIEndDiatance = 0f;

        /// <summary>
        /// 导航路径
        /// </summary>
        private Path _AIPath;
        /// <summary>
        /// 寻路器
        /// </summary>
        private Seeker _AISeeker;
        /// <summary>
        /// 刚体
        /// </summary>
        private Rigidbody2D _rigid;
        /// <summary>
        /// 渲染器
        /// </summary>
        private SpriteRenderer _renderer;
        /// <summary>
        /// 当前导航点
        /// </summary>
        private int _AICurrentWayPoint = 0;
        private float _deadTimeLeft;
        private Boolean _isDying = false;

        void Start()
        {
            this._rigid = this.GetComponent<Rigidbody2D>();
            this._AISeeker = this.GetComponent<Seeker>();
            this._renderer = this.GetComponent<SpriteRenderer>();

            this._rigid.freezeRotation = true;
        }

        #region AI寻路
        private void Update()
        {
            if (_isDying)
            {
                this._deadTimeLeft -= Time.deltaTime;
                if (this._deadTimeLeft < 0)
                {
                    Destroy(this.gameObject);
                    return;
                }
                Color c = Color.white;
                c.a = this._deadTimeLeft / this.DeadTime;
                this.GetComponent<SpriteRenderer>().color = c;
            }
            else
            {
                var targetDist = Vector2.Distance(this.transform.position, this.Target.GetComponent<Transform>().position);
                if (!this.EnableAI && targetDist <= this.TrackingDistance)
                {
                    SetAIEnable(true);
                }
                else if (this.EnableAI && targetDist > this.TrackingDistance)
                {
                    SetAIEnable(false);
                }
                if (this.EnableAI)
                {
                    if (this._rigid.velocity.x > 0)
                    {
                        this._renderer.flipX = true;
                    }
                    else if (this._rigid.velocity.x < 0)
                    {
                        this._renderer.flipX = false;
                    }
                }
            }
        }

        public void SetAIEnable(Boolean enable = false)
        {
            // 重置AI
            CancelInvoke();
            this._AICurrentWayPoint = 0;

            if (enable)
            {
                // 启动AI
                this.EnableAI = true;
                InvokeRepeating(nameof(AIUpdatePath), 0f, this.AIUpdatePathInterval);
            }
            else
            {
                // 关闭AI
                this.EnableAI = false;
            }
        }

        public void AIUpdatePath()
        {
            if (this._AISeeker.IsDone())
            {
                this._AISeeker.StartPath(this.transform.position, this.Target.GetComponent<Transform>().position, AIOnPathComplete);
            }
        }

        /// <summary>
        /// 路径搜索完成
        /// </summary>
        public void AIOnPathComplete(Path p)
        {
            if (!p.error)
            {
                this._AIPath = p;
                _AICurrentWayPoint = 0;
            }
        }

        void FixedUpdate()
        {
            if (this.EnableAI)
            {
                // 无导航路径则结束
                if (this._AIPath == null)
                {
                    return;
                }

                // 判断是否完成导航
                if (_AICurrentWayPoint >= this._AIPath.vectorPath.Count)
                {
                    this._AICurrentWayPoint = 0;
                    return;
                }

                // 与小马距离
                var targetDist = Vector2.Distance(this.transform.position, this.Target.GetComponent<Transform>().position);
                // 到达距离停止
                if (targetDist <= this.AIEndDiatance)
                {
                    return;
                }

                // 加力
                Vector2 dir = ((Vector2)this._AIPath.vectorPath[this._AICurrentWayPoint] - (Vector2)this.transform.position).normalized;
                Vector2 force = this.Speed * Time.deltaTime * dir;
                this._rigid.AddForce(force);



                // 距离当前导航点距离
                var wayPointDist = Vector2.Distance(this.transform.position, this._AIPath.vectorPath[this._AICurrentWayPoint]);
                // 下一导航点
                if (wayPointDist <= this.AINextWayPointDistance)
                {
                    this._AICurrentWayPoint++;
                }
            }
        }
        #endregion

        public void Dead()
        {
            this.EnableAI = false;
            this.SetAIEnable(false);
            this._rigid.velocity = Vector2.zero;
            this._deadTimeLeft = this.DeadTime;
            this._isDying = true;

            Destroy(this.GetComponent<CircleCollider2D>());
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Pony"))
            {
                Dead();
                other.GetComponent<GameLogic>().CharacterDead(true, 3);
            }
        }
    }
}