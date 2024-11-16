using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Games.WiresGame
{
    public class WireScript : MonoBehaviour
    {
        [SerializeField] private bool blocked;
        [SerializeField] private LayerMask wirePlaneLayer;
        [HideInInspector] public List<GameObject> connectors;
        [HideInInspector] public List<WireTipScript> tips;

        private WiresGameManager manager;

        private void Start()
        {
            manager = WiresGameManager.instance;
        }

        public void Update ()
        {
            if (blocked) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitData, Mathf.Infinity, wirePlaneLayer))
            {
                DrawLine(transform.position, hitData.point);
            }
        }

        public void DrawLine(Vector3 start, Vector3 end) //This is where I draw the line
        {
            Vector3 dir = start - end;
            transform.position = start;
            transform.right = dir;
            float lengthOffset = transform.parent.localScale.x;
            float length = Rounder(dir.magnitude * 1590, 2) / lengthOffset;
            transform.localScale = new Vector3(length, 1, 1);
        }

        public float Rounder(float value, int numDecimalSpaces)
        {
            int x10 = (int)Mathf.Pow(10, numDecimalSpaces);
            return Mathf.Round(value * x10) / x10;
        }

        public void SetIsBlock(bool decision)
        {
            blocked = decision;
        }

        private void OnMouseDown()
        {
            if (manager.UsingPliers())
            {
                manager.GetPliers().AnimatePlier();
                manager.DestroyWire(this);
            }

        }
    }

}
