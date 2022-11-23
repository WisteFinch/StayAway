using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace StayAwayGameScript
{
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// ׷�ٶ���
        /// </summary>
        public UnityEngine.Object Obj;
        /// <summary>
        /// ׷����
        /// </summary>
        public float TrackingRatio = 0.1f;
        /// <summary>
        /// ׷����ֵ
        /// </summary>
        public float TrackingThreshold = 0.1f;

        /// <summary>
        /// ׷�ٶ��������
        /// </summary>
        private Transform _targetTransform;
        /// <summary>
        /// Ŀ��ƫ����
        /// </summary>
        private Boolean _isChangingTarget;

        void LateUpdate()
        {
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
                    Vector2 pos = Vector2.MoveTowards(this.transform.position, this._targetTransform.position, this.TrackingRatio);
                    this.transform.position = new Vector3(pos.x, pos.y, -10);
                }
            }
        }

        public void SetTarget(UnityEngine.Object target)
        {
            this.Obj = target;
            this._targetTransform = this.Obj.GetComponent<Transform>();
            this._isChangingTarget = true;
        }
    }
}