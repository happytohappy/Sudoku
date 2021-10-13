using UnityEngine;
using UnityEngine.UI;

public class UIPopupToastMessage : UIWindowBase
{
    [SerializeField]
    private Text m_Message = null;

    private const int MESSAGETIME = 2;
    private float m_Time = 0f;

    public override void Awake()
    {
        base.Awake();

        this.Window_ID = WindowID.UIPopupToastMessage;
        this.Window_Mode = WindowMode.WindowOverlay | WindowMode.WindowJustClose;
    }

    public override void OpenUI(WindowParam wp)
    {    
        base.OpenUI(wp);

        ToastParam toastParam = wp as ToastParam;
        Init(toastParam.Message);
    }

    public void OnClickClose()
    {
        Managers.UI.CloseLast();
    }

    private void Init(string message)
    {
        m_Time = 0f;
        m_Message.text = message;
    }

    private void Update()
    {
        m_Time += Time.deltaTime;
        if (m_Time > MESSAGETIME)
        {
            OnClickClose();
        }
    }
}