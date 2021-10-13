using UnityEngine;

public class MainController : MonoBehaviour
{
    private void Start()
    {
        Managers.UI.OpenWindow(WindowID.UIWindowMain);
    }
}