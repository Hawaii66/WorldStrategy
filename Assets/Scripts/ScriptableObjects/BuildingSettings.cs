using NaughtyAttributes;
using UnityEngine;

namespace WorldStrategy.Buildings
{
    [CreateAssetMenu(fileName="Building",menuName="Custom/Building")]
    public class BuildingSettings : ScriptableObject
    {
        public new string name;
        public BuildingType type;
        [ShowAssetPreview] public Mesh defaultMesh;
        public int BuildTime;
        public Sprite icon;
        public bool buildInTerritory;

        [ShowIf("type", BuildingType.City)] public CityStats city;
    }

    [System.Serializable]
    public struct CityStats
    {
        public float maxHealth;
    }
}
