using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu()]
public class KitchenObjectSO : ScriptableObject
{
    //Set up fields for all the info we'd need for the object 
    public Transform prefab;
    public Sprite sprite;
    public string objectName;


}
