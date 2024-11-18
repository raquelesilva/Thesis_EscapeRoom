using CoreSystems.Extensions.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ChemMix.FillBottle;

namespace ChemMix
{
    public class ChemMixManager : MonoBehaviour
    {
        public enum Elements
        {
            Fe, Cu, Al, Pb
        }

        [Serializable]
        private class ElementCount
        {
            public Elements element = Elements.Fe;
            public int count = 0;

            public ElementCount(Elements element, int count)
            {
                this.element = element;
                this.count = count;
            }
        }

        [SerializeField] private List<Liquid> liquids = new();

        [SerializeField] private FillBottle target;
        [SerializeField] private List<FillBottle> sources = new();

        [SerializeField] private List<ElementCount> correct = new();
        [SerializeField] private List<ElementCount> answers = new();

        [SerializeField] private GameObject portal;
        [SerializeField] private ParticleSystem explosion;

        [SerializeField] private Transform secretWall;
        [SerializeField] private UnityEvent onComplete;
        [SerializeField] private UnityEvent onDefocus;

        [SerializeField, Disable] bool focusedInGame;

        [SerializeField] GameObject lastCode;

        public static ChemMixManager instance;

        private void OnValidate()
        {
            target.name = "Target";
            for (int i = 0; i < sources.Count; i++)
            {
                sources[i].name = ((Elements)i).ToString() + "_Source";
            }
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            Restart();
        }

        private void Update()
        {
            if (focusedInGame && Input.GetKeyDown(KeyCode.Escape))
            {
                SetIsFocus(false);
                onDefocus?.Invoke();
            }
        }

        public void SetIsFocus(bool isPlaying)
        {
            focusedInGame = isPlaying;
        }

        public void Answer(Elements element)
        {
            answers[(int)element].count++;
            if (!(target.percentage < 100f))
            {
                for (int i = 0; i < answers.Count; i++)
                {
                    if (answers[i].count != correct[i].count)
                    {
                        explosion.Play();
                        Restart();
                        return;
                    }
                }
                Complete();
            }
        }

        private void Restart()
        {
            target.Init();
            answers.Clear();
            for (int i = 0; i < sources.Count; i++)
            {
                sources[i].Init(target, (Elements)i, liquids[i]);
                answers.Add(new ElementCount((Elements)i, 0));
            }
        }

        private void Complete()
        {
            for (int i = 0; i < sources.Count; i++)
            {
                sources[i].enabled = false;
            }

            portal.SetActive(true);
            lastCode.SetActive(true);
            target.GetComponent<Rigidbody>().useGravity = true;
            RotateSecretWall();
            StartCoroutine(DelayedTrigger());
        }

        public void RotateSecretWall()
        {
            secretWall.Rotate(0, 0, -90);
        }

        private IEnumerator DelayedTrigger()
        {
            yield return new WaitForSecondsRealtime(1f);
            onComplete.Invoke();
            onDefocus.Invoke();
        }
    }
}