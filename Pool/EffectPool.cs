using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : ObjectPool
{

    public override GameObject Alloc(float lifetime)
    {

        return new GameObject();
    }

    public override void Recycle(GameObject obj)
    {

    }
}


