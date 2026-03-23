using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Managers;
using System;

public class FXManager : MonoSingleton<FXManager>
{
    public GameObject[] prefabs;

    private Dictionary<string, GameObject> effectPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, Queue<GameObject>> effectPools = new Dictionary<string, Queue<GameObject>>();

    protected override void OnStart()
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            string name = prefabs[i].name;
            effectPrefabs[name] = prefabs[i];
            effectPools[name] = new Queue<GameObject>();
            // 预创建3个实例
            for (int j = 0; j < 3; j++)
            {
                var go = Instantiate(prefabs[i], transform, true);
                go.SetActive(false);
                effectPools[name].Enqueue(go);
            }
        }
    }

    EffectController GetEffectFromPool(string name, Vector3 pos)
    {
        if (!effectPools.ContainsKey(name))
            return null;

        Queue<GameObject> pool = effectPools[name];
        GameObject go;

        if (pool.Count > 0)
        {
            // 从池中取出
            go = pool.Dequeue();
            go.transform.position = pos;
            go.SetActive(true);
        }
        else
        {
            // 池中没有，创建新的
            go = Instantiate(effectPrefabs[name], transform, true);
            go.transform.position = pos;
        }

        return go.GetComponent<EffectController>();
    }

    public void ReturnToPool(GameObject effect)
    {
        string name = effect.name;
        if (effectPools.ContainsKey(name))
        {
            effect.SetActive(false);
            effectPools[name].Enqueue(effect);
        }
        else
        {
            Destroy(effect);
        }
    }

    internal void PlayEffect(EffectType type, string name, Transform target, Vector3 pos, float duration)
    {
        EffectController effect = GetEffectFromPool(name, pos);
        if (effect == null)
        {
            Debug.LogErrorFormat("Effect:{0} not found", name);
            return;
        }
        effect.Init(type, this.transform, target, pos, duration);
    }
}
