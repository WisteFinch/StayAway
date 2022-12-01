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
        /// Ŀ��
        /// </summary>
        public GameObject Target;
        /// <summary>
        /// �ٶ�
        /// </summary>
        public float Speed = 300;
        /// <summary>
        /// �����ʼ������
        /// </summary>
        public Boolean EnableStartForce = true;
        /// <summary>
        /// ��ʼ��ϵ��
        /// </summary>
        public float StartForceRatio = 50;
        /// <summary>
        /// ���Ŀ���Իپ���
        /// </summary>
        public float MissOutDestructDistance = 2;
        /// <summary>
        /// ��Զ����
        /// </summary>
        public float MaxDistance = 10;
        /// <summary>
        /// ��ʧʱ��
        /// </summary>
        public float DisappearTime = 1;

        /// <summary>
        /// ��ʼ׷��
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
                // ��ʼ��
                if(this._startForceUsability && this.EnableStartForce)
                {
                    this._rigid.AddForce(this.Speed * this.StartForceRatio * Time.deltaTime * this._startForceRotation);
                    this._startForceUsability = false;
                }
                // �Ƿ���Ŀ��
                if (this._hasTarget) // ��Ŀ�꣺����Ŀ��
                {
                    // �ж�Ŀ���Ƿ�����
                    if(this.Target.gameObject == null)
                    {
                        this._hasTarget = false;
                    }
                    // ����
                    Vector2 dir = ((Vector2)this.Target.transform.position - (Vector2)this.transform.position).normalized;
                    this._targetPos = dir;
                    Vector2 force = this.Speed * Time.deltaTime * dir;
                    this._rigid.AddForce(force);

                    // ��ʧ�ж�
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
                else // ��Ŀ�꣺���г�ʼ����
                {
                    Vector2 force = this.Speed * Time.deltaTime * this._targetPos;
                    this._rigid.AddForce(force);
                }

                // ��ת
                float z = Vector2.SignedAngle(Vector2.right, this._rigid.velocity);
                this.transform.rotation = Quaternion.Euler(0, 0, z);

                // ��������ʧ
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