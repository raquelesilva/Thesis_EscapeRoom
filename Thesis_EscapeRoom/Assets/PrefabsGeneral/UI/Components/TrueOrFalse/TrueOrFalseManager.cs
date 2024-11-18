using CoreSystems.Extensions.Attributes;
using IGameMech.Dropdown;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace IGameMech.TrueOrFalse
{
    public class TrueOrFalseManager : IGameMech
    {
        [Space(10)]
        [HorizontalLine]
        [Header("Game - for debugging")]
        [Space(10)]
        [ShowOnly] public List<TrueFalsePair> slots;

        [ShowOnly] public string trueChar;
        [ShowOnly] public string falseChar;

        [ShowOnly] public SwapPair normalImg;
        [ShowOnly] public SwapPair hoverImg;
        [ShowOnly] public SwapPair selectedImg;

        [ShowOnly] public Color32 normalTxt = Color.black;
        [ShowOnly] public Color32 hoverTxt = Color.black;
        [ShowOnly] public Color32 selectedTxt = Color.white;

        void Start()
        {
            slots = slots.OrderBy(item => new System.Random().Next()).ToList();
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].transform.SetAsLastSibling();
            }
        }

        [ContextMenu("CheckAnswers")]
        public override void CheckAnswers()
        {
            if (IsComplete())
            {
                bool isCorrect = true;
                for (int i = 0; i < slots.Count; i++)
                {
                    if (!slots[i].CheckAnswer())
                    {
                        isCorrect = false;
                    }
                }

                done = (byte)(isCorrect ? 1 : 0);
            }
            else
            {
                done = 2;
            }

            base.CheckAnswers();
        }

        public void AutoCheck(TrueOrFalseSlot slot)
        {
            if (checkSelf())
            {
                CheckAnswers();
            }

            if (checkEach())
            {
                TrueFalsePair pair = slots.Find(p => p._true == slot ||  p._false == slot);
                if (pair.CheckAnswer())
                {
                    onRightEach();
                }
                else
                {
                    onWrongEach();
                }
            }
        }

        private bool IsComplete()
        {
            if (slots.Exists(s => !s._true.IsOn() && !s._false.IsOn()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [Serializable]
    public class TrueFalsePair
    {
        public Transform transform;

        private TextMeshProUGUI _tmp;

        public TrueOrFalseSlot _true;
        public TrueOrFalseSlot _false;
        [SerializeField] private bool isTrue;

        public TrueFalsePair(Transform pair, TrueOrFalseManager _tfm, BoolOption option)
        {
            transform = pair;
            _tmp = transform.GetComponentInChildren<TextMeshProUGUI>();
            _tmp.text = option.text;
            isTrue = option.isTrue;
            List<TrueOrFalseSlot> slots = transform.GetComponentsInChildren<TrueOrFalseSlot>().ToList();
            _true = slots[0];
            _true.Setup(_tfm, _tfm.trueChar);
            _false = slots[1];
            _false.Setup(_tfm, _tfm.falseChar);
        }

        public bool CheckAnswer()
        {
            if (isTrue)
            {
                return _true.IsOn();
            }
            else
            {
                return _false.IsOn();
            }
        }
    }
}