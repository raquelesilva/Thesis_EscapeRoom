using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button3D : MonoBehaviour
{
    [SerializeField] private UnityEvent onMouseDown;

    private void OnMouseDown()
    {
        onMouseDown.Invoke();
    }
}
