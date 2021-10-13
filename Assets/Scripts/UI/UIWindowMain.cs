using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using LitJson;

public class UIWindowMain : UIWindowBase
{
    [Header("CreateNickName")]
    [SerializeField]
    private GameObject m_NickName = null;
    [SerializeField]
    private InputField m_NickNameTxt = null;

    [Header("Main")]
    [SerializeField]
    private Text m_UserNickName = null;
    [SerializeField]
    private GameObject m_Competiteve = null;
    [SerializeField]
    private Text m_MyScore = null;
    [SerializeField]
    private Text m_MyRankPer = null;
    [SerializeField]
    private Text m_MyTopRank = null;

    [Header ("Competiteve")]
    [SerializeField]
    private Text m_MyCompetiteveScore = null;
    [SerializeField]
    private Text m_MyCompetiteveRankPer = null;
    [SerializeField]
    private Text m_MyCompetiteveTopRank = null;

    public override void Awake()
    {
        base.Awake();

        this.Window_ID = WindowID.UIWindowMain;
        this.Window_Mode = WindowMode.WindowClose;

        AwakeInit();
    }

    public override void OpenUI(WindowParam wp)
    {    
        base.OpenUI(wp);
    }

    public void OnClickClose()
    {
        Managers.UI.CloseLast();
    }

    private void AwakeInit()
    {
        if (Managers.BackEnd.Token)
        {
            m_NickName.SetActive(false);
            Init();
        }
        else
        {
            m_NickName.SetActive(true);
        }
    }

    private void Init()
    {   
        if (Managers.BackEnd.MatchServer == false)
            Managers.BackEnd.MatchServerInit();

        DataBaseInit();

        m_UserNickName.text = string.Format("Hi, {0} 님", Backend.UserNickName);
        m_MyScore.text = string.Format("점수 {0}", Managers.Player.MyMMR);
        m_MyRankPer.text = string.Format("상위 {0} % ▼", 100);
        m_MyTopRank.text = "임시 45등";

        m_MyCompetiteveScore.text = string.Format("점수 {0}", Managers.Player.MyMMR);
        m_MyCompetiteveRankPer.text = string.Format("상위 {0} % ▼", 100);
        m_MyCompetiteveTopRank.text = "임시 45등";
    }

    private void DataBaseInit()
    {
        bool FirstRank = false;
        var broDB = Backend.GameData.GetMyData("DB_UserMMR", new Where(), 1);
        if (broDB.GetReturnValuetoJSON()["rows"].Count == 0)
        {
            Param param = new Param();
            param.Add("NickName", Backend.UserNickName);
            param.Add("MMR", 1000);

            Backend.GameData.Insert("DB_UserMMR", param);
            broDB = Backend.GameData.GetMyData("DB_UserMMR", new Where(), 1);
            FirstRank = true;
        }

        Managers.Player.MMRDBIndate = broDB.GetReturnValuetoJSON()["rows"][0]["inDate"]["S"].ToString();
        var MMRStr = broDB.GetReturnValuetoJSON()["rows"][0]["MMR"]["N"].ToString();
        Managers.Player.MyMMR = int.Parse(MMRStr);

        if (FirstRank)
        {
            Param rankParam = new Param();
            rankParam.Add("MMR", Managers.Player.MyMMR);
            rankParam.Add("NickName", Backend.UserNickName);

            var aaa = Backend.URank.User.UpdateUserScore("2dfb50a0-2c30-11ec-84b9-659f6192d902", "DB_UserMMR", Managers.Player.MMRDBIndate, rankParam);
            Debug.LogAssertion(aaa);
        }
    }

    public void OnClickCreateNickName()
    {
        var bro = Backend.BMember.UpdateNickname(m_NickNameTxt.text);
        switch (bro.GetStatusCode())
        {
            case "204":
                Init();
                m_NickName.SetActive(false);
                break;
            case "400":
                {
                    ToastParam toastParam = new ToastParam();
                    toastParam.Message = "잘못된 값 입니다";
                    Managers.UI.OpenWindow(WindowID.UIPopupToastMessage, toastParam);
                    break;
                }
            case "409":
                {
                    ToastParam toastParam = new ToastParam();
                    toastParam.Message = "중복된 닉네임 입니다.";
                    Managers.UI.OpenWindow(WindowID.UIPopupToastMessage, toastParam);
                break;
                }
        }
    }

    public void OnClickCompetiteve(bool search)
    {
        if (Managers.BackEnd.MatchServer == false)
        {
            ToastParam toastParam = new ToastParam();
            toastParam.Message = "매칭 서버가 불안정 합니다.";
            Managers.UI.OpenWindow(WindowID.UIPopupToastMessage, toastParam);
            return;
        }

        if (search)
        {
            Managers.BackEnd.MatchMakingTry();
            m_Competiteve.SetActive(true);
        }
        else
        {
            Backend.Match.CancelMatchMaking();
            m_Competiteve.SetActive(false);
        }
    }
}