using CoreSystems.Extensions.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PipeRotater : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private List<Image> images = new();

    [SerializeField] private Color32 originalColour;
    [SerializeField] private Color32 hoveredColour;
    [Header("Debug:")]
    [SerializeField, Disable] bool canRotate;


    private void Start()
    {
        SetImagesActive(true);
        SetRaycast(true);
    }

    private void RotatePipe()
    {
        transform.Rotate(Vector3.forward, 90);
    }

    public void SetImagesActive(bool activeness)
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].gameObject.SetActive(activeness);
        }

        SetRaycast(activeness);
    }

    public void SetRaycast(bool activeness)
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].raycastTarget = activeness;
        }
    }

    public int GetAngle()
    {
        int angle = 0;

        Vector3 angles = transform.rotation.eulerAngles;
        angle = Convert.ToInt32(angles.z);

        return angle;
    }

    /// Color changing stuff

    private void ChangeToHoveredColour()
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].color = hoveredColour;
        }
    }

    private void ChangeToOriginalColour()
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].color = originalColour;
        }
    }

    /// Interações
    public void OnPointerClick(PointerEventData eventData)
    {
        if (canRotate)
        {
            RotatePipe();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canRotate)
        {
            ChangeToHoveredColour();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (canRotate)
        {
            ChangeToOriginalColour();
        }
    }

    public bool GetCanRotate()
    {
        return canRotate;
    }

    public void SetCanRotate(bool decision)
    {
        canRotate = decision;
    }
}