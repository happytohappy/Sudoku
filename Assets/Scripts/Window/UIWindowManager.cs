using System.Collections.Generic;
using UnityEngine;

public class UIWindowManager : MonoBehaviour
{
    private readonly Dictionary<WindowID, UIWindowBase> dic_WindowInsts = new Dictionary<WindowID, UIWindowBase>();
    private readonly LinkedList<UIWindowBase> llist_Window = new LinkedList<UIWindowBase>();

    public void Init()
    {
        //Managers.UI.OpenWindow(WindowID.UIWindowGame);
    }

    public void WindowInit()
    {
        for (int i = 0; i < Managers.UICanvas.transform.childCount; i++)
            Destroy(Managers.UICanvas.transform.GetChild(i).gameObject);
        
        dic_WindowInsts.Clear();
        llist_Window.Clear();
    }

    public UIWindowBase OpenWindow(WindowID windowID, WindowParam wp = null)
    {
        UIWindowBase openbase = null;
        openbase = this.GetWindow(windowID, true);

        LinkedListNode<UIWindowBase> lastNode = this.llist_Window.Last;
        if (lastNode != null)
        {
            UIWindowBase closeWindow = lastNode.Value;

            if ((openbase.Window_Mode & WindowMode.WindowOverlay) != WindowMode.WindowOverlay)
                closeWindow.gameObject.SetActive(false);
        }

        this.llist_Window.AddLast(openbase);

        openbase.gameObject.SetActive(true);
        openbase.gameObject.transform.SetAsLastSibling();

        openbase.OpenUI(wp);
        return openbase;
    }

    public UIWindowBase GetWindow(WindowID windowID, bool ExistCreate)
    {
        UIWindowBase result = null;
        if (this.dic_WindowInsts.ContainsKey(windowID))
            return this.dic_WindowInsts[windowID];

        if (ExistCreate)
        {
            var prefabs = Utility.LoadPrefab(windowID.ToString());
            prefabs.name = windowID.ToString();

            result = prefabs.GetComponent(typeof(UIWindowBase)) as UIWindowBase;
            this.dic_WindowInsts.Add(windowID, result);
        }

        return result;
    }

    public void AllDisableWindow()
    {
        llist_Window.Clear();
        foreach (var data in dic_WindowInsts)
        {
            data.Value.gameObject.SetActive(false);
        }
    }

    public bool ActiveWindow(WindowID windowID)
    {
        if (this.dic_WindowInsts.ContainsKey(windowID))
        {
            return this.dic_WindowInsts[windowID].gameObject.activeInHierarchy;
        }
        else
        {
            return false;
        }
    }

    public void CloseLast()
    {
        if (this.llist_Window.Last == null)
            return;

        var closeWindow = this.llist_Window.Last.Value;

        this.llist_Window.RemoveLast();
        if ((closeWindow.Window_Mode & WindowMode.WindowJustClose) == WindowMode.WindowJustClose)
        {
            closeWindow.gameObject.SetActive(false);
            return;
        }

        LinkedListNode<UIWindowBase> lastNode = this.llist_Window.Last;
        UIWindowBase openWindow = lastNode.Value;

        openWindow.OpenUI(openWindow.Window_Param);
    }
}