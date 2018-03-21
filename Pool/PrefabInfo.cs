using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabInfo : MonoBehaviour
{
    public PoolType poolType;

    /// <summary>
    /// The lifetime.调用Manager.Recycle的时间
    /// </summary>
    [HideInInspector]
    public float lifetime = 0;
    void OnEnable()
    {
        if (lifetime > 0)
        {
            StartCoroutine(CountTime());
        }
    }

    IEnumerator CountTime()
    {
        yield return new WaitForSeconds(lifetime);
        ObjectPoolManager.Recycle(gameObject);
    }


}