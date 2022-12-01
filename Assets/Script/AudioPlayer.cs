using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StayAwayGameScript
{
    public class AudioPlayer : MonoBehaviour
    {
        public enum AudioTypes
        {
            Trot,
            Fly,
            Dash
        }

        /// <summary>
        /// “Ù∆µ◊Èº˛
        /// </summary>
        public AudioSource Audio;

        /// <summary>
        /// ‘ –Ì≤•∑≈
        /// </summary>
        public Boolean EnableAudio = true;

        public float PitchOffset;

        [Header("“Ù∆µ")]
        /// <summary>
        /// Ã„…˘
        /// </summary>
        public List<AudioClip> TrotAudios = new();
        /// <summary>
        /// ∑…œË…˘
        /// </summary>
        public List<AudioClip> FlyAudios = new();
        /// <summary>
        /// ≥Â¥Ã…˘
        /// </summary>
        public List<AudioClip> DashAudios = new();

        public void SetPitchOffset(float offset = 0)
        {
            if (this.EnableAudio)
            {
                this.PitchOffset = Mathf.Clamp(1 + offset, 0.1f, 10f);
                this.Audio.pitch = this.PitchOffset;
            }
        }

        public void PlayAudio(AudioTypes type = AudioTypes.Trot)
        {
            if (this.EnableAudio)
            {
                // ≤•∑≈Ã„…˘
                if (type == AudioTypes.Trot)
                {
                    this.Audio.clip = this.TrotAudios[UnityEngine.Random.Range(0, this.TrotAudios.Count)];
                }
                else if (type == AudioTypes.Fly)
                {
                    this.Audio.clip = this.FlyAudios[UnityEngine.Random.Range(0, this.FlyAudios.Count)];
                }
                else if (type == AudioTypes.Dash)
                {
                    this.Audio.clip = this.DashAudios[UnityEngine.Random.Range(0, this.DashAudios.Count)];
                }
                this.Audio.Play();
            }
        }
    }
}