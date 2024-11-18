using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [SerializeField] private Players currentPlayer = Players.None;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public int GetPlayer()
    {
        return (int)currentPlayer;
    }

    public void SetPlayer(int player)
    {
        currentPlayer = (Players)player;
    }

    public enum Players
    {
        None = 0,
        Player1 = 1,
        Player2 = 2,
    }
}
