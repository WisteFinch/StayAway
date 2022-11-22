using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace StayAwayGameController
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
        public float TrackingRate = 0.5f;

        /// <summary>
        /// 追踪对象变形器
        /// </summary>
        private Transform _targetTransform;

        void Start()
        {
            this._targetTransform = Obj.GetComponent<Transform>();
        }
        void Update()
        {
            this.transform.position = new Vector3(Mathf.MoveTowards(this.transform.position.x, this._targetTransform.position.x, this.TrackingRate * Time.deltaTime), Mathf.MoveTowards(this.transform.position.y, this._targetTransform.position.y, this.TrackingRate * Time.deltaTime), -10);
        }

        public void SetTarget(UnityEngine.Object target)
        {
            this.Obj = target;
            this._targetTransform = Obj.GetComponent<Transform>();
        }
    }
}