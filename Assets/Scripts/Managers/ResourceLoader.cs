using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldStrategy.Buildings;
using WorldStrategy.Borders;
using WorldStrategy.Animations;

namespace WorldStrategy
{
    public class ResourceLoader : MonoBehaviour
    {
        public static ResourceLoader Instance;
        private Dictionary<CellType, CellSettings> cells;
        private Dictionary<UnitType, UnitSettings> units;
        private Dictionary<BuildingType, BuildingSettings> buildings;
        private Dictionary<int, Border> borders;
        private Dictionary<AnimationType, AnimationSettings> animations;

        public GameObject buildingPrefab;

        private void Awake()
        {
            LoadResources();
        }

        [Button("Reload resoureces")]
        public void LoadResources()
        {
            Instance = this;
            cells = new Dictionary<CellType, CellSettings>();
            CellSettings[] loadedResources = Resources.LoadAll<CellSettings>("Scriptable/Cells");
            for (int i = 0; i < loadedResources.Length; i++)
            {
                cells.Add(loadedResources[i].type, loadedResources[i]);
            }

            units = new Dictionary<UnitType, UnitSettings>();
            UnitSettings[] loadedUnits = Resources.LoadAll<UnitSettings>("Scriptable/Units");
            for (int i = 0; i < loadedUnits.Length; i++)
            {
                units.Add(loadedUnits[i].type, loadedUnits[i]);
            }

            buildings = new Dictionary<BuildingType, BuildingSettings>();
            BuildingSettings[] loadedBuildings = Resources.LoadAll<BuildingSettings>("Scriptable/Buildings");
            for (int i = 0; i < loadedBuildings.Length; i++)
            {
                buildings.Add(loadedBuildings[i].type, loadedBuildings[i]);
            }

            borders = new Dictionary<int, Border>();
            Border[] loadedBoarders = Resources.LoadAll<Border>("Scriptable/Borders");
            for (int i = 0; i < loadedBoarders.Length; i++)
            {
                borders.Add(loadedBoarders[i].index, loadedBoarders[i]);
            }

            animations = new Dictionary<AnimationType, AnimationSettings>();
            AnimationSettings[] loadedAnimations = Resources.LoadAll<AnimationSettings>("Scriptable/Animations");
            for (int i = 0; i < loadedAnimations.Length; i++)
            {
                animations.Add(loadedAnimations[i].type, loadedAnimations[i]);
            }
        }

        public static UnitSettings[] GetAllUnits()
        {
            UnitSettings[] sprites = new UnitSettings[Instance.units.Count];

            int count = 0;
            foreach(UnitSettings unit in Instance.units.Values)
            {
                sprites[count] = unit;
                count += 1;
            }

            return sprites;
        }

        public static Sprite[] GetAllUnitIcons()
        {
            Sprite[] sprites = new Sprite[Instance.units.Count];

            int count = 0;
            foreach (UnitSettings unit in Instance.units.Values)
            {
                sprites[count] = unit.icon;
                count += 1;
            }

            return sprites;
        }

        public static BuildingSettings[] GetAllBuildings()
        {
            BuildingSettings[] sprites = new BuildingSettings[Instance.buildings.Count];

            int count = 0;
            foreach (BuildingSettings unit in Instance.buildings.Values)
            {
                sprites[count] = unit;
                count += 1;
            }

            return sprites;
        }

        public static Sprite[] GetAllBuildingIcons()
        {
            Sprite[] sprites = new Sprite[Instance.units.Count];

            int count = 0;
            foreach (BuildingSettings unit in Instance.buildings.Values)
            {
                sprites[count] = unit.icon;
                count += 1;
            }

            return sprites;
        }

        public static CellSettings GetCell(CellType type)
        {
            if (Instance == null || Instance.cells == null) { Instance.LoadResources(); }

            Instance.cells.TryGetValue(type, out CellSettings value);
            return value;
        }

        public static UnitSettings GetUnit(UnitType type)
        {
            if (Instance == null || Instance.units == null) { Instance.LoadResources(); }

            Instance.units.TryGetValue(type, out UnitSettings value);
            return value;
        }

        public static BuildingSettings GetBuilding(BuildingType type)
        {
            if (Instance == null || Instance.units == null) { Instance.LoadResources(); }

            Instance.buildings.TryGetValue(type, out BuildingSettings value);
            return value;
        }

        public static Border GetBorder(int index)
        {
            if (Instance == null || Instance.borders == null) { Instance.LoadResources(); }

            Instance.borders.TryGetValue(index, out Border value);
            return value;
        }

        public static AnimationSettings GetAnimation(AnimationType type)
        {
            if (Instance == null || Instance.animations == null) { Instance.LoadResources(); }

            Instance.animations.TryGetValue(type, out AnimationSettings value);
            return value;
        }
    }
}