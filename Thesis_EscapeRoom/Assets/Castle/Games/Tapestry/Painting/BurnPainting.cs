using CoreSystems.Extensions.Attributes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BurnPainting : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float min = -0.5f;
    [SerializeField] private float max = 9f;
    [SerializeField, ShowOnly] private SpriteRenderer painting;
    [SerializeField, ShowOnly] private Material burn;

    private void Start()
    {
        painting = GetComponent<SpriteRenderer>();
        burn = painting.material;
    }

    [ContextMenu(nameof(Burn))]
    public void Burn()
    {
        StartCoroutine(Burnrotine());
    }

    private IEnumerator Burnrotine()
    {
        float t = min;
        while (t < max)
        {
            t += Time.deltaTime * speed;
            burn.SetFloat("_BurnRadius", t);
            yield return null;
        }
        burn.SetFloat("_BurnRadius", max);
    }

    [ContextMenu(nameof(UnBurn))]
    public void UnBurn()
    {
        StartCoroutine(UnBurnrotine());
    }

    private IEnumerator UnBurnrotine()
    {
        float t = max;
        while (t > min)
        {
            t -= Time.deltaTime * speed;
            burn.SetFloat("_BurnRadius", t);
            yield return null;
        }
        burn.SetFloat("_BurnRadius", min);
    }
}