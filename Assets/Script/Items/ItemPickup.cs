using JetBrains.Annotations;
using StayAwayGameScript;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameScript
{
    public class ItemPickup : MonoBehaviour
    {
        public StayAwayGame.Item ItemType;
        public string TargetName = "Pony";

        void Start()
        {
            this.GetComponent<CircleCollider2D>().isTrigger = true;
        }

        void OnTriggerEnter2D(Collider2D other)
        {

            if (other.gameObject.CompareTag(this.TargetName))
            {
                other.GetComponent<GameLogic>().GetItem(this.ItemType, this.gameObject);
                Destroy(this.GetComponent<CircleCollider2D>());
                var list = this.gameObject.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem p in list)
                {
                    p.Stop();
                }    
            }
        }

        public void DestryThisLater(float time)
        {
            Invoke(nameof(DestroyThis), time);
        }

        public void DestroyThis()
        {
            Destroy(this.gameObject);
        }
    }
}