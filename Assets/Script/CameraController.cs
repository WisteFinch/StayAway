using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace StayAwayGameScript
{
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// 追踪对象
        /// </summary>
        public UnityEngine.Object Obj;
        /// <summary>
        /// 追踪率
        /// </summary>
        public float TrackingRatio = 0.1f;
        /// <summary>
        /// 追踪阈值
        /// </summary>
        public float TrackingThreshold = 0.1f;
        /// <summary>
        /// 摄像机大小
        /// </summary>
        public float CameraSize = 5;
        /// <summary>
        /// 缩放变化率
        /// </summary>
        public float ResizeRatio = 2;


        /// <summary>
        /// 背景
        /// </summary>
        public GameObject BackGround;
        /// <summary>
        /// 背景偏移率
        /// </summary>
        public float BackGroundOffsetRatio;

        /// <summary>
        /// 追踪对象变形器
        /// </summary>
        private Transform _targetTransform;
        /// <summary>
        /// 目标偏移量
        /// </summary>
        private Boolean _isChangingTarget;
        /// <summary>
        /// 目标缩放
        /// </summary>
        private float _targetSize;
        /// <summary>
        /// 相机对象
        /// </summary>
        private Camera _camera;
        private void Start()
        {
            this._camera = this.GetComponent<Camera>();
            _isChangingTarget = true;
        }

        void LateUpdate()
        {
            if(this._camera.orthographicSize != this._targetSize)
            {
                this._camera.orthographicSize = Mathf.MoveTowards(this._camera.orthographicSize, this._targetSize, this.ResizeRatio * Time.deltaTime);
            }

            var distance = Vector2.Distance(this.transform.position, this._targetTransform.position);
            if (!_isChangingTarget)
            {
                this.transform.position = new Vector3(this._targetTransform.position.x, this._targetTransform.position.y, -10);
            }
            else
            {
                if (distance <= this.TrackingThreshold)
                {
                    this._isChangingTarget = false;
                }
                else
                {
                    Vector2 pos = Vector2.MoveTowards(this.transform.position, this._targetTransform.position, this.TrackingRatio * Time.deltaTime);
                    this.transform.position = new Vector3(pos.x, pos.y, -10);
                }
            }

            Vector2 offset = -this.transform.position * this.BackGroundOffsetRatio + this.transform.position;
            this.BackGround.transform.position = new Vector3(offset.x, offset.y, 0);
        }

        public void SetTarget(UnityEngine.Object target)
        {
            this.Obj = target;
            this._targetTransform = this.Obj.GetComponent<Transform>();
            this._isChangingTarget = true;
        }

        public void Resize(float offset = 0)
        {
            this._targetSize = this.CameraSize + offset;
        }

        public void SetSize(float size)
        {
            this.CameraSize = size;
            this.GetComponent<Camera>().orthographicSize = this.CameraSize;
            this._targetSize = this.CameraSize;
        }
    }
}