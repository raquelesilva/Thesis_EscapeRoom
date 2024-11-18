using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuanPipeTip : MonoBehaviour
{
    private Collider otherTip;
    private Collider thisCollider;

    private void Start()
    {
        thisCollider = GetComponent<Collider>();
    }
}
