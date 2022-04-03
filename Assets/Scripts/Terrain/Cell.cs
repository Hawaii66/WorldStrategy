using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WorldStrategy.Buildings;
using WorldStrategy.Clouds;

namespace WorldStrategy
{
    public enum CellType { Grass, Mine, Mountain, Water, Forest, Sand, Farmland };
    
    public class Cell : MonoBehaviour
    {
        public Coord position;
        public Vector3 WorldPosition
        {
            get
            {
                return position * TerrainManager.CellSize;
            }
        }

        public int owner;
        public Unit unit;
        public Building building;

        public CellType type;
        public MeshFilter groundMesh;
        [SerializeField] private MeshFilter borderMesh;
        public Canvas actionIcon;
        [SerializeField] private int borderIndex;

        private void Start()
        {
            if(position == new Coord(8,0,8))
            {
                unit = UnitManager.Instance.SpawnUnit(position, UnitType.Sword, 0);
            }
            if (position == new Coord(7, 0, 8))
            {
                unit = UnitManager.Instance.SpawnUnit(position, UnitType.Settler, 0);
            }

            if (position == new Coord(4, 0, 8))
            {
                unit = UnitManager.Instance.SpawnUnit(position, UnitType.Default, 1);
            }

            UpdateMesh();
        }

        public Cell GetOwner()
        {
            if(building != null && building.type == BuildingType.City) { return this; }

            int size = 1;
            while (size < 5)
            {
                for (int x = -size; x <= size; x++)
                {
                    for (int z = -size; z <= size; z++)
                    {
                        Coord pos = new Coord(x, 0, z) + position;
                        if (Utils.InsideBound(pos))
                        {
                            Cell cell = TerrainManager.Instance.GetCell(pos);
                            if (cell.building != null && cell.building.type == BuildingType.City)
                            {
                                City city = (City)cell.building;
                                if (city.ownedCells.Contains(position))
                                {
                                    return cell;
                                }
                            }
                        }
                    }
                }
                size += 1;
            }

            return null;
        }

        public void UpdateMesh()
        {
            int currentPlayer = TurnManager.Instance.GetCurrentPlayer().id;

            if (currentPlayer != owner)
            {
                if (!CloudMerge.Instance.Visible(position, currentPlayer))
                {
                    groundMesh.mesh = null;
                    borderMesh.mesh = null;
                    return;
                }
            }

            UpdateBorders();
            groundMesh.mesh = type.GetCell().defaultMesh;
        }

        [Button("Update borders")]
        public void UpdateBorders()
        {
            BorderData border = BorderManager.GetBorder(position, out int index);
            borderMesh.mesh = border.mesh;
            borderMesh.transform.rotation = Quaternion.Euler(0, border.rotation, 0);
            if(owner == -1) { return; }
            borderMesh.GetComponent<MeshRenderer>().material.color = owner.GetPlayer().color;
            borderIndex = index;
        }

        public float GetWalkCost(Unit unit)
        {
            float mult = 1f;
            if(unit.owner != owner && owner != -1)
            {
                mult *= 1.5f;
            }else if(building != null)
            {
                if(building.type == BuildingType.City) { mult *= 0.8f; }
                if(building.type == BuildingType.Road) { mult *= 0.1f; }
            }

            return mult;
        }

        public void SetBuilding(BuildingType type)
        {
            GameObject temp = Instantiate(ResourceLoader.Instance.buildingPrefab);
            temp.transform.SetParent(transform);
            temp.transform.position = position;
            temp.transform.position += new Vector3(0, 0.33f, 0);

            switch (type)
            {
                case BuildingType.City:
                    {
                        City city = temp.AddComponent<City>();
                        city.position = position;
                        city.type = BuildingType.City;
                        city.health = ResourceLoader.GetBuilding(BuildingType.City).city.maxHealth;

                        city.UpdateMesh();
                        building = city;
                        break;
                    }
                case BuildingType.Road:
                    {
                        Road road = temp.AddComponent<Road>();
                        road.position = position;
                        road.type = BuildingType.Road;
                        road.UpdateMesh();
                        building = road;
                        break;
                    }
                case BuildingType.Farm:
                    {
                        Farm farm = temp.AddComponent<Farm>();
                        farm.position = position;
                        farm.type = BuildingType.Farm;
                        farm.UpdateMesh();
                        building = farm;
                        break;
                    }
                case BuildingType.Lumber:
                    {
                        Lumber lumber = temp.AddComponent<Lumber>();
                        lumber.position = position;
                        lumber.type = BuildingType.Lumber;
                        lumber.UpdateMesh();
                        building = lumber;
                        break;
                    }
            }
        }
    }
}