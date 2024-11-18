using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PipeScreen : MonoBehaviour
{
    [SerializeField] private List<PipeRotater> pipes = new(); // --> preencher esta lista com os pipes usados no jogo

    public int codePart = 1;

    [SerializeField] public BallPlayerMovement cursorBall;

    [SerializeField] public UnityEvent onWinScreen;
    bool ballGameRunning = false;

    // TO DO:
    // Fazer a verificação algures de se o pipe está com o ângulo correto
    // No final desligar a lista dos pipes
    // Fazer a animação dos quadradinhos a ligar

    // criar referências para a entrance e para a exit tiles
    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        ballGameRunning = false;
        StartBallGame(false);

        PipeRandomRotation();
    }

    private void PipeRandomRotation()
    {
        for (int i = 0; i < pipes.Count; i++)
        {
            int randomRotation = Random.Range(0, 5);
            int rotation = 0;

            switch (randomRotation)
            {
                case 0:
                    rotation = 90;
                    break;

                case 1:
                    rotation = 180;
                    break;

                case 2:
                    rotation = 270;
                    break;

                case 3:
                    rotation = 0;
                    break;
            }

            pipes[i].transform.Rotate(Vector3.forward, rotation);
            pipes[i].GetComponent<PipeRotater>().SetImagesActive(true);
            pipes[i].GetComponent<PipeRotater>().SetRaycast(true);
        }
    }

    public void EnableDisableRotation(bool decision)
    {
        foreach(var pipe in pipes)
        {
            pipe.GetComponent<PipeRotater>().SetCanRotate(decision);
        }
    }

    public void StartBallGame(bool decision)
    {
        ballGameRunning = decision;
        cursorBall.BallActive(decision);
        EnableDisableRotation(!decision);
    }

    public void StartStopBallGame()
    {
        print("Is this working?");
        ballGameRunning = !ballGameRunning;
        print("ballGameRunning: " + ballGameRunning);
        cursorBall.gameObject.SetActive(ballGameRunning);
        cursorBall.ResetPosition();
        EnableDisableRotation(!ballGameRunning);
    }

    public void WinScreen()
    {
        StartBallGame(false);
        onWinScreen?.Invoke();
    }
}