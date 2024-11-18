using CoreSystems.Extensions.Attributes;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PCManager : MonoBehaviour
{
    [Header("Boot")]
    [SerializeField] private GameObject pcBootScreen;
    [SerializeField] private float bootAnimationDuration;
    [SerializeField] private GameObject loadingIcon;

    [Header("Login")]
    [SerializeField] private GameObject pcLoginScreen;
    [SerializeField] private string pin;
    [SerializeField] private TMP_InputField pinInputField;
    [SerializeField] private TextMeshProUGUI pinIncorrectMessage;
    [SerializeField] private Sprite showPINImage;
    [SerializeField] private Sprite hidePINImage;

    [Header("Desktop")]
    [SerializeField] private GameObject pcDesktopScreen;
    [SerializeField] private List<PCWindow> allWindows;

    [Header("BrowserTabs")]
    [SerializeField] private List<GameObject> tabs = new();

    [Header("Turn Off")]
    [SerializeField] private GameObject pcOffScreen;
    [SerializeField] private TextMeshProUGUI offLoadingText;
    [SerializeField] private UnityEvent onTurnOffPC;

    [Header("Focus")]
    [SerializeField] bool canFocus;
    [SerializeField, Disable("canFocus")] UnityEvent onDefocus;

    [SerializeField, Disable] bool isFocused;

    private void Awake()
    {
        pinInputField.characterLimit = pin.Length;
    }

    private void Update()
    {
        if (canFocus)
        {
            if (isFocused && Input.GetKeyDown(KeyCode.Escape))
            {
                isFocused = false;
                onDefocus?.Invoke();
            }
        }
    }

    public void SetFocus()
    {
        isFocused = true;
    }

    public void BootPC()
    {
        pcBootScreen.SetActive(true);
        pcLoginScreen.SetActive(false);
        pcDesktopScreen.SetActive(false);
        pcOffScreen.SetActive(false);
        StartCoroutine(BootingAnimation());
    }
    IEnumerator BootingAnimation()
    {
        loadingIcon.SetActive(true);
        Tween tween = loadingIcon.transform.DOLocalRotate(new Vector3(0, 0, -1800), bootAnimationDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear);
        yield return tween.WaitForCompletion();

        pcBootScreen.GetComponent<Animator>().SetTrigger("FadeOut");
        pcLoginScreen.SetActive(true);
    }

    public void TogglePinVisibility(Image image)
    {
        bool isVisible = pinInputField.contentType == TMP_InputField.ContentType.IntegerNumber;
        isVisible = !isVisible;

        pinInputField.contentType = isVisible ? TMP_InputField.ContentType.IntegerNumber : TMP_InputField.ContentType.Pin;
        image.sprite = isVisible ? hidePINImage : showPINImage;

        pinInputField.Select();
    }

    public void CheckPassword()
    {
        if (pinInputField.text.Equals(pin))
        {
            pinIncorrectMessage.gameObject.SetActive(false);
            pcLoginScreen.GetComponent<Animator>().SetTrigger("FadeOut");
            pcDesktopScreen.SetActive(true);
        }
        else
        {
            pinIncorrectMessage.gameObject.SetActive(true);
        }
    }

    public void ChangeWindows(PCWindow windowToSkip)
    {
        foreach (var item in allWindows)
        {
            if (!item.gameObject.Equals(windowToSkip.gameObject))
            {
                item.DisableWindow();
            }
        }
    }

    public void ChangeBrowserTabs(GameObject windowToOpen)
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            tabs[i].SetActive(false);
        }

        windowToOpen.SetActive(true);
    }

    public void TurnOffPC()
    {
        pcDesktopScreen.GetComponent<Animator>().SetTrigger("FadeOut");
        pcOffScreen.SetActive(true);
        StartCoroutine(OffAnimation());
    }

    IEnumerator OffAnimation()
    {
        string baseText = "A Encerrar";
        for (int i = 0; i < 3; i++)
        {
            offLoadingText.text = baseText;
            yield return new WaitForSeconds(.5f);
            for (int j = 0; j < 3; j++)
            {
                offLoadingText.text += ".";
                yield return new WaitForSeconds(.5f);
            }
        }

        pcBootScreen.SetActive(false);
        pcLoginScreen.SetActive(false);
        pcDesktopScreen.SetActive(false);
        pcOffScreen.SetActive(false);

        onTurnOffPC?.Invoke();

        yield return null;
    }
    public void LogOff()
    {
        foreach (var item in allWindows)
        {
            item.DisableWindow();
        }
        pcDesktopScreen.GetComponent<Animator>().SetTrigger("FadeOut");
        pcLoginScreen.SetActive(true);
    }
}
