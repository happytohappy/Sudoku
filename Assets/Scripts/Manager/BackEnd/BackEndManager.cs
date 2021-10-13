using UnityEngine;
using BackEnd;

public partial class BackEndManager : MonoBehaviour
{
    public bool ServerStatus { get; private set; }
    public bool Token { get; private set; }

    public void Init()
    {
        var bro = Backend.Initialize(true);
        if (bro.IsSuccess())
        {           
            // 초기화 성공 시 로직
            Debug.Log(Backend.Utils.GetServerStatus());

            if (TokenCheck())
            {
                ServerStatus = true;
                Token = Backend.UserNickName != string.Empty;
                Debug.LogAssertion("Backend.UserNickName : " + Backend.UserNickName);
            }
            else
            {
                ServerStatus = false;
                Debug.LogError("로그인 실패");
            }
        }
        else
        {
            // 초기화 실패 시 로직
            ServerStatus = false;
            Debug.LogError("Failed to initialize the backend");
        }
    }

    private bool TokenCheck()
    {
        if (PlayerPrefs.HasKey("Account"))
        {
            BackendReturnObject BRO = Backend.BMember.CustomLogin(PlayerPrefs.GetString("Account"), PlayerPrefs.GetString("Account"));
            Debug.LogAssertion(BRO.GetStatusCode());

            if (BRO.GetStatusCode() == "200")
            {
                return true;
            }
            else
            {
                if (BRO.GetMessage() == "bad customId, 잘못된 customId 입니다")
                {
                    CreateCustomAccount();
                    return true;
                }

                return false;
            }
        }
        else
        {
            CreateCustomAccount();

            return true;
        }
    }

    private void CreateCustomAccount()
    {
        string UID = "";
        for (int i = 0; i < 20; i++)
        {
            int ranC = UnityEngine.Random.Range(0, 100);
            if (ranC % 2 == 0)
            {
                int ranA = UnityEngine.Random.Range(65, 91);
                UID += (char)ranA;
            }
            else
            {
                int ranN = UnityEngine.Random.Range(0, 10);
                UID += ranN;
            }
        }

        Backend.BMember.CustomSignUp(UID, UID);
        Backend.BMember.CustomLogin(UID, UID);

        PlayerPrefs.SetString("Account", UID);
    }

    private void Update()
    {
        Backend.Match.Poll();
        Backend.Chat.Poll();
    }

    public void UserDataUpdate()
    {
        //Param updateParam = new Param();
        //updateParam.AddCalculation("UserNickName", GameInfoOperator., 10); // 기존 데이터 atk에서 10만큼 더 추가
        //updateParam.AddCalculation("def", GameInfoOperator.subtraction, 20); // 기존 데이터 def에서 20만큼 감소
        //updateParam.AddCalculation("exp", GameInfoOperator.division, 2); // 기존 데이터 exp 1/2 감소
        //updateParam.AddCalculation("money", GameInfoOperator.multiplication, 1.25); // 기존 데이터 money 1.25배 증가


        //// itemCode가 sword_dragon인 제일 최근 데이터 조회
        //Where where = new Where();
        //where.Equal("itemCode", "sword_dragon");

        //Backend.GameData.UpdateWithCalculation("tableName", where, updateParam);
    }    

    private void OnApplicationQuit()
    {
        Managers.BackEnd.LeaveServer();
        Backend.Notification.DisConnect();
    }
}