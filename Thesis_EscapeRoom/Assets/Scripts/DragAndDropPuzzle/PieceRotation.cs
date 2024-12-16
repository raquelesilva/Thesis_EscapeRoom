using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity.FantasyKingdom
{
    public class PieceRotation : MonoBehaviour, IDragHandler
    {
        [Header("Rotation Settings")]
        public float rotationSpeed = 5f; // Speed of rotation

        private void Awake()
        {
            if (Camera.main == null)
            {
                Debug.LogError("Main Camera is missing or not tagged as 'MainCamera'!");
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Calculate rotation based on mouse drag
            float rotationDelta = eventData.delta.x * rotationSpeed * Time.deltaTime;

            // Apply rotation around the object's own Y-axis
            transform.Rotate(Vector3.forward, -rotationDelta, Space.Self);
        }
    }
}
