using CoreSystems.Extensions.Attributes;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IGameMech.Drag_n_Drop
{
    public class TheDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField, ShowOnly] private DragManager _dm;

        [SerializeField, ShowOnly] private Canvas _canvas;
        [SerializeField, ShowOnly] private RectTransform _thisRect;
        [SerializeField, ShowOnly] private CanvasGroup _canvasGroup;

        [SerializeField, ShowOnly] private TheDrop currentDrop = null;

        [SerializeField, ShowOnly] private Sprite img = null;
        [SerializeField, ShowOnly] private string txt = string.Empty;

        private void Start()
        {
            _canvas = GetComponentInParent<Canvas>();
            _thisRect = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _thisRect.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0.5f;
            _dm.isDragging = true;

            currentDrop = null;
            transform.SetParent(_dm.dragsArea);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1f;
            _dm.isDragging = false;

            if (eventData.pointerEnter)
            {
                TheDrop newDrop = eventData.pointerEnter.GetComponent<TheDrop>();

                TheDrag opp = eventData.pointerEnter.GetComponentInParent<TheDrag>();
                if (opp != null)
                {
                    newDrop = opp.GetCurrentDrop();
                }

                if (newDrop != null)
                {
                    currentDrop = newDrop.SetDrop(transform);
                }
                else
                {
                    UnsetCurrentDrop();
                }
            }
            else
            {
                UnsetCurrentDrop();
            }
        }

        public TheDrop GetCurrentDrop()
        {
            return currentDrop;
        }

        public void UnsetCurrentDrop()
        {
            currentDrop = null;
            transform.SetParent(_dm.dragsParent);
        }

        public List<object> GetCurrentValues()
        {
            return new() { img, txt };
        }

        public void SetValues(Sprite newImg, string newTxt)
        {
            img = newImg;
            txt = newTxt;

            Image imgComp = GetComponent<Image>();
            RectTransform imgRect = imgComp.GetComponent<RectTransform>();
            if (img != null)
            {
                imgComp.sprite = img;
                imgComp.SetNativeSize();
                imgComp.type = Image.Type.Simple;
                imgComp.preserveAspect = true;
                float scale = imgRect.sizeDelta.x / 200;
                imgRect.sizeDelta /= scale;
                DestroyImmediate(imgComp.transform.GetChild(0).gameObject);
            }
            else
            {
                TextMeshProUGUI tmpComp = GetComponentInChildren<TextMeshProUGUI>();
                tmpComp.text = txt;

                RectTransform tmpRect = tmpComp.GetComponent<RectTransform>();

                ContentSizeFitter tmpCSF = tmpRect.AddComponent<ContentSizeFitter>();
                tmpCSF.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                tmpCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                LayoutRebuilder.ForceRebuildLayoutImmediate(tmpRect);

                LayoutElement tmpLE = tmpRect.AddComponent<LayoutElement>();
                if (tmpRect.sizeDelta.x > 200f)
                {
                    tmpLE.preferredWidth = 200f;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(tmpRect);
                }
                DestroyImmediate(tmpCSF);
                DestroyImmediate(tmpLE);

                HorizontalLayoutGroup imgHLG = imgRect.AddComponent<HorizontalLayoutGroup>();
                ContentSizeFitter imgCSF = imgRect.AddComponent<ContentSizeFitter>();
                imgCSF.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                imgCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                LayoutRebuilder.ForceRebuildLayoutImmediate(imgRect);

                DestroyImmediate(imgCSF);
                DestroyImmediate(imgHLG);

                tmpRect.anchorMin = Vector2.one * 0.5f;
                tmpRect.anchorMax = Vector2.one * 0.5f;
                tmpRect.localPosition = Vector3.zero;
                //LayoutRebuilder.ForceRebuildLayoutImmediate(tmpRect);
            }
        }

        public void SetManager(DragManager dm)
        {
            _dm = dm;
        }
    }
}