using UnityEngine;

namespace PathCreation.Kendir
{
    using ControlMode = BezierPath.ControlMode;
    [RequireComponent(typeof(PathCreator))]

    public class PathCreatorFromTransforms : MonoBehaviour
    {
        [SerializeField] private Transform[] transforms;
        [SerializeField] private bool isClosed = false;
        [SerializeField] private PathSpace space = PathSpace.xyz;
        [SerializeField] private ControlMode controlMode = ControlMode.Automatic;
        [SerializeField, Range(0.01f, 1)] private float controlSpacing = 0.01f;
        [SerializeField, Range(0, 360)] private float globalAngle = 90;
        [HideInInspector] public PathCreator creator;

        private BezierPath BezierPath
        {
            get => creator.bezierPath;
            set => creator.bezierPath = value;
        }

        private void OnValidate()
        {
            creator = GetComponent<PathCreator>();
            UpdatePath();
        }

        [ContextMenu("UdpadePath")]
        public void UpdatePath()
        {
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            if (transforms.Length > 1)
            {
                BezierPath = new(transforms, isClosed, space);

                if (controlMode == ControlMode.Automatic)
                { BezierPath.AutoControlLength = controlSpacing; }
                BezierPath.ControlPointMode = controlMode;
                BezierPath.GlobalNormalsAngle = globalAngle;
            }
        }
    }
}