using CoreSystems.Extensions.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialAnimManager : MonoBehaviour
{
    public enum State
    {
        Look, Zoom, Move, Sprint, Jump, Interact, Click, Back, Grab
    }

    [Serializable]
    public class Steps
    {
        public State state;
        public Animator animator;
        [ShowOnly] public AnimationClip clip;
        [ShowOnly] public float clipLength;
        [ShowOnly] public Slider indicator;
    }

    [SerializeField] private List<Steps> steps = new();
    [SerializeField] private int loopsPer = 5;
    [SerializeField] private int scrollTime = 2;

    [SerializeField] private Scrollbar carouselScroll;
    [SerializeField] private Transform indicatorsParent;
    [SerializeField] private Slider indicatorPrefab;

    [SerializeField] private RuntimeAnimatorController controller;
    [SerializeField, ShowOnly] private List<AnimationClip> clips;

    private void OnValidate()
    {
        clips.Clear();
        if (controller != null)
        {
            for (int i = 0; i < controller.animationClips.Length; i++)
            {
                clips.Add(controller.animationClips[i]);
            }

            for (int i = 0; i < steps.Count; i++)
            {
                steps[i].clip = clips[(int)steps[i].state];
                steps[i].clipLength = steps[i].clip.length;
                steps[i].indicator = null;
            }
        }
    }

    void Start()
    {
        int count = indicatorsParent.childCount;
        for (int i = 0; i < count; i++)
        {
            Destroy(indicatorsParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < steps.Count; i++)
        {
            steps[i].indicator = Instantiate(indicatorPrefab, indicatorsParent);
            steps[i].indicator.maxValue = steps[i].clipLength;
            steps[i].indicator.name = "Indicator " + (i + 1);
        }
    }

    public void StartCarousel()
    {
        StartCoroutine(Carousel());
    }

    private IEnumerator Carousel()
    {
        float mult = 1f / (steps.Count - 1);
        while (true)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                steps[i].indicator.fillRect.gameObject.SetActive(false);
            }

            for (int i = 0; i < steps.Count; i++)
            {
                Steps step = steps[i];

                // # Play animation
                //Start animation 'i'
                step.animator.enabled = true;
                step.animator.Play(step.state.ToString());

                //Let animation play 'loopsPer' times
                //yield return new WaitForSeconds(step.clipLength * loopsPer);

                //Increase indicator
                Slider currIndicator = step.indicator;
                currIndicator.fillRect.gameObject.SetActive(true);
                float max = currIndicator.maxValue;
                float t = 0f;
                while (t < max)
                {
                    t += Time.deltaTime / loopsPer;
                    currIndicator.value = t;//Mathf.Lerp(0, max, t);
                    yield return null;
                }
                currIndicator.value = max;

                //Stop animation
                step.animator.enabled = false;

                // # Scroll carousel
                //Calculate travel 
                float from = mult * i;
                float to = from + mult;

                //Check if already at the end
                if (to > 1)
                {
                    from = 1;
                    to = 0;
                }

                //Lerp scroll
                t = 0f;
                while (t < 1)
                {
                    t += Time.deltaTime / scrollTime;
                    carouselScroll.value = Mathf.Lerp(from, to, t);
                    yield return null;
                }
                carouselScroll.value = to;
            }
        }
    }
}
