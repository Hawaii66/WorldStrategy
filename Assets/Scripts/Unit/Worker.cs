using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WorldStrategy.Animations;
using WorldStrategy.Buildings;
using WorldStrategy.UI;

namespace WorldStrategy
{
    public class Worker : Unit
    {
        private BuildingType crafting;
        private int craftCount;

        public override void Init(Coord pos, UnitType t, int o)
        {
            base.Init(pos, t, o);

            crafting = BuildingType.None;
            craftCount = 0;
        }

        public override bool ShowIcon()
        {
            if(crafting != BuildingType.None) { return false; }
            return GetPossibleBuildings().Length > 0;
        }

        private BuildingSettings[] GetPossibleBuildings()
        {
            BuildingSettings[] buildings = ResourceLoader.GetAllBuildings();
            List<BuildingSettings> aprovedForConstructing = new List<BuildingSettings>();

            Cell c = TerrainManager.Instance.GetCell(position);

            for (int i = 0; i < buildings.Length; i++)
            {
                if(c.building != null) { continue; }
                if(buildings[i].buildInTerritory && owner != c.owner) { continue; }

                if (buildings[i].type == BuildingType.Farm && c.type == CellType.Farmland)
                {
                    aprovedForConstructing.Add(buildings[i]);
                }

                if (buildings[i].type == BuildingType.Road && c.type == CellType.Grass)
                {
                    aprovedForConstructing.Add(buildings[i]);
                }

                if(buildings[i].type == BuildingType.Lumber && c.type == CellType.Forest)
                {
                    aprovedForConstructing.Add(buildings[i]);
                }
            }

            return aprovedForConstructing.ToArray();
        }

        public override void ActionButtonClicked()
        {
            BuildingSettings[] buildings = GetPossibleBuildings();
            Sprite[] sprites = new Sprite[buildings.Length];
            for(int i = 0; i < buildings.Length; i ++)
            {
                sprites[i] = buildings[i].icon;
            }

            BottomSelect.Instance.NewSelect(sprites, (int index) =>
            {
                craftCount = 0;
                crafting = buildings[index].type;
            });
        }

        public override void AfterTurn()
        {
            if(crafting == BuildingType.None) { return; }

            craftCount += 1;
            int craftAmount = ResourceLoader.GetBuilding(crafting).BuildTime;
            if(craftCount >= craftAmount)
            {
                Cell cell = TerrainManager.Instance.GetCell(position);
                if(cell.building == null)
                {
                    craftCount -= craftAmount;
                    cell.type = CellType.Grass;
                    cell.UpdateMesh();
                    cell.owner = owner;
                    cell.SetBuilding(crafting);
                    crafting = BuildingType.None;

                    gameObject.AddComponent<AnimationHandler>().Init(AnimationType.FadeOut, transform.localPosition, () =>
                    {
                        Kill();
                    });
                }
            }
        }
    }
}
