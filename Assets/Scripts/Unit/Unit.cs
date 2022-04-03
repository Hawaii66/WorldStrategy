using NaughtyAttributes;
using UnityEngine;
using WorldStrategy.Clouds;

namespace WorldStrategy
{
    public enum UnitType { None, Default, Settler, Worker, Sword, Scout}

    public class Unit : MonoBehaviour
    {
        public Coord position;
        public Vector3 WorldPosition { get { return transform.position; } }
        public UnitType type;
        public int owner;
        public bool canMove;
        public float health;

        [HideInInspector] public Transform actionIcon;

        public virtual void Init(Coord pos, UnitType t, int o)
        {
            position = pos;
            type = t;
            owner = o;

            health = ResourceLoader.GetUnit(type).startHealth;

            if (owner != -1)
            {
                CloudMerge.Instance.RemoveMultipleClouds(owner, position, 2);
            }
        }

        private void Update()
        {
            if(ShowIcon())
            {
                actionIcon.gameObject.SetActive(true);
            }
            else
            {
                actionIcon.gameObject.SetActive(false);
            }
        }

        [Button("Action button")]
        public virtual void ActionButtonClicked()
        {
            Debug.Log("Action button clicked");
        }

        public bool MyTurn()
        {
            return TurnManager.Instance.GetCurrentPlayer().id == owner && canMove;
        }

        public virtual void AfterTurn()
        {

        }

        public void UpdateMesh()
        {
            Cell c = TerrainManager.Instance.GetCell(position);
            int currentPlayer = TurnManager.Instance.GetCurrentPlayer().id;

            if (currentPlayer != c.owner)
            {
                if (!CloudMerge.Instance.Visible(position, currentPlayer))
                {
                    GetComponentInChildren<MeshFilter>().mesh = null;
                    return;
                }
            }

            GetComponentInChildren<MeshFilter>().mesh = type.GetUnit().defaultMesh;
        }

        public bool TakeDamage(float damage)
        {
            health -= damage;
            if(health <= 0)
            {
                Kill();
                return true;
            }

            return false;
        }

        public void Kill()
        {
            TurnManager.Instance.GetPlayerIndex(owner).units.Remove(position);
            TerrainManager.Instance.GetCell(position).unit = null;

            Destroy(gameObject);
        }

        public virtual bool ShowIcon()
        {
            return false;
        }

    }
}
