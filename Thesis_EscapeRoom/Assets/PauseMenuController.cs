using CoreSystems.Extensions.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("Tutorial")]
    [SerializeField, Disable] private int currentDisplayAnimation = 0;
    [SerializeField] private List<Animations> tutorialAnimations;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private void Start()
    {
        // Initialize the tutorial display
        SetTutorial();
    }

    public void NextTutorial()
    {
        if (currentDisplayAnimation < tutorialAnimations.Count - 1)
        {
            currentDisplayAnimation++;
            SetTutorial();
        }
    }

    public void PreviousTutorial()
    {
        if (currentDisplayAnimation > 0)
        {
            currentDisplayAnimation--;
            SetTutorial();
        }
    }

    private void SetTutorial()
    {
        nextButton.interactable = currentDisplayAnimation < tutorialAnimations.Count - 1;
        previousButton.interactable = currentDisplayAnimation > 0;

        for (int i = 0; i < tutorialAnimations.Count; i++)
        {
            tutorialAnimations[i].animator.gameObject.SetActive(i == currentDisplayAnimation);
            tutorialAnimations[i].animator.gameObject.GetComponent<Animator>().Play(tutorialAnimations[i].animationName);
        }
    }

    [Serializable]
    struct Animations
    {
        public string animationName;
        public Animator animator;
    }
}
