using NaughtyAttributes;
using UnityEngine;
using WorldStrategy.UI;

namespace WorldStrategy
{
    public class ClickManager : StaticInstance<ClickManager>
    {
        private enum SelectedState {NULL, CELL, UNIT, BUILDING };
        private SelectedState state;
        private Cell selectedCell;
        private Unit selectedUnit;
        private WalkableTiles walkableTiles;

        [SerializeField] Camera cam;
        [SerializeField] LayerMask groundMask;
        [SerializeField] MeshFilter selectedMesh;
        [SerializeField] Transform unitWalkableParent;
        [SerializeField, ShowAssetPreview] Mesh cellMesh;
        [SerializeField, ShowAssetPreview] Mesh unitMesh;
        [SerializeField, ShowAssetPreview] GameObject unitSelectPrefab;

        private void Update()
        {
            if (Utils.IsOverUI()) { return; }

            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
                {
                    Coord position = new Coord(hit.point);

                    if (!Utils.InsideBound(position)) { return; }

                    Cell cell = TerrainManager.Instance.GetCell(position);
                    Unit unit = cell.unit;
                    Unit oldUnit = selectedUnit;

                    if(selectedCell != null && cell != selectedCell) { state = SelectedState.NULL; }

                    UnselectAll();

                    if(state == SelectedState.NULL)
                    {
                        state = SelectedState.CELL;
                        selectedCell = cell;
                        if(selectedCell.building != null && selectedCell.building.type == BuildingType.City)
                        {
                            CityMenu.Instance.ShowMenu((City)selectedCell.building);
                        }
                    }
                    else if(state == SelectedState.CELL && unit != null)
                    {
                        state = SelectedState.UNIT;
                        selectedUnit = unit;
                        UpdateUnitMovement(UnitManager.Instance.GetUnitMovement(unit));
                    }
                    else if(state == SelectedState.UNIT && oldUnit.position != position && oldUnit != null && ContainsWalkable(position))
                    {
                        selectedUnit = oldUnit;
                        UnitManager.Instance.MoveUnit(oldUnit, position, ()=>
                        {
                            state = SelectedState.NULL;
                            selectedCell = null;
                            selectedUnit = null;
                            unitWalkableParent.DestroyChildren();
                            UpdateSelectedTool();
                        });
                    }
                    else if(state == SelectedState.UNIT && oldUnit.position != position && oldUnit != null && ContainsAttach(position, out int index))
                    {
                        Cell targetCell = TerrainManager.Instance.GetCell(walkableTiles.attackable[index]);
                        if (targetCell.building != null && targetCell.building.type == BuildingType.City)
                        {
                            UnitManager.Instance.MoveUnit(oldUnit, walkableTiles.attackTiles[index], () =>
                            {
                                state = SelectedState.NULL;
                                selectedCell = null;
                                selectedUnit = null;
                                unitWalkableParent.DestroyChildren();

                                City city = (City)targetCell.building;
                                if(city.TakeDamage(ResourceLoader.GetUnit(oldUnit.type).attachStrength, oldUnit.owner))
                                {
                                    UnitManager.Instance.MoveUnit(oldUnit, walkableTiles.attackable[index], () => { });
                                }

                                UpdateSelectedTool();
                            });
                        }
                        else
                        {
                            selectedUnit = oldUnit;
                            UnitManager.Instance.MoveUnit(oldUnit, walkableTiles.attackTiles[index], () =>
                            {
                                state = SelectedState.NULL;
                                selectedCell = null;
                                selectedUnit = null;
                                unitWalkableParent.DestroyChildren();

                                bool died = UnitManager.Instance.GetUnit(position).TakeDamage(ResourceLoader.GetUnit(oldUnit.type).attachStrength);
                                if (died)
                                {
                                    UnitManager.Instance.MoveUnit(oldUnit, walkableTiles.attackable[index], () => { });
                                }

                                UpdateSelectedTool();
                            });
                        }
                    }
                    else
                    {
                        state = SelectedState.NULL;
                    }

                    UpdateSelectedTool();
                }
            }
       
            if(Input.GetKeyDown(KeyCode.H))
            {
                if(state == SelectedState.UNIT && selectedUnit != null && selectedUnit.MyTurn())
                {
                    switch(selectedUnit.type)
                    {
                        case UnitType.Settler:
                            {
                                Settler settler = (Settler)selectedUnit;
                                if (!settler.Settle())
                                {
                                    settler.canMove = false;
                                }
                                UnselectAll();
                                state = SelectedState.NULL;
                                UpdateSelectedTool();
                                break;
                            }
                    }
                }

                if(state == SelectedState.CELL && selectedCell != null && selectedCell.owner == TurnManager.Instance.GetCurrentPlayer().id)
                {
                    if(selectedCell.building != null)
                    {
                        BuildingType type = selectedCell.building.type;
                        switch(type)
                        {
                            case BuildingType.City:
                                {
                                    City city = (City)selectedCell.building;
                                    UnitSettings[] units = ResourceLoader.GetAllUnits();

                                    BottomSelect.Instance.NewSelect(ResourceLoader.GetAllUnitIcons(), (int index) =>
                                    {
                                        Debug.Log(units[index].name);
                                    });
                                    
                                    break;
                                }
                        }
                    }
                }
            }
        }

        public void UnselectAll()
        {
            selectedCell = null;
            selectedUnit = null;
            unitWalkableParent.DestroyChildren();
            CityMenu.Instance.Hide();
        }

        public void SuperUnselectAll()
        {
            state = SelectedState.NULL;
            UpdateSelectedTool();
            UnselectAll();
        }

        private bool ContainsAttach(Coord pos, out int index)
        {
            for (int i = 0; i < walkableTiles.attackable.Length; i++)
            {
                index = i;
                if (walkableTiles.attackable[i] == pos) { return true; }
            }
            index = -1;
            return false;
        }

        private bool ContainsWalkable(Coord pos)
        {
            for(int i = 0; i < walkableTiles.walkable.Length; i ++)
            {
                if(walkableTiles.walkable[i] == pos) { return true; }
            }

            return false;
        }

        private void UpdateUnitMovement(WalkableTiles tiles)
        {
            walkableTiles = tiles;

            unitWalkableParent.DestroyChildren();

            for(int i = 0; i < tiles.walkable.Length; i ++)
            {
                GameObject temp = Instantiate(unitSelectPrefab);
                temp.transform.SetParent(unitWalkableParent);
                temp.transform.position = tiles.walkable[i];
                temp.transform.position += new Vector3(0, 0.38f, 0);

                temp.GetComponent<MeshRenderer>().material.color = Color.blue;
            }

            for (int i = 0; i < tiles.attackable.Length; i++)
            {
                GameObject temp = Instantiate(unitSelectPrefab);
                temp.transform.SetParent(unitWalkableParent);
                temp.transform.position = tiles.attackable[i];
                temp.transform.position += new Vector3(0, 0.38f, 0);

                temp.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }

        public void UpdateSelectedTool()
        {
            switch(state)
            {
                case SelectedState.NULL:
                    {
                        selectedMesh.mesh = null;
                        break;
                    }
                case SelectedState.CELL:
                    {
                        selectedMesh.mesh = cellMesh;
                        selectedMesh.transform.position = new CoordF(selectedCell.WorldPosition) * new CoordF(1,0,1) + new CoordF(0,0.38f,0);
                        break;
                    }
                case SelectedState.UNIT:
                    {
                        selectedMesh.mesh = unitMesh;
                        selectedMesh.transform.position = new CoordF(selectedUnit.WorldPosition) * new CoordF(1, 0, 1) + new CoordF(0, 0.38f, 0);
                        break;
                    }
            }
        }
    }
}
