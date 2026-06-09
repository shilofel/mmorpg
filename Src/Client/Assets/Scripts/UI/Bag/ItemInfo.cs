using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Common.Data;

public class ItemInfo : MonoBehaviour
{

    public Text title;

    public Text description;

    // 关闭按钮
    public UnityEngine.UI.Button closeButton;

    // 使用按钮
    public UnityEngine.UI.Button useButton;

    // 丢弃按钮
    public UnityEngine.UI.Button discardButton;

    private ItemDefine itemInfo;
    private int itemId;

    // Start is called before the first frame update
    void Start()
    {
        // 绑定关闭按钮事件
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnClickClose);
        }

        // 绑定使用按钮事件
        if (useButton != null)
        {
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(OnClickUse);
        }

        // 绑定丢弃按钮事件
        if (discardButton != null)
        {
            discardButton.onClick.RemoveAllListeners();
            discardButton.onClick.AddListener(OnClickDiscard);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetItemInfo(int ItemId)
    {
        this.itemId = ItemId;
        ItemDefine item;
        DataManager.Instance.Items.TryGetValue(ItemId, out item);
        if (item != null)
        {
            itemInfo = item;

            this.title.text = string.Format("{0}", item.Name);

            if (description != null)
            {
                this.description.text = item.Description;
            }

            // 根据道具类型显示/隐藏使用按钮
            if (useButton != null)
            {
                // 只有消耗品才能使用
                useButton.gameObject.SetActive(item.Type == SkillBridge.Message.ItemType.Normal);
            }
        }
    }

    /// <summary>
    /// 清除道具信息
    /// </summary>
    public void ClearInfo()
    {
        this.itemInfo = null;
        this.itemId = 0;

        if (title != null)
        {
            title.text = "";
        }

        if (description != null)
        {
            description.text = "";
        }
    }

    /// <summary>
    /// 关闭按钮点击事件
    /// </summary>
    public void OnClickClose()
    {
        // 隐藏面板
        this.gameObject.SetActive(false);
        ClearInfo();
    }

    /// <summary>
    /// 使用按钮点击事件
    /// </summary>
    public void OnClickUse()
    {
        Debug.LogFormat("使用道具: {0}", itemId);
        // TODO: 实现使用道具的逻辑
        // 可以在这里调用 BagManager 或其他管理器的方法
    }

    /// <summary>
    /// 丢弃按钮点击事件
    /// </summary>
    public void OnClickDiscard()
    {
        Debug.LogFormat("丢弃道具: {0}", itemId);
        // TODO: 实现丢弃道具的逻辑
        // 可以在这里调用 BagManager 或其他管理器的方法
    }
}
