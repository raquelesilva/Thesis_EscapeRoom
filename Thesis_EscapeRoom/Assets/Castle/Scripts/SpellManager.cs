using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Chandelier")
        {
            collision.transform.GetChild(0).gameObject.SetActive(true);
            collision.transform.GetComponent<Collider>().enabled = false;
            Destroy(gameObject);
            StaffManager.instance.CheckChandelier();
        }
    }
}