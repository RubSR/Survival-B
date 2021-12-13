using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CicloDiaNoche : MonoBehaviour
{
    //Variables
    //El timpo va a ser de 0 a 1
    // Siendo 0 las 24:00, y 1 las 23:59
    [Range(0.0f, 1.0f)] 
    public float tiempo;
    //Establecer en tiempo real cuanto dura un dia
    public float duracionDia;
    public float horaInicio = 0.4f;
    public float ratioTiempo;
    public Vector3 mediodia;

    [Header("Sol")] 
    public Light sol;

    public Gradient colorSol;

    public AnimationCurve intensidadSol;

    [Header("Luna")] 
    public Light luna;

    public Gradient colorLuna;
    public AnimationCurve intensidadLuna;

    [Header("Otros")] 
    public AnimationCurve intensidadGlobal;

    public AnimationCurve reflejosGlobales;
    
    // Start is called before the first frame update
    void Start()
    {
        ratioTiempo = 1.0f / duracionDia;
        tiempo = horaInicio;

    }

    // Update is called once per frame
    void Update()
    {
        //Incrementado el tiempo
        tiempo += ratioTiempo * Time.deltaTime;
        //El tiempo tiene que hacer un loop
        if (tiempo >= 1.0f)
        {
            tiempo = 0.0f;
        }
        //1- Rotacion sol y la luna
        sol.transform.eulerAngles = (tiempo - 0.25f) * mediodia * 4.0f;
        luna.transform.eulerAngles = (tiempo - 0.75f) * mediodia * 4.0f;
        //2-Intensidades
        sol.intensity = intensidadSol.Evaluate(tiempo);
        luna.intensity = intensidadLuna.Evaluate(tiempo);
        //3-Color
        sol.color = colorSol.Evaluate(tiempo);
        luna.color = colorLuna.Evaluate(tiempo);
        //4- Activar/desactivar luces
        if (tiempo >= 0.82f && sol.gameObject.activeInHierarchy)
        {
            sol.gameObject.SetActive(false);
        }
        else if (tiempo > 0.22f && tiempo < 0.23f && !sol.gameObject.activeInHierarchy)
        {
            sol.gameObject.SetActive(true);
            print("activo");
        }
        
        if (luna.intensity == 0 && luna.gameObject.activeInHierarchy)
        {
            luna.gameObject.SetActive(false);
        }
        else if (luna.intensity > 0 && !luna.gameObject.activeInHierarchy)
        {
            luna.gameObject.SetActive(true);
        }
        
        //Intensidad y reflejos globales
        RenderSettings.ambientIntensity = intensidadGlobal.Evaluate(tiempo);
        RenderSettings.reflectionIntensity = reflejosGlobales.Evaluate(tiempo);
    }
}
