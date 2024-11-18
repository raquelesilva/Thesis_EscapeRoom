using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class SettingsMenuManager : MonoBehaviour
{
    [Header("Boutons")]
    public Button ChangeControls;

    [Header("Windows")]
    public GameObject ControlsMenu;

    private void Start()
    {
        ControlsMenu.SetActive(false);

    }

    public void OnClickChangeControls()
    {
        
        ControlsMenu.SetActive(true);
    }

    public void OnClickLeaveControls()
    {
        
        ControlsMenu.SetActive(false);
    }
}
