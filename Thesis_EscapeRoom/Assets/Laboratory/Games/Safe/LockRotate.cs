using UnityEngine;

namespace SafeGame
{
    public class LockRotate : MonoBehaviour
    {
        public bool canInteract;

        [SerializeField] private Transform target;

        [SerializeField] private float baseAngle = 0f;

        [SerializeField] private Camera mainCam;
        private BoxCollider plane;
        private int layerMask = -1;

        public void Start()
        {
            if (mainCam == null)
            {
                mainCam = Camera.main;
            }
            plane = GetComponentInChildren<BoxCollider>();

            int layer = LayerMask.NameToLayer("RotationAxis");
            layerMask = 1 << layer;
        }

        private void OnMouseDown()
        {
            if (!canInteract) return;

            plane.enabled = true;

            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitData, 2f, layerMask))
            {
                Vector3 pos = transform.InverseTransformPoint(hitData.point);
                baseAngle = Mathf.Atan2(pos.y, pos.x);
                baseAngle *= Mathf.Rad2Deg;
                if (baseAngle < 0) baseAngle = 360 + baseAngle;
            }
        }

        private void OnMouseDrag()
        {
            if (!canInteract) return;

            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitData, 2f, layerMask))
            {
                Vector3 pos = transform.InverseTransformPoint(hitData.point);
                float angle = Mathf.Atan2(pos.y, pos.x);

                angle *= Mathf.Rad2Deg;
                if (angle < 0) angle = 360 + angle;
                float degrees = angle - baseAngle;

                target.Rotate(degrees * Vector3.forward);

                baseAngle = angle;
            }
        }

        private void OnMouseUp()
        {
            if (!canInteract) return;

            plane.enabled = false;
            SafeGameManager.instance.GetClosestElectron();
        }
    }
}