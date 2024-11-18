using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector : MonoBehaviour
{
    public void SetPlayer(int player)
    {
        PlayerManager.instance.SetPlayer(player);
    }
}
