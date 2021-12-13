using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class JugadorNecesidades : MonoBehaviour, IRecibeDaño
{
    
    //Necesidades:
    public Necesidad vida;

    public Necesidad hambre;

    public Necesidad sed;

    public Necesidad dormir;

    //Cantidad de vida por tiempo que pierdo
    // si tengo hambre o sed
    public float noComidaCantidadVidaQueDecae;
    public float noAguaCantidadVidaQueDecae;

    public UnityEvent alRecibirDaño;
    void Start()
    {
        vida.valorActual = vida.valorComienzo;
        hambre.valorActual = hambre.valorComienzo;
        sed.valorActual = sed.valorComienzo;
        dormir.valorActual = dormir.valorComienzo;

    }

    // Update is called once per frame
    void Update()
    {
        // Perdida por tiempo
        hambre.Restar(hambre.ratioPerdida  * Time.deltaTime);
        sed.Restar(sed.ratioPerdida * Time.deltaTime);
        dormir.Añadir(dormir.ratioRegeneracion * Time.deltaTime);
        
        // Nos quitamos vida por tiempo, si tenemos hambre y/o sed
        if (hambre.valorActual == 0.0f)
        {
            vida.Restar(noComidaCantidadVidaQueDecae * Time.deltaTime);
        }

        if (sed.valorActual == 0.0f)
        {
            vida.Restar(noAguaCantidadVidaQueDecae * Time.deltaTime);
        }
        //Si la vida llega a cero, muero
        if (vida.valorActual == 0.0f)
        {
            Morir();
        }
        
        //Una vez hechos los calculos updateamos las barras de la interfaz
        vida.barraUi.fillAmount = vida.GetPorcetanje();
        hambre.barraUi.fillAmount = hambre.GetPorcetanje();
        sed.barraUi.fillAmount = sed.GetPorcetanje();
        dormir.barraUi.fillAmount = dormir.GetPorcetanje();

    }
    //Bloque de funciones basicas
    //Curar
    public void Curar(float cantidad)
    {
        vida.Añadir(cantidad);
    }
    //Comer
    public void Comer(float cantidad)
    {
        hambre.Añadir(cantidad);
    }
    //Beber 
    public void Beber(float cantidad)
    {
        sed.Añadir(cantidad);
    }
    //Domir
    public void Dormir(float cantidad)
    {
        dormir.Restar(cantidad);
    }
    //Recibir daño
    public void RecibirDaño(int cantidad)
    {
        vida.Restar(cantidad);
        alRecibirDaño?.Invoke();
    }
  
    
    //Morir
    public void Morir()
    {
        Debug.Log("Estoy Muerto");
    }

}// Fin de la clase principal

[System.Serializable]
public class Necesidad
{
    [HideInInspector]
    public float valorActual;
    public float valorMaximo;
    public float valorComienzo;
    public float ratioRegeneracion;
    public float ratioPerdida;
    public Image barraUi;
    
    //Añadir a la necesidad
    public void Añadir(float cantidad)
    {
        valorActual = Mathf.Min(valorActual + cantidad, valorMaximo);
    }
    
    //Restar a la necesidad
    public void Restar(float cantidad)
    {
        valorActual = Mathf.Max(valorActual - cantidad, 0.0f);
    }
    
    // Devolver valor actual en porcentajes ( 0.0 - 1.0)
    public float GetPorcetanje()
    {
        return valorActual / valorMaximo;
    }
} //Cierre de la clase Necesidad

public interface IRecibeDaño
{
    void RecibirDaño(int cantidad);
}
