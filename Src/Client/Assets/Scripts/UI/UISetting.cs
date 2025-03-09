using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UISetting:UIWindow
{
    public void ExitToCharSelect()
    {
        UIPopCharMenu menu = UIManager.Instance.Show<UIPopCharMenu>();
        //SceneManager.Instance.LoadScene("CharSelect");
        //Services.UserService.Instance.SendGameLeave();
    }

    public void ExitGame()
    {
        Services.UserService.Instance.SendGameLeave(true);
    }
}