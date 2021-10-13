using UnityEngine;

public class MultiGameController : MonoBehaviour
{
    private void Start()
    {
        Managers.UI.OpenWindow(WindowID.UIWindowMultiGame);
    }
}