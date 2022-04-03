using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WorldStrategy.Animations;
using WorldStrategy.Clouds;
using WorldStrategy.Names;

namespace WorldStrategy
{
    public class City : Building
    {
        public float health;
        public float maxHealth;
        public int production;
        public int productionPerTurn;
        public UnitType training = UnitType.None;
        public UniqueList<Coord> ownedCells;

        private List<Canvas> activebuttons;

        public void Start()
        {
            gameObject.AddComponent<AnimationHandler>().Init(AnimationType.Bounce, new Vector3(0,0.33f,0));
            productionPerTurn = 1;

            name = TownNames.GetTownName();
            health = ResourceLoader.GetBuilding(type).city.maxHealth;
            maxHealth = ResourceLoader.GetBuilding(type).city.maxHealth;
            int owner = TerrainManager.Instance.GetCell(position).owner;

            ownedCells = new UniqueList<Coord>() { position };

            for (int x = -1; x < 2; x++)
            {
                for (int z = -1; z < 2; z++)
                {
                    Coord pos = new Coord(x, 0, z) + position;
                    if (Utils.InsideBound(pos))
                    {
                        Cell cell = TerrainManager.Instance.GetCell(pos);
                        if (cell.owner != -1 && cell.owner != owner) { continue; }

                        cell.owner = owner;
                        if (!TurnManager.Instance.GetPlayerIndex(owner).cells.ContainsKey(pos))
                        {
                            TurnManager.Instance.GetPlayerIndex(owner).cells.Add(pos, cell);
                            ownedCells.Add(pos);
                        }
                    }
                }
            }

            TerrainManager.Instance.UpdateBorders();
        }

        public override void AfterTurn()
        {
            if(training == UnitType.None) { return; }

            production += productionPerTurn;

            int target = ResourceLoader.GetUnit(training).productionCost;
            if(target <= production)
            {
                if (TerrainManager.Instance.GetCell(position).unit == null)
                {
                    UnitManager.Instance.SpawnUnit(position, training, TerrainManager.Instance.GetCell(position).owner);
                    production -= target;
                    training = UnitType.None;
                }
            }
        }

        public bool TakeDamage(float dmg, int owner)
        {
            health -= dmg;
            if(health <= 0)
            {
                Cell cell = TerrainManager.Instance.GetCell(position);
                int oldOwner = cell.owner;

                foreach (Coord c in ownedCells)
                {
                    cell = TerrainManager.Instance.GetCell(c);
                    cell.owner = owner;
                    TurnManager.Instance.GetPlayerIndex(oldOwner).cells.Remove(c);
                    TurnManager.Instance.GetPlayerIndex(owner).cells.Add(c, cell);
                    TerrainManager.Instance.UpdateBorders();
                }
                return true;
            }

            return false;
        }
    
        public void BuyGround(Action<Coord> callback)
        {
            List<Coord> searched = new List<Coord>();
            List<BuyGroundDistance> toSearch = new List<BuyGroundDistance>();
            List<BuyGroundDistance> buyable = new List<BuyGroundDistance>();
            toSearch.Add(new BuyGroundDistance(position, 0));

            Coord[] straitghtOffsets = Utils.offsets;
            List<Coord> offsetsStraigth = new List<Coord>(straitghtOffsets);
            Coord[] offsets = Utils.allOffsets;

            int max = 200;
            while (toSearch.Count > 0 && max > 0)
            {
                max -= 1;

                searched.Add(toSearch[0].position);

                foreach (Coord c in offsets)
                {
                    Coord pos = c + toSearch[0].position;
                    if (Utils.InsideBound(pos) && !searched.Contains(pos))
                    {
                        Cell cell = TerrainManager.Instance.GetCell(pos);
                        if (ownedCells.Contains(pos))
                        {
                            toSearch.Add(new BuyGroundDistance(pos, toSearch[0].distance + 1));
                        }
                        else if (!ContatainsCoord(buyable, pos))
                        {
                            if (!offsetsStraigth.Contains(c)) { continue; }
                            if (cell.owner != -1) { continue; }
                            if(!CloudMerge.Instance.Visible(pos, TerrainManager.Instance.GetCell(position).owner)) { continue; }
                            buyable.Add(new BuyGroundDistance(pos, toSearch[0].distance)); ;
                        }
                    }
                }

                toSearch.RemoveAt(0);
            }

            activebuttons = new List<Canvas>();
            int index = 0;
            foreach(BuyGroundDistance c in buyable)
            {
                int i = index;
                Cell cell = TerrainManager.Instance.GetCell(c.position);
                cell.actionIcon.gameObject.SetActive(true);
                activebuttons.Add(cell.actionIcon);
                cell.actionIcon.GetComponentInChildren<TextMeshProUGUI>().text = $"{c.distance}";
                cell.actionIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    callback(buyable[i].position);
                    ClearButtons();
                });

                index += 1;
            }
        }

        private bool ContatainsCoord(List<BuyGroundDistance> list, Coord pos)
        {
            foreach(BuyGroundDistance b in list)
            {
                if(b.position == pos) { return true; }
            }

            return false;
        }

        public void ClearButtons()
        {
            if(activebuttons == null) { return; }
            foreach(Canvas b in activebuttons)
            {
                b.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                b.gameObject.SetActive(false);
            }
        }

        private struct BuyGroundDistance
        {
            public Coord position;
            public int distance;

            public BuyGroundDistance(Coord pos, int dist)
            {
                position = pos;
                distance = dist;
            }
        }
    }
}
