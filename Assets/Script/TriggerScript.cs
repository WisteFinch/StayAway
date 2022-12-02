using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace StayAwayGameScript
{
    public class TriggerScript : MonoBehaviour
    {
        public UnityEvent TriggerEvent;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Pony"))
            {
                this.TriggerEvent.Invoke();
            }
        }
    }
}