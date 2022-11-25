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
            var targetDist = Vector2.Distance(this.transform.position, this.Target.GetComponent<Transform>().position);
            if(!this.EnableAI && targetDist <= this.TrackingDistance)
            {
                SetAIEnable(true);
            }
            else if (this.EnableAI && targetDist > this.TrackingDistance)
            {
                SetAIEnable(false);
            }
            if(this._rigid.velocity.x > 0)
            {
                this._renderer.flipX = true;
            }
            else if (this._rigid.velocity.x < 0)
            {
                this._renderer.flipX = false;
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
                InvokeRepeating("AIUpdatePath", 0f, this.AIUpdatePathInterval);
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

                // ���㷽��
                Vector2 dir = ((Vector2)this._AIPath.vectorPath[this._AICurrentWayPoint] - (Vector2)this.transform.position).normalized;
                Vector2 force = dir * this.Speed * Time.deltaTime;

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
    }
}