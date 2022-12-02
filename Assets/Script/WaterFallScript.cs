using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace StayAwayGameScript
{
    public class WaterFallScript : MonoBehaviour
    {
        public float DestroyTime = 4f;
        public Boolean EnableGather = false;

        private AudioSource _audio;
        private Boolean _isDisappearing = false;

        private void Start()
        {
            this._audio = this.GetComponentInChildren<AudioSource>();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Soul"))
            {
                collision.GetComponent<SoulController>().Pony.GetComponent<GameLogic>().CharacterDead(false, 2);
            }
            else if(collision.gameObject.CompareTag("Pony") && this.EnableGather)
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
                this._isDisappearing = true;
            }
        }

        private void Update()
        {
            if(this._isDisappearing)
            {
                this._audio.volume = Mathf.MoveTowards(this._audio.volume, 0, Time.deltaTime / this.DestroyTime);
            }
        }

        void DestroySelf()
        {
            Destroy(this.gameObject);
        }
    }
}