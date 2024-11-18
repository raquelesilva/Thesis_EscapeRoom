using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    [Header("Menus")]
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject resetControls;
    [SerializeField] private GameObject warningLeaveMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private GameObject miniMenu;

    [Header("Animation")]
    [SerializeField] private Animator leaveMenu;
    [SerializeField] private Animator openMinMenu;
    [SerializeField] private Image imageFade;

    [Header("Keys")]
    [SerializeField] private KeyCode leaveSettings;

    [Header("Raycast Image")]
    [SerializeField] private GameObject raycastLeaveGame;

    //Private variables
    private bool isInMainMenu;
    private bool openMini = false;
    Tween imageFadeTween;

    private void Start()
    {
        isInMainMenu = true;

        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        raycastLeaveGame.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(leaveSettings))
        {
            if (settingsMenu.activeSelf)
            {
                OnClickBackSettings();

                resetControls.SetActive(false);
                isInMainMenu = true;
            }
            else if (creditsMenu.activeSelf)
            {
                OnClickLeaveCreditos();
            }
            else if (resetControls.activeSelf)
            {
                resetControls.SetActive(false);
            }
        }
    }

    #region Scene Loading

    public void LoadSceneWithFade(string sceneToLoad)
    {
        imageFade.gameObject.SetActive(true);
        imageFadeTween = imageFade.DOFade(1, 2.9f).OnComplete(() => { SceneManager.LoadScene(sceneToLoad); });
    }

    public void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    #endregion

    public void OpenSettings()
    {
        imageFade.gameObject.SetActive(true);
        imageFadeTween = imageFade.DOFade(1, 1f).OnComplete(FadeSettingsComplete);
    }
    private void FadeSettingsComplete()
    {
        settingsMenu.SetActive(true);
        isInMainMenu = false;
        imageFadeTween = imageFade.DOFade(0, 1f).OnComplete(ResetFade);
    }

    public void OnClickBackSettings()
    {
        imageFade.gameObject.SetActive(true);
        imageFadeTween = imageFade.DOFade(1, 1f).OnComplete(FadeMenuComplete);
    }
    private void FadeMenuComplete()
    {
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        isInMainMenu = true;
        imageFadeTween = imageFade.DOFade(0, 1).OnComplete(ResetFade);
    }

    //Credits
    public void OnClickCredits()
    {
        imageFade.gameObject.SetActive(true);
        imageFadeTween = imageFade.DOFade(1, 1f).OnComplete(FadeCreditsComplete);
    }
    private void FadeCreditsComplete()
    {
        creditsMenu.SetActive(true);
        isInMainMenu = false;
        imageFadeTween = imageFade.DOFade(0, 1).OnComplete(ResetFade);
    }

    public void OnClickLeaveCreditos()
    {
        imageFade.gameObject.SetActive(true);
        imageFadeTween = imageFade.DOFade(1, 1f).OnComplete(FadeMenuComplete);
    }

    //Quit Game
    public void OpenLeaveWindow(bool decision)
    {
        Animator animator_LeaveGame = warningLeaveMenu.GetComponent<Animator>();

        if (animator_LeaveGame != null)
        {
            animator_LeaveGame.SetBool("IsOpen", decision);
        }
        raycastLeaveGame.SetActive(decision);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    //Reset Fade
    public void ResetFade()
    {
        if (!imageFadeTween.IsComplete()) return;
        imageFade.gameObject.SetActive(false);
    }
}