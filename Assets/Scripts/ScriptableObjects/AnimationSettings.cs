using UnityEngine;
using WorldStrategy.Animations;

namespace WorldStrategy
{
    [CreateAssetMenu(fileName="Animation",menuName="Custom/Animation")]
    public class AnimationSettings : ScriptableObject
    {
        public AnimationType type;
        public AnimationClip clip;
    }
}
