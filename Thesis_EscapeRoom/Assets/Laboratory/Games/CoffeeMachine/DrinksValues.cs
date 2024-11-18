using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrinksValues : MonoBehaviour
{
    public int intensity;
    public int quantity;
    public int temperature;

    [SerializeField] TextMeshProUGUI drinkName;
    [SerializeField] Image image;

    public static DrinksValues instance;

    private void Awake()
    {
        instance = this;
    }

    public void SetIntensity(int intensity)
    {
        this.intensity = intensity;
    }
    public void SetQuantity(int quantity)
    {
        this.quantity = quantity;
    }
    public void SetTemperature(int temperature)
    {
        this.temperature = temperature;
    }


    public void SetDrink(string drinkName, Sprite image)
    {
        this.drinkName.text = drinkName;
        this.image.sprite = image;
    }

    public void MakeDrink()
    {
        CoffeeManager.instance.GetDrink(drinkName.text.ToLower(), intensity, quantity, temperature);
    }
}