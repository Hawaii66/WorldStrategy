using System.Collections;
using UnityEngine;
using WorldStrategy.Animations;
using WorldStrategy.Clouds;

namespace WorldStrategy
{
    public enum BuildingType {City, Farm, Lumber, Mine, Road, None };

    public abstract class Building : MonoBehaviour
    {
        public Coord position;
        public BuildingType type;
        [SerializeField] private MeshFilter defaultMesh;

        public void UpdateMesh()
        {
            Cell c = TerrainManager.Instance.GetCell(position);
            int currentPlayer = TurnManager.Instance.GetCurrentPlayer().id;

            if (currentPlayer != c.owner)
            {
                if(!CloudMerge.Instance.Visible(position, currentPlayer))
                {
                    defaultMesh.mesh = null;
                    return;
                }
            }

            defaultMesh = GetComponent<MeshFilter>();

            defaultMesh.mesh = ResourceLoader.GetBuilding(type).defaultMesh;
        }

        public virtual void AfterTurn()
        {

        }
    }
}