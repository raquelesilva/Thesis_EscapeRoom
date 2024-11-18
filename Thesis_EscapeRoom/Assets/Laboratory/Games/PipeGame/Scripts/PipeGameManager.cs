using CoreSystems.Extensions.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PipeGameManager : MonoBehaviour
{
    [SerializeField] private UnityEvent onDefocus;
    [SerializeField] private UnityEvent onStart;

    [Header("Debug:")]
    [SerializeField, Disable] bool focusedInGame;
    [SerializeField, Disable] Transform currentTransform;  

    public static PipeGameManager instance;

    private void Awake()
    {
        instance = this;
        onStart?.Invoke();
    }

    public void SetIsFocus(bool isPlaying)
    {
        focusedInGame = isPlaying;
    }

    private void Update()
    {
        if (focusedInGame && Input.GetKeyDown(KeyCode.Escape))
        {
            SetIsFocus(false);
            onDefocus?.Invoke();
        }
    }

    public void FocusCameraInGame(float delay)
    {
        if (currentTransform == null) return;
        StartCoroutine(Routine());

        IEnumerator Routine()
        {
            yield return new WaitForSeconds(delay);
            FirstPersonController.instance.AnimateCamera(currentTransform);
        }
    }

    public void SetGameFocusTransform(Transform newTransform)
    {
        currentTransform = newTransform;
    }
}
