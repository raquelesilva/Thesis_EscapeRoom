using System.Collections.Generic;
using UnityEngine;

public class SymmetryLineController : MonoBehaviour
{
    public List<Transform> dots;
    public bool isValid = true;

    public void SetDots(Transform start, Transform end)
    {
        dots.Clear();
        dots.Add(start);
        dots.Add(end);
    }

    public void DrawLine(Vector3 start, Vector3 end) //This is where I draw the line
    {
        Vector3 dir = start - end;
        transform.position = start;
        transform.right = dir;
        float lengthOffset = transform.parent.localScale.x;
        float length = Rounder(dir.magnitude * 50, 2) / lengthOffset;
        transform.localScale = new Vector3(length, 5f, 5f);
    }

    public void DrawLine(Transform start, Transform end)
    {
        DrawLine(start.position, end.position);
    }

    public float Rounder(float value, int numDecimalSpaces)
    {
        int x10 = (int)Mathf.Pow(10, numDecimalSpaces);
        return Mathf.Round(value * x10) / x10;
    }

    public bool CheckAnswer(SymmetryLineController other)
    {
        Vector2 this1 = GetCoords(dots[0]);
        Vector2 this2 = GetCoords(dots[1]);

        Vector2 other1 = GetCoords(other.dots[0]);
        Vector2 other2 = GetCoords(other.dots[1]);

        bool check1 = this1 == other1 || this1 == other2;
        bool check2 = this2 == other1 || this2 == other2;

        return check1 && check2;
    }

    private Vector2 GetCoords(Transform point)
    {
        string xName = point.name;
        string yName = point.parent.name;

        xName = xName.Split(' ')[1];
        yName = yName.Split(' ')[1];

        xName = xName.Trim('(', ')');
        yName = yName.Trim('(', ')');

        int x = int.Parse(xName);
        int y = int.Parse(yName);

        return new Vector2(x, y);
    }
}