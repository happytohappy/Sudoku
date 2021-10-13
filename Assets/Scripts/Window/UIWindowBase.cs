using UnityEngine;

public class UIWindowBase : MonoBehaviour
{
    public WindowID Window_ID { get; set; }
    public WindowMode Window_Mode { get; set; }
    public WindowParam Window_Param { get; private set; }

    public virtual void Awake()
    {

    }

    public virtual void OpenUI(WindowParam wp)
    {
        this.transform.SetAsLastSibling();
        this.Window_Param = wp;
    }
}