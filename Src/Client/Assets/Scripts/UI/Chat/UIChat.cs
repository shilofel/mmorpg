using UnityEngine.UI;
using Managers;
using System;
using TMPro;

public class UIChat : UIWindow
{
    public TextMeshProUGUI textArea;

    public TabView channelTab;

    public InputField chatText;
    public Text chatTarget;

    public Dropdown channelSelect;

    private void Start()
    {
        this.channelSelect.onValueChanged.AddListener(OnSendChannelChanged);
        this.channelTab.OnTabSelect += OnDisplayChannelSelected;
        ChatManager.Instance.OnChat += RefreshUI;
    }

    private void OnDestroy()
    {
        ChatManager.Instance.OnChat -= RefreshUI;
    }

    private void Update()
    {
        InputManager.Instance.IsInputMode = chatText.isFocused;
    }

    public void OnDisplayChannelSelected(int idx)
    {
        ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)idx;
        RefreshUI();
    }

    public void RefreshUI()
    {
        this.textArea.text = ChatManager.Instance.GetCurrentMeesage();
        this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        if(ChatManager.Instance.SendChannel == SkillBridge.Message.ChatChannel.Private)
        {
            this.chatTarget.gameObject.SetActive(true);
            if (ChatManager.Instance.PrivateID != 0)
            {
                this.chatTarget.text = ChatManager.Instance.PrivateName + ":";
            }
            else
                this.chatTarget.text = "<无>:";
        }
        else
        {
            this.chatTarget.gameObject.SetActive(false);
        }
    }
    //点击聊天中的超链接
    /*
    public void OnClickChatLink(HyperText text, HyperText.LinkInfo link)
    {
        if (string.IsNullOrEmpty(link.Name))
            return;
        //<a name="c:1001:Name" class="player">Name</a>  自定义超链接字符串规则
        if(link.Name.StartsWith("c:"))
        {
            string[] strs = link.Name.Split(":".ToCharArray());
            UIPopCharMenu menu = UIManager.Instance.Show<UIPopCharMenu>();
            menu.targetId = int.Parse(strs[1]);
            menu.targetName = strs[2];
        }
    }*/

    public void ClickSend()
    {
        OnEndInput(this.chatText.text);
    }

    public void OnEndInput(string text)
    {
        if (!string.IsNullOrEmpty(text))
            this.SendChat(text);

        this.chatText.text = "";
    }

    public void SendChat(string content)
    {
        ChatManager.Instance.SendChat(content, ChatManager.Instance.PrivateID, ChatManager.Instance.PrivateName);
    }
    //切换聊天频道
    public void OnSendChannelChanged(Int32 idx)
    {
        //idx = channelSelect.value;
        //发送频道没有综合，即所有频道
        if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(idx + 1))
            return;
        //设置失败，回去
        if (!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)idx + 1))
        {
            this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        }
        else
        {
            this.RefreshUI();
        }
    }
}
