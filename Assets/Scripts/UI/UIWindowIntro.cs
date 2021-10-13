using UnityEngine;
using UnityEngine.SceneManagement;

public class UIWindowIntro : UIWindowBase
{
    private bool IsInit = false;

    public override void Awake()
    {
        this.Window_ID = WindowID.UIWindowIntro;
        this.Window_Mode = WindowMode.WindowClose;

        base.Awake();
    }

    public override void OpenUI(WindowParam wp)
    {
        base.OpenUI(wp);
    }

    public void Init()
    {
        IsInit = true;
    }

    private void Update()
    {
        if (Managers.BackEnd.ServerStatus && IsInit)
        {
            IsInit = false;
            Managers.UI.WindowInit();
            SceneManager.LoadScene("Main");
        }
    }
}