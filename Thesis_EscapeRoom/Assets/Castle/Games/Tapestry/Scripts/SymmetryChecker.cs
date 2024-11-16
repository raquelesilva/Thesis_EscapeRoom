using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymmetryChecker : MonoBehaviour
{
    public int timesDone = 0;
    [SerializeField] Animator doorAnimator;

    public static SymmetryChecker instance;

    private void Awake()
    {
        instance = this;
    }

    public void CheckGame()
    {
        timesDone++;

        if (timesDone == 4)
        {
            doorAnimator.enabled = true;
            NotificationManager.instance.SetMessage("Parabéns! Acabaste o jogo!", NotificationManager.instance.GetColorRight());
        }
    }
}