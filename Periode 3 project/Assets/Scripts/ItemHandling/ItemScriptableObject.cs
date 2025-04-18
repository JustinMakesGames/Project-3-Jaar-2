using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
public class ItemScriptableObject : ScriptableObject
{
    public string itemName;
    public int price;
    public GameObject animationPrefab;
    public GameObject uiPrefab;
    public GameObject obj;
    public string itemDescription;
}
