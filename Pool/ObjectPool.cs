using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    protected Queue queue = new Queue();//用来保存池中对象  
    protected int _freeObjCount = 0;//池中待分配对象数量  
    protected bool _binit = false;//是否初始化  
    [HideInInspector]
    public GameObject prefab;//prefab引用  
    public PoolType poolType;

    /// <summary>
    /// Init this instance.初始化
    /// </summary>
    public void _init()
    {

    }

    public virtual GameObject Alloc(float lifetime, string assetName)
    {
        //如果没有进行过初始化，先初始化创建池中的对象  
        if (!_binit)
        {
            _init();
            _binit = true;
        }

        //lifetime<0时，创建对象池并返回null  
        if (lifetime < 0)
        {
            Debug.LogWarning("lifetime <= 0, return null");
            return null;
        }
        GameObject returnObj;

        //池中有待分配对象  
        if (_freeObjCount > 0)
        {
            returnObj = (GameObject)queue.Dequeue();//分配  
            _freeObjCount--;
        }
        else
        {//池中没有对象了，实例化一个  
            returnObj = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            returnObj.SetActive(false);//防止挂在returnObj上的脚本自动开始执行  
            returnObj.transform.parent = this.transform;
        }
        //使用PrefabInfo脚本保存returnObj的一些信息  
        PrefabInfo info = returnObj.GetComponent<PrefabInfo>();
        if (info == null)
        {
            info = returnObj.AddComponent<PrefabInfo>();
        }
        if (lifetime > 0)
        {
            info.lifetime = lifetime;
        }
        info.poolType = poolType;
        returnObj.SetActive(true);
        return returnObj;
    }

    public virtual void Recycle(GameObject obj)
    {
        //待分配对象已经在对象池中  
        if (queue.Contains(obj))
        {
            Debug.LogWarning("the obj " + obj.name + " be recycle twice!");
            return;
        }
        queue.Enqueue(obj);//入队，并进行reset  
        obj.transform.parent = this.transform;
        obj.SetActive(false);
        _freeObjCount++;
    }
}