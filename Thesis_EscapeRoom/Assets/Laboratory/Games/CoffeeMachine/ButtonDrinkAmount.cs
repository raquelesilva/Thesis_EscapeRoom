using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDrinkAmount : MonoBehaviour
{
    enum DrinkValuesType
    {
        Intensity = 1,
        Quantity = 2,
        Temperature = 3
    }

    [SerializeField] private DrinkValuesType typeOfValue;
    [Space(5)]
    [SerializeField] List<Image> lights;

    private DrinksValues drinksValues;
    private int value = 1;


    private void Awake()
    {
        drinksValues = GetComponentInParent<DrinksValues>();
    }

    public void ChangePresets()
    {
        value++;
        if (value > lights.Count)
        {
            value = 1;
        }

        for (int i = 0; i < lights.Count; i++)
        {
            if (i+1 <= value)
            {
                lights[i].color = Color.white;
            }
            else
            {
                lights[i].color = Color.black;
            }
        }

        SetValues();
    }

    public void SetValues()
    {
        switch (typeOfValue)
        {
            case DrinkValuesType.Intensity:
                drinksValues.SetIntensity(value);
                break;
            case DrinkValuesType.Quantity:
                drinksValues.SetQuantity(value);
                break;
            case DrinkValuesType.Temperature:
                drinksValues.SetTemperature(value);
                break;
        }
    }
}