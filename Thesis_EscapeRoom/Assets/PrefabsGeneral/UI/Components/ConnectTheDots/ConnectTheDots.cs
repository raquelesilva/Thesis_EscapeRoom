using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IGameMech
{
    public class ConnectTheDots : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private ConnectTheDotsManager _cdm;
        private Canvas canvas;
        private bool canvas3D = false;

        public Image answer;
        private Image hoverItem = null;

        private GameObject linePrefab;
        private GameObject line = null;

        public void Init(ConnectTheDotsManager manager)
        {
            _cdm = manager;
            canvas = manager.canvas;
            canvas3D = canvas.renderMode == RenderMode.WorldSpace;
            linePrefab = manager.linePrefab;
        }

        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitData))
            {
                Vector3 cameraPos = new(0, 1, -10);
                Debug.DrawLine(cameraPos, hitData.point, Color.red);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (canvas3D)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitData))
                {
                    UpdateLine(hitData.point);
                }
            }
            else
            {
                UpdateLine(eventData.position);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Clear();
            line = Instantiate(linePrefab, transform.position, Quaternion.identity, transform);
            UpdateLine(transform.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerEnter)
            {
                Image hover = eventData.pointerEnter.GetComponent<Image>();

                if (hover != null && _cdm.IsEnd(hover))
                {
                    hoverItem = hover;
                    UpdateLine(hoverItem.transform.position);
                    hoverItem.raycastTarget = false;

                    if (_cdm.checkSelf())
                    {
                        _cdm.CheckAnswers();
                    }

                    if (_cdm.checkEach())
                    {
                        CheckAnswer(true);
                    }
                    return;
                }
            }

            Clear();
        }

        public bool CheckAnswer(bool individual = false)
        {
            bool isCorrect = answer.Equals(hoverItem);
            if (individual)
            {
                if (isCorrect)
                {
                    _cdm.onRightEach();
                }
                else
                {
                    _cdm.onWrongEach();
                    Clear();
                }
            }
            return isCorrect;
        }

        public bool NotDone()
        {
            return line == null;
        }

        public void Clear()
        {
            if (line != null)
            {
                Destroy(line);
                line = null;
            }

            if (hoverItem != null)
            {
                hoverItem.raycastTarget = true;
                hoverItem = null;
            }
        }

        public void UpdateLine(Vector3 position)
        {
            Vector3 direction = position - transform.position;
            line.transform.right = direction;

            RectTransform lineRect = line.GetComponent<RectTransform>();
            float canvasScale = canvas3D ? canvas.transform.localScale.x : canvas.scaleFactor;
            lineRect.sizeDelta = new Vector2(direction.magnitude / canvasScale, 8.5f);
        }
    }
}