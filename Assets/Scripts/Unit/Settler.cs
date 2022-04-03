using UnityEngine;
using WorldStrategy.Animations;

namespace WorldStrategy
{
    public class Settler : Unit
    {
        public override bool ShowIcon()
        {
            return MyTurn() && TerrainManager.Instance.GetCell(position).type == CellType.Grass;
        }

        public override void ActionButtonClicked()
        {
            if (ShowIcon())
            {
                Settle();
            }
        }

        public bool Settle()
        {
            Cell c = TerrainManager.Instance.GetCell(position);
            if(c.type != CellType.Grass) { return false; }

            c.owner = owner;
            c.SetBuilding(BuildingType.City);

            gameObject.AddComponent<AnimationHandler>().Init(AnimationType.FadeOut, transform.localPosition, () =>
            {
                Kill();
            });

            return true;
        }
    }
}
