using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CoffeeManager : MonoBehaviour
{
    [SerializeField] public List<TypesOfDrinks> typesOfDrinks;
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] GameObject maze;
    [SerializeField] Transform buttonParent;
    [SerializeField] Transform drinkParent;
    [SerializeField] TypesOfDrinks correctDrink;
    [SerializeField] UnityEvent startMaze;
    [SerializeField] PutDownObjects cupHolder;

    [Header("Screens")]
    [SerializeField] GameObject selectionScreen;
    [SerializeField] GameObject configurationScreen;
    [SerializeField] DrinksValues drinksValues;

    public GameObject instantiatedDrink;

    public static CoffeeManager instance;

    private bool hasInteractedOnce = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DisplayDrinks();
    }

    public void DisplayDrinks()
    {
        for (int i = 0; i < typesOfDrinks.Count; i++)
        {
            GameObject currentDrink = Instantiate(buttonPrefab, buttonParent);
            currentDrink.GetComponentInChildren<TextMeshProUGUI>().text = typesOfDrinks[i].drink;
            currentDrink.GetComponent<Image>().sprite = typesOfDrinks[i].image;
            currentDrink.GetComponent<DrinkSelector>().drinksValue = drinksValues;
        }
    }

    public void GetDrink(string currentDrink, int intensity, int amount, int temperature)
    {
        if(this.instantiatedDrink != null)
        {
            Destroy(this.instantiatedDrink.gameObject);
        }

        int drinkNumber = 0;
     
        for (int i = 0; i < typesOfDrinks.Count; i++)
        {
            if (currentDrink == typesOfDrinks[i].drink.ToLower())
            {
                drinkNumber = i;
            }
        }

        GameObject instantiatedDrink = Instantiate(typesOfDrinks[drinkNumber].drinkPrefab, drinkParent);
        instantiatedDrink.GetComponent<FinalDrink>().SetValues(intensity, amount, temperature);
        instantiatedDrink.GetComponent<FinalDrink>().GetMaze(maze);
        this.instantiatedDrink = instantiatedDrink;

        cupHolder.correctItems.Add(instantiatedDrink.GetComponent<Interactable>());
    }

    public void VerifyDrink()
    {
        if (hasInteractedOnce) { return; }

        hasInteractedOnce = true;

        if (instantiatedDrink == null)
        {
            print("Current Obj in Holder is null");
            return;
        }

        if(instantiatedDrink.GetComponent<FinalDrink>().IsCorrect())
        {
            maze.SetActive(true);
            startMaze?.Invoke();
            NotificationManager.instance.GetMessage(2);

        }
        else
        {
            NotificationManager.instance.GetMessage(5);
        }
    }

    public void ToggleSelectionScreen(bool decision)
    {
        selectionScreen.SetActive(decision);
    }
    public void ToggleConfigurationScreen(bool decision)
    {
        configurationScreen.SetActive(decision);
    }
}

[Serializable]
public class TypesOfDrinks
{
    public string drink;
    public Sprite image;
    public GameObject drinkPrefab;

    public TypesOfDrinks(string drink, Sprite image, GameObject drinkPrefab) 
    {
        this.drink = drink;
        this.image = image;
        this.drinkPrefab = drinkPrefab;
    }
}