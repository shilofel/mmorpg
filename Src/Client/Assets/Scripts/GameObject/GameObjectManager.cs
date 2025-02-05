using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entities;
using Services;
using SkillBridge.Message;
using Models;
using System;

//使用单例确保切换场景，对象不被销毁
public class GameObjectManager : MonoSingleton<GameObjectManager>
{

    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();
    // Use this for initialization
    //重载start
    protected override void OnStart()
    {
        StartCoroutine(InitGameObjects());
        CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave += OnCharacterLeave;
    }

    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave -= OnCharacterLeave;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCharacterEnter(Character cha)
    {
        CreateCharacterObject(cha);
    }

    private void OnCharacterLeave(Character cha)
    {
        if (!Characters.ContainsKey(cha.entityId))
            return;

        //对象包含且不为空，删除对象，并从字典中移除
        if(Characters[cha.entityId]!=null)
        {
            Destroy(Characters[cha.entityId]);
            this.Characters.Remove(cha.entityId);
        }
    }

    IEnumerator InitGameObjects()
    {
        foreach (var cha in CharacterManager.Instance.Characters.Values)
        {
            CreateCharacterObject(cha);
            yield return null;
        }
    }

    private void CreateCharacterObject(Character cha)
    {
        if(!Characters.ContainsKey(cha.entityId) ||Characters[cha.entityId] == null)
        {
            UnityEngine.Object obj = Resloader.Load<UnityEngine.Object>(cha.Define.Resource);
            if (obj == null)
            {
                Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", cha.Define.TID, cha.Define.Resource);
                return;
            }

            GameObject go = (GameObject)Instantiate(obj, this.transform);
            go.name = "Character_" + cha.Id + "_" + cha.Name;

            Characters[cha.entityId] = go;
            
            UIWorldElementManager.Instance.AddCharacterNameBar(go.transform, cha);
        }
        this.InitGameObject(Characters[cha.entityId], cha);
    }

    private void InitGameObject(GameObject go, Character cha)
    {
        go.transform.position = GameObjectTool.LogicToWorld(cha.position);
        go.transform.forward = GameObjectTool.LogicToWorld(cha.direction);

        //赋值实体控制器
        EntityController ec = go.GetComponent<EntityController>();
        if (ec != null)
        {
            ec.entity = cha;
            ec.isPlayer = cha.IsCurrentPlayer;
        }
        //赋值InputPlayerController
        PlayerInputController pc = go.GetComponent<PlayerInputController>();
        if (pc != null)
        {
            if (cha.IsCurrentPlayer)
            {
                User.Instance.CurrentCharacterObject = go;
                MainPlayerCamera.Instance.player = go;
                pc.enabled = true;
                pc.character = cha;
                pc.entityController = ec;
            }
            else
            {
                pc.enabled = false;
            }
        }
    }
}

