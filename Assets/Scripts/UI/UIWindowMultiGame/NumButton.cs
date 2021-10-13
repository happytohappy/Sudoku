using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumButton : MonoBehaviour
{
    [SerializeField]
    private int m_Number;

    public void OnClickNumber()
    {
        var UIGame = Managers.UI.GetWindow(WindowID.UIWindowMultiGame, false) as UIWindowMultiGame;
        if (UIGame != null)
            UIGame.SetCheckNumber(m_Number);
    }
}
