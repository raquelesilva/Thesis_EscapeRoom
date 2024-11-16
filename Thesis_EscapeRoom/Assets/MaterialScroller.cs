using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialScroller : MonoBehaviour
{
    enum Directions
    {
        Right,
        Left,
        Up,
        Down
    }

    [SerializeField] bool canScroll;
    [SerializeField] private Directions direction;
    [SerializeField] private float speed;

    private Material mat;

    private void Start()
    {
        Image image = GetComponent<Image>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if(image != null)
        {
            if(image.material != null)
            {
                mat = image.material;
            }
            return;
        }

        if(meshRenderer != null)
        {
            if(meshRenderer.material != null)
            {
                mat = meshRenderer.material;
            }
        }
    }

    void Update()
    {
        if (!canScroll) return;
        if (mat == null) return;


        switch (direction)
        {
            case Directions.Right:
                mat.mainTextureOffset = mat.mainTextureOffset + Vector2.right * speed * Time.deltaTime;
                break;
            case Directions.Left:
                mat.mainTextureOffset = mat.mainTextureOffset + Vector2.left * speed * Time.deltaTime;
                break;
            case Directions.Up:
                mat.mainTextureOffset = mat.mainTextureOffset + Vector2.up * speed * Time.deltaTime;
                break;
            case Directions.Down:
                mat.mainTextureOffset = mat.mainTextureOffset + Vector2.down * speed * Time.deltaTime;
                break;
        }
    }

    private void OnDestroy()
    {
        mat.mainTextureOffset = Vector2.zero;
    }
}
