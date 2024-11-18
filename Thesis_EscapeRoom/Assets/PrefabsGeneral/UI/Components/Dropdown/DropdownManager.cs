using CoreSystems.Extensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace IGameMech.Dropdown
{
    [Serializable]
    public class DropdownAnswersPair
    {
        public DropdownAnswersPair(TMP_Dropdown drop, List<string> answers)
        {
            this.drop = drop;
            this.answers = answers;
        }

        public TMP_Dropdown drop = null;
        public List<string> answers = new();

        public void Shuffle()
        {
            List<string> options = new();
            for (int i = 0; i < drop.options.Count; i++)
            {
                options.Add(drop.options[i].text);
            }
            options = options.OrderBy(item => new System.Random().Next()).ToList();
            options.Insert(0, string.Empty);
            drop.ClearOptions();
            drop.AddOptions(options);            
        }

        public bool CheckAnswer()
        {
            return answers.Contains(drop.captionText.text);
        }
    }
    public class DropdownManager : IGameMech
    {
        [Space(10)]
        [HorizontalLine]
        [Header("Game - for debugging")]
        [Space(10)]
        [ShowOnly] public List<DropdownAnswersPair> pairs = new();

        private void Awake()
        {
            for (int i = 0; i < pairs.Count; i++)
            {
                DropdownAnswersPair pair = pairs[i];
                pair.drop.onValueChanged.AddListener(_ => AutoCheck(pair));
            }

            for (int i = 0; i < pairs.Count; i++)
            {
                pairs[i].Shuffle();
            }
        }

        public void AutoCheck(DropdownAnswersPair pair)
        {
            if (checkSelf())
            {
                CheckAnswers();
            }

            if (checkEach())
            {
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

        [ContextMenu("CheckAnswers")]
        public override void CheckAnswers()
        {
            if (pairs.Exists(p => p.drop.value == 0))
            {
                done = 2;
            }
            else
            {
                bool isCorrect = true;
                for (int i = 0; i < pairs.Count; i++)
                {
                    if (!pairs[i].CheckAnswer())
                    {
                        isCorrect = false;
                    }
                }

                done = (byte)(isCorrect ? 1 : 0);
            }

            base.CheckAnswers();
        }
    }
}