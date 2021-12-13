using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    
    //A traves de un raycast para a comprobar si tenemos 
    // delante un obejto con el que interactuar
    public float checkRate = 0.05f;
    //Momento en el que se hizo la ultima comprobacion
    private float ultimaVezComprobado;
    //Distancia a la que tiene que detectar
    public float maxDistancia;
    public LayerMask layer;

    private Camera cam;

    private GameObject objetoActual;
    private IInteractuable interactuableActual;

    public TextMeshProUGUI mensaje;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        // Time.time -> Devuelve en segundos  el tiempo pasado desde el frame
        //checkear el ratio
        if (Time.time - ultimaVezComprobado > checkRate)
        {
            ultimaVezComprobado = Time.time;
            //Creamos el rayo desde el centro de la pantalla
            //ScreenPointToRay, devuelve un Ray desde un punto concreto
            // de la pantalla que pinta la camara.
            Ray rayo = cam.ScreenPointToRay(new Vector3(
                Screen.width/2, Screen.height/2, 0
            ));
            RaycastHit hit;
            //Hemos chocado con algo?
            if (Physics.Raycast(rayo, out hit, maxDistancia, layer))
            {
                //El nuevo objeto es el mismo que el actual??
                if (hit.collider.gameObject != objetoActual)
                {
                    objetoActual = hit.collider.gameObject;
                    interactuableActual = hit.collider.GetComponent<IInteractuable>();
                    SetMensaje();
                }
                
            }
            else
            {
                objetoActual = null;
                interactuableActual = null;
                mensaje.gameObject.SetActive(false);

            }

        }
        
    }

    private void SetMensaje()
    {
        mensaje.gameObject.SetActive(true);
        mensaje.text = string.Format("<b> [E] </b> {0}",
            interactuableActual.GetMensajeInteraccion());

    }
    
    //Callback del evento E

    public void Interactuar(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && interactuableActual != null)
        {
            interactuableActual.Interactuar();
            objetoActual = null;
            interactuableActual = null;
            mensaje.gameObject.SetActive(false);
        }
    }
}//Fin de la clase principal

public interface IInteractuable
{
    string GetMensajeInteraccion();

    void Interactuar();
}

