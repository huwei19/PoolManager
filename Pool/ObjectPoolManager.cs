using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager _instacne;

    /// <summary>
    /// The pool dic.
    /// 对象池字典
    /// </summary>
    private Dictionary<PoolType, ObjectPool> poolDic = new Dictionary<PoolType, ObjectPool>();

    /// <summary>
    /// Gets the instance.单例入口
    /// </summary>
    /// <returns>The instance.</returns>
    public static ObjectPoolManager GetInstance()
    {
        if (_instacne == null)
        {
            _instacne = new ObjectPoolManager();
        }
        return _instacne;
    }


    /// <summary>
    /// Getpool the specified pt.取出所需类型的对象池
    /// </summary>
    /// <returns>The getpool.</returns>
    /// <param name="pt">Point.</param>
    ObjectPool _getpool(PoolType pt)
    {
        //从对象池中取，若存在则直接返回
        if (poolDic.ContainsKey(pt))
        {
            return poolDic[pt];
        }

        //不存在，则需要重新创建，并且添加到poolDic
        ObjectPool returnpool = null;
        switch (pt)
        {
            case PoolType.EffectType:
                returnpool = new EffectPool();
                break;
            default:
                returnpool = new EffectPool();
                break;
        }

        poolDic.Add(pt, returnpool);
        return returnpool;
    }

    /// <summary>
    /// Alloc the specified type and lifetime.
    ///  lifeTime = 0 不自动回收对象，需游戏主动调用recycle回收
    /// lifeTime 小于  创建Pool实例并实例化Pool中的对象，但不返回对象，返回值null 预创建对象池，可以用这个方法先把对象池创建起来，避免创建对象池造成掉帧
    /// 当lifeTime 大于 0时，分配出去的GameObject上挂的PrefabInfo脚本会执行倒计时协程，计时器为0时调用recycle方法回收自己。
    /// </summary>
    /// <returns>The alloc.</returns>
    /// <param name="type">Type.pool类型</param>
    /// <param name="lifetime">Lifetime.存活时间</param>
    public static GameObject Alloc(PoolType type, string assetName, float lifetime = 0)
    {
        //根据传入type取出或创建对应类型对象池  
        ObjectPool subPool = _instacne._getpool(type);
        //从对象池中取出或创建一个对象返回  
        GameObject returnObj = subPool.Alloc(lifetime, assetName);
        return returnObj;
    }

    /// <summary>
    /// Recycle the specified recycleObj.回收对象
    /// </summary>
    /// <returns>The recycle.</returns>
    /// <param name="recycleObj">Recycle object.</param>
    public static void Recycle(GameObject recycleObj)
    {
        //可以获取Prefab上面的信息 然后再决定后面的回收操作
        PrefabInfo prefabInfo = recycleObj.GetComponent<PrefabInfo>();

        Destroy(recycleObj);
    }
}


public enum PoolType
{
    EffectType = 1,
    CubeType = 2
}