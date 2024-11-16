using CoreSystems.Extensions.Attributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IGameMech.InputFields
{
    [Serializable]
    public class InputAnswerPair
    {
        public InputAnswerPair(TMP_InputField field, string answer)
        {
            this.field = field;
            this.answer = answer;
        }

        public TMP_InputField field = null;
        public string answer = string.Empty;

        public bool CheckAnswer()
        {
            return field.text == answer;
        }
    }

    public class InputFieldsManager : IGameMech
    {
        [Space(10)]
        [HorizontalLine]
        [Header("Game - for debugging")]
        [Space(10)]
        [ShowOnly] public List<InputAnswerPair> pairs = new();

        private void Awake()
        {
            for (int i = 0; i < pairs.Count; i++)
            {
                InputAnswerPair pair = pairs[i];
                pair.field.onEndEdit.AddListener(_ => AutoCheck(pair));
            }
        }

        public void AutoCheck(InputAnswerPair pair)
        {
            pair.field.text = pair.field.text.Trim();

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
            if (pairs.Exists(p => p.field.text == string.Empty))
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