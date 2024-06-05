using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entities;
using Services;
using SkillBridge.Message;

public class GameObjectManager : MonoBehaviour
{

    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();
    // Use this for initialization
    void Start()
    {
        StartCoroutine(InitGameObjects());
        CharacterManager.Instance.OnCharacterEnter = OnCharacterEnter;
    }

    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCharacterEnter(Character cha)
    {
        CreateCharacterObject(cha);
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
        if(!Characters.ContainsKey(cha.Info.Id)||Characters[cha.Info.Id] == null)
        {
            Object obj = Resloader.Load<Object>(cha.Define.Resource);
            if (obj == null)
            {
                Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", cha.Define.TID, cha.Define.Resource);
                return;
            }

            GameObject go = (GameObject)Instantiate(obj);
            go.name = "Character_" + cha.Info.Id + "_" + cha.Info.Name;

            go.transform.position = GameObjectTool.LogicToWorld(cha.position);
            go.transform.forward = GameObjectTool.LogicToWorld(cha.direction);
            Characters[cha.Info.Id] = go;

            //赋值实体控制器
            EntityController ec = go.GetComponent<EntityController>();
            if (ec != null)
            {
                ec.entity = cha;
                ec.isPlayer = cha.IsPlayer;
            }

            //赋值InputPlayerController
            PlayerInputController pc = go.GetComponent<PlayerInputController>();
            if (pc != null)
            {
                if (cha.Info.Id == Models.User.Instance.CurrentCharacter.Id)
                {
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
            //UIWorldElementManager.Instance.AddCharacterNameBar(go.transform, character);
        }
    }
}
