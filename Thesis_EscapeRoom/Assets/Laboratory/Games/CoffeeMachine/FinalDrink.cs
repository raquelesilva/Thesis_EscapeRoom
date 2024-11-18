using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FinalDrink : MonoBehaviour
{
    [SerializeField] List<int> values;
    [SerializeField] List<int> correctValues;

    [SerializeField] GameObject labirinto;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("MazeBall"))
        {
            labirinto.SetActive(false);
        }
    }

    public void SetValues(int intensity, int quantity, int temperature)
    {
        values.Add(intensity);
        values.Add(quantity);
        values.Add(temperature);
    }

    public void GetMaze(GameObject maze)
    {
        labirinto = maze;
    }

    public bool IsCorrect()
    {
        if (values.Count == correctValues.Count)
        {
            for (int i = 0; i < correctValues.Count; i++)
            {
                if (correctValues[i] != values[i])
                {
                    return false;
                }
            }

            return true;
        }
        return false;
    }
}