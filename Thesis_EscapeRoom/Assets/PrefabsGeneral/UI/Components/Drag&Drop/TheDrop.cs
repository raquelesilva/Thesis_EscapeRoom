using CoreSystems.Extensions.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IGameMech.Drag_n_Drop
{
    public class TheDrop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, ShowOnly] private DragManager _dm;
        [SerializeField, ShowOnly] private CanvasGroup _canvasGroup;

        [SerializeField, ShowOnly] private List<TheDrag> correctAnswers = new();

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_dm.isDragging) _canvasGroup.alpha = 0.5f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_dm.isDragging) _canvasGroup.alpha = 1f;
        }

        public TheDrop SetDrop(Transform newDrag)
        {
            _canvasGroup.alpha = 1f;

            if (correctAnswers.Count == 1 && transform.childCount > 0)
                GetComponentInChildren<TheDrag>().UnsetCurrentDrop();

            newDrag.SetParent(transform);

            if (_dm.checkSelf())
            {
                _dm.CheckAnswers();
            }

            if (_dm.checkEach())
            {
                CheckAnswers(true);
            }

            return this;
        }

        public bool CheckAnswers(bool individual = false)
        {
            bool isCorrect = true;
            List<TheDrag> givenAnswers = GetComponentsInChildren<TheDrag>().ToList();
            for (int i = 0; i < givenAnswers.Count; i++)
            {
                TheDrag g = givenAnswers[i];
                bool check = !correctAnswers.Contains(g);
                if (check)
                {
                    isCorrect = false;
                }

                if (individual)
                {
                    if (check)
                    {
                        _dm.onWrongEach();
                        g.UnsetCurrentDrop();
                    }
                    else
                    {
                        _dm.onRightEach();
                    }
                }
            }
            return isCorrect;
        }

        public void SetManager(DragManager dm)
        {
            _dm = dm;
        }

        public void SetAnswers(List<TheDrag> answers)
        {
            correctAnswers = answers;
        }

        public List<TheDrag> GetAnswers()
        {
            return correctAnswers;
        }
    }
}