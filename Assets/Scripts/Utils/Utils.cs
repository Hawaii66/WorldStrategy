using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WorldStrategy
{
    public enum Direction { Up, Down, Left, Right}

    public static class Utils
    {
        public static Coord[] offsets = new Coord[]
        {
            new Coord(1,0,0),
            new Coord(-1,0,0),
            new Coord(0,0,1),
            new Coord(0,0,-1)
        };

        public static Coord[] diagonal = new Coord[]
        {
            new Coord(1,0,1),
            new Coord(-1,0,-1),
            new Coord(-1,0,1),
            new Coord(1,0,-1)
        };

        public static Coord[] offsets0 = new Coord[]
        {
            new Coord(0,0,0),
            new Coord(0,0,1),
            new Coord(0,0,-1),
            new Coord(-1,0,0),
            new Coord(1,0,0)
        };

        public static Coord[] allOffsets = new Coord[]
        {
            new Coord(1,0,0),
            new Coord(-1,0,0),
            new Coord(0,0,1),
            new Coord(0,0,-1),
            new Coord(1,0,1),
            new Coord(-1,0,-1),
            new Coord(-1,0,1),
            new Coord(1,0,-1)
        };

        public static Coord GetOffset(this Direction d)
        {
            if(d == Direction.Up) { return new Coord(0, 0, 1); }
            if(d == Direction.Down) { return new Coord(0, 0, -1); }
            if(d == Direction.Right) { return new Coord(1, 0, 0); }
            if(d == Direction.Left) { return new Coord(-1, 0, 0); }

            return Coord.Zero();
        }

        public static void DestroyChildren(this Transform t)
        {
            foreach (Transform _t in t) Object.Destroy(_t.gameObject);
        }

        public static CellSettings GetCell(this CellType type)
        {
            return ResourceLoader.GetCell(type);
        }

        public static UnitSettings GetUnit(this UnitType type)
        {
            return ResourceLoader.GetUnit(type);
        }

        public static bool InsideBound(Coord pos)
        {
            return pos.x >= 0 && pos.x < TerrainManager.GridSize && pos.z >= 0 && pos.z < TerrainManager.GridSize;
        }

        public static bool InsideBound(Coord pos, int maxX, int maxZ)
        {
            return pos.x >= 0 && pos.x < maxX && pos.z >= 0 && pos.z < maxZ;
        }

        public static Player GetPlayer(this int index)
        {
            return TurnManager.Instance.GetPlayerIndex(index);    
        }


        private static PointerEventData pointerEventData;
        private static List<RaycastResult> results;
        public static bool IsOverUI()
        {
            pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);
            return results.Count > 0;
        }
    }
}