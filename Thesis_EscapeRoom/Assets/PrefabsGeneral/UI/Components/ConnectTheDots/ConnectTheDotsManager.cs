using CoreSystems.Extensions.Attributes;
using CoreSystems.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace IGameMech
{
    public class ConnectTheDotsManager : IGameMech
    {
        [Space(10)]
        [HorizontalLine]
        [Header("Game - for debugging")]
        [Space(10)]

        public Transform startsParent;
        public Transform endsParent;

        private List<ConnectTheDots> startPoints = new();
        [HideInInspector]public List<Image> endPoints = new();

        public GameObject linePrefab;

        [ShowOnly] public Canvas canvas;

        void Start()
        {
            startPoints = startsParent.GetComponentsInChildren<ConnectTheDots>().ToList();
            endPoints = endsParent.GetComponentsInChildren<Image>().ToList();

            startPoints = startPoints.OrderBy(item => new System.Random().Next()).ToList();
            for (int i = 0; i < startPoints.Count; i++)
            {
                startPoints[i].transform.SetAsLastSibling();
            }

            endPoints = endPoints.OrderBy(item => new System.Random().Next()).ToList();
            for (int i = 0; i < endPoints.Count; i++)
            {
                endPoints[i].transform.SetAsLastSibling();
            }

            canvas = GetComponentInParent<Canvas>();
            for (int i = 0; i < startPoints.Count; i++)
            {
                startPoints[i].Init(this);
            }
        }

        public bool IsEnd(Image hover)
        {
            return endPoints.Contains(hover);
        }

        [ContextMenu("CheckAnswers")]
        public override void CheckAnswers()
        {
            if (startPoints.Exists(p => p.NotDone()))
            {
                done = 2;
            }
            else
            {
                bool isCorrect = true;
                for (int i = 0; i < startPoints.Count; i++)
                {
                    if (!startPoints[i].CheckAnswer())
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