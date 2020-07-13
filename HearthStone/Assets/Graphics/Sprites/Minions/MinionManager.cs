using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : MonoBehaviour
{
    public static MinionManager instance;

    public List<Material> minionMat = new List<Material>();
    public Dictionary<string, Material> minionMaterial = new Dictionary<string, Material>(); 

    #region[Awake]
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            for (int i = 0; i < minionMat.Count; i++)
                minionMaterial.Add(minionMat[i].name, minionMat[i]);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion
}

