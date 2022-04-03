using NaughtyAttributes;
using System.Collections;
using UnityEngine;

namespace WorldStrategy
{
    public class TerrainManager : StaticInstance<TerrainManager>
    {
        public static int CellSize = 1;
        public static int GridSize = 16;

        public Cell[,] cells;
        
        [ShowAssetPreview, SerializeField] private GameObject cellPrefab;
        [SerializeField] private Transform worldParent;

        private void Start()
        {
            SpawnGrid();
        }

        private void SpawnGrid()
        {
            cells = new Cell[GridSize, GridSize];

            float[,] height = SimplexNoise.Noise.Calc2D(GridSize, GridSize, 0.11923719237f, 0, 0);

            for(int x = 0; x < GridSize; x ++)
            {
                for (int z = 0; z < GridSize; z ++)
                {
                    GameObject temp = Instantiate(cellPrefab, new Coord(x * CellSize, 0, z * CellSize), Quaternion.identity);
                    temp.transform.SetParent(worldParent);
                    Cell cell = temp.GetComponent<Cell>();
                    cells[x, z] = cell;
                    cells[x, z].position = new Coord(x, 0, z);
                    cells[x, z].owner = -1;

                    //Calculate biome
                    float noise = height[x, z] / 255f; // Range 0 - 1
                    if(noise < 0.30f) { cells[x, z].type = CellType.Water; }
                    else if (noise < 0.45f) { cells[x, z].type = CellType.Sand; }
                    else if(noise < 0.9f) { cells[x, z].type = CellType.Grass; }
                    else if (noise < 2f) { cells[x, z].type = CellType.Mountain; }

                    if(cells[x, z].type == CellType.Grass)
                    {
                        float randValue = Random.Range(0f, 1f);
                        if (randValue < 0.1) { cells[x, z].type = CellType.Forest; }
                        else if (randValue < 0.15f) { cells[x, z].type = CellType.Mine; }
                        else if (randValue < 0.25f) { cells[x, z].type = CellType.Farmland; }
                    }
                }
            }
        }

        public void ToggleCellVisible()
        {
            for (int x = 0; x < GridSize; x++)
            {
                for (int z = 0; z < GridSize; z++)
                {
                    cells[x, z].UpdateMesh();
                }
            }
        }

        [Button("Update borders")]
        public void UpdateBorders()
        {
            for (int x = 0; x < GridSize; x++)
            {
                for (int z = 0; z < GridSize; z++)
                {
                    cells[x, z].UpdateBorders();
                }
            }
        }

        public Cell GetCell(Coord position)
        {
            return cells[position.x, position.z];
        }

        public bool Walkable(Coord pos, int owner)
        {
            bool isCity = cells[pos.x, pos.z].building != null && cells[pos.x, pos.z].building.type == BuildingType.City;
            bool myCity = isCity && cells[pos.x, pos.z].owner != owner;
            return cells[pos.x, pos.z].type.GetCell().walkable && cells[pos.x, pos.z].unit == null && !myCity;
        }
    }
}