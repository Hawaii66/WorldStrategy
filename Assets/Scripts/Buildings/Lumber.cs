using UnityEngine;

namespace WorldStrategy.Buildings
{
    public class Lumber : Building
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
            city.productionPerTurn += 1;
        }
    }
}
