using BackEnd;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIWindowMultiGame : UIWindowBase
{
    private const int COLUMNS = 9;
    private const int ROWS = 9;
    private const float SMALLGAP = 5f;
    private const float BIGGAP = 10f;

    private int m_LastSelectNum;
    private int m_LastSelectColumn;
    private int m_LastSelectRow;
    private int m_LastSelectColumnArea;
    private int m_LastSelectRowArea;

    private List<GameObject> m_GridList = new List<GameObject>();
    private int[] m_SolvedNumCnt = new int[9];
    private int m_SelectIndex = -1;
    private int m_Life = 3;
    private float m_Timer = 0f;
    private bool m_IsMemo = false;
    private int m_ResultClearCnt = 0;           // 총 맞춰야 하는 숫자 갯수
    private int m_ClearCnt = 0;                 // 게임 시작 후 맞춘 숫자 갯수
    private int m_FirstSolvedCnt = 0;           // 처음부터 맞추어져 있는 숫자 갯수

    private Stack<CommandPattern> commandStack = new Stack<CommandPattern>();

    [SerializeField]
    private Transform m_BoardBG = null;
    [SerializeField]
    private GameObject m_Square = null;
    [SerializeField]
    private List<GameObject> m_NumberList = new List<GameObject>();
    [SerializeField]
    private List<GameObject> m_LifeList = new List<GameObject>();
    [SerializeField]
    private Text m_TimerTxt = null;
    [SerializeField]
    private Image m_Memo = null;
    [SerializeField]
    private Sprite m_MemoOn = null;
    [SerializeField]
    private Sprite m_MemoOff = null;
    [SerializeField]
    private Image m_MyFlag = null;
    [SerializeField]
    private Text m_MyNickName = null;
    [SerializeField]
    private Text m_MyCount = null;
    [SerializeField]
    private Image m_EnemyFlag = null;
    [SerializeField]
    private Text m_EnemyNickName = null;
    [SerializeField]
    private Text m_EnemyCount = null;
    [SerializeField]
    private RectTransform m_MyBG = null;
    [SerializeField]
    private RectTransform m_EnemyBG = null;


    public override void Awake()
    {
        this.Window_ID = WindowID.UIWindowMultiGame;
        this.Window_Mode = WindowMode.WindowClose;

        base.Awake();
    }

    public override void OpenUI(WindowParam wp)
    {
        base.OpenUI(wp);

        Init();
    }

    private void Update()
    {
        m_Timer += Time.deltaTime;
        TimeSpan span = TimeSpan.FromSeconds(m_Timer);

        string hour = LeadingZero(span.Hours);
        string minute = LeadingZero(span.Minutes);
        string seconds = LeadingZero(span.Seconds);

        m_TimerTxt.text = string.Format("{0}:{1}:{2}", hour, minute, seconds);
    }

    private string LeadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }

    private void Init()
    {
        m_Life = 3;
        m_ClearCnt = 0;

        ProtoSessionID protoData = new ProtoSessionID(Managers.BackEnd.MySessionId, Backend.UserNickName);
        Managers.BackEnd.SendData(protoData);

        CreateInit();
        CreateSquare();
        SetNumber();
        UIInit();   
    }

    private void UIInit()
    {
        m_MyNickName.text = Backend.UserNickName;
        m_MyBG.sizeDelta = new Vector2(Screen.width / 2, 150f);
        m_EnemyBG.sizeDelta = new Vector2(Screen.width / 2, 150f);
    }

    private void ScoreInit()
    {
        m_MyCount.text = string.Format("0/{1}", 0, m_ResultClearCnt);
        m_EnemyCount.text = string.Format("0/{1}", 0, m_ResultClearCnt);
    }

    private void CreateSquare()
    {
        int Index = 0;
        for (int row = 0; row < ROWS; row++)
        {       
            for (int column = 0; column < COLUMNS; column++)
            {
                GameObject go = Instantiate(m_Square);
                go.transform.SetParent(m_BoardBG);
                go.transform.localScale = Vector2.one;

                RectTransform rectTransform = go.GetComponent<RectTransform>();
                float width = rectTransform.rect.width;
                float height = rectTransform.rect.height;

                int columnArea = (int)(column / 3);
                int rowArea = (int)(row / 3);
                float BigGapX = (columnArea - 1) * BIGGAP;
                float BigGapY = (rowArea - 1) * BIGGAP;

                float PosX = -width * (4 - column) + -SMALLGAP * (4 - column) + BigGapX;
                float PoxY = -height * (4 - row) + -SMALLGAP * (4 - row) + BigGapY;

                rectTransform.anchoredPosition = new Vector2(PosX, PoxY * -1);

                var item = go.GetComponent<Square>();
                item.SquareIndex = Index;
                item.ColumnLine = column;
                item.RowLine = row;
                item.ColumnArea = columnArea;
                item.RowArea = rowArea;

                m_GridList.Add(go);
                Index++;
            }
        }
    }

    private void SetNumber()
    {
        if (Managers.Player.IsSuper)
        {
            ProtoCreateBoard protoData = new ProtoCreateBoard(m_RiddleGrid, m_SolvedGrid);
            Managers.BackEnd.SendData(protoData);

            for (int index = 0; index < m_GridList.Count; index++)
            {
                Square square = m_GridList[index].GetComponent<Square>();
                square.SquareSolvedNumber = m_SolvedGrid[index];
                square.SquareNumber = m_RiddleGrid[index];
                if (square.SquareNumber == 0)
                    m_ResultClearCnt++;
                else
                    m_FirstSolvedCnt++;
            }

            ScoreInit();
        }
    }

    public void CreateBoardNumber(int[] riddleGrid, int[] solvedGrid)
    {
        for (int index = 0; index < m_GridList.Count; index++)
        {
            Square square = m_GridList[index].GetComponent<Square>();
            square.SquareSolvedNumber = solvedGrid[index];
            square.SquareNumber = riddleGrid[index];

            if (square.SquareNumber == 0)
                m_ResultClearCnt++;
            else
                m_FirstSolvedCnt++;
        }

        ScoreInit();
    }

    public void EnemyNickName()
    {
        m_EnemyNickName.text = Managers.BackEnd.EnemyNickName;
    }

    public void EnemeyScore(int enemyClearCnt)
    {
        m_EnemyCount.text = string.Format("{0}/{1}", enemyClearCnt, m_ResultClearCnt);
    }

    public void SetColor(int SelectIndex, int SelectNum, int SelectColumn, int SelectRow, int SelectColumnArea, int SelectRowArea)
    {
        Managers.Sound.SetEffect(eEffectSound.Click);

        m_SelectIndex = SelectIndex;
        m_LastSelectNum = SelectNum;
        m_LastSelectColumn = SelectColumn;
        m_LastSelectRow = SelectRow;
        m_LastSelectColumnArea = SelectColumnArea;
        m_LastSelectRowArea = SelectRowArea;

        for (int index = 0; index < m_GridList.Count; index++)
        {
            Square square = m_GridList[index].GetComponent<Square>();
            bool Area = SelectColumnArea == square.ColumnArea && SelectRowArea == square.RowArea;
            bool Num = SelectNum == square.SquareNumber;
            bool Column = SelectColumn == square.ColumnLine;
            bool Row = SelectRow == square.RowLine;

            square.SetBGColer(new Color32(255, 255, 255, 255));

            if (Area || Column || Row)
                square.SetBGColer(new Color32(150, 150, 150, 255));

            if (Num && SelectNum != 0)
                square.SetBGColer(new Color32(255, 255, 0, 255));

            if (SelectIndex == index)
                square.SetBGColer(new Color32(0, 255, 0, 255));
        }
    }

    public void SetCheckNumber(int number)
    {
        if (m_SelectIndex == -1)
            return;

        Square square = m_GridList[m_SelectIndex].GetComponent<Square>();
        if (square.Solved)
            return;

        Managers.Sound.SetEffect(eEffectSound.Pen);
        if (m_IsMemo)
        {   
            SetCheckMemoNumber(number);
            return;
        }

        CommandPop();
        square.SquareNumber = number;
        if (square.Solved)
        {
            CheckSolved(number);
        }
        else
        {
            square.SetBGColer(new Color32(255, 0, 0, 255));

            m_Life--;
            for (int i = 0; i < m_LifeList.Count; i++)
                m_LifeList[i].SetActive(m_Life > i);

            if (m_Life == 0)
            {
                ProtoGameResult protoGameResult = new ProtoGameResult(false);
                Managers.BackEnd.SendData(protoGameResult);
                Managers.BackEnd.GameResult(false);
            }
        }
    }

    private void SetCheckMemoNumber(int number)
    {
        CommandPop();

        Square square = m_GridList[m_SelectIndex].GetComponent<Square>();
        square.SetMemoNumber(number);        
    }

    private void CheckSolved(int number)
    {
        int SolvedCnt = 0;
        for (int i = 0; i < m_SolvedNumCnt.Length; i++)
            m_SolvedNumCnt[i] = 0;

        for (int index = 0; index < m_GridList.Count; index++)
        {
            Square square = m_GridList[index].GetComponent<Square>();
            if (square.Solved)
            {
                SolvedCnt++;
                m_SolvedNumCnt[square.SquareSolvedNumber - 1]++;
            }
        }

        for (int i = 0; i < m_SolvedNumCnt.Length; i++)
        {
            m_NumberList[i].GetComponent<Button>().enabled = true;
            m_NumberList[i].transform.Find("Text").GetComponent<Text>().text = (i + 1).ToString();

            if (m_SolvedNumCnt[i] == 9)
            {
                m_NumberList[i].GetComponent<Button>().enabled = false;
                m_NumberList[i].transform.Find("Text").GetComponent<Text>().text = "";

                for (int index = 0; index < m_GridList.Count; index++)
                {
                    Square square = m_GridList[index].GetComponent<Square>();
                    square.SolvedMemoClear(i);
                }
            }
        }

        m_ClearCnt = SolvedCnt - m_FirstSolvedCnt;
        SetColor(m_SelectIndex, number, m_LastSelectColumn, m_LastSelectRow, m_LastSelectColumnArea, m_LastSelectRowArea);

        m_MyCount.text = string.Format("{0}/{1}", m_ClearCnt, m_ResultClearCnt);
        ProtoScore protoScore = new ProtoScore(m_ClearCnt);
        Managers.BackEnd.SendData(protoScore);

        if (m_ClearCnt == m_ResultClearCnt)
        {
            ProtoGameResult protoGameResult = new ProtoGameResult(true);
            Managers.BackEnd.SendData(protoGameResult);
            Managers.BackEnd.GameResult(true);
        }
    }

    public void OnClickErase()
    {
        if (m_SelectIndex == -1)
            return;

        Square square = m_GridList[m_SelectIndex].GetComponent<Square>();
        if (square.Solved)
            return;

        CommandPop();

        Managers.Sound.SetEffect(eEffectSound.Erase);
        square.SquareNumber = 0;        
    }

    public void OnClickMeno()
    {
        m_IsMemo = !m_IsMemo;
        if (m_IsMemo)
            m_Memo.sprite = m_MemoOn;
        else
            m_Memo.sprite = m_MemoOff;
    }

    public void OnClickGiveUP()
    {
        ProtoGameResult protoGameResult = new ProtoGameResult(false);
        Managers.BackEnd.SendData(protoGameResult);
        Managers.BackEnd.GameResult(false);
    }

    private void CommandPop()
    {
        int[] riddleArr = new int[81];
        int[,] memoArr = new int[81, 9];

        for (int i = 0; i <  m_GridList.Count; i++)
        {
            Square square = m_GridList[i].GetComponent<Square>();
            riddleArr[i] = square.SquareNumber;

            var MemoList = square.GetMomoList();
            for (int j = 0; j < MemoList.Count; j++)
            {
                memoArr[i, j] = MemoList[j].gameObject.activeSelf ? 1 : 0;
            }
        }

        commandStack.Push(new NumberInputCommand(m_LastSelectNum, riddleArr, memoArr));
    }

    public void OnClickUndo()
    {
        Managers.Sound.SetEffect(eEffectSound.Undo);

        int num = 0;
        if (commandStack.Count > 0)
        {
            var command = commandStack.Pop();
            num = command.m_LastNum;

            for (int index = 0; index < m_GridList.Count; index++)
            {
                Square square = m_GridList[index].GetComponent<Square>();
                square.SquareNumber = command.m_RiddleArr[index];

                for (int j = 0; j < 9; j++)
                {
                    bool active = command.m_MemoArr[index, j] == 1;
                    square.SetMemoActive(j + 1, active);
                }
            }
        }

        CheckSolved(num);
    }
}