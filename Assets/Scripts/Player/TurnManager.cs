using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldStrategy.Clouds;

namespace WorldStrategy
{
    public class TurnManager : StaticInstance<TurnManager>
    {
        public Player[] players;
        public int playerIndex;
        private bool hasStarted;

        private void Start()
        {
            players = new Player[]
            {
                new Player(0, Color.red),
                new Player(1, Color.green)
            };

            playerIndex = 0;
        }

        private void Update()
        {
            if (!hasStarted)
            {
                hasStarted = true;
                foreach (Unit u in players[playerIndex].units.Values)
                {
                    u.canMove = true;
                }
            }

            if(Input.GetKeyDown(KeyCode.K))
            {
                NextTurn();
            }
        }

        public Player GetCurrentPlayer()
        {
            return players[playerIndex];
        }

        public Player GetPlayerIndex(int i)
        {
            return players[i];
        }

        public void NextTurn()
        {
            ClickManager.Instance.SuperUnselectAll();

            //Before Change

            foreach (Cell cell in players[playerIndex].cells.Values)
            {
                if (cell.building != null)
                {
                    cell.building.AfterTurn();
                }
            }

            foreach (Unit unit in players[playerIndex].units.Values)
            {
                if (unit.type == UnitType.Worker)
                {
                    Worker worker = (Worker)unit;
                    worker.AfterTurn();
                }
            }

            StartCoroutine(InternalNextTurnAfter());
        }

        private IEnumerator InternalNextTurnAfter()
        {
            yield return new WaitForSeconds(0.5f);

            //After Change
            int newPlayerIndex = playerIndex + 1;
            if (newPlayerIndex >= players.Length) { newPlayerIndex = 0; }

            int oldPlayerIndex = playerIndex;
            playerIndex = newPlayerIndex;

            foreach (Cell b in players[oldPlayerIndex].cells.Values)
            {
                if (b.building != null)
                {
                    b.building.UpdateMesh();
                }
            }

            TerrainManager.Instance.ToggleCellVisible();
            UnitManager.Instance.ToggleUnitVissible();

            foreach (Unit u in players[playerIndex].units.Values)
            {
                u.canMove = true;
            }

            foreach (Cell b in players[playerIndex].cells.Values)
            {
                if (b.building != null)
                {
                    b.building.UpdateMesh();
                }
            }

            CloudMerge.Instance.GenerateClouds();
        }
    }   
}