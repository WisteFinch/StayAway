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
        /// ��ʼ׷��
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
                // ��ʼ��
                if(this._startForceEnable && this.EnableStartForce)
                {
                    this._rigid.AddForce(this.Speed * this.StartForceRatio * Time.deltaTime * this._startForceRotation);
                    this._startForceEnable = false;
                }

                // ����
                Vector2 dir = ((Vector2)this.Target.transform.position - (Vector2)this.transform.position).normalized;
                Vector2 force = this.Speed * Time.deltaTime * dir;
                this._rigid.AddForce(force);

                // ��ת
                float z = Vector2.SignedAngle(Vector2.right, this._rigid.velocity);
                this.transform.rotation = Quaternion.Euler(0, 0, z);

                // ��ʧ�ж�
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