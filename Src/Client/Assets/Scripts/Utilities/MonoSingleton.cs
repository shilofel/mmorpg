using UnityEngine;


public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool global = true;
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance =(T)FindObjectOfType<T>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (global)
        {
            //单例对象产生多个，检测到已经存在实例，销毁自身
            if(instance!=null)
            {
                Destroy(this.gameObject);
                return;
            }
            
            // 设置instance后再调用DontDestroyOnLoad，避免场景切换时的竞态条件
            instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        this.OnStart();
    }

    protected virtual void OnStart()
    {

    }
}