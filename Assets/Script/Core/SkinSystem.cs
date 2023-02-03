using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StayAwayGameScript
{
    public class SkinSystem : MonoBehaviour
    {
        public List<RuntimeAnimatorController> Animators;
        public List<Sprite> Idles;

        public void AddSkin(RuntimeAnimatorController a, Sprite s)
        {
            this.Animators.Append(a);
            this.Idles.Append(s);
        }

        public RuntimeAnimatorController GetAnimator(int index)
        {
            if(index >= this.Animators.Count || index < 0)
            {
                return this.Animators.First();
            }
            return this.Animators[index];
        }

        public Sprite GetIdles(int index)
        {
            if (index >= this.Idles.Count || index < 0)
            {
                return this.Idles.First();
            }
            return this.Idles[index];
        }

        public int GetCount()
        {
            return this.Animators.Count;
        }
    }
}
