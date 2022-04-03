using System.Collections.Generic;
using UnityEngine;

namespace WorldStrategy
{
    public class Player
    {
        public int id;
        public Color color;
        public Dictionary<Coord, Cell> cells;
        public Dictionary<Coord, Unit> units;

        public Player(int i, Color c)
        {
            id = i;
            color = c;

            cells = new Dictionary<Coord, Cell>();
            units = new Dictionary<Coord, Unit>();
        }
    }
}
