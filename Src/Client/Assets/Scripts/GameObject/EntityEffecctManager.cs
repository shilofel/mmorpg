using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class EntityEffecctManager:MonoBehaviour
{
    public Transform Root;
    private Dictionary<string, GameObject> Effects = new Dictionary<string, GameObject>();

    public Transform[] Props;

    private void Start()
    {
        this.Effects.Clear();

        if (Root.childCount>0)
        {
            for(int i=0;i<this.Root.childCount;i++)
            {
                this.Effects[this.Root.GetChild(i).name] = this.Root.GetChild(i).gameObject;
            }
        }

        if(Props!=null)
        {
            for(int i=0;i<this.Props.Length;i++)
            {
                this.Effects[this.Props[i].name] = this.Props[i].gameObject;
            }
        }
    }

    public void PlayEffect(string name)
    {
        Debug.LogFormat("EntityEffecctManager PlayEffect:{0},{1} ", this.name, name);
        if (this.Effects.ContainsKey(name))
        {
            this.Effects[name].SetActive(true);
        }
    }

    internal void PlayEffect(EffectType type, string name, Transform target, Vector3 pos,float duration)
    {
        if (type == EffectType.Bullet)
        {
            EffectController effect = InstantiateEffect(name);
            effect.Init(type, this.transform, target,pos, duration);
            effect.gameObject.SetActive(true);
        }
        else
            PlayEffect(name);
    }

    private EffectController InstantiateEffect(string name)
    {
        GameObject prefab;
        if(this.Effects.TryGetValue(name,out prefab))
        {
            GameObject go = Instantiate(prefab, GameObjectManager.Instance.transform, true);
            go.transform.position = prefab.transform.position;
            go.transform.rotation = prefab.transform.rotation;
            EffectController effect = go.GetComponent<EffectController>();
            if (effect != null)
                effect.isPooled = true;
            return effect;
        }
        return null;
    }
}
