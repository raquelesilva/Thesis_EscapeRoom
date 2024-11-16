using CoreSystems.Extensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct OnDots
{
    public Material onDefault;
    public Material onHover;
    public Material onSelected;
    public Material onSucess;
}

[Serializable]
public struct OnDash
{
    public Material onDefault;
    public Material onInvalid;
    public Material onValid;
    public Material onSucess;
}

[Serializable]
public class Shape
{
    public List<Vector2> points = new();
    [HideInInspector] public List<SymmetryLineController> lines = new();
}

public class SymmetryManager : MonoBehaviour
{
    [Header("Prefabs")]
    public OnDots onDots;
    public OnDash onDash;
    public SymmetryLineController linePrefab;
    public Transform raycastPlane;
    [SerializeField] private Transform copyPlane;

    [Header("Polygon")]
    [ShowOnly] public List<SymmetryLineController> lines = new();
    [ShowOnly] public List<Transform> dots = new();
    [ShowOnly] public bool closed;

    [Header("Question Managers")]
    [SerializeField] private List<Shape> shapes;
    [SerializeField] private int counter;

    [Header("Unity Events")]
    [SerializeField] private UnityEvent onWinning;

    [Header("Player")]
    [SerializeField, ShowOnly] private FirstPersonController player;
    [SerializeField, ShowOnly] public new Camera camera;

    private void Start()
    {
        counter = 0;
        shapes = shapes.OrderBy(item => new System.Random().Next()).ToList();
        AssignManager();

        player = FindObjectOfType<FirstPersonController>();
        camera = player.playerCamera;

        SetLevel(counter);
    }

    private void AssignManager()
    {
        List<SymmetryDotController> allDots = raycastPlane.GetComponentsInChildren<SymmetryDotController>().ToList();
        for (int i = 0; i < allDots.Count; i++) allDots[i].sm = this;
    }

    private void Update()
    {
        if (lines.Count > 0 && !closed)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitData, Mathf.Infinity))
            {
                DrawLine(dots[^1].position, hitData.point);
            }
        }
    }

    private void SetLevel(int level)
    {
        List<Vector2> currShape = shapes[level].points;

        for (int i = 0; i < currShape.Count - 1; i++)
        {
            shapes[level].lines.Add(Instantiate(linePrefab));
            SymmetryLineController newLine = shapes[level].lines[^1];
            newLine.transform.SetParent(copyPlane);
            Destroy(newLine.GetComponent<Collider>());
            Destroy(newLine.GetComponent<Rigidbody>());
            Transform start = FindWithCoords(currShape[i]);
            Transform end = FindWithCoords(currShape[i + 1]);
            newLine.DrawLine(start, end);
            newLine.SetDots(start, end);
            Destroy(newLine.GetComponent<SymmetryLineController>());
        }

        {
            shapes[level].lines.Add(Instantiate(linePrefab));
            SymmetryLineController lastLine = shapes[level].lines[^1];
            lastLine.transform.SetParent(copyPlane);
            Destroy(lastLine.GetComponent<Collider>());
            Destroy(lastLine.GetComponent<Rigidbody>());
            Transform start = FindWithCoords(currShape[^1]);
            Transform end = FindWithCoords(currShape[0]);
            lastLine.DrawLine(start, end);
            lastLine.SetDots(start, end);
            Destroy(lastLine.GetComponent<SymmetryLineController>());
        }
    }

    private Transform FindWithCoords(Vector2 coords)
    {
        Transform row = copyPlane.GetChild((int)coords.y);
        Transform point = row.GetChild((int)coords.x);
        return point;
    }

    private void ResetLevel()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = 0; j < lines[i].dots.Count; j++)
            {
                lines[i].dots[j].GetComponent<MeshRenderer>().material = onDash.onDefault;
            }
            Destroy(lines[i].gameObject);
        }

        lines.Clear();
        dots.Clear();

        closed = false;
    }

    public void DrawLine(Vector3 start, Vector3 end, int index = -1)
    {
        if (index == -1) index = lines.Count - 1;

        SymmetryLineController controller = lines[index];
        controller.DrawLine(start, end);
    }

    public void DrawLine(Transform start, Transform end, int index = -1)
    {
        if (index == -1) index = lines.Count - 1;

        SymmetryLineController controller = lines[index];
        controller.SetDots(start, end);
        controller.DrawLine(start, end);
    }

    public void Complete()
    {
        closed = true;

        CheckAnswer();
    }

    private void CheckAnswer()
    {
        if (Verifications())
        {
            for (int i = 0; i < dots.Count; i++)
                dots[i].GetComponent<MeshRenderer>().material = onDots.onSucess;
            for (int i = 0; i < lines.Count; i++)
                lines[i].GetComponent<MeshRenderer>().material = onDash.onSucess;

            SymmetryChecker.instance.CheckGame();
        }
        else
        {
            ResetLevel();
        }
    }

    private bool Verifications()
    {
        List<SymmetryLineController> shapeLines = shapes[counter].lines;

        if (shapeLines.Count != lines.Count) return false;

        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = 0; j < shapeLines.Count; j++)
            {
                if (lines[i].CheckAnswer(shapeLines[j]))
                {
                    shapeLines.RemoveAt(j);
                    break;
                }
            }
        }

        return shapeLines.Count == 0;
    }
}