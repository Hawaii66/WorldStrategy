using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldStrategy.Animations
{
    public enum AnimationType {Bounce, FadeOut };

    public class AnimationHandler : MonoBehaviour
    {
        Action c;
        Vector3 o;
        public void Init(AnimationType type, Vector3 offset, Action callback)
        {
            AnimationSettings anim = ResourceLoader.GetAnimation(type);
            Animation clip = gameObject.AddComponent<Animation>();

            o = offset;

            clip.playAutomatically = true;
            clip.clip = anim.clip;
            clip.AddClip(anim.clip, anim.type.ToString());
            clip.Play();
            c = callback;
        }

        public void Init(AnimationType type, Vector3 offset)
        {
            AnimationSettings anim = ResourceLoader.GetAnimation(type);
            Animation clip = gameObject.AddComponent<Animation>();

            o = offset;

            clip.playAutomatically = true;
            clip.clip = anim.clip;
            clip.AddClip(anim.clip, anim.type.ToString());
            clip.Play();
        }

        public void AnimEnd()
        {
            gameObject.GetComponent<Animation>().Stop();
            Destroy(gameObject.GetComponent<Animation>());
            Destroy(this);
            transform.localPosition = o;
            transform.localScale = Vector3.one;
            if (c != null)
            {
                c();
            }
        }

        private void LateUpdate()
        {
            transform.localPosition += o;
        }
    }
}