using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StayAwayGameScript
{
    public class ConfigItemSync : MonoBehaviour
    {
        public StayAwayGame.WidgetType Type;
        public StayAwayGame.Config Config;
        void Start()
        {
            Fetch();
        }

        /// <summary>
        /// ȡ����ֵ
        /// </summary>
        public void Fetch()
        {
            float v = (float)GameManager.Instance.GetConfigData(this.Config);
            switch(Type)
            {
                case StayAwayGame.WidgetType.Slider: this.gameObject.GetComponent<Slider>().SetValueWithoutNotify(v); break;
            }
        }

        /// <summary>
        /// ������ֵ
        /// </summary>
        public void Push()
        {
            switch (Type)
            {
                case StayAwayGame.WidgetType.Slider: GameManager.Instance.SetConfigData(this.Config, this.gameObject.GetComponent<Slider>().value); break;
            }
            GameManager.Instance.StoreConfigData();
        }
    }
}
