using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;


public class Inventario : MonoBehaviour
{
    
    //Array de Interfaz de slot
    public ItemSlotUI[] uiSlots;
    //Array de items de los slots
    // Creamos un clase que guarda el itemData y la cantidad
    public ItemSlot[] slots;
    // Objeto canvas inventario, para cerrarlo, abrirlo, actualizarlo etc
    public GameObject inventoryWindow;
    // Posicion de dropeo
    public Transform dropPosition;
    
    [Header("Item seleccionado")]
    // Item del al array de slot seleccionado
    private ItemSlot selectedItem;
    // Indice del slot item selccionado
    private int selectedItemIndex;
    //Para controlar la parte derecha del inventario
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatNames;
    public TextMeshProUGUI selectedItemStatValues;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;
    //Indice del objeto equipo actual
    private int curEquipIndex;
    //Componentes que necesitaremos mas delante
    private PlayerController controller;

    private JugadorNecesidades needs;
        
    [Header("Eventos")] 
    // Lo tulizaremo para congelar el juego mientra miramos el inventario
    public UnityEvent onOpenInventory;
    // para descongelar el juego
    public UnityEvent onCloseInventory;
    
    //Singletton
    // Para acceder a esta clase desde fuera, creando un unica instancia.
    public static Inventario instancia;

    private void Awake()
    {
        //Inicializamos el singleton
        instancia = this;
        //Nos traemos los componentes
        controller = GetComponent<PlayerController>();
    }

    
    void Start()
    {
        //Escondemos el inventario
        inventoryWindow.SetActive(false);
        //Creamos un array de itemsSLots y la llenamos de Items vacios
        //Lo mismo con la UI.
        // No inicialiciom la UI porle añadiremos los slots a mano desde Unity
        slots = new ItemSlot[uiSlots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new ItemSlot();
            //Antes de nada vamos el itemSlotUI script 
            uiSlots[i].index = i;
            uiSlots[i].Clear();

        }
        
        ClearSelectedItemWindow();

    }

    // Abre y cierra el inventario
    public void Toggle ()
    {
        if(inventoryWindow.activeInHierarchy)
        {
            inventoryWindow.SetActive(false);
            //2.4
            onCloseInventory.Invoke();
            controller.ActivarCursor(false);
           
        }
        else
        {
            inventoryWindow.SetActive(true);
            //2.4 Seteamos el player input para abrir el inventario
            onOpenInventory.Invoke();
            ClearSelectedItemWindow();
            controller.ActivarCursor(true);
            
            //2.5 Vamos a hacer en player controller para que se vea el raton
        }
    }
    
    // Comporbar si esta abierto
    public bool IsOpen ()
    {
        return inventoryWindow.activeInHierarchy;
    }
    
    // Añadir item al inventario
    // La funcina principal para añadir
    public void AddItem (ItemData item)
    {
        // Puede stackear??
        if(item.puedeStackear)
        {
            //Slot a en el que puede estakeaer
            ItemSlot slotToStackTo = GetItemStack(item);
            if(slotToStackTo != null)
            {
                //Cogemos ese slot y aumentamos su cantidad
                slotToStackTo.cantidad++;
                //Hacemos un update del iventario
                UpdateUI();
                return;
            }
        }
        //Si no es estackeable
        ItemSlot emptySlot = GetEmptySlot();
        // Comprobamos si hay un slot vacio
        if(emptySlot != null)
        {
            //Asignamos en item a ese slot,
            // y actulizamo datos y vista de ese slot
            emptySlot.item = item;
            emptySlot.cantidad = 1;
            UpdateUI();
            return;
        }
        // Si no stackea y no hay espacio lo tiramos
        ThrowItem(item);
    }
    
    // Tiramos el oprefab del item con rotacion random
    void ThrowItem (ItemData item)
    {
        
        Instantiate(item.prefabDrop, dropPosition.position, Quaternion.Euler(Vector3.one
                                                                             * Random.value * 360.0f));
    }
    
    // update la parte el inventario
    void UpdateUI ()
    {
        //Recorremos todoso los slots
        // si tienen item, seteamo en el UI l cantidad y el itemData y el icono
        // En caso contrario limpiamos el slot.
        for(int x = 0; x < slots.Length; x++)
        {
            if(slots[x].item != null)
                uiSlots[x].Set(slots[x]);
            else
                uiSlots[x].Clear();
        }
    }
    
    // Devuelve el eslot en cual puede estakear el item que le pasemos
// returns null if there is no stack available
    ItemSlot GetItemStack (ItemData item)
    {
        for(int x = 0; x < slots.Length; x++)
        {
            // Si el item del slot es igual a mi item y su cantidad no sobrepasa el maxStack
            // devuelvo el slot
            if(slots[x].item == item && slots[x].cantidad < item.maxNumStack)
                return slots[x];
        }
        // en caso contrario devuelvo null
        return null;
    }
    
    // Devuelve un slot vacio si hay, en caso contrario null

    ItemSlot GetEmptySlot ()
    {
        for(int x = 0; x < slots.Length; x++)
        {
            if(slots[x].item == null)
                return slots[x];
        }
        return null;
    }
    
    // Metodo que se llama cuando seleccionamos un item
    // 2.1 Cuando seleccionamos un item del iventario tenemos que comprobar que tenga un item
    public void SelectItem (int index)
    {
        if (slots[index].item == null)
        {
            return;
        }

        selectedItem = slots[index];
        selectedItemIndex = index;
        
        //Seteamos textos
        selectedItemName.text = selectedItem.item.nombre;
        selectedItemDescription.text = selectedItem.item.descripcion;
        //Botones
        useButton.SetActive(selectedItem.item.tipo == ItemType.Consumible);
        equipButton.SetActive(selectedItem.item.tipo == ItemType.Equipable && !uiSlots[index].equipped);
        unEquipButton.SetActive(selectedItem.item.tipo == ItemType.Equipable && uiSlots[index].equipped);
        dropButton.SetActive(true);
        
        
    }
    
    
    //2.2se llama cuando se abre el inventario o el artículo seleccionado actualmente se ha agotado
    // o se deja de seleecionar un slot-> ir aabajo del todo
    void ClearSelectedItemWindow ()
    {
        
        // clear the text elements
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;
        // disable buttons
        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }
    
    // called when the "Use" button is pressed
    public void OnUseButton ()
    {
    }
    
    // called when the "Equip" button is pressed
    public void OnEquipButton ()
    {
    }
// unequips the requested item
    void UnEquip (int index)
    {
    }
// called when the "UnEquip" button is pressed
    public void OnUnEquipButton ()
    {
    }
// called when the "Drop" button is pressed
    public void OnDropButton ()
    {
        ThrowItem(selectedItem.item);
        RemoveSelectedItem();
    }
    //Metodo que elimiara el item seleccionado del inventario
    void RemoveSelectedItem ()
    {

    }
    // metodo que elimina el item que le pasemos
    public void RemoveItem (ItemData item)
    {
    }
    
    // Para comporbar el item y la cantidad
    public bool HasItems (ItemData item, int quantity)
    {
        return false;
    }
    
    //2.3
    // called when we give an inventory input - managed by the Input System
    // Ir a toggle
    public void OnInventoryButton (InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            Toggle();
        }
        
    }
}

//Cuando acabemos vamos al itemController

public class ItemSlot
{
    public ItemData item;
    public int cantidad;
}
