using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Unity.FantasyKingdom
{
    public class PuzzleLogic : MonoBehaviour
    {
        [Header("Dialogue")]
        [SerializeField] DialogueTrigger dialogueTrigger;
        [SerializeField] TextAsset poemDialogue;
        [SerializeField] TextAsset assembleDialogue;

        [Header("Objects")]
        [SerializeField] GameObject playerGO;
        [SerializeField] GameObject puzzleCamera;

        [Header("Find Pieces")]
        [SerializeField] int catchedPieces = 0;
        [SerializeField] GameObject findPiecesGO;

        [Header("Focus Settings")]
        [SerializeField] Transform puzzleParent; // Parent transform of the puzzle pieces
        [SerializeField] Transform cameraTransform; // Reference to the camera
        [SerializeField] float moveSpeed = 2f; // Speed at which the puzzle moves
        [SerializeField] float focusOffset = 1.5f; // Offset distance from the camera to the focus point

        [Header("Blur Settings")]
        [SerializeField] Volume postProcessingVolume; // Post-processing volume for the blur effect
        [SerializeField] float blurIntensity = 10f; // Intensity of the blur
        [SerializeField] float blurTransitionSpeed = 1f; // Speed of blur effect transitions

        [SerializeField] int numPieces = 4; // Number of puzzle piec  es
        [SerializeField] int _piecesAligned = 0; // Number of aligned pieces

        [SerializeField] Vector3 _puzzleInitialPosition;
        [SerializeField] DepthOfField _depthOfField;

        private void Awake()
        {
            enabled = true;
            findPiecesGO.SetActive(true);
            puzzleParent.gameObject.SetActive(false);

            if (puzzleCamera == null)
            {
                puzzleCamera = GetComponentInChildren<Camera>(true).gameObject;
            }

            if (playerGO != null)
            {
                playerGO = FirstPersonController.instance.gameObject;
            }
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            // Smoothly reduce blur intensity over time
            if (_depthOfField != null)
            {
                _depthOfField.aperture.value = Mathf.Lerp(_depthOfField.aperture.value, 0f, Time.deltaTime * blurTransitionSpeed);
            }
        }

        public void GetOnePiece(GameObject piece)
        {
            Debug.Log("GetPiece");
            catchedPieces++;
            piece.SetActive(false);

            if (catchedPieces == numPieces)
            {
                findPiecesGO.SetActive(false);
                puzzleParent.gameObject.SetActive(true);
                dialogueTrigger.ChangeDialogue(assembleDialogue);
                Camera.main.gameObject.SetActive(false);
            }
        }

        public void StartPuzzleGame()
        {
            puzzleCamera.SetActive(true);
            playerGO.SetActive(false);

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
            puzzleCamera.SetActive(false);
            playerGO.SetActive(true);
            dialogueTrigger.ChangeDialogue(poemDialogue);
            Camera.main.gameObject.SetActive(true);

            StartCoroutine(MoveToFocusPoint(puzzleParent, _puzzleInitialPosition));
        }
    }
}