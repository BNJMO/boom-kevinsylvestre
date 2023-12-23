using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public Menu[] menus;
    public static MenuManager Instance;
    //open menu through scripts
    private void Awake()
    {
        Instance = this;
    }
    public void OpenMenu(string menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if(menus[i].menuName == menuName)
            {
                 menus[i].Open();
            }else if (menus[i].isOpen)
            {
                CloseMenu(menus[i]);
            }
        }
    }
    
    //open menu through button 
    public void OpenMenu(Menu menuObj)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].isOpen)
            {
                CloseMenu(menus[i]);
            }
        }
        menuObj.Open();
    }

    public void CloseMenu(Menu menuObj)
    {
        menuObj.Close();
    }

    public void CloseMenu(string menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName)
            {
                menus[i].Close();
            }
        }
    }
}
