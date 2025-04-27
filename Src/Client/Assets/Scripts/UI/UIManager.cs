using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIManager:Singleton<UIManager>
{
    class UIElement
    {
        //资源路径
        public string Resources;
        //是否缓存
        public bool Cache;
        //实例
        public GameObject Instance;
    }

    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

    public UIManager()
    {
        //this.UIResources.Add(typeof(UITest), new UIElement() { Resources = "UI/UITest", Cache = true });
        this.UIResources.Add(typeof(UIBag), new UIElement() { Resources = "UI/UIBag", Cache = true });
        this.UIResources.Add(typeof(UIShop), new UIElement() { Resources = "UI/UIShop", Cache = true });
        this.UIResources.Add(typeof(UICharEquip), new UIElement() { Resources = "UI/UICharEquip", Cache = true });
        this.UIResources.Add(typeof(UIQuestSystem), new UIElement() { Resources = "UI/UIQuest", Cache = true });
        this.UIResources.Add(typeof(UIQuestDialog), new UIElement() { Resources = "UI/UIQuestDialog", Cache = true });
        this.UIResources.Add(typeof(UIFriends), new UIElement() { Resources = "UI/UIFriends", Cache = true });
        this.UIResources.Add(typeof(UIGuild), new UIElement() { Resources = "UI/Guild/UIGuild", Cache = true });
        this.UIResources.Add(typeof(UIGuildList), new UIElement() { Resources = "UI/Guild/UIGuildList", Cache = true });
        this.UIResources.Add(typeof(UIGuildPopNoGuild), new UIElement() { Resources = "UI/Guild/UIGuildPopNoGuild", Cache = true });
        this.UIResources.Add(typeof(UIGuildPopCreate), new UIElement() { Resources = "UI/Guild/UIGuildPopCreate", Cache = true });
        this.UIResources.Add(typeof(UIGuildApplyList), new UIElement() { Resources = "UI/Guild/UIGuildApplyList", Cache = true });
        this.UIResources.Add(typeof(UISetting), new UIElement() { Resources = "UI/UISetting", Cache = true });
        this.UIResources.Add(typeof(UIPopCharMenu), new UIElement() { Resources = "UI/UIPopCharMenu", Cache = true });
        this.UIResources.Add(typeof(UIRide), new UIElement() { Resources = "UI/UIRide", Cache = true });
        this.UIResources.Add(typeof(UISkill), new UIElement() { Resources = "UI/UISkill", Cache = true });
        this.UIResources.Add(typeof(UISystemConfig), new UIElement() { Resources = "UI/UISystemConfig", Cache = true });
    }

    ~UIManager()
    {

    }

    public T Show<T>()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Open);
        Type type = typeof(T);
        if (this.UIResources.ContainsKey(type))
        {
            UIElement info = UIResources[type];
            if (info.Instance != null)
            {
                info.Instance.SetActive(true);
            }
            else
            {
                UnityEngine.Object prefab = Resources.Load(info.Resources);
                if (prefab == null)
                {
                    return default(T);
                }
                info.Instance = (GameObject)GameObject.Instantiate(prefab);
            }
            return info.Instance.GetComponent<T>();
        }
        return default(T);
    }

    public void Close(Type type)
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Close);
        if (this.UIResources.ContainsKey(type))
        {
            UIElement info = UIResources[type];
            if(info.Cache)
            {
                info.Instance.SetActive(false);
            }
            else
            {
                GameObject.Destroy(info.Instance);
                info.Instance = null;
            }
        }
    }

    public void Close<T>()
    {
        this.Close(typeof(T));
    }
}
