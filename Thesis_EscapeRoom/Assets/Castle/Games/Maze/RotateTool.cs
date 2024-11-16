using CoreSystems.Extensions.Attributes;
using UnityEngine;

namespace CoreSystems.Tools
{
    public enum Axis { x, y, z, xn }

    public class RotateTool : MonoBehaviour
    {
        public Transform target;
        [Range(1, 360)] public int snapdegrees;
        public Vector3 distanceFrom0 = Vector3.zero;
        public Camera mainCam;

        [ShowOnly] public GameObject xAxis, yAxis, zAxis;

        private void OnValidate()
        {
            if (mainCam == null)
            {
                mainCam = Camera.main;
            }
        }

        public void Lock(Axis axis)
        {
            switch (axis)
            {
                case Axis.x:
                    if (yAxis != null) yAxis.SetActive(false);
                    if (zAxis != null) zAxis.SetActive(false);
                    break;

                case Axis.y:
                    if (xAxis != null) xAxis.SetActive(false);
                    if (zAxis != null) zAxis.SetActive(false);
                    break;

                case Axis.z:
                    if (xAxis != null) xAxis.SetActive(false);
                    if (yAxis != null) yAxis.SetActive(false);
                    break;
            }
        }

        public void Unlock()
        {
            transform.localEulerAngles = target.localEulerAngles;
            if (xAxis != null) xAxis.SetActive(true);
            if (yAxis != null) yAxis.SetActive(true);
            if (zAxis != null) zAxis.SetActive(true);

            if (ValveManager.instance != null)
                ValveManager.instance.CheckValveRotations();
        }
    }
}