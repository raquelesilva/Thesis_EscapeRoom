using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity.FantasyKingdom
{
    public class DraggableItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private GameObject target; // Reference to the target piece
        [SerializeField] private float positionThreshold = 0.05f; // Max distance allowed for placement
        [SerializeField] private float rotationThreshold = 3f; // Max rotation difference allowed for placement (degrees) 

        [SerializeField]
        private PuzzleLogic puzzleLogic;

        private bool isCloseToTarget = false; // Checks if position and rotation are aligned

        private Vector3 _offset; // Offset between mouse and object
        private Camera _mainCamera; // Cached main camera for better 

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                Debug.LogError("Main Camera is missing or not tagged as 'MainCamera'!");
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {

            // Calculate the offset between the object's position and the mouse position
            _offset = transform.position - GetMouseWorldPosition(eventData.position);
            HideCursor();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            DeHideCursor();
            if (target != null)
            {
                // Check if the draggable piece is close to the target
                CheckAlignment();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // Calculate the initial offset between the object's position and the world position of the mouse
            _offset = transform.position - GetMouseWorldPosition(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Get the world position of the mouse
            Vector3 newPosition = GetMouseWorldPosition(eventData.position) + _offset;

            // Lock movement to world axis
            newPosition.y = transform.position.y; // For XZ plane; adjust for other planes if necessary

            UpdateTransformPosition(newPosition);
        }

        private Vector3 GetMouseWorldPosition(Vector2 screenPosition)
        {
            if (_mainCamera == null) return Vector3.zero;

            // Use a fixed distance (z) from the camera to maintain consistent world space mapping
            Plane dragPlane = new Plane(Vector3.up, transform.position); // Drag along the XZ plane (adjust normal for other planes)
            Ray ray = _mainCamera.ScreenPointToRay(screenPosition);

            if (dragPlane.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }

            return transform.position; // Fallback to current position if raycast fails
        }

        private void UpdateTransformPosition(Vector3 position)
        {
            transform.position = position;
        }

        private void HideCursor()
        {
            UnityEngine.Cursor.visible = false;
        }

        private void DeHideCursor()
        {
            UnityEngine.Cursor.visible = true;
        }

        private void CheckAlignment()
        {
            // Check position proximity
            float distance = Vector3.Distance(transform.position, target.transform.position);
            bool positionAligned = distance <= positionThreshold;
            

            // Check rotation proximity
            float rotationDifference = Quaternion.Angle(transform.rotation, target.transform.rotation);
            bool rotationAligned = rotationDifference <= rotationThreshold;

            Debug.Log("position: "+ distance + positionAligned);
            Debug.Log("rotation: "+ rotationDifference + rotationAligned);
            // Combine both checks
            isCloseToTarget = positionAligned && rotationAligned;

            // Optional: Snap the piece into place when aligned
            if (isCloseToTarget) // On mouse release
            {
                
                SnapToTarget();
            }
        }

        private void SnapToTarget()
        {
            // Snap position and rotation to match the target
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;

            target.GetComponent<MeshRenderer>().material = this.GetComponent<MeshRenderer>().material;
            this.GetComponent<MeshRenderer>().enabled = false;

            puzzleLogic.PieceAligned();

            enabled = false; // Or call a custom function to disable drag functionality
        }
    }
}