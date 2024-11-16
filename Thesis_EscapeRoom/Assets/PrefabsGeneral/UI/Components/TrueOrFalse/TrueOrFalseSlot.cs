using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IGameMech.TrueOrFalse
{
    public class TrueOrFalseSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Toggle _toggle;
        [SerializeField] private TrueOrFalseManager _tfm;

        [SerializeField] private Image _img;
        [SerializeField] private TextMeshProUGUI _tmp;

        private void Start()
        {
            Toggle();
        }

        public void Setup(TrueOrFalseManager _tfm, string _char)
        {
            _toggle = GetComponent<Toggle>();

            this._tfm = _tfm;

            _img = _toggle.GetComponent<Image>();

            _tmp = _toggle.GetComponentInChildren<TextMeshProUGUI>();
            _tmp.text = _char;
        }

        public void Toggle()
        {
            if (_toggle.isOn)
            {
                _tmp.color = _tfm.selectedTxt;
                _tfm.selectedImg.SwapValues(_img);
            }
            else
            {
                _tmp.color = _tfm.normalTxt;
                _tfm.normalImg.SwapValues(_img);
            }
        }

        public void AutoCheck()
        {
            _tfm.AutoCheck(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_toggle.isOn)
            {
                _tmp.color = _tfm.hoverTxt;
                _tfm.hoverImg.SwapValues(_img);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_toggle.isOn)
            {
                _tmp.color = _tfm.normalTxt;
                _tfm.normalImg.SwapValues(_img);
            }
        }

        public bool IsOn()
        {
            return _toggle.isOn;
        }
    }
}