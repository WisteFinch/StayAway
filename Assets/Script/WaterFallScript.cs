using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace StayAwayGameScript
{
    public class WaterFallScript : MonoBehaviour
    {
        public float DestroyTime = 10f;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.CompareTag("Soul"))
            {
                collision.GetComponent<SoulController>().Pony.GetComponent<GameLogic>().CharacterDead(false, 2);
            }
            else if(collision.gameObject.CompareTag("Pony"))
            {
                collision.GetComponent<GameLogic>().GetMagic(StayAwayGame.Magic.MagicWaterBall);
                var list = this.GetComponentsInChildren<ParticleSystem>();
                foreach(ParticleSystem p in list)
                {
                    p.Stop();
                    var e = p.emission;
                    e.enabled = false;
                }
                Destroy(this.GetComponent<BoxCollider2D>());
                Invoke(nameof(DestroySelf), this.DestroyTime);
            }
        }

        void DestroySelf()
        {
            Destroy(this.gameObject);
        }
    }
}