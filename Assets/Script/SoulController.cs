using System;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;

namespace StayAwayGameScript
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SoulController : MonoBehaviour
    {
        public struct FrameInput
        {
            /// <summary>
            /// X轴
            /// </summary>
            public float XAxis;
            /// <summary>
            /// Y轴
            /// </summary>
            public float YAxis;
        }

        #region 公有变量
        [Header("信息")]
        /// <summary>
        /// 启用控制
        /// </summary>
        public Boolean EnableControl;
        /// <summary>
        /// 启用冻结
        /// </summary>
        public Boolean EnableForzen;
        /// <summary>
        /// 启用AI
        /// </summary>
        public Boolean EnableAI;
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
        /// 碰撞面
        /// </summary>
        public bool ColUp, ColDown, ColLeft, ColRight;
        /// <summary>
        /// 小马对象
        /// </summary>
        public UnityEngine.Object Pony;
        /// <summary>
        /// 死亡动画时间
        /// </summary>
        public float DeadTime = 1f;

        [Header("AI寻路")]
        /// <summary>
        /// AI速度率
        /// </summary>
        public Vector2 AIVelocityRatio = new(1, 1);
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
        public float AIEndDiatance = 3f;

        [Header("速度控制")]
        /// <summary>
        /// 最大速度
        /// </summary>
        public Vector2 MaxVelocity = new(2f, 2f);
        /// <summary>
        /// 加速度
        /// </summary>
        public Vector2 Acceleration = new(5f, 5f);
        /// <summary>
        /// 减速度
        /// </summary>
        public Vector2 Deacceleration = new(2f, 2f);

        [Header("移动控制")]
        /// <summary>
        /// 最小移动距离
        /// </summary>
        public float MinMoveDistance = 0.001f;

        [Header("碰撞控制")]
        /// <summary>
        /// 碰撞物层
        /// </summary>
        public LayerMask LayerMask = 1 << 0;
        /// <summary>
        /// 碰撞体圆角
        /// </summary>
        public float CollisionRadius = 0.1f;
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
        /// 启用反弹
        /// </summary>
        public bool EnableBounce = false;


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
        /// 上一帧位置
        /// </summary>
        private Vector2 _lastPosition;
        /// <summary>
        /// 死亡动画剩余时间
        /// </summary>
        private float _deadTimeLeft;
        /// <summary>
        /// 是否已死亡
        /// </summary>
        private Boolean _isDying = false;
        /// <summary>
        /// 不透明度
        /// </summary>
        private float _pellucidity;

        #endregion

        #region AI寻路
        /// <summary>
        /// 导航路径
        /// </summary>
        private Path _AIPath;
        /// <summary>
        /// 寻路器
        /// </summary>
        private Seeker _AISeeker;
        /// <summary>
        /// 当前导航点
        /// </summary>
        private int _AICurrentWayPoint = 0;

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

            // 初始化AI
            this._AISeeker = this.GetComponent<Seeker>();

            this._pellucidity = this.Pony.GetComponent<GameLogic>().RenderSoulPellucidity;
        }

        void Update()
        {

            // 计算相对速度
            this.RelativeVelocity = (this._rigidbody.position - _lastPosition) / Time.deltaTime;
            this._lastPosition = this._rigidbody.position;

            if (_isDying)
            {
                this._deadTimeLeft -= Time.deltaTime / 2;
                if (this._deadTimeLeft < 0)
                {
                    this.EnableForzen = true;
                    return;
                }
                Color c = Color.white;
                c.a = Mathf.Lerp(0, this._pellucidity, this._deadTimeLeft / this.DeadTime);
                this.GetComponentInChildren<SpriteRenderer>().color = c;
            }
            else
            {
                // 获取输入
                GatherInput();
            }

            if (!this.EnableForzen)
            {
                // 计算行走
                CalcWalk();
                // 综合数据，计算位移
                CalcMove();
            }
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
            if (this.EnableControl)
            {
                this.Input = new FrameInput
                {
                    XAxis = GameManager.Instance.Input.MoveAxis.x,
                    YAxis = GameManager.Instance.Input.MoveAxis.y
                };
            }
            else if(this.EnableAI)
            {
                this.Input = new FrameInput
                {
                    XAxis = 0,
                    YAxis = 0
                };
                UpdateAI();
            }
            else
            {
                this.Input = new FrameInput
                {
                    XAxis = 0,
                    YAxis = 0
                };
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
        }

        private void CalcCollision(Vector2 dir)
        {
            // 计算碰撞面
            // 下碰撞
            if (Vector2.Dot(dir, Vector2.up) >= 0.5)
            {
                this.ColDown = true;
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

        #region 移动计算
        private void CalcWalk()
        {
            // X轴
            if (this.Input.XAxis != 0)
            {
                // 计算水平速度
                this.Velocity.x += this.Input.XAxis * this.Acceleration.x * Time.deltaTime;
            }
            else
            {
                // 松开按键后减速
                this.Velocity.x = Mathf.MoveTowards(this.Velocity.x, 0, this.Deacceleration.x * Time.deltaTime);
            }
            // 限制水平速度
            this.Velocity.x = Mathf.Clamp(this.Velocity.x, -this.MaxVelocity.x, this.MaxVelocity.x);
            // 左右撞墙后停止
            if (this.Velocity.x < 0 && this.ColLeft || this.Velocity.x > 0 && this.ColRight)
            {
                this.Velocity.x = this.EnableBounce ? Mathf.Abs(this.Velocity.x) >= this.BounceThreshold ? -this.Velocity.x * this.BounceCoefficient : 0 : 0;
            }
            
            // Y轴
            if (this.Input.YAxis != 0)
            {
                // 计算垂直速度
                this.Velocity.y += this.Input.YAxis * this.Acceleration.y * Time.deltaTime;
            }
            else
            {
                // 松开按键后减速
                this.Velocity.y = Mathf.MoveTowards(this.Velocity.y, 0, this.Deacceleration.y * Time.deltaTime);
            }
            // 限制水平速度
            this.Velocity.y = Mathf.Clamp(this.Velocity.y,- this.MaxVelocity.y, this.MaxVelocity.y);
            // 左右撞墙后停止
            if (this.Velocity.y < 0 && this.ColDown || this.Velocity.y > 0 && this.ColUp)
            {
                this.Velocity.y = this.EnableBounce ? Mathf.Abs(this.Velocity.y) >= this.BounceThreshold ? -this.Velocity.y * this.BounceCoefficient : 0 : 0;
            }
        }

        #endregion

        #region AI寻路

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
                this._AISeeker.StartPath(this.transform.position, this.Pony.GetComponent<Transform>().position + Vector3.up, AIOnPathComplete);
            }
        }

        /// <summary>
        /// 路径搜索完成
        /// </summary>
        public void AIOnPathComplete(Path p)
        {
            if(!p.error)
            {
                this._AIPath = p;
                _AICurrentWayPoint = 0;
            }
        }

        public void UpdateAI()
        {
            // 无导航路径则结束
            if (this._AIPath == null)
            {
                return;
            }

            // 判断是否完成导航
            if(_AICurrentWayPoint >= this._AIPath.vectorPath.Count)
            {
                this._AICurrentWayPoint = 0;
                return;
            }

            // 与小马距离
            var targetDist = Vector2.Distance(this.transform.position, this.Pony.GetComponent<Transform>().position);
            // 到达距离停止
            if(targetDist <= this.AIEndDiatance)
            {
                return;
            }

            // 计算方向
            Vector2 dir = ((Vector2)this._AIPath.vectorPath[this._AICurrentWayPoint] - (Vector2)this.transform.position).normalized;
            // 模拟输入
            this.Input = new FrameInput
            {
                XAxis = dir.x * this.AIVelocityRatio.x,
                YAxis = dir.y * this.AIVelocityRatio.y
            };

            // 距离当前导航点距离
            var wayPointDist = Vector2.Distance(this.transform.position, this._AIPath.vectorPath[this._AICurrentWayPoint]);
            // 下一导航点
            if(wayPointDist <= this.AINextWayPointDistance)
            {
                this._AICurrentWayPoint++;
            }
        }

        #endregion

        public void Dead()
        {
            this.EnableAI = false;
            this.SetAIEnable(false);
            this._deadTimeLeft = this.DeadTime;
            this._isDying = true;
        }
    }
}