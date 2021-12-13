using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour, IInteractuable
{
    public ItemData item;

    public string GetMensajeInteraccion()
    {
        return string.Format("Recoger {0}", item.nombre);
    }

    public void Interactuar()
    {
        //Añdimpos al interactuar el additem pasandole el itemdataq del objeto en cuestion y ya podemos hacerlo funcionar
        Inventario.instancia.AddItem(item);
        Destroy(gameObject);
    }
    
}
