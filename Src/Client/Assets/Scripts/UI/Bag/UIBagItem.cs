using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIBagItem : UIIconItem, IPointerClickHandler
{
    // 点击事件回调
    public System.Action<UIBagItem> OnItemClick;

    // 道具ID
    private int itemId;
    
    /// <summary>
    /// 获取道具ID
    /// </summary>
    public int ItemId
    {
        get { return itemId; }
    }

    public void SetBagItemIcon(string iconName, string text, int itemId)
    {
        this.itemId = itemId;
        this.mainImage.overrideSprite = Resloader.Load<Sprite>(iconName);
        this.mainText.text = text;
        if (secondImage != null)
        {
            secondImage.enabled = true;
        }
    }
    
    /// <summary>
    /// 实现IPointerClickHandler接口
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnItemClick != null)
        {
            OnItemClick(this);
        }
    }
}
