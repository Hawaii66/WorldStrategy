using UnityEngine;

namespace WorldStrategy.Buildings
{
    public class Farm : Building
    {
        private void Start()
        {
            Cell cell = TerrainManager.Instance.GetCell(position);

            int owner = cell.owner;
            if (!TurnManager.Instance.GetPlayerIndex(owner).cells.ContainsKey(position))
            {
                TurnManager.Instance.GetPlayerIndex(owner).cells.Add(position, cell);
            }

            Cell ownerCell = TerrainManager.Instance.GetCell(position).GetOwner();
            if (ownerCell == null || ownerCell.building == null) { return; }

            City city = (City)ownerCell.building;
            city.maxHealth += 1;
        }

        public override void AfterTurn()
        {
            Cell owner = TerrainManager.Instance.GetCell(position).GetOwner();
            if(owner == null || owner.building == null) { return; }

            City city = (City)owner.building;
            city.health = Mathf.Min(city.maxHealth, city.health + 1);
        }
    }
}
