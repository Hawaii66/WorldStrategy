using UnityEngine;
using WorldStrategy.Borders;

namespace WorldStrategy
{
    public static class BorderManager
    {
        public static BorderData GetBorder(Coord pos, out int borderIndex)
        {
            if(Utils.InsideBound(pos) && TerrainManager.Instance.GetCell(pos).owner == -1)
            {
                borderIndex = -1;
                return new BorderData(null, 0);
            }

            int maxX = TerrainManager.GridSize;
            int maxZ = TerrainManager.GridSize;
            int playerIndex = TerrainManager.Instance.GetCell(pos).owner;

            Coord[] offsets = Utils.offsets;

            bool[] neg = new bool[4];
            for (int i = 0; i < 4; i++)
            {   
                Coord offset = pos + offsets[i];

                if (Utils.InsideBound(offset, maxX, maxZ))
                {
                    Cell cell = TerrainManager.Instance.GetCell(offset);
                    if (cell.owner == -1) { neg[i] = true; continue; }
                    Player owner = cell.owner.GetPlayer();
                    if (owner == null || owner.id != playerIndex)
                    {
                        neg[i] = true;
                    }
                }
            }

            int index = 0;
            if (neg[0]) index |= 1;
            if (neg[1]) index |= 2;
            if (neg[2]) index |= 4;
            if (neg[3]) index |= 8;
            borderIndex = index;

            Border border = ResourceLoader.GetBorder(index);
            if (border == null)
            {
                Debug.Log("Border null: " + index + " : " + neg[0] + " : " + neg[1] + " : " + neg[2] + " : " + neg[3]);
                return new BorderData(null, 0);
            }

            return new BorderData(border.mesh, border.rotation);
        }
    }

    public struct BorderData
    {
        public Mesh mesh;
        public int rotation;

        public BorderData(Mesh m, int rot)
        {
            mesh = m;
            rotation = rot;
        }
    }
}
