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
            Fly
        }

        /// <summary>
        /// “Ù∆µ◊Èº˛
        /// </summary>
        public AudioSource Audio;

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

        public void SetPitchOffset(float offset = 0)
        {
            this.PitchOffset = Mathf.Clamp(1 + offset, 0.1f, 10f);
            this.Audio.pitch = this.PitchOffset;
        }

        public void PlayAudio(AudioTypes type = AudioTypes.Trot)
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
            this.Audio.Play();
        }
    }
}