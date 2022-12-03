using System;
using Unity.VisualScripting;
using UnityEngine;

namespace StayAwayGameScript
{
    [RequireComponent(typeof(LineRenderer))]
    public class CircleRender : MonoBehaviour
    {
        /// <summary>
        /// ������
        /// </summary>
        public Boolean Enable;
        /// <summary>
        /// �뾶
        /// </summary>
        public float R = 1;
        /// <summary>
        /// �Ǹ���
        /// </summary>
        public int AngleCount = 90;
        /// <summary>
        /// ת��
        /// </summary>
        private float _angle;
        /// <summary>
        /// ��Ԫ��
        /// </summary>
        private Quaternion _quaternion;
        /// <summary>
        /// LineRenderer���
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
                    //Ĭ��Χ��z�ửԲ������zֵ���ӣ�����ֵΪÿ�����㵽Բ�ĵļн�
                    this._quaternion = Quaternion.Euler(this._quaternion.eulerAngles.x, this._quaternion.eulerAngles.y, this._quaternion.eulerAngles.z + this._angle);
                }
                // ��תԲ
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