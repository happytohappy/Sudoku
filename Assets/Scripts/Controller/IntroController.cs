using UnityEngine;

public class IntroController : MonoBehaviour
{
    private void Start()
    {
        Managers.UI.OpenWindow(WindowID.UIWindowIntro);
    }
}