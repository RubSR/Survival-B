using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [Header("Camara")]
    //Traernos el contenedor de la camara
    public Transform cameraContainer;
    public float maxXMirar;
    public float minXMirar;
    private float camCurXRotation;
    public float sensibilidadCamara;

    [Header("Movimiento")] 
    public float moveSpeed;

    private Vector2 inputActualMoviento;

    private Vector2 mouseDelta;

    private Rigidbody rigidbody;

    [Header("Salto")] 
    public float fuerzaSalto;

    public LayerMask capasSuelo;

    [HideInInspector] public bool puedeMirar = true;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        //Queremos que se mueve en base a su propio eje local

        Vector3 dir = transform.forward * inputActualMoviento.y + transform.right * inputActualMoviento.x;
        dir *= moveSpeed;
        dir.y = rigidbody.velocity.y;
        rigidbody.velocity = dir;
    }

    private void LateUpdate()
    {
        
        //2.8 congelar si estamos en el inventario
        if (puedeMirar)
        {
            CamaraMirar();
        }
        
    }

    public void CapturarCamaraInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    private void CamaraMirar()
    {
        //Rotar la camara en ambos ejes en base al
        // mouseDelta (Vector2) que nos devuelve Input System
        camCurXRotation += mouseDelta.y * sensibilidadCamara;
        camCurXRotation = Mathf.Clamp(camCurXRotation, minXMirar, maxXMirar);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRotation, 0, 0);
        
        //Rotamos al jugador de izquierda a derecha
        transform.eulerAngles += new Vector3(0, mouseDelta.x * sensibilidadCamara, 0);
    }

    public void CapturarInputMovimiento(InputAction.CallbackContext context)
    {
        //Comprobamos si se esta apretando
        if (context.phase == InputActionPhase.Performed)
        {
            inputActualMoviento = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            inputActualMoviento = Vector2.zero;
        }
    }
    
    //Funcion que devolvera si isGrounded o no
    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {   new Ray(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f)+ (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f)+ (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f)+ (Vector3.up * 0.01f), Vector3.down),
        };
        //Comprobamos si toca o no
        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, capasSuelo))
            {
                return true;
            }
        }

        return false;
    }
    //PIntar los rays 
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        Gizmos.DrawRay(transform.position + (transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (transform.right * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.right * 0.2f), Vector3.down);
    }

    //Funcion que es llamada por Input Sistem cuando se presiona espacio
    public void CapturarTeclaSalto(InputAction.CallbackContext context)
    {
        //Es el primer frame en el que lo estamos apretando?-per
        if (context.phase == InputActionPhase.Started)
        {
            //Estamos tocando tierra?
            if (IsGrounded())
            {
                //Saltamos
                rigidbody.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            }
        }
    }
    
    
    //2.6 Activar o desactivar cursor
    public void ActivarCursor(bool activar)
    {
        Cursor.lockState = activar ? CursorLockMode.None : CursorLockMode.Locked;
        puedeMirar = !activar;
        
        //2.7 en el late Update hay que congelar la camara
    }
    
    
    
}// Cierre de la clase
