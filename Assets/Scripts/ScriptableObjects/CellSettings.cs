using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldStrategy
{
    [CreateAssetMenu(fileName="Cell",menuName="Custom/Cell")]
    public class CellSettings : ScriptableObject
    {
        public new string name;
        public CellType type;
        [ShowAssetPreview] public Mesh defaultMesh;

        public bool walkable;
    }
}