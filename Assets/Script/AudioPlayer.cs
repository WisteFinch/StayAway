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
        /// ��Ƶ���
        /// </summary>
        public AudioSource Audio;

        public float PitchOffset;

        [Header("��Ƶ")]
        /// <summary>
        /// ����
        /// </summary>
        public List<AudioClip> TrotAudios = new();
        /// <summary>
        /// ������
        /// </summary>
        public List<AudioClip> FlyAudios = new();

        public void SetPitchOffset(float offset = 0)
        {
            this.PitchOffset = Mathf.Clamp(1 + offset, 0.1f, 10f);
            this.Audio.pitch = this.PitchOffset;
        }

        public void PlayAudio(AudioTypes type = AudioTypes.Trot)
        {
            // ��������
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