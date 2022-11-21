using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace StayAwayGameController
{

    [RequireComponent(typeof(BoxCollider2D))]
    public class PonyController : MonoBehaviour
    { 
        public struct FrameInput
        {
            /// <summary>
            /// X轴
            /// </summary>
            public float XAxis;
            /// <summary>
            /// 按下跳跃
            /// </summary>
            public bool JumpDown;
            /// <summary>
            /// 按住跳跃
            /// </summary>
            public bool JumpPressed;
            /// <summary>
            /// 松开跳跃
            /// </summary>
            public bool JumpUp;
            /// <summary>
            /// 按下加速
            /// </summary>
            public bool ShiftDown;
        }

        #region 公有变量
        [Header("信息")]
        /// <summary>
        /// 速度
        /// </summary>
        public Vector2 Velocity;
        /// <summary>
        /// 相对速度
        /// </summary>
        public Vector2 RelativeVelocity;
        /// <summary>
        /// 输入
        /// </summary>
        public FrameInput Input;
        /// <summary>
        /// 是否在跳跃
        /// </summary>
        public bool IsJumping;
        /// <summary>
        /// 是否在空中
        /// </summary>
        public bool IsInAir;
        /// <summary>
        /// 是否在滑翔
        /// </summary>
        public bool IsGliding;
        /// <summary>
        /// 碰撞面
        /// </summary>
        public bool ColUp, ColDown, ColLeft, ColRight;

        [Header("速度控制")]
        /// <summary>
        /// 最大速度
        /// </summary>
        public Vector2 MaxVelocity = new(20f, 120f);
        /// <summary>
        /// 重力加速度
        /// </summary>
        public float GravityAcceleration = 100;
        /// <summary>
        /// 顶点重力减益
        /// </summary>
        public float GravityApexDerogation = 30;
        /// <summary>
        /// 加速度
        /// </summary>
        public float AccelerationX = 90;
        /// <summary>
        /// 减速度
        /// </summary>
        public float DeaccelerationX = 60;
        /// <summary>
        /// 顶点移速加成
        /// </summary>
        public float JumpApexBonus = 3;
        /// <summary>
        /// 跳跃提前结束缩减系数
        /// </summary>
        public float JumpEndEarlyGravityCoefficient = 3;
        /// <summary>
        /// 冲刺速度倍率
        /// </summary>
        public float DashSpeedRatio = 3.0f;
        /// <summary>
        /// 滑翔速度倍率
        /// </summary>
        public float GlideSpeedRatio = 0.2f;
        /// <summary>
        /// 滑翔重力减速度
        /// </summary>
        public float GlideGravityDeaccelerationRatio = 0.2f;
        /// <summary>
        /// 冲刺减速度
        /// </summary>
        public float DashDeaccelerationRatio = 0.5f;


        [Header("移动控制")]
        /// <summary>
        /// 跳跃高度
        /// </summary>
        public float JumpVelocity = 30;
        /// <summary>
        /// 顶点阈值
        /// </summary>
        public float JumpApexThreshold = 10;
        /// <summary>
        /// 野狼阈值
        /// </summary>
        public float CoyoteTimeThreshold = 0.1f;
        /// <summary>
        /// 跳跃缓存阈值
        /// </summary>
        public float JumpBufferThreshold = 0.1f;
        /// <summary>
        /// 最小移动距离
        /// </summary>
        public float MinMoveDistance = 0.001f;
        /// <summary>
        /// 允许滑翔前摇
        /// </summary>
        public float EnableGlideLeadUpTime = 0.1f;
        /// <summary>
        /// 冲刺冷却
        /// </summary>
        public float DashCoolDownTime = 1f;
        

        [Header("碰撞控制")]
        /// <summary>
        /// 碰撞物层
        /// </summary>
        public LayerMask LayerMask = 1 << 0;
        /// <summary>
        /// 跳跃撞顶损失
        /// </summary>
        public float JumpUpCollidisionLoss = 50;
        /// <summary>
        /// 碰撞体圆角
        /// </summary>
        public float CollisionRadius = 0.3f;
        /// <summary>
        /// 反弹系数
        /// </summary>
        public float BounceCoefficient = 0.1f;
        /// <summary>
        /// 反弹阈值
        /// </summary>
        public float BounceThreshold = 1;

        [Header("功能")]
        /// <summary>
        /// 启用野狼时间
        /// </summary>
        public bool EnableCoyoteTime = true;
        /// <summary>
        /// 启用顶点移速加成
        /// </summary>
        public bool EnableApexBonus = false;
        /// <summary>
        /// 启用跳跃缓存
        /// </summary>
        public bool EnableJumpBuffer = true;
        /// <summary>
        /// 启用反弹
        /// </summary>
        public bool EnableBounce = false;
        /// <summary>
        /// 启用滑翔
        /// </summary>
        public bool EnableGlide = true;
        /// <summary>
        /// 启用冲刺
        /// </summary>
        public bool EnableDash = true;


        #endregion

        #region 私有变量
        /// <summary>
        /// 自身刚体
        /// </summary>
        private Rigidbody2D _rigidbody;
        /// <summary>
        /// 自身碰撞体
        /// </summary>
        private BoxCollider2D _collison;
        /// <summary>
        /// <summary>
        /// 碰撞筛选
        /// </summary>
        private ContactFilter2D _contactFilter;
        /// <summary>
        /// 碰撞列表
        /// </summary>
        private readonly List<RaycastHit2D> _hitList = new();
        /// <summary>
        /// 切线方向碰撞列表
        /// </summary>
        private readonly List<RaycastHit2D> _tangentHitList = new();
        /// <summary>
        /// 上次按下跳跃时间
        /// </summary>
        private float _lastJumpPressedTime;
        /// <summary>
        /// 上一帧位置
        /// </summary>
        private Vector2 _lastPosition; 
        /// <summary>
        /// 离开地面时间（服务于野狼时间）
        /// </summary>
        private float _leftGroundTimeForCoyote;
        /// <summary>
        /// 离开地面时间
        /// </summary>
        private float _leftGroundTime;
        /// <summary>
        /// 提前结束跳跃
        /// </summary>
        private bool _endedJumpEarly = true;
        /// </summary>
        /// 接近顶点系数，到达顶点时变成1
        /// </summary>
        private float _apexCoefficient;
        /// <summary>
        /// 当前下落加速度
        /// </summary>
        private float _currentGravityAcceleration;
        /// <summary>
        /// 上次按下冲刺时间
        /// </summary>
        private float _lastUseDashTime;

        /// <summary>
        /// 冲刺可用性
        /// </summary>
        private bool DashUsability => this.EnableDash && Time.time - this._lastUseDashTime >= this.DashCoolDownTime;
        /// <summary>
        /// 计算野狼可用性
        /// </summary>
        private bool CoyoteUsability => this.EnableCoyoteTime && !this.IsJumping && !this.ColDown && this._leftGroundTimeForCoyote + this.CoyoteTimeThreshold > Time.time;
        /// <summary>
        /// 计算跳跃缓存启用性
        /// </summary>
        private bool BufferedJumpUsability => this.EnableJumpBuffer && this.ColDown && this._lastJumpPressedTime + this.JumpBufferThreshold > Time.time;
        /// <summary>
        /// 计算普通跳跃可用性
        /// </summary>
        private bool NormalJumpUsability => this.Input.JumpDown && this.ColDown;
        #endregion

        #region Unity消息
        void Start()
        {
            // 初始化刚体
            this._rigidbody = GetComponent<Rigidbody2D>();
            if (this._rigidbody == null)
                this._rigidbody = gameObject.AddComponent<Rigidbody2D>();
            // 设置刚体为静态，禁用自带物理
            this._rigidbody.bodyType = RigidbodyType2D.Kinematic;
            this._rigidbody.simulated = true;
            this._rigidbody.useFullKinematicContacts = false;
            this._rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
            // 设置碰撞体圆角
            this._collison = GetComponent<BoxCollider2D>();
            this._collison.edgeRadius = this.CollisionRadius;
            this._collison.size = new Vector2(this._collison.size.x - this.CollisionRadius * 2, this._collison.size.x - this.CollisionRadius * 2);
            // 设置碰撞过滤
            this._contactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                useTriggers = false,
                layerMask = this.LayerMask
            };
        }

        void Update()
        {
            // 计算相对速度
            this.RelativeVelocity = (this._rigidbody.position - _lastPosition) / Time.deltaTime;
            this._lastPosition = this._rigidbody.position;
            
            // 获取输入
            GatherInput();
            // 计算行走
            CalcWalk();
            // 计算顶点增益
            CalcJumpApex();
            // 计算重力
            CalcGravity();
            // 计算跳跃
            CalcJump();
            // 综合数据，计算位移
            CalcMove();
        }

        private void OnValidate()
        {
            // 设置碰撞层
            this._contactFilter.layerMask = this.LayerMask;
        }
        #endregion

        #region 输入控制
        private void GatherInput()
        {
            // 获取键盘输入
            this.Input = new FrameInput
            {
                XAxis = UnityEngine.Input.GetAxisRaw("Horizontal"),
                JumpDown = UnityEngine.Input.GetButtonDown("Jump"),
                JumpPressed = UnityEngine.Input.GetButton("Jump"),
                JumpUp = UnityEngine.Input.GetButtonUp("Jump"),
                ShiftDown = UnityEngine.Input.GetKeyDown(KeyCode.LeftShift),
            };
            if (this.Input.JumpDown)
            {
                this._lastJumpPressedTime = Time.time;
            }
        }
        #endregion

        #region 碰撞和位置计算
        private void CalcMove()
        {
            // 无速度直接结束
            if (this.Velocity == Vector2.zero)
                return;

            // 清除碰撞面标记
            this.ColUp = this.ColDown = this.ColRight = this.ColLeft = false;
            this.IsInAir = true;
            // 初始化下一次移动
            Vector2 nextDeltaPosition = Vector2.zero;
            // 获取未处理的移动信息
            float rawDistance = this.Velocity.magnitude * Time.deltaTime;
            Vector2 rawDirection = this.Velocity.normalized;

            // 最小移动距离，防止抽搐
            if (rawDistance < this.MinMoveDistance)
            {
                rawDistance = this.MinMoveDistance;
            }

            // 获取下一次移动后的碰撞信息
            this._rigidbody.Cast(rawDirection, this._contactFilter, this._hitList, rawDistance);

            Vector2 finalDirection = rawDirection;
            float finalDistance = rawDistance;

            // 遍历碰撞信息
            foreach (var hit in this._hitList)
            {
                float distance = hit.distance;

                // 计算碰撞方向与碰撞法线夹角
                float angle = Vector2.Dot(hit.normal, rawDirection);

                CalcCollision(hit.normal);

                if (angle >= 0)
                {
                    // 移动方向与碰撞法线方向相同，可以移动
                    distance = rawDistance;
                }
                else
                {
                    // 移动方向与碰撞法线方向相反，计算碰撞和滑动

                    // 获取碰撞切线方向
                    Vector2 tangentDirection = new(hit.normal.y, -hit.normal.x);
                    float tangentAngle = Vector2.Dot(tangentDirection, rawDirection);
                    if (tangentAngle < 0)
                    {
                        tangentDirection = -tangentDirection;
                        tangentAngle = -tangentAngle;
                    }

                    // 计算滑动距离
                    float tangentDistance = tangentAngle * rawDistance;
                    // 存在滑动，检测滑动方向碰撞
                    if (tangentAngle != 0)
                    {
                        // 获取滑动碰撞信息
                        this._rigidbody.Cast(tangentDirection, this._contactFilter, this._tangentHitList, tangentDistance);
                        // 遍历滑动碰撞信息
                        foreach (var tangentHit in this._tangentHitList)
                        {
                            // 计算滑动碰撞方向与滑动碰撞法线夹角
                            if (Vector2.Dot(tangentHit.normal, tangentDirection) < 0 && tangentHit.distance < tangentDistance)
                            {
                                tangentDistance = tangentHit.distance;
                                CalcCollision(tangentHit.normal);
                            }
                        }
                        nextDeltaPosition += tangentDirection * tangentDistance;
                    }
                }
                // 设置最终移动距离为最小碰撞距离
                if (distance < finalDistance)
                {
                    finalDistance = distance;
                }
            }

            // 合并滑动距离和移动距离
            nextDeltaPosition += finalDirection * finalDistance;
            // 移动
            GetComponent<Rigidbody2D>().position += nextDeltaPosition;
            //设置离开地面时间
            if(!this.IsInAir)
            {
                this._leftGroundTimeForCoyote = Time.time;
                this._leftGroundTime = Time.time;
            }
        }

        private void CalcCollision(Vector2 dir)
        {
            // 计算碰撞面
            // 下碰撞
            if(Vector2.Dot(dir, Vector2.up) >= 0.5)
            {
                this.ColDown = true;
                this.IsJumping = false;
                this.IsInAir = false;
            }
            // 上碰撞
            else if (Vector2.Dot(dir, Vector2.down) >= 0.5)
            {
                this.ColUp = true;
            }
            // 左碰撞
            else if (Vector2.Dot(dir, Vector2.right) >= 0.5)
            {
                this.ColLeft = true;
            }
            // 右碰撞
            else if (Vector2.Dot(dir, Vector2.left) >= 0.5)
            {
                this.ColRight = true;
            }
        }

        #endregion

        #region 重力计算

        private void CalcGravity()
        {
            // 底部碰撞
            if (ColDown)
            {
                // 结束下落
                if (this.Velocity.y < 0)
                {
                    this.Velocity.y = 0;
                }
                // 滑翔结束
                this.IsGliding = false;
            }
            else
            {
                // 提前松开跳跃键则施加向下的速度
                var fallSpeed = this._endedJumpEarly && this.Velocity.y > 0 ? this._currentGravityAcceleration * this.JumpEndEarlyGravityCoefficient : this._currentGravityAcceleration;
                // 计算下落速度
                this.Velocity.y -= fallSpeed * Time.deltaTime;
                // 限制下落速度
                this.Velocity.y = Mathf.Clamp(this.Velocity.y, -this.MaxVelocity.y, this.MaxVelocity.y);
                // 计算滑翔
                this.IsGliding = this.EnableGlide && Input.JumpPressed && Time.time - this._leftGroundTime >= this.EnableGlideLeadUpTime;
                if (IsGliding && this.Velocity.y  < - this.MaxVelocity.y * this.GlideSpeedRatio)
                {
                    this.Velocity.y = Mathf.MoveTowards(this.Velocity.y, -this.MaxVelocity.y * this.GlideSpeedRatio, this.GlideGravityDeaccelerationRatio);
                }
            }
        }

        #endregion

        #region 移动计算

        private void CalcJumpApex()
        {
            // 计算顶点
            if (!this.ColDown)
            {
                this._apexCoefficient = this.EnableApexBonus ? Mathf.InverseLerp(this.JumpApexThreshold, 0, Mathf.Abs(RelativeVelocity.y)) : 0;
                this._currentGravityAcceleration = this.EnableApexBonus ? Mathf.Lerp(this.GravityAcceleration - this.GravityApexDerogation, this.GravityAcceleration, this._apexCoefficient) : this.GravityAcceleration;
            }
            else
            {
                this._apexCoefficient = 0;
            }
        }

        private void CalcJump()
        {
            // 计算野狼时间和跳跃缓存
            if (this.Input.JumpDown && this.CoyoteUsability || this.BufferedJumpUsability || this.NormalJumpUsability)
            {
                this.Velocity.y = this.JumpVelocity;
                this._endedJumpEarly = false;
                this._leftGroundTimeForCoyote = float.MinValue;
                this.IsJumping = true;
            }

            // 提前松开按键
            if (!this.ColDown && this.Input.JumpUp && !this._endedJumpEarly && this.RelativeVelocity.y > 0)
            {
                this._endedJumpEarly = true;
            }
            // 撞顶减速
            if (ColUp)
            {
                if (this.Velocity.y > 0)
                {
                    this.Velocity.y -= this.JumpUpCollidisionLoss * Time.deltaTime;
                }
            }
        }
        private void CalcWalk()
        {
            if(Input.ShiftDown && this.DashUsability)
            {
                this._lastUseDashTime = Time.time;
                // 计算冲刺速度
                if(this.Velocity.x > 0)
                    this.Velocity.x += this.MaxVelocity.x * this.DashSpeedRatio;
                else if(this.Velocity.x < 0)
                    this.Velocity.x -= this.MaxVelocity.x * this.DashSpeedRatio;
            }
            if (this.Input.XAxis != 0)
            {
                // 计算水平速度
                this.Velocity.x += this.Input.XAxis * this.AccelerationX * Time.deltaTime;
                
                // 顶点加速是否启用
                var apexBonus = Mathf.Sign(this.Input.XAxis) * this.JumpApexBonus * _apexCoefficient;
                this.Velocity.x += apexBonus * Time.deltaTime;
            }
            else
            {
                // 松开按键后减速
                this.Velocity.x = Mathf.MoveTowards(this.Velocity.x, 0, this.DeaccelerationX * Time.deltaTime);
            }

            // 限制水平速度
            if (this.Velocity.x > this.MaxVelocity.x)
                this.Velocity.x = Mathf.MoveTowards(this.Velocity.x, this.MaxVelocity.x, 0.5f);
            if (this.Velocity.x < -this.MaxVelocity.x)
                this.Velocity.x = Mathf.MoveTowards(this.Velocity.x, -this.MaxVelocity.x, 0.5f);

            // 左右撞墙后停止
            if (this.Velocity.x < 0 && this.ColLeft || this.Velocity.x > 0 && this.ColRight)
            {
                this.Velocity.x = this.EnableBounce ? Mathf.Abs(this.Velocity.x) >= this.BounceThreshold ? - this.Velocity.x * this.BounceCoefficient : 0 : 0;
            }
        }

        #endregion

    }
}
