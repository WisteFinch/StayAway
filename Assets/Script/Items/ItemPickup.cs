using JetBrains.Annotations;
using StayAwayGameScript;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameScript
{
    [RequireComponent(typeof(CircleCollider2D))]
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
                other.GetComponent<GameLogic>().GetItem(this.ItemType);
                Destroy(this.gameObject);
            }
        }
    }
}