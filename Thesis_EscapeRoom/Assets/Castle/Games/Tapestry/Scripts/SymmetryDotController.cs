using UnityEngine;

public class SymmetryDotController : MonoBehaviour
{
    public SymmetryManager sm;
    private MeshRenderer meshRend;

    private void Awake()
    {
        meshRend = GetComponent<MeshRenderer>();
    }

    private void OnMouseExit()
    {
        if (!sm.closed)
        {
            if (sm.lines.Count > 0)
            {
                if (sm.lines[^1].isValid)
                {
                    if (!sm.dots.Contains(transform))
                    {
                        meshRend.material = sm.onDots.onDefault;
                    }
                    if ((!sm.dots.Contains(transform) || (sm.dots.Count > 2 && sm.dots[0] == transform)) && sm.lines.Count > 0)
                    {
                        sm.lines[^1].GetComponent<MeshRenderer>().material = sm.onDash.onDefault;
                    }
                }
            }
            else
            {
                if (!sm.dots.Contains(transform))
                {
                    meshRend.material = sm.onDots.onDefault;
                }
            }
        }
    }

    private void OnMouseEnter()
    {
        if (!sm.closed)
        {
            if (sm.lines.Count > 0)
            {
                if (sm.lines[^1].isValid)
                {
                    if (!sm.dots.Contains(transform))
                    {
                        meshRend.material = sm.onDots.onHover;
                    }
                    if ((!sm.dots.Contains(transform) || (sm.dots.Count > 2 && sm.dots[0] == transform)) && sm.lines.Count > 0)
                    {
                        sm.lines[^1].GetComponent<MeshRenderer>().material = sm.onDash.onValid;
                    }
                }
            }
            else
            {
                if (!sm.dots.Contains(transform))
                {
                    meshRend.material = sm.onDots.onHover;
                }
            }
        }
    }

    private void OnMouseUpAsButton()
    {
        if (!sm.closed)
        {
            if (sm.lines.Count > 0)
            {
                if (!sm.lines[^1].isValid)
                {
                    return;
                }
            }

            if (!sm.dots.Contains(transform))
            {
                meshRend.material = sm.onDots.onSelected;

                sm.dots.Add(transform);

                if (sm.lines.Count > 0)
                {
                    sm.DrawLine(sm.dots[^2], sm.dots[^1]);
                    sm.lines[^1].GetComponent<Collider>().isTrigger = false;
                }

                Ray ray = sm.camera.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out RaycastHit hitData, 1000);
                Vector3 position = hitData.point + (transform.position - hitData.point) / 2;
                sm.lines.Add(Instantiate(sm.linePrefab, position, Quaternion.identity));
                sm.lines[^1].transform.SetParent(sm.raycastPlane);
            }
            else if (sm.dots[^1] == transform)
            {
                meshRend.material = sm.onDots.onHover;

                sm.dots.Remove(transform);
                Destroy(sm.lines[^1].gameObject);
                sm.lines.RemoveAt(sm.lines.Count - 1);
                if (sm.lines.Count > 0) sm.lines[^1].GetComponent<Collider>().isTrigger = true;
            }
            else if (sm.dots.Count > 2 && sm.dots[0] == transform)
            {
                sm.DrawLine(sm.dots[^1], sm.dots[0]);
                sm.Complete();
            }
        }
    }
}