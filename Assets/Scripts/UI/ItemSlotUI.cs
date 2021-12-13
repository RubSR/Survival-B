using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    //para capturar el boton
    public Button button;
    //Icono que muestra
    public Image icon;
    //TExto cantidad
    public TextMeshProUGUI quantityText;
    //Slot actual
    private ItemSlot curSlot;
    //Borde que se activara en el slot que tengamos equipado
    // Lo veremos mas adelante
    private Outline outline;
    //indice del slotUI seleccionado
    public int index;
    // Controlar si esta equipado o no
    public bool equipped;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        outline.enabled = equipped;
    }

    //Setear la parte visual del slot
    public void Set(ItemSlot slot)
    {   // Le enviamos el slot con el itemData y la cantidad
        curSlot = slot;
        //Lo activamos
        icon.gameObject.SetActive(true);
        //Le ponemos el icono que toca
        icon.sprite = slot.item.icono;
        //Pintamos la cantidad
        quantityText.text = slot.cantidad > 1 ? slot.cantidad.ToString() : string.Empty;
        //Controlamos el outline
        if(outline != null)
            outline.enabled = equipped;
    }
    // Limpiar el slot
    public void Clear ()
    {
        curSlot = null;
        icon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
    }
    
    // Evento del click en el UISlot
    
    //Vamos a unity a configurar las variables
    //Añadir el componente outline
    public void OnButtonClick ()
    {
        Inventario.instancia.SelectItem(index);
    }

    
}
