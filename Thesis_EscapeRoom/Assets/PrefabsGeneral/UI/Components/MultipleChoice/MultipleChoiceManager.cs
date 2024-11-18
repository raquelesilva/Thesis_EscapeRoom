using CoreSystems.Extensions.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IGameMech.MultipleChoice
{
    public class MultipleChoiceManager : IGameMech
    {
        [Space(10)]
        [HorizontalLine]
        [Header("Game - for debugging")]
        [Space(10)]
        [ShowOnly] public List<MultipleChoice> toggles;
        [ShowOnly] public List<BoolOption> answers = new();

        [ShowOnly] public SwapPair normalImg;
        [ShowOnly] public SwapPair hoverImg;
        [ShowOnly] public SwapPair selectedImg;

        [ShowOnly] public Color32 normalTxt = Color.black;
        [ShowOnly] public Color32 hoverTxt = Color.black;
        [ShowOnly] public Color32 selectedTxt = Color.white;

        private readonly string abc = "ABCDEFGHIJKLMOPQRSTUVWXYZ";

        private byte flag = 0;

        private void OnValidate()
        {
            bool cs = checkSelf();
            bool ce = checkEach();

            if (!cs && !ce)
            {
                flag = 0;
            }
            else if (cs && !ce)
            {
                flag = 1;
            }
            else if (!cs && ce)
            {
                flag = 2;
            }
            else
            {
                if (flag == 1)
                {
                    uncheckSelf();
                    flag = 2;
                }
                else
                {
                    uncheckEach();
                    flag = 1;
                }
            }
        }

        private void Awake()
        {
            answers = answers.OrderBy(item => new System.Random().Next()).ToList();

            for (int i = 0; i < toggles.Count; i++)
            {
                string slot = $"<font=IBMPlexSans-BoldSDF>({abc[i]})</font> ";
                string text = slot + answers[i].text;
                toggles[i].Setup(this, text, answers[i].isTrue);
            }
        }

        public void Toggle()
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                toggles[i].Toggle();
            }
        }

        public void AutoCheck(MultipleChoice toggle)
        {
            if (checkSelf())
            {
                CheckAnswers();
            }

            if (checkEach())
            {
                if (toggle.CheckAnswer())
                {
                    onRightEach();
                }
                else
                {
                    onWrongEach();
                }
            }
        }

        [ContextMenu("CheckAnswers")]
        public override void CheckAnswers()
        {
            bool isCorrect = true;
            for (int i = 0; i < toggles.Count; i++)
            {
                if (!toggles[i].CheckAnswer())
                {
                    isCorrect = false;
                }
            }

            done = (byte)(isCorrect ? 1 : 0);
            base.CheckAnswers();
        }
    }
}