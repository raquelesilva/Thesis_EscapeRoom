using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrinkSelector : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI drinkName;
    [SerializeField] Image drinkImage;

    [SerializeField] public DrinksValues drinksValue;

    public void SetDrink()
    {
        CoffeeManager.instance.ToggleSelectionScreen(false);
        CoffeeManager.instance.ToggleConfigurationScreen(true);
        drinksValue.SetDrink(drinkName.text, drinkImage.sprite);
    }
}