using CoreSystems.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ValveManager : MonoBehaviour
{
    [SerializeField] private List<ValveRotate> valves = new();
    [SerializeField] private List<Transform> gauges = new();
    [SerializeField] private List<Transform> slots = new();
    [SerializeField] private UnityEvent winingEvent;
    [Range(1, 360)] public int snapDegrees;
    [Range(1, 1000)] public int distance;
    public static ValveManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gauges = gauges.OrderBy(item => new System.Random().Next()).ToList();
        slots = slots.OrderBy(item => new System.Random().Next()).ToList();

        for (int i = 0; i < valves.Count; i++)
        {
            valves[i].Init();

            int side = Random.Range(0, 2) * 305;
            float rndAngle = Random.Range(0, 46) + side;
            rndAngle -= rndAngle % snapDegrees;
            valves[i].rotated = rndAngle;
            valves[i].Rotate();

            gauges[i].position = slots[i].position;
            gauges[i].rotation = slots[i].rotation;
        }
    }

    public void CheckValveRotations()
    {
        int valvesChecked = 0;
        for (int i = 0; i < valves.Count; i++)
        {
            if (156 < valves[i].rotated && valves[i].rotated < 204)
            {
                valvesChecked++;
            }
        }

        if (valvesChecked == valves.Count)
        {
            WinAnimation();
        }
    }

    public void DeactivateGame()
    {
        foreach (var valve in valves)
        {
            valve.canInteract = false;
        }
        foreach (var item in gauges)
        {
            item.gameObject.SetActive(false);   
        }
    }
    [ContextMenu("Win Game")]
    public void WinAnimation()
    {
        NotificationManager.instance.SetMessage("Completaste o jogo!", NotificationManager.instance.GetColorRight());
        winingEvent.Invoke();
    }
}