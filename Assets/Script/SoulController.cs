using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameController
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SoulController : MonoBehaviour
    {
        public struct FrameInput
        {
            /// <summary>
            /// X��
            /// </summary>
            public float XAxis;
            /// <summary>
            /// Y��
            /// </summary>
            public float YAxis;
        }

        #region ���б���
        [Header("��Ϣ")]
        /// <summary>
        /// �ٶ�
        /// </summary>
        public Vector2 Velocity;
        /// <summary>
        /// ����ٶ�
        /// </summary>
        public Vector2 RelativeVelocity;
        /// <summary>
        /// ����
        /// </summary>
        public FrameInput Input;
        /// <summary>
        /// ��ײ��
        /// </summary>
        public bool ColUp, ColDown, ColLeft, ColRight;

        [Header("�ٶȿ���")]
        /// <summary>
        /// ����ٶ�
        /// </summary>
        public Vector2 MaxVelocity = new(20f, 20f);
        /// <summary>
        /// ���ٶ�
        /// </summary>
        public Vector2 Acceleration = new(100, 100);
        /// <summary>
        /// ���ٶ�
        /// </summary>
        public Vector2 Deacceleration = new(50, 50);

        [Header("�ƶ�����")]
        /// <summary>
        /// ��С�ƶ�����
        /// </summary>
        public float MinMoveDistance = 0.001f;

        [Header("��ײ����")]
        /// <summary>
        /// ��ײ���
        /// </summary>
        public LayerMask LayerMask = 1 << 0;
        /// <summary>
        /// ��ײ��Բ��
        /// </summary>
        public float CollisionRadius = 0.3f;
        /// <summary>
        /// ����ϵ��
        /// </summary>
        public float BounceCoefficient = 0.1f;
        /// <summary>
        /// ������ֵ
        /// </summary>
        public float BounceThreshold = 1;

        [Header("����")]
        /// <summary>
        /// ���÷���
        /// </summary>
        public bool EnableBounce = false;


        #endregion

        #region ˽�б���
        /// <summary>
        /// �������
        /// </summary>
        private Rigidbody2D _rigidbody;
        /// <summary>
        /// ������ײ��
        /// </summary>
        private BoxCollider2D _collison;
        /// <summary>
        /// <summary>
        /// ��ײɸѡ
        /// </summary>
        private ContactFilter2D _contactFilter;
        /// <summary>
        /// ��ײ�б�
        /// </summary>
        private readonly List<RaycastHit2D> _hitList = new();
        /// <summary>
        /// ���߷�����ײ�б�
        /// </summary>
        private readonly List<RaycastHit2D> _tangentHitList = new();
        /// <summary>
        /// ��һ֡λ��
        /// </summary>
        private Vector2 _lastPosition;

        #endregion

        #region Unity��Ϣ
        void Start()
        {
            // ��ʼ������
            this._rigidbody = GetComponent<Rigidbody2D>();
            if (this._rigidbody == null)
                this._rigidbody = gameObject.AddComponent<Rigidbody2D>();
            // ���ø���Ϊ��̬�������Դ�����
            this._rigidbody.bodyType = RigidbodyType2D.Kinematic;
            this._rigidbody.simulated = true;
            this._rigidbody.useFullKinematicContacts = false;
            this._rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
            // ������ײ��Բ��
            this._collison = GetComponent<BoxCollider2D>();
            this._collison.edgeRadius = this.CollisionRadius;
            this._collison.size = new Vector2(this._collison.size.x - this.CollisionRadius * 2, this._collison.size.x - this.CollisionRadius * 2);
            // ������ײ����
            this._contactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                useTriggers = false,
                layerMask = this.LayerMask
            };
        }

        void Update()
        {
            // ��������ٶ�
            this.RelativeVelocity = (this._rigidbody.position - _lastPosition) / Time.deltaTime;
            this._lastPosition = this._rigidbody.position;

            // ��ȡ����
            GatherInput();
            // ��������
            CalcWalk();
            // �ۺ����ݣ�����λ��
            CalcMove();
        }

        private void OnValidate()
        {
            // ������ײ��
            this._contactFilter.layerMask = this.LayerMask;
        }
        #endregion

        #region �������
        private void GatherInput()
        {
            // ��ȡ��������
            this.Input = new FrameInput
            {
                XAxis = UnityEngine.Input.GetAxisRaw("Horizontal"),
                YAxis = UnityEngine.Input.GetAxisRaw("Vertical")
            };
        }
        #endregion

        #region ��ײ��λ�ü���
        private void CalcMove()
        {
            // ���ٶ�ֱ�ӽ���
            if (this.Velocity == Vector2.zero)
                return;
            // �����ײ����
            this.ColUp = this.ColDown = this.ColRight = this.ColLeft = false;
            // ��ʼ����һ���ƶ�
            Vector2 nextDeltaPosition = Vector2.zero;
            // ��ȡδ������ƶ���Ϣ
            float rawDistance = this.Velocity.magnitude * Time.deltaTime;
            Vector2 rawDirection = this.Velocity.normalized;

            // ��С�ƶ����룬��ֹ�鴤
            if (rawDistance < this.MinMoveDistance)
            {
                rawDistance = this.MinMoveDistance;
            }

            // ��ȡ��һ���ƶ������ײ��Ϣ
            this._rigidbody.Cast(rawDirection, this._contactFilter, this._hitList, rawDistance);

            Vector2 finalDirection = rawDirection;
            float finalDistance = rawDistance;

            // ������ײ��Ϣ
            foreach (var hit in this._hitList)
            {
                float distance = hit.distance;

                // ������ײ��������ײ���߼н�
                float angle = Vector2.Dot(hit.normal, rawDirection);

                CalcCollision(hit.normal);

                if (angle >= 0)
                {
                    // �ƶ���������ײ���߷�����ͬ�������ƶ�
                    distance = rawDistance;
                }
                else
                {
                    // �ƶ���������ײ���߷����෴��������ײ�ͻ���

                    // ��ȡ��ײ���߷���
                    Vector2 tangentDirection = new(hit.normal.y, -hit.normal.x);
                    float tangentAngle = Vector2.Dot(tangentDirection, rawDirection);
                    if (tangentAngle < 0)
                    {
                        tangentDirection = -tangentDirection;
                        tangentAngle = -tangentAngle;
                    }

                    // ���㻬������
                    float tangentDistance = tangentAngle * rawDistance;
                    // ���ڻ�������⻬��������ײ
                    if (tangentAngle != 0)
                    {
                        // ��ȡ������ײ��Ϣ
                        this._rigidbody.Cast(tangentDirection, this._contactFilter, this._tangentHitList, tangentDistance);
                        // ����������ײ��Ϣ
                        foreach (var tangentHit in this._tangentHitList)
                        {
                            // ���㻬����ײ�����뻬����ײ���߼н�
                            if (Vector2.Dot(tangentHit.normal, tangentDirection) < 0 && tangentHit.distance < tangentDistance)
                            {
                                tangentDistance = tangentHit.distance;
                                CalcCollision(tangentHit.normal);
                            }
                        }
                        nextDeltaPosition += tangentDirection * tangentDistance;
                    }
                }
                // ���������ƶ�����Ϊ��С��ײ����
                if (distance < finalDistance)
                {
                    finalDistance = distance;
                }
            }

            // �ϲ�����������ƶ�����
            nextDeltaPosition += finalDirection * finalDistance;
            // �ƶ�
            GetComponent<Rigidbody2D>().position += nextDeltaPosition;
        }

        private void CalcCollision(Vector2 dir)
        {
            // ������ײ��
            // ����ײ
            if (Vector2.Dot(dir, Vector2.up) >= 0.5)
            {
                this.ColDown = true;
            }
            // ����ײ
            else if (Vector2.Dot(dir, Vector2.down) >= 0.5)
            {
                this.ColUp = true;
            }
            // ����ײ
            else if (Vector2.Dot(dir, Vector2.right) >= 0.5)
            {
                this.ColLeft = true;
            }
            // ����ײ
            else if (Vector2.Dot(dir, Vector2.left) >= 0.5)
            {
                this.ColRight = true;
            }
        }

        #endregion

        #region �ƶ�����
        private void CalcWalk()
        {
            // X��
            if (this.Input.XAxis != 0)
            {
                // ����ˮƽ�ٶ�
                this.Velocity.x += this.Input.XAxis * this.Acceleration.x * Time.deltaTime;
            }
            else
            {
                // �ɿ����������
                this.Velocity.x = Mathf.MoveTowards(this.Velocity.x, 0, this.Deacceleration.x * Time.deltaTime);
            }
            // ����ˮƽ�ٶ�
            this.Velocity.x = Mathf.Clamp(this.Velocity.x, -this.MaxVelocity.x, this.MaxVelocity.x);
            // ����ײǽ��ֹͣ
            if (this.Velocity.x < 0 && this.ColLeft || this.Velocity.x > 0 && this.ColRight)
            {
                this.Velocity.x = this.EnableBounce ? Mathf.Abs(this.Velocity.x) >= this.BounceThreshold ? -this.Velocity.x * this.BounceCoefficient : 0 : 0;
            }
            
            // Y��
            if (this.Input.YAxis != 0)
            {
                // ����ˮƽ�ٶ�
                this.Velocity.y += this.Input.YAxis * this.Acceleration.y * Time.deltaTime;
            }
            else
            {
                // �ɿ����������
                this.Velocity.y = Mathf.MoveTowards(this.Velocity.y, 0, this.Deacceleration.y * Time.deltaTime);
            }
            // ����ˮƽ�ٶ�
            this.Velocity.y = Mathf.Clamp(this.Velocity.y,- this.MaxVelocity.y, this.MaxVelocity.y);
            // ����ײǽ��ֹͣ
            if (this.Velocity.y < 0 && this.ColDown || this.Velocity.y > 0 && this.ColUp)
            {
                this.Velocity.y = this.EnableBounce ? Mathf.Abs(this.Velocity.y) >= this.BounceThreshold ? -this.Velocity.y * this.BounceCoefficient : 0 : 0;
            }
        }

        #endregion
    }

}