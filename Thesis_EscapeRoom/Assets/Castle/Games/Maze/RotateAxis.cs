using UnityEngine;

namespace CoreSystems.Tools
{
    public class RotateAxis : MonoBehaviour
    {
        public Axis axis;

        private Camera mainCam;
        private RotateTool tool;
        private Transform target;
        private BoxCollider plane;
        private float baseAngle = 0f;

        private int layerMask = -1;

        public void Start()
        {
            plane = GetComponentInChildren<BoxCollider>();
            tool = GetComponentInParent<RotateTool>();
            if (tool.mainCam != null)
            {
                mainCam = tool.mainCam;
            }
            else
            {
                mainCam = Camera.main;
            }
            target = tool.target;

            int layer = LayerMask.NameToLayer("RotationAxis");
            layerMask = 1 << layer;

            switch (axis)
            {
                case Axis.x:
                    tool.xAxis = gameObject;
                    break;
                case Axis.y:
                    tool.yAxis = gameObject;
                    break;
                case Axis.z:
                    tool.zAxis = gameObject;
                    break;
            }
        }

        private void Update()
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitData, Mathf.Infinity, layerMask))
            {
                Debug.DrawLine(mainCam.transform.position, hitData.point, Color.red);
            }
        }

        private void OnMouseDown()
        {
            tool.Lock(axis);
            plane.enabled = true;

            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitData, Mathf.Infinity, layerMask))
            {
                Vector3 pos = transform.InverseTransformPoint(hitData.point);
                switch (axis)
                {
                    case Axis.x:
                        baseAngle = Mathf.Atan2(pos.z, pos.y) * Mathf.Rad2Deg;
                        break;

                    case Axis.y:
                        baseAngle = Mathf.Atan2(pos.x, pos.z) * Mathf.Rad2Deg;
                        break;

                    case Axis.z:
                        baseAngle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
                        break;
                }
            }
        }

        private void OnMouseDrag()
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitData, Mathf.Infinity, layerMask))
            {
                Vector3 pos = transform.InverseTransformPoint(hitData.point);

                Vector3 around = Vector3.zero;
                float angle = 0;

                switch (axis)
                {
                    case Axis.x:
                        angle = Mathf.Atan2(pos.z, pos.y) * Mathf.Rad2Deg;
                        around = Vector3.right;
                        break;

                    case Axis.y:
                        angle = Mathf.Atan2(pos.x, pos.z) * Mathf.Rad2Deg;
                        around = Vector3.up;
                        break;

                    case Axis.z:
                        angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
                        around = Vector3.forward;
                        break;
                }

                float degrees = angle - baseAngle;

                target.Rotate(degrees * around);

                baseAngle = angle;
            }
        }

        private void OnMouseUp()
        {
            plane.enabled = false;
            tool.Unlock();
        }
    }
}