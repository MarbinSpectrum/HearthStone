using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    
    protected List<GameObject> objectPool = new List<GameObject>();

    protected GameObject FindPool(string s)
    {
        for (int i = 0; i < objectPool.Count; i++)
            if (!objectPool[i].activeSelf && objectPool[i].transform.name.Equals(s))
                return objectPool[i];
        return null;

    }

    protected void AddPool(GameObject obj)
    {
        objectPool.Add(obj);
        obj.transform.parent = transform;
    }
}
