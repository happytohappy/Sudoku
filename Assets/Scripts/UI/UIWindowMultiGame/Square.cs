using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
    [SerializeField]
    private Text m_Number = null;
    [SerializeField]
    private Image m_BG = null;
    [SerializeField]
    private List<GameObject> m_MemoNumber = new List<GameObject>();

    private int m_SquareNumber = 0;

    public int SquareIndex { get; set; }
    public int ColumnLine { get; set; }
    public int RowLine { get; set; }
    public int ColumnArea { get; set; }
    public int RowArea { get; set; }
    public int SquareSolvedNumber { get; set; }     // 정답 숫자, 몇 개만 공개되어 있으며 정답을 비교하는 용도이다.

    public int SquareNumber                         // 유저가 현재 풀고 있는 숫자.
    {
        get
        {
            return m_SquareNumber;
        }
        set
        {
            m_SquareNumber = value;
            m_Number.text = m_SquareNumber == 0 ? "" : m_SquareNumber.ToString();
            for (int i = 0; i < m_MemoNumber.Count; i++)
                m_MemoNumber[i].SetActive(false);
        }
    }

    public bool Solved
    {
        get
        {
            return SquareSolvedNumber == SquareNumber;
        }
    }

    public void OnClickNumber()
    {
        var UIGame = Managers.UI.GetWindow(WindowID.UIWindowMultiGame, false) as UIWindowMultiGame;
        if (UIGame != null)
            UIGame.SetColor(SquareIndex, SquareNumber, ColumnLine, RowLine, ColumnArea, RowArea);
    }

    public void SetBGColer(Color32 color)
    {
        m_BG.color = color;
    }

    public void SetMemoNumber(int number)
    {
        bool Active = m_MemoNumber[number - 1].activeSelf;
        m_MemoNumber[number - 1].SetActive(!Active);
    }

    public void SetMemoActive(int number, bool active)
    {
        m_MemoNumber[number - 1].SetActive(active);
    }

    public void SolvedMemoClear(int index)
    {
        m_MemoNumber[index].SetActive(false);
    }

    public List<GameObject> GetMomoList()
    {
        return m_MemoNumber;
    }
}