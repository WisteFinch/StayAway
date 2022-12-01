using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameScript
{
    public class MagicBoom : MonoBehaviour
    {
        public AudioClip Audio;
        public float DisappearTime = 2;
        void Start()
        {
            this.GetComponentInChildren<AudioSource>().loop = false;
            this.GetComponentInChildren<AudioSource>().clip = Audio;
            this.GetComponentInChildren<AudioSource>().Play();
            Invoke(nameof(DestoryThis), this.DisappearTime);
        }

        void DestoryThis()
        {
            Destroy(this.gameObject);
        }
    }
}