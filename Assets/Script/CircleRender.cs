using System;
using Unity.VisualScripting;
using UnityEngine;

namespace StayAwayGameScript
{
    [RequireComponent(typeof(LineRenderer))]
    public class CircleRender : MonoBehaviour
    {
        /// <summary>
        /// 可用性
        /// </summary>
        public Boolean Enable;
        /// <summary>
        /// 半径
        /// </summary>
        public float R = 1;
        /// <summary>
        /// 角个数
        /// </summary>
        public int AngleCount = 90;
        /// <summary>
        /// 转角
        /// </summary>
        private float _angle;
        /// <summary>
        /// 四元数
        /// </summary>
        private Quaternion _quaternion;
        /// <summary>
        /// LineRenderer组件
        /// </summary>
        private LineRenderer _line;

        void Start()
        {
            this._angle = 360f / (AngleCount - 1);
            this._line = this.GetComponent<LineRenderer>();
            this._line.positionCount = this.AngleCount;
        }
        void LateUpdate()
        {
            if (this.Enable)
            {
                DrawCircle();
            }
        }

        void OnValidate()
        {
            if (this.Enable)
            {
                DrawCircle();
            }
        }

        public void DrawCircle()
        {
            for (int i = 0; i < this.AngleCount; i++)
            {
                if (i != 0)
                {
                    //默认围着z轴画圆，所以z值叠加，叠加值为每两个点到圆心的夹角
                    this._quaternion = Quaternion.Euler(this._quaternion.eulerAngles.x, this._quaternion.eulerAngles.y, this._quaternion.eulerAngles.z + this._angle);
                }
                // 旋转圆
                Vector3 forwardPosition = _quaternion * Vector3.down * R;
                _line.SetPosition(i, forwardPosition);
            }
        }

        public void SetColor(Color c)
        {
            this._line.startColor = c;
            this._line.endColor = c;
        }
    }
}