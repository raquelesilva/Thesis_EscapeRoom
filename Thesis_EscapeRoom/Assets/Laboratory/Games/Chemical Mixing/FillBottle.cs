using CoreSystems.Extensions.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ChemMix.ChemMixManager;

namespace ChemMix
{
    public class FillBottle : MonoBehaviour
    {
        [Serializable]
        public class Liquid
        {
            public Color32 color;
            [Range(0, 100)] public float amount;

            public Liquid(Color32 color, float amount)
            {
                this.color = color;
                this.amount = amount;
            }
        }

        [Space(10)]
        [SerializeField, ShowOnly] private int totalSteps = 0;
        [SerializeField] private List<int> steps = new();

        [Space(10)]
        [Range(0, 100)] public float percentage = 0;
        [SerializeField] private Color32 avgColor = Color.white;

        [Space(10)]
        [SerializeField, ShowOnly] private SkinnedMeshRenderer liquidMesh;
        [SerializeField, ShowOnly] private Material liquidMat;

        [Space(10)]
        [SerializeField, ShowOnly] private ChemMixManager manager;
        [SerializeField] private List<Liquid> liquids = new();
        [SerializeField, ShowOnly] private FillBottle target;
        [SerializeField, ShowOnly] private Elements element;
        [SerializeField] private TextMeshPro label;

        public void Init()
        {
            manager = instance;

            liquidMesh = GetComponentInChildren<SkinnedMeshRenderer>(true);
            liquidMat = liquidMesh.material;
            totalSteps = 0;
            for (int i = 0; i < steps.Count; i++)
            {
                totalSteps += steps[i];
            }

            percentage = 0;
            liquids.Clear();
            Fill();
        }

        public void Init(FillBottle target, Elements element, Liquid liquid = null)
        {
            manager = instance;

            this.target = target;
            this.element = element;
            label.text = element.ToString();

            liquidMesh = GetComponentInChildren<SkinnedMeshRenderer>(true);
            liquidMat = liquidMesh.material;
            totalSteps = 0;
            for (int i = 0; i < steps.Count; i++)
            {
                totalSteps += steps[i];
            }

            percentage = 0;
            liquids.Clear();
            Add(liquid.color, liquid.amount);
        }

        public void Fill()
        {
            float value = 100 - percentage;
            float fill = totalSteps / 100f * value;

            for (int i = 0; i < steps.Count; i++)
            {
                liquidMesh.SetBlendShapeWeight(i, 0);
            }

            if (fill < totalSteps)
            {
                liquidMesh.gameObject.SetActive(true);

                float availableSteps = fill;
                for (int i = 0; i < steps.Count; i++)
                {
                    if (availableSteps < steps[i])
                    {
                        liquidMesh.SetBlendShapeWeight(i, 100f / steps[i] * availableSteps);
                    }
                    else
                    {
                        liquidMesh.SetBlendShapeWeight(i, 100);
                    }

                    availableSteps -= steps[i];
                    if (availableSteps < 0.1f)
                    {
                        return;
                    }
                }
            }
            else
            {
                liquidMesh.gameObject.SetActive(false);
            }
        }

        public void Add(Color32 color, float amount)
        {
            int index = liquids.FindIndex(l => l.color.Equals(color));
            if (index == -1)
            {
                liquids.Add(new Liquid(color, amount));
            }
            else
            {
                liquids[index].amount += amount;
            }

            percentage += amount;
            Fill();
            Mix();
        }

        private void Mix()
        {
            float r = 0, g = 0, b = 0;
            for (int i = 0; i < liquids.Count; i++)
            {
                float per = liquids[i].amount / percentage;

                Color32 currColor = liquids[i].color;
                r += currColor.r * per;
                g += currColor.g * per;
                b += currColor.b * per;
            }
            avgColor = new((byte)r, (byte)g, (byte)b, 255);
            liquidMat.SetColor("_BaseColor", avgColor);
        }

        private void Spill(float amount)
        {
            float remaining = percentage - amount;
            if (remaining < 0)
            {
                amount += remaining;
            }

            float overflow = target.percentage + amount;
            if (overflow > 100)
            {
                amount -= overflow - 100;
            }

            if (percentage > 0 && target.percentage < 100)
            {
                target.Add(avgColor, amount);
                Add(avgColor, -amount);
                manager.Answer(element);
            }
        }

        //private void OnMouseDown()
        //{
        //    StartCoroutine(Flow());
        //}

        //private void OnMouseUp()
        //{
        //    StopAllCoroutines();
        //}

        private void OnMouseUpAsButton()
        {
            if (target != null)
            {
                Spill(10f);
            }
        }

        private IEnumerator Flow()
        {
            while (true)
            {
                Spill(1f);
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
    }
}