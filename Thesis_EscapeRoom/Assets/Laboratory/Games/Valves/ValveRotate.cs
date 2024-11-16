using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace CoreSystems.Tools
{
    public class ValveRotate : MonoBehaviour
    {
        public bool canInteract;

        public Axis axis;

        [SerializeField] private Transform target;
        [HideInInspector] public int snapDegrees = 1;
        [HideInInspector] public int distance = 2;
        [HideInInspector] public float rotated = 0f;

        [SerializeField] private TextMeshProUGUI counter;
        [SerializeField] private Transform pointer;
        [SerializeField] private MeshFilter hint;
        [SerializeField] private UnityEvent onMouseDown;

        private Vector3 otherAxes;
        private Vector3 around = Vector3.zero;
        private int dir = 1;

        private float baseAngle = 0f;
        private float lastDegrees = 0f;

        private Camera mainCam;
        private BoxCollider plane;
        private int layerMask = -1;

        private void OnValidate()
        {
            if (target != null)
            {
                Mesh mesh = target.GetComponent<MeshFilter>().sharedMesh;

                MeshCollider[] colliders = GetComponents<MeshCollider>();
                colliders[0].sharedMesh = mesh;
                colliders[1].sharedMesh = mesh;

                hint.sharedMesh = mesh;
            }
        }

        public void Init()
        {
            mainCam = Camera.main;
            plane = GetComponentInChildren<BoxCollider>();
            snapDegrees = ValveManager.instance.snapDegrees;
            distance = ValveManager.instance.distance;
            dir = Random.Range(0, 2) * 2 - 1;

            int layer = LayerMask.NameToLayer("RotationAxis");
            layerMask = 1 << layer;

            otherAxes = target.localEulerAngles;
            switch (axis)
            {
                case Axis.xn:
                    otherAxes.x = 0;
                    around = Vector3.right * dir;
                    break;
                case Axis.z:
                    otherAxes.z = 0;
                    around = Vector3.forward * dir;
                    break;
            }
        }

        private void OnMouseDown()
        {
            if(!canInteract) return;

            onMouseDown?.Invoke();

            plane.enabled = true;

            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitData, distance, layerMask))
            {
                otherAxes = target.localEulerAngles;
                Vector3 pos = transform.InverseTransformPoint(hitData.point);
                switch (axis)
                {
                    case Axis.xn:
                        baseAngle = -Mathf.Atan2(pos.z, pos.y);
                        otherAxes.x = 0;
                        break;

                    case Axis.z:
                        baseAngle = Mathf.Atan2(pos.y, pos.x);
                        otherAxes.z = 0;
                        break;
                }
                baseAngle *= Mathf.Rad2Deg;
                if (baseAngle < 0) baseAngle = 360 + baseAngle;
            }
        }

        private void OnMouseDrag()
        {
            if (!canInteract) return;

            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitData, distance, layerMask))
            {
                Debug.DrawLine(ray.origin, hitData.point);
                float angle = 0;

                Vector3 pos = transform.InverseTransformPoint(hitData.point);
                switch (axis)
                {
                    case Axis.xn:
                        angle = Mathf.Atan2(pos.z, pos.y) * dir;
                        break;

                    case Axis.z:
                        angle = Mathf.Atan2(pos.y, pos.x) * dir;
                        break;
                }

                angle *= Mathf.Rad2Deg;
                if (angle < 0) angle = 360 + angle;
                float degrees = angle - baseAngle;
                degrees -= degrees % snapDegrees;

                if (degrees < 0f) degrees += 360;

                float max = 360 - snapDegrees;
                if (degrees != lastDegrees)
                {
                    if (degrees > lastDegrees)
                    {
                        if (degrees == max && lastDegrees == 0)
                        {
                            rotated -= snapDegrees;
                        }
                        else
                        {
                            rotated += snapDegrees;
                        }
                    }
                    else
                    {
                        if (degrees == 0 && lastDegrees == max)
                        {
                            rotated += snapDegrees;
                        }
                        else
                        {
                            rotated -= snapDegrees;
                        }
                    }
                }

                lastDegrees = degrees;

                Rotate();
            }
        }

        public void Rotate()
        {
            if (!canInteract) return;

            rotated = Mathf.Clamp(rotated, 0f, 360f);
            ///AngleAxis sets the target's rotation to Vectro3.zero and then rotates "degrees" around "around" 
            target.rotation = Quaternion.AngleAxis(rotated, around);
            target.localEulerAngles += otherAxes;
            pointer.localEulerAngles = Vector3.forward * (rotated / 3);
            counter.text = rotated + " psi";
        }

        private void OnMouseUp()
        {
            if (!canInteract) return;

            lastDegrees = 0;
            plane.enabled = false;
            ValveManager.instance.CheckValveRotations();
        }
    }
}