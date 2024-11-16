using CoreSystems.Extensions.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IGameMech.Drag_n_Drop
{
    public class DragManager : IGameMech
    {
        [Space(10)]
        [HorizontalLine]
        [Header("Game - for debugging")]
        [Space(10)]
        [ShowOnly] public bool isDragging = false;
        [ShowOnly] public Transform dragsParent;
        [ShowOnly] public Transform dragsArea;

        [ShowOnly] public List<TheDrop> drops = new();
        [ShowOnly] public List<TheDrag> drags = new();

        private void Start()
        {
            List<TheDrag> shuffle = drags.OrderBy(item => new System.Random().Next()).ToList();
            for (int i = 0; i < shuffle.Count; i++)
            {
                shuffle[i].transform.SetAsLastSibling();
            }
        }

        [ContextMenu("CheckAnswers")]
        public override void CheckAnswers()
        {
            if (dragsParent.childCount == 0)
            {
                bool isCorrect = true;
                for (int i = 0; i < drops.Count; i++)
                {
                    if (!drops[i].CheckAnswers())
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
    }
}