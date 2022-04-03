using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WorldStrategy.UI
{
    public class CityMenu : StaticInstance<CityMenu>
    {
        [SerializeField] private GameObject parent;
        [SerializeField] private TextMeshProUGUI cityName;
        [SerializeField] private Image healthBar;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Gradient healthbarGradient;
        [SerializeField] private TextMeshProUGUI productionText;
        [SerializeField] private Image productionIcon;
        [SerializeField] private TextMeshProUGUI unitName;

        private City currentCity;

        public void ShowMenu(City city)
        {
            currentCity = city;
            float maxHealth = city.maxHealth;
            float health = city.health;
            float percent = health / maxHealth;

            healthBar.color = healthbarGradient.Evaluate(percent);
            healthBar.fillAmount = percent;
            healthText.text = $"{health}/{maxHealth}";

            productionText.text = $"+{city.productionPerTurn}";
            cityName.text = city.name;

            UnitSettings unit = ResourceLoader.GetUnit(city.training);
            if (unit != null && unit.type != UnitType.None)
            {
                productionIcon.sprite = unit.icon;
                unitName.text = $"{unit.name} {city.production}/{unit.productionCost}";
            }
            else
            {
                productionIcon.sprite = null;
                unitName.text = "Nothing to train";
            }

            //parent.transform.position = city.position + new Vector3(0, 2.5f, 0);
            parent.SetActive(true);
        }

        public void Hide()
        {
            parent.SetActive(false);
            if (currentCity) { currentCity.ClearButtons(); }
        }

        public void ChangeTraining()
        {
            UnitSettings[] units = ResourceLoader.GetAllUnits();

            BottomSelect.Instance.NewSelect(ResourceLoader.GetAllUnitIcons(), (int index) =>
            {
                currentCity.training = units[index].type;
                ShowMenu(currentCity);
            });
        }

        public void BuyGround()
        {
            currentCity.BuyGround((Coord target) =>
            {
                currentCity.ownedCells.Add(target);
                TerrainManager.Instance.GetCell(target).owner = TerrainManager.Instance.GetCell(currentCity.position).owner;
                TerrainManager.Instance.UpdateBorders();
            });
        }
    }
}