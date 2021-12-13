using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("info")] 
    public string nombre;

    public string descripcion;
    public Sprite icono;
    public GameObject prefabDrop;
    public ItemType tipo;

    [Header("Stackable")] 
    public bool puedeStackear;
    public int maxNumStack;

}
public enum ItemType
{
    Recurso,
    Equipable,
    Consumible
}
