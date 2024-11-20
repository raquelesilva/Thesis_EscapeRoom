using System.Collections;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera thisCamera;

    Animator animator;

    [SerializeField] GameObject backButtonUI;
    [SerializeField] Collider safe;

    [SerializeField] FirstPersonController player;

    private void Start()
    {
        mainCamera = Camera.main;
        thisCamera = GetComponent<Camera>();
        animator = GetComponent<Animator>();
    }

    public void SwitchOnMainCamera()
    {
        Debug.Log("ON");
        mainCamera.enabled = true;
        thisCamera.enabled = false;

        //player.currentPlayerState = PlayerStates.playing;
    }

    public void SwitchOffMainCamera()
    {
        Debug.Log("OFF");
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.01f);

        thisCamera.enabled = true;
        Debug.Log("Step 1");
        mainCamera.enabled = false;
        Debug.Log("Step 2");

        //player.currentPlayerState = PlayerStates.paused;
    }

    public void BackUIButtonOn()
    {
        backButtonUI.SetActive(true); 
    }

    public void BackUIButtonOff()
    {
        backButtonUI.SetActive(false);
    }

    public void SafeDisable()
    {
        safe.enabled = false;
    }

    public void SafeEnable()
    {
        safe.enabled = true;
    }
}