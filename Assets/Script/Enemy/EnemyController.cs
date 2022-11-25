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

        [Header("AIѰ·")]
        /// <summary>
        /// AI�ٶ�
        /// </summary>
        public float Speed = 200f;
        /// <summary>
        /// �л���һ���������
        /// </summary>
        public float AINextWayPointDistance = 1f;
        /// <summary>
        /// AI����Ѱ·���
        /// </summary>
        public float AIUpdatePathInterval = 0.5f;
        /// <summary>
        /// ֹͣ����
        /// </summary>
        public float AIEndDiatance = 0f;

        /// <summary>
        /// ����·��
        /// </summary>
        private Path _AIPath;
        /// <summary>
        /// Ѱ·��
        /// </summary>
        private Seeker _AISeeker;
        /// <summary>
        /// ����
        /// </summary>
        private Rigidbody2D _rigid;
        /// <summary>
        /// ��Ⱦ��
        /// </summary>
        private SpriteRenderer _renderer;
        /// <summary>
        /// ��ǰ������
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

        #region AIѰ·
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
            // ����AI
            CancelInvoke();
            this._AICurrentWayPoint = 0;

            if (enable)
            {
                // ����AI
                this.EnableAI = true;
                InvokeRepeating(nameof(AIUpdatePath), 0f, this.AIUpdatePathInterval);
            }
            else
            {
                // �ر�AI
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
        /// ·���������
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
                // �޵���·�������
                if (this._AIPath == null)
                {
                    return;
                }

                // �ж��Ƿ���ɵ���
                if (_AICurrentWayPoint >= this._AIPath.vectorPath.Count)
                {
                    this._AICurrentWayPoint = 0;
                    return;
                }

                // ��С�����
                var targetDist = Vector2.Distance(this.transform.position, this.Target.GetComponent<Transform>().position);
                // �������ֹͣ
                if (targetDist <= this.AIEndDiatance)
                {
                    return;
                }

                // ����
                Vector2 dir = ((Vector2)this._AIPath.vectorPath[this._AICurrentWayPoint] - (Vector2)this.transform.position).normalized;
                Vector2 force = this.Speed * Time.deltaTime * dir;
                this._rigid.AddForce(force);



                // ���뵱ǰ���������
                var wayPointDist = Vector2.Distance(this.transform.position, this._AIPath.vectorPath[this._AICurrentWayPoint]);
                // ��һ������
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