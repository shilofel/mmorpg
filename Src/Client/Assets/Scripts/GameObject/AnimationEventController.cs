using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class AnimationEventController:MonoBehaviour
{
    public EntityEffecctManager EffecctMgr;

    void PlayEffect(string name)
    {
        Debug.LogFormat("PlayEffect:{0},{1} ", this.name,name);
        EffecctMgr.PlayEffect(name);
    }

    void PlaySound(string name)
    {
        Debug.LogFormat("PlaySound:{0},{1} ", this.name, name);
    }
}
