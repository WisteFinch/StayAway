using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StayAwayGameScript
{
    public class UISliderSync : MonoBehaviour
    {
        public GameObject OBJText;

        public void Start()
        {
            Sync();
        }

        private void OnValidate()
        {
            Sync();
        }

        /// <summary>
        /// ͬ������
        /// </summary>
        /// <param name="cent">ʹ�ðٷ���</param>
        public void Sync(bool cent = true)
        {
            if(cent)
            {
                OBJText.GetComponent<Text>().text = $"{(int)(this.gameObject.GetComponent<Slider>().value * 100)}%";
            }
            else
            {
                OBJText.GetComponent<Text>().text = this.gameObject.GetComponent<Slider>().value.ToString("#0.00");
            }
        }
    }
}