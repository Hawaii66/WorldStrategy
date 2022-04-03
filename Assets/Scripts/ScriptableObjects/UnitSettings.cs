using UnityEngine;

namespace WorldStrategy
{
    [CreateAssetMenu(fileName="Unit",menuName="Custom/Unit")]
    public class UnitSettings : ScriptableObject
    {
        public new string name;
        public UnitType type;
        public Mesh defaultMesh;
        public int walkdistance;
        public float startHealth;
        public float attachStrength;
        public Sprite icon;
        public int productionCost;
        public bool dash;
    }
}
