using CoreSystems.Extensions.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IGameMech.MultipleChoice
{
    public class MultipleChoice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Toggle _toggle;
        private Image _img;
        private TextMeshProUGUI _tmp;
        private MultipleChoiceManager _mcm;

        [ShowOnly] public bool isCorrect = false;

        private void Start()
        {
            Toggle();
        }

        public void Setup(MultipleChoiceManager manager, string text, bool isCorrect)
        {
            _toggle = GetComponent<Toggle>();
            _mcm = manager;
            _img = GetComponent<Image>();
            _tmp = GetComponentInChildren<TextMeshProUGUI>();
            _tmp.text = text;
            this.isCorrect = isCorrect;
        }

        public void Toggle()
        {
            if (_toggle.isOn)
            {
                _tmp.color = _mcm.selectedTxt;
                _mcm.selectedImg.SwapValues(_img);
            }
            else
            {
                _tmp.color = _mcm.normalTxt;
                _mcm.normalImg.SwapValues(_img);
            }
        }

        public void AutoCheck()
        {
            _mcm.AutoCheck(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_toggle.isOn)
            {
                _tmp.color = _mcm.hoverTxt;
                _mcm.hoverImg.SwapValues(_img);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_toggle.isOn)
            {
                _tmp.color = _mcm.normalTxt;
                _mcm.normalImg.SwapValues(_img);
            }
        }

        public bool CheckAnswer()
        {
            return _toggle.isOn == isCorrect;
        }
    }
}