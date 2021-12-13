using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cactus : MonoBehaviour
{
    //
    public int daño;
    //Mientras estemos en colision con el cactus
    //el numero de golpes por segundo
    public float ratioDaño;

    private List<IRecibeDaño> cosasADañar = new List<IRecibeDaño>();
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RepartirDaño());

    }
    IEnumerator RepartirDaño()
    {
        while (true)
        {
            for (int i = 0; i < cosasADañar.Count; i++)
            {
                cosasADañar[i].RecibirDaño(daño);
            }
            yield return new WaitForSeconds(ratioDaño);
        }
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<IRecibeDaño>() != null)
        {
            cosasADañar.Add(other.gameObject.GetComponent<IRecibeDaño>());
        }
    } 
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.GetComponent<IRecibeDaño>() != null)
        {
            cosasADañar.Remove(other.gameObject.GetComponent<IRecibeDaño>());
        }
    }
}
