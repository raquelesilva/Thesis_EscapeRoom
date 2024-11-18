using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PromtManager : MonoBehaviour
{
    [SerializeField] private List<Promt> promts;

    [SerializeField] private TextMeshProUGUI textPromt;

    bool isOn;
    Promt currentPromt;

    public static PromtManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateTooltip(int index)
    {
        Promt newPromt = promts[index];
        if (newPromt == null || newPromt == currentPromt) return;

        isOn = true;
        currentPromt = newPromt;

        textPromt.gameObject.SetActive(true);
        textPromt.text = currentPromt.promtToDisplay;
    }
    public void UpdateTooltip(string promtName)
    {
        Promt newPromt = promts.Find(p => p.promtName == promtName);
        if (newPromt == null || newPromt == currentPromt) return;

        isOn = true;
        currentPromt = newPromt;

        textPromt.gameObject.SetActive(true);
        textPromt.text = currentPromt.promtToDisplay;
    }

    public void TurnOffPromt()
    {
        if (!isOn) return;
        
        isOn = false;
        currentPromt = null;

        textPromt.text = "";
        textPromt.gameObject.SetActive(false);
    }
    public void UpdateTooltipDelayed(string promtName)
    {
        StartCoroutine(Routine());
        IEnumerator Routine()
        {
            yield return new WaitForSeconds(.2f);
            UpdateTooltip(promtName);
        }
    }
}
[System.Serializable]
public class Promt
{
    public string promtName;
    [TextArea] public string promtToDisplay;
}
