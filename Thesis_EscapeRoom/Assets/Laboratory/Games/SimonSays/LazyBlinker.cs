using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazyBlinker : MonoBehaviour
{
    //Note: I couldn't get SetEmissive GlobalIllumination methods to work, so fuck off unity I'm doing this the lazy KISS way

    //Shit with no light
    [SerializeField] List<GameObject> blinkoffs = new List<GameObject>();

    //Shit with light
    [SerializeField] List<GameObject> blinkons = new List<GameObject>();

    [SerializeField] bool notBusy = true;

    public void FlashBlinkers(int blinkNum = 1, float frequency = 1.5f, bool leaveTheLightsOn = false)
    {
        if (notBusy)
        {
            notBusy =false;
            StartCoroutine(FlashyBlinkies(blinkNum, frequency, leaveTheLightsOn));
        }
    }



    IEnumerator FlashyBlinkies(int blinkNum = 1, float frequency = 1.5f, bool leaveTheLightsOn = false)
    {
        for (int i = 0; i < blinkNum; i++)
        {
            //turn off
            for (int j = 0; j < blinkons.Count; j++)
            {
                blinkons[j].SetActive(false);
                blinkoffs[j].SetActive(true);
            }
            yield return new WaitForSeconds(frequency);
            //turn on
            for (int j = 0; j < blinkons.Count; j++)
            {
                blinkons[j].SetActive(true);
                blinkoffs[j].SetActive(false);
            }
            yield return new WaitForSeconds(frequency);

        }

        yield return new WaitForSeconds(frequency);
        if (leaveTheLightsOn)
        {
            for (int j = 0; j < blinkons.Count; j++)
            {
                blinkons[j].SetActive(true);
                blinkoffs[j].SetActive(false);
            }
        }
        else
        {
            for (int j = 0; j < blinkons.Count; j++)
            {
                blinkons[j].SetActive(false);
                blinkoffs[j].SetActive(true);
            }
        }
        notBusy = true;
    }
}
