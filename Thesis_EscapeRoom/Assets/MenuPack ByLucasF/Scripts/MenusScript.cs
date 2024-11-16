using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenusScript : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuGameObj;
    [SerializeField] private OptionsMenu optionsMenu;
    [SerializeField] private Image imageFade;

    Tween imageFadeTween;

    private static bool isPaused;
    void Start()
    {
        if (optionsMenu == null) return;
        //optionsMenu.audioMixer.SetFloat("Volume", PlayerPrefs.GetFloat("gameVolume"));
        //QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("qualityInt"));
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (FirstPersonController.instance.currentPlayerState == PlayerStates.playing || FirstPersonController.instance.currentPlayerState == PlayerStates.paused)
            {
                if (isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

    }

    #region pauseMenu
    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        FirstPersonController.instance.SetPlayerState((int)PlayerStates.playing);
        pauseMenuGameObj.SetActive(false);
    }
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        FirstPersonController.instance.SetPlayerState((int)PlayerStates.paused);
        pauseMenuGameObj.SetActive(true);
    }
    #endregion



    public void LoadScene(string sceneName)
    {
        //Time.timeScale = 1f;
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = false;
        //imageFade.gameObject.SetActive(true);
        //imageFadeTween = imageFade.DOFade(1,2.9f).OnComplete(() => StartCoroutine(LoadSceneCouroutine(sceneName)));
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(LoadSceneCouroutine(sceneName));
    }
    public void ReloadScene()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(LoadSceneCouroutine(SceneManager.GetActiveScene().ToString()));
    }
    IEnumerator LoadSceneCouroutine(string sceneName)
    {
        
        yield return new WaitForSeconds(.5f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(sceneName);
    }
    public void QuitApp()
    {
        Application.Quit();
    }
}


