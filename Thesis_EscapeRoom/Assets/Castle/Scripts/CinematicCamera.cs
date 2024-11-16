using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicCamera : MonoBehaviour
{
    [SerializeField] Animator floorAnimator;

    public void MoveFloor()
    {
        floorAnimator.enabled = true;
    }

    public void DisableCamera()
    {
        Camera.main.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}