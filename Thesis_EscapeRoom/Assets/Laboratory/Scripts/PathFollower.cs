using UnityEngine;

namespace PathCreation.Kendir
{
    public class PathFollower : MonoBehaviour
    {
        [SerializeField] private PathCreator pathCreator = null;
        [SerializeField] private EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Stop;
        [SerializeField] private float speed = 1;
        float distanceTravelled;

        public void FollowPath(PathCreatorFromTransforms newPath)
        {
            FollowPath(newPath.creator);
        }

        public void FollowPath(PathCreator newPath)
        {
            distanceTravelled = 0;

            enabled = true;
            if (pathCreator != null)
            {
                pathCreator.pathUpdated -= OnPathChanged;
            }

            pathCreator = newPath;
            pathCreator.pathUpdated += OnPathChanged;
        }

        private void Update()
        {
            if (pathCreator != null)
            {
                distanceTravelled += speed * Time.deltaTime;
                transform.SetPositionAndRotation
                (
                    pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction),
                    pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction)
                );
            }
        }

        private void OnPathChanged()
        {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}