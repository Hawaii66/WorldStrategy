using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldStrategy.Borders
{
    public enum BorderType { Full, Straight, Corner, Crossing, Empty, Cap }

    [CreateAssetMenu(fileName = "Border", menuName = "Custom/Border")]
    public class Border : ScriptableObject
    {
        public int index;
        public BorderType type;
        public int rotation;
        public Mesh mesh;

        [Button("Update")]
        private void Update()
        {
            ResourceLoader.Instance.LoadResources();
            TerrainManager.Instance.UpdateBorders();
        }
    }
}
