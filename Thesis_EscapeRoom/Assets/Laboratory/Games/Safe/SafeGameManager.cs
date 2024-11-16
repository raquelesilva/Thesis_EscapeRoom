using CoreSystems.Extensions.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SafeGame
{
    public class SafeGameManager : MonoBehaviour
    {
        [SerializeField] private Animator door;
        [SerializeField] private Transform tip;
        [SerializeField] private LockRotate lockTool;

        [SerializeField, Disable] bool focusedInGame;
        [SerializeField] private UnityEvent onDefocus;

        [SerializeField] private List<Transform> electrons = new();

        [SerializeField] private List<MeshRenderer> lights = new();
        [SerializeField] private List<Material> colors = new();

        [SerializeField] private List<Transform> correct = new();
        [SerializeField] private List<Transform> answers = new();

        public static SafeGameManager instance;

        private void Awake()
        {
            instance = this;
        }

        public void SetIsFocus(bool isPlaying)
        {
            focusedInGame = isPlaying;
        }

        private void Update()
        {
            if (focusedInGame && Input.GetKeyDown(KeyCode.Escape))
            {
                SetIsFocus(false);
                onDefocus?.Invoke();
            }
        }

        public void GetClosestElectron()
        {
            Transform closest = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 tipPosition = tip.position;
            for (int i = 0; i < electrons.Count; i++)
            {
                Vector3 directionToTarget = electrons[i].position - tipPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closest = electrons[i];
                }
            }

            CheckAnswer(closest);
        }

        private void CheckAnswer(Transform electron)
        {
            answers.Add(electron);
            if (answers.Count < correct.Count)
            {
                for (int i = 0; i < answers.Count; i++)
                {
                    lights[i].material = colors[electrons.IndexOf(answers[i])];
                }
            }
            else
            {
                lights[^1].material = colors[electrons.IndexOf(answers[^1])];
                for (int i = 0; i < answers.Count; i++)
                {
                    if (answers[i] != correct[i])
                    {
                        Wrong();
                        return;
                    }
                }
                Right();
            }
        }

        private void Wrong()
        {
            for (int i = 0; i < lights.Count; i++)
            {
                lights[i].material = colors[^1];
            }
            answers.Clear();
        }

        private void Right()
        {
            //for (int i = 0; i < lights.Count; i++)
            //{
            //    lights[i].material = colors[2];
            //}
            door.enabled = true;
            lockTool.canInteract = false;
        }
    }
}