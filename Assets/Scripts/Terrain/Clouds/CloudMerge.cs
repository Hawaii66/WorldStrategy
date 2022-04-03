using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldStrategy.Clouds
{
    public class CloudMerge : StaticInstance<CloudMerge>
    {
        [SerializeField] private Transform cloudParent;
        [ShowAssetPreview, SerializeField] private Mesh cloudMesh;
        [ShowAssetPreview, SerializeField] private GameObject cloudPrefab;
        private PlayerVisibilitiy[,] visible;

        private void Start()
        {
            StartUp();
        }

        private void StartUp()
        {
            int size = TerrainManager.GridSize;
            int playerCount = TurnManager.Instance.players.Length;

            visible = new PlayerVisibilitiy[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    visible[x, z] = new PlayerVisibilitiy(new bool[playerCount]);
                }
            }
        }

        public bool Visible(Coord position, int player)
        {
            return visible[position.x, position.z].playerIndexes[player];
        }

        public void RemoveMultipleClouds(int player, Coord center, int size)
        {
            for(int x = -size; x <= size; x ++)
            {
                for(int z = -size; z <= size; z ++)
                {
                    Coord pos = center + new Coord(x, 0, z);
                    if (Utils.InsideBound(pos))
                    {
                        RemoveCloud(player, pos, false);
                    }
                }
            }

            ToggleEverythingVisible(TurnManager.Instance.playerIndex);

            GenerateClouds();
        }

        public void ToggleEverythingVisible(int oldIndex)
        {
            Player[] players = TurnManager.Instance.players;
            foreach (Cell b in players[oldIndex].cells.Values)
            {
                if (b.building != null)
                {
                    b.building.UpdateMesh();
                }
            }

            TerrainManager.Instance.ToggleCellVisible();
            UnitManager.Instance.ToggleUnitVissible();
        }

        public void RemoveCloud(int player, Coord position, bool redraw)
        {
            if(visible == null) { StartUp(); }

            visible[position.x, position.z].playerIndexes[player] = true;

            if (redraw)
            {
                GenerateClouds();
            }
        }

        public void GenerateClouds()
        {
            cloudParent.DestroyChildren();

            int size = TerrainManager.GridSize;
            int playerIndex = TurnManager.Instance.GetCurrentPlayer().id;

            List<CombineInstance> meshes = new List<CombineInstance>();

            int maxVerts = 65000;
            int vertCount = 0;

            GameObject cloud = GetNewCloud();

            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    if(!visible[x,z].playerIndexes[playerIndex])
                    {
                        CombineInstance instance = new CombineInstance()
                        {
                            mesh = cloudMesh,
                            transform = Matrix4x4.Translate(new Vector3(x, -1.5f, z))
                        };

                        int newCount = vertCount + cloudMesh.vertexCount;
                        if(newCount > maxVerts)
                        {
                            Mesh mesh = new Mesh();
                            mesh.CombineMeshes(meshes.ToArray(), true, true);
                            cloud.GetComponent<MeshFilter>().mesh = mesh;
                            meshes.Clear();

                            vertCount = 0;
                            cloud = GetNewCloud();
                        }

                        vertCount += cloudMesh.vertexCount;
                        meshes.Add(instance);
                    }
                }
            }

            Mesh finalMesh = new Mesh();
            finalMesh.CombineMeshes(meshes.ToArray(), true, true);
            cloud.GetComponent<MeshFilter>().mesh = finalMesh;
        }

        private GameObject GetNewCloud()
        {
            GameObject cloud = Instantiate(cloudPrefab);
            cloud.transform.SetParent(cloudParent);
            cloud.transform.position = Vector3.zero;
            return cloud;
        }

        private struct PlayerVisibilitiy
        {
            public bool[] playerIndexes;

            public PlayerVisibilitiy(bool[] players)
            {
                playerIndexes = players;
            }
        }
    }
}