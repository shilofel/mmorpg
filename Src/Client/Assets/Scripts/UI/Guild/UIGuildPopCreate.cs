using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using Services;
public class UIGuildPopCreate : UIWindow
{
    public InputField inputName;
    public InputField inputNotice;

    private void Start()
    {
        GuildService.Instance.OnGuildCreateResult = OnGuildCreated;
    }

    private void OnDestory()
    {
        GuildService.Instance.OnGuildCreateResult = null;
    }

    public override void OnYesClick()
    {
        if (string.IsNullOrEmpty(inputName.text))
        {
            MessageBox.Show("请输入公会名称", "错误",MessageBoxType.Error);
            return;
        }

        if (inputName.text.Length<4||inputName.text.Length>10)
        {
            MessageBox.Show("公会名称应在4-10个字符之间", "错误", MessageBoxType.Error);
            return;
        }

        if (string.IsNullOrEmpty(inputNotice.text))
        {
            MessageBox.Show("请输入公会宣言", "错误", MessageBoxType.Error);
            return;
        }


        if (inputNotice.text.Length < 4 || inputNotice.text.Length > 50)
        {
            MessageBox.Show("公会宣言应在4-50个字符之间", "错误", MessageBoxType.Error);
            return;
        }
        GuildService.Instance.SendGuildCreate(inputName.text, inputNotice.text);
    }

    void OnGuildCreated(bool result)
    {
        if (result)
            this.Close(WindowResult.Yes);
    }
}