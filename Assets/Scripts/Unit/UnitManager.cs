using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WorldStrategy.Clouds;

namespace WorldStrategy
{
    public class UnitManager : StaticInstance<UnitManager>
    {
        [ShowAssetPreview, SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform unitParent;

        public Unit SpawnUnit(Coord position, UnitType type, int owner)
        {
            GameObject temp = Instantiate(unitPrefab);
            temp.transform.SetParent(unitParent);
            temp.transform.position = position;
            temp.transform.position += new Vector3(0, 0.33f, 0);

            switch(type)
            {
                case UnitType.Default:
                    Unit def = temp.AddComponent<Unit>();
                    def.Init(position, type, owner);
                    break;
                case UnitType.Sword:
                    Swordman man = temp.AddComponent<Swordman>();
                    man.Init(position, type, owner);
                    break;
                case UnitType.Settler:
                    Settler settler = temp.AddComponent<Settler>();
                    settler.Init(position, type, owner);
                    break;
                case UnitType.Worker:
                    Worker worker = temp.AddComponent<Worker>();
                    worker.Init(position, type, owner);
                    break;
                case UnitType.Scout:
                    Scout scout = temp.AddComponent<Scout>();
                    scout.Init(position, type, owner);
                    break;
            }

            Unit unit = temp.GetComponent<Unit>();
            unit.UpdateMesh();
            
            unit.actionIcon = temp.GetComponentInChildren<Canvas>().transform;
            temp.GetComponentInChildren<Button>().onClick.AddListener(unit.ActionButtonClicked);

            TurnManager.Instance.GetPlayerIndex(owner).units.Add(position, unit);
            TerrainManager.Instance.GetCell(position).unit = unit;

            return unit;
        }

        public Unit GetUnit(Coord position)
        {
            Player[] players = TurnManager.Instance.players;
            for(int i = 0; i < players.Length; i ++)
            {
                if(players[i].units.TryGetValue(position, out Unit y))
                {
                    return y;
                }
            }

            return null;
        }

        public void ToggleUnitVissible()
        {
            Player[] players = TurnManager.Instance.players;
            for (int i = 0; i < players.Length; i++)
            {
                foreach(Unit u in players[i].units.Values)
                {
                    u.UpdateMesh();
                }
            }
        }

        public void MoveUnit(Unit unit, Coord target, Action callback)
        {
            StartCoroutine(MoveUnitInternal(unit, target, callback));
        }

        private IEnumerator MoveUnitInternal(Unit u, Coord end, Action callback)
        {
            Coord startPos = u.position;
            Vector3 newPos = u.position;
            while (Vector3.Distance(newPos, end) > 0.01f)
            {
                newPos = Vector3.Lerp(newPos, end, 0.2f);

                ForceMoveUnit(u, newPos);
                yield return new WaitForSeconds(0);
            }

            ForceMoveUnit(u, end);
            u.position = end;
            u.canMove = false;

            TerrainManager.Instance.GetCell(startPos).unit = null;
            TerrainManager.Instance.GetCell(u.position).unit = u;

            TurnManager.Instance.GetPlayerIndex(u.owner).units.Remove(startPos);
            TurnManager.Instance.GetPlayerIndex(u.owner).units.Add(u.position, u);

            CloudMerge.Instance.RemoveMultipleClouds(u.owner, new Coord(end.x, 0, end.z), u.type == UnitType.Scout ? 2 : 1);

            callback();
        }

        private void ForceMoveUnit(Unit u, Vector3 position)
        {
            u.transform.position = position;
            u.transform.position += new Vector3(0, 0.33f, 0);
        }

        public WalkableTiles GetUnitMovement(Unit unit)
        {
            if (!unit.MyTurn()) { return new WalkableTiles(); }

            int maxDistance = ResourceLoader.GetUnit(unit.type).walkdistance;

            UniqueQueue<WalkableNode> queue = new UniqueQueue<WalkableNode>();
            List<Coord> searched = new List<Coord>();
            queue.Enqueue(new WalkableNode(unit.position, maxDistance));

            Coord[] cartesian = Utils.offsets;
            Coord[] diagonal = Utils.diagonal;

            int size = TerrainManager.GridSize;

            List<Coord> walkable = new List<Coord>();
            List<Coord> attackable = new List<Coord>();
            List<Coord> attackTiles = new List<Coord>();

            int max = 0;
            while(queue.Count > 0 && max < 200)
            {
                max += 1;

                WalkableNode currentNode = queue.Dequeue();
                searched.Add(currentNode.position);
                walkable.Add(currentNode.position);

                if(currentNode.distanceToNode >= 10)
                {
                    for (int i = 0; i < cartesian.Length; i++)
                    {
                        Coord offset = currentNode.position + cartesian[i];
                        if (Utils.InsideBound(offset, size, size))
                        {
                            if (searched.Contains(offset))
                            {
                                continue;
                            }
                            searched.Add(offset);

                            Unit unitOnTile = Instance.GetUnit(offset);
                            Building building = TerrainManager.Instance.GetCell(offset).building;

                            if(!CloudMerge.Instance.Visible(offset, unit.owner))
                            {
                                continue;
                            }

                            if (TerrainManager.Instance.Walkable(offset, unit.owner))
                            {
                                float walkMult = TerrainManager.Instance.GetCell(offset).GetWalkCost(unit);
                                int walkCost = Mathf.RoundToInt(10 * walkMult);
                                if (currentNode.distanceToNode >= walkCost)
                                {
                                    queue.Enqueue(new WalkableNode(offset, currentNode.distanceToNode - walkCost));
                                }
                            }
                            else if ((unitOnTile != null && unitOnTile.owner != unit.owner) || (building != null && building.type == BuildingType.City && TerrainManager.Instance.GetCell(offset).owner != unit.owner))
                            {
                                attackable.Add(offset);
                                attackTiles.Add(currentNode.position);
                            }
                        }
                    }
                }
                if (currentNode.distanceToNode >= 14)
                {
                    for (int i = 0; i < diagonal.Length; i++)
                    {
                        Coord offset = currentNode.position + diagonal[i];
                        if (Utils.InsideBound(offset, size, size))
                        {
                            if (searched.Contains(offset))
                            {
                                continue;
                            }
                            searched.Add(offset);

                            Unit unitOnTile = Instance.GetUnit(offset);
                            Building building = TerrainManager.Instance.GetCell(offset).building;

                            if (!CloudMerge.Instance.Visible(offset, unit.owner))
                            {
                                continue;
                            }

                            if (TerrainManager.Instance.Walkable(offset, unit.owner))
                            {
                                float walkMult = TerrainManager.Instance.GetCell(offset).GetWalkCost(unit);
                                int walkCost = Mathf.RoundToInt(14 * walkMult);
                                if (currentNode.distanceToNode >= walkCost)
                                {
                                    queue.Enqueue(new WalkableNode(offset, currentNode.distanceToNode - walkCost));
                                }
                            }
                            else if ((unitOnTile != null && unitOnTile.owner != unit.owner) || (building != null && building.type == BuildingType.City && TerrainManager.Instance.GetCell(offset).owner != unit.owner))
                            {
                                attackable.Add(offset);
                                attackTiles.Add(currentNode.position);
                            }
                        }
                    }
                }
            }

            if(ResourceLoader.GetUnit(unit.type).dash)
            {
                Coord[] offsets = Utils.allOffsets;
                foreach(Coord c in walkable)
                {
                    foreach(Coord offset in offsets)
                    {
                        Coord pos = c + offset;
                        if(pos == new Coord(4, 0, 8))
                        {

                        }
                        if(Utils.InsideBound(pos))
                        {
                            if (attackable.Contains(pos) || !CloudMerge.Instance.Visible(pos, unit.owner)) { continue; }

                            Cell cell = TerrainManager.Instance.GetCell(pos);
                            if (cell.building != null && cell.building.type == BuildingType.City)
                            {
                                if(cell.owner != unit.owner)
                                {
                                    attackable.Add(pos);
                                    attackTiles.Add(c);
                                }
                            }
                            else
                            {
                                Unit unitOnTile = Instance.GetUnit(pos);
                                if (unitOnTile != null && unitOnTile.owner != unit.owner)
                                {
                                    attackable.Add(pos);
                                    attackTiles.Add(c);
                                }
                            }
                        }
                    }
                }
            }

            return new WalkableTiles(walkable.ToArray(), attackable.ToArray(), attackTiles.ToArray());
        }
    
        private struct WalkableNode
        {
            public Coord position;
            public int distanceToNode;

            public WalkableNode(Coord pos, int dist)
            {
                position = pos;
                distanceToNode = dist;
            }
        }
    }
    public class WalkableTiles
    {
        public Coord[] walkable;
        public Coord[] attackable;
        public Coord[] attackTiles;

        public WalkableTiles(Coord[] w, Coord[] a, Coord[] at)
        {
            walkable = w;
            attackable = a;
            attackTiles = at;
        }

        public WalkableTiles()
        {
            walkable = new Coord[0];
            attackable = new Coord[0];
            attackTiles = new Coord[0];
        }
    }
}