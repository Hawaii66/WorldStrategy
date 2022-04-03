using UnityEngine;

namespace WorldStrategy.Buildings
{
    public class Road : Building
    {
        private void Start()
        {
            Cell cell = TerrainManager.Instance.GetCell(position);

            int owner = cell.owner;
            if (!TurnManager.Instance.GetPlayerIndex(owner).cells.ContainsKey(position))
            {
                TurnManager.Instance.GetPlayerIndex(owner).cells.Add(position, cell);
            }
        }
    }
}
