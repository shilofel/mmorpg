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
    }

    ~UIManager()
    {

    }

    public T Show<T>()
    {
        //SceneManager.Instance.PlaySound();
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
        if(this.UIResources.ContainsKey(type))
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
}
