using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScenePlayersManager : MonoBehaviour
{
    public static ScenePlayersManager instance;
    private PlayerManager playerManager;

    [Header("Player 1: ")]
    [SerializeField] private Transform player1InitialPosition;
    [Space(10)]
    [SerializeField] private UnityEvent onPlayer1Spawn;
    [Header("Player 2: ")]
    [SerializeField] private Transform player2InitialPosition;
    [Space(10)]
    [SerializeField] private UnityEvent onPlayer2Spawn;

    [Header("General")]
    [SerializeField] private FirstPersonController playerController;

    [Header("Timer")]
    [SerializeField] private int timer;
    [SerializeField] private int timerLimit;
    [SerializeField] private int timerWarning = 300;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image timerMask;
    [SerializeField] private Image timerImage;

    [Header("Demo")]
    [SerializeField] private bool isDemo;
    [SerializeField] private int timerDemoLimit = 360;
    [SerializeField] private int timerDemoWarning = 120;
    [SerializeField] private GameObject videoIntro;
    [SerializeField] private GameObject demoVideoIntro;
    [SerializeField] private GameObject video2min;
    [SerializeField] private GameObject video4min;
    [SerializeField] private GameObject videoEnd;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartTimer();

        playerManager = PlayerManager.instance;

        if (playerManager.GetPlayer() == (int)PlayerManager.Players.Player1)
        {
            playerController.transform.position = player1InitialPosition.position;
            playerController.transform.rotation = player1InitialPosition.rotation;

            onPlayer1Spawn?.Invoke();
        }
        else if (playerManager.GetPlayer() == (int)PlayerManager.Players.Player2)
        {
            playerController.transform.position = player2InitialPosition.position;
            playerController.transform.rotation = player2InitialPosition.rotation;

            onPlayer2Spawn?.Invoke();
        }
    }

    private IEnumerator TimerRoutine(int timerlimit, int timerWarning)
    {
        yield return new WaitForSeconds(1f);

        timer--;

        int seconds = timer % 60;
        int minutes = timer / 60;

        timerMask.fillAmount = 1 - ((float)timer / (float)timerlimit);
        timerText.text = minutes + ":" + seconds.ToString("D2");

        if (timer % timerWarning == 0)
        {
            ActivateWarning();
        }

        if (timer <= 0)
        {
            timerImage.color = Color.red;

            if (!isDemo)
            {
                timerText.text = timerText.text.Replace("-", "");
                timerText.text = "-" + timerText.text;
            }
            else
            {
                playerController.SetPlayerState(2);
                yield break;
            }
        }
        
        StartCoroutine(TimerRoutine(timerlimit, timerWarning));
    }

    // Method to start the timer
    public void StartTimer()
    {
        timerMask.gameObject.SetActive(true);

        int newTimerLimit;
        int newTimerWarning;

        if (isDemo)
        {
            demoVideoIntro.SetActive(true);
            newTimerLimit = timerDemoLimit;
            newTimerWarning = timerDemoWarning;
        }
        else
        {
            videoIntro.SetActive(true);
            newTimerLimit = timerLimit;
            newTimerWarning = timerWarning;
        }

        timer = newTimerLimit;
        timerMask.fillAmount = 0f;
        timerImage.color = Color.white;

        int seconds = timer % 60;
        int minutes = timer / 60;
        timerMask.fillAmount = (float)(timer / newTimerLimit);
        timerText.text = minutes + ":" + seconds.ToString("D2");

        StartCoroutine(TimerRoutine(newTimerLimit, newTimerWarning));
    }

    public void ActivateWarning()
    {
        StartCoroutine(FlashRed());
        audioSource.Play();

        if (!isDemo) { return; }

        if (timer <= 0)
        {
            ChangeActiveVideo(videoEnd);
        }
        else if (timer <= timerDemoLimit - (timerDemoWarning * 2))
        {
            ChangeActiveVideo(video4min);
        }
        else if (timer <= timerDemoLimit - timerDemoWarning)
        {
            ChangeActiveVideo(video2min);
        }
    }

    private void ChangeActiveVideo(GameObject currentVideo)
    {
        demoVideoIntro.SetActive(false);
        video2min.SetActive(false);
        video4min.SetActive(false);
        videoEnd.SetActive(false);

        currentVideo.SetActive(true);
    }

    private IEnumerator FlashRed()
    {
        timerImage.color = Color.red;

        yield return new WaitForSeconds(1f);

        timerImage.color = Color.white;
    }

    public bool IsDemo()
    {
        return isDemo;
    }
}