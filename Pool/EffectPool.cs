using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : ObjectPool
{

    /// <summary>
    /// The dic effects instance asset name to free.
    /// 第二个bool是指当前 特效是否空闲
    /// 参考实例：   [{"122323124312",{[arrowInstance,true]}}]
    /// </summary>
    Dictionary<string, Dictionary<GameObject, bool>> dic_effectsInstanceAssetNameToFree = new Dictionary<string, Dictionary<GameObject, bool>>();


    public override GameObject Alloc(float lifetime, string assetName)
    {
        prefab = AssetsPool.GetInstance().GetPrefab(assetName);
        return GetOneFreeGameobjInstance(assetName);
    }


    /// <summary>
    /// Recycle the specified obj.回收特效，里面有重置操作
    /// </summary>
    /// <returns>The recycle.</returns>
    /// <param name="obj">Object.</param>
    public override void Recycle(GameObject obj)
    {
        //将dic中的特效 空闲标志置为 true
        foreach (var v1 in dic_effectsInstanceAssetNameToFree)
        {
            if (dic_effectsInstanceAssetNameToFree[v1.Key].ContainsKey(obj))
            {
                //空闲标志置为 true
                dic_effectsInstanceAssetNameToFree[v1.Key][obj] = true;
            }
        }
		    
        //todo 将特效的还原为初始状态
		RestoreScript(obj);
    }




    /// <summary>
    /// Gets the one free gameobj.从dic中获取/新建空闲的特效实例
    /// </summary>
    /// <returns>The one free gameobj.</returns>
    /// <param name="assetName">Asset name.</param>
    public GameObject GetOneFreeGameobjInstance(string assetName)
    {
        GameObject go = null;
        if (prefab == null)
        {
            return null;
        }
        //根据assetName从dic中查找特效
        if (dic_effectsInstanceAssetNameToFree.ContainsKey(assetName))
        {
            //查找dic中的空闲特效
            foreach (var v in dic_effectsInstanceAssetNameToFree[assetName])
            {
                if (v.Key)
                {

					Recycle (v.Key);
                    //找到空闲特效，则返回，并将空闲标志为false
                    return v.Key;
                }
            }
            //没有找到空闲的特效实例，则新创建一份
            go = Instantiate(prefab);
            go.name = go.name + (dic_effectsInstanceAssetNameToFree[assetName].Count + 1).ToString();


            //添加到当前的dic中，进行统一管理
            dic_effectsInstanceAssetNameToFree[assetName].Add(go, false);
        }
        else
        {//从没初始化过 assetName的特效

            go = Instantiate(prefab);
            go.name = go.name + "1";

            //添加到当前的dic中，进行统一管理
            Dictionary<GameObject, bool> dic_temp = new Dictionary<GameObject, bool>();
            dic_temp.Add(go, false);
            dic_effectsInstanceAssetNameToFree.Add(assetName, dic_temp);
        }
        return go;
    }

	//循环遍历重置脚本
	public void RestoreScript(GameObject effect){
		//gameObject重置状态
		effect.SetActive (false);

		//重置脚本
		EffectSettings es = effect.GetComponent<EffectSettings> ();
		if (es != null) {
			es.enabled = false;
			es.enabled = true;
			float speed = es.MoveSpeed;
			float distance = es.MoveDistance;
			float destroyTime = es.DeactivateTimeDelay;
			Destroy (es);
			es = effect.AddComponent<EffectSettings> ();
			es.MoveSpeed = speed;
			es.MoveDistance = distance;
			es.DeactivateTimeDelay = destroyTime;
		}

//		重置脚本
		UnrealBulletController utc = effect.GetComponent<UnrealBulletController> ();
		if (utc != null) {
			utc.enabled = false;
			utc.enabled = true;
			EffectEnum.AttachPoint point = utc.attachPoint;
			EffectEnum.ForceType forceType = utc.forceType;
			float radius = utc.forceRadius;
			float destroyTime = utc.destroyTime;
			float forceDelay = utc.forceDelayTime;
			Destroy (utc);
			utc = effect.AddComponent<UnrealBulletController> ();
			utc.attachPoint = point;
			utc.forceType = forceType;
			utc.forceRadius = radius;
			utc.destroyTime = destroyTime;
			utc.forceDelayTime = forceDelay;

		}

		CollisionActiveBehaviour coll = effect.GetComponent<CollisionActiveBehaviour> ();
		if (coll != null) {
			Destroy (coll);
			coll = effect.AddComponent<CollisionActiveBehaviour> ();
			ProjectileCollisionBehaviour pcb = effect.GetComponent<ProjectileCollisionBehaviour> ();
			if (pcb != null) {
				Destroy (pcb);
				pcb = effect.AddComponent<ProjectileCollisionBehaviour> ();
				coll.IsReverse = false;
				coll.IsLookAt = false;
			} else {
				coll.IsReverse = true;
				coll.IsLookAt = true;
			}
		}

//		if (effect.GetComponent<ParticleSystem> () != null) {
////			effect.GetComponent<ParticleSystem> ().Play ();
//		}
		for (int i = 0; i < effect.transform.childCount; i++) {
			RestoreScript (effect.transform.GetChild (i).gameObject);
		}
		effect.SetActive (true);
	} 
}