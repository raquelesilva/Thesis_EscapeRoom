using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Unity.FantasyKingdom
{
    public class PuzzleLogic : MonoBehaviour
    {
        [Header("Focus Settings")]
        public Transform puzzleParent; // Parent transform of the puzzle pieces
        public Transform cameraTransform; // Reference to the camera
        public float moveSpeed = 2f; // Speed at which the puzzle moves
        public float focusOffset = 1.5f; // Offset distance from the camera to the focus point

        [Header("Blur Settings")]
        public Volume postProcessingVolume; // Post-processing volume for the blur effect
        public float blurIntensity = 10f; // Intensity of the blur
        public float blurTransitionSpeed = 1f; // Speed of blur effect transitions

        public  int numPieces = 4; // Number of puzzle piec  es
        private int _piecesAligned = 0; // Number of aligned pieces

        private Vector3 _puzzleInitialPosition;
        private DepthOfField _depthOfField;

        private void Start()
        {
            // Initialize post-processing effects
            if (postProcessingVolume != null && postProcessingVolume.profile.TryGet(out _depthOfField))
            {
                _depthOfField.active = true;
                _depthOfField.focusDistance.value = 10f;
                _depthOfField.aperture.value = blurIntensity;
            }

            _puzzleInitialPosition = puzzleParent.position;

            // Move the puzzle parent to the calculated focus point
            StartFocus();
        }

        private void Update()
        {
            // Smoothly reduce blur intensity over time
            if (_depthOfField != null)
            {
                _depthOfField.aperture.value = Mathf.Lerp(_depthOfField.aperture.value, 0f, Time.deltaTime * blurTransitionSpeed);
            }
            

        }

        private void StartFocus()
        {
            if (puzzleParent != null && cameraTransform != null)
            {
                Vector3 focusPoint = CalculateFocusPoint();
                StartCoroutine(MoveToFocusPoint(puzzleParent, focusPoint));
            }
        }

        private Vector3 CalculateFocusPoint()
        {
            // Calculate a point in front of the camera using its forward vector and offset
            return cameraTransform.position + cameraTransform.forward * focusOffset;
        }

        private System.Collections.IEnumerator MoveToFocusPoint(Transform puzzle, Vector3 focusPoint)
        {
            while (Vector3.Distance(puzzle.position, focusPoint) > 0.01f)
            {
                puzzle.position = Vector3.Lerp(puzzle.position, focusPoint, Time.deltaTime * moveSpeed);
                yield return null;
            }
        }

        public void PieceAligned()
        {
            _piecesAligned++;
            Debug.Log("Piece aligned: " + _piecesAligned);

            if (_piecesAligned == numPieces)
            {
                // All pieces are aligned, stop the blur effect
                if (_depthOfField != null)
                {
                    _depthOfField.active = false;
                }
                EndPuzzle();
            }
        }

        private void EndPuzzle()
        {
            Debug.Log("Puzzle completed!");
            StartCoroutine(MoveToFocusPoint(puzzleParent, _puzzleInitialPosition));
        }
    }
}