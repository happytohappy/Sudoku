using UnityEngine;
using System.Collections.Generic;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine.SceneManagement;

public partial class BackEndManager : MonoBehaviour
{
    private string roomToken = string.Empty;

    public bool MatchServer { get; private set; }
    public SessionId MySessionId { get; set; }
    public SessionId EnemySessionId { get; set; }
    public string EnemyNickName { get; set; }

    // 뒤끝 매치 초기화
    public void MatchServerInit()
    {
        // 이벤트 핸들러 초기화
        MatchMakingHandler();
        MatchHandler();

        // 매칭 서버 접속 시도
        MatchServerJoin();
    }

    private void MatchServerJoin()
    {
        ErrorInfo errorInfo;
        Backend.Match.JoinMatchMakingServer(out errorInfo);

        if (errorInfo.Category == ErrorCode.Success)
            Debug.LogAssertion("매칭 서버 접속 시도");
        else
            Debug.LogAssertion("뒤끝 매칭 서버 접속 실패");
    }


    private void MatchMakingHandler()
    {
        // 매칭 서버 접속 관련 작업에 대한 호출
        Backend.Match.OnJoinMatchMakingServer += (args) =>
        {
            if (args.ErrInfo.Category == ErrorCode.Success)
            {
                MatchServer = true;
                Debug.LogAssertion("매칭 서버 접속 성공");
            }
            else
            {
                MatchServer = false;
                Debug.LogAssertion("매칭 서버 접속 실패 : " + args.ErrInfo.Reason);
            }
        };

        // 매칭 신청 관련 작업에 대한 호출
        Backend.Match.OnMatchMakingResponse += (args) =>
         {
             Debug.Log("OnMatchMakingResponse : " + args.ErrInfo);
             ProcessMatchMakingResponse(args);
         };

        Backend.Match.OnLeaveMatchMakingServer += (args) =>
        {
            Debug.Log("OnLeaveMatchMakingServer : " + args.ErrInfo);

             // 매칭 서버에서 접속 종료할 때 호출
             Debug.Log("매치메이킹 서버 접속 종료 : " + args.ErrInfo);

            MatchServerInit();
        };

        Backend.Match.OnSessionOffline += (args) =>
        {

        };
    }


    public void MatchMakingTry()
    {
        Backend.Match.CreateMatchRoom();
        Backend.Match.RequestMatchMaking(MatchType.Point, MatchModeType.OneOnOne, "2021-09-21T10:18:17.838Z");
    }

    private void ProcessMatchMakingResponse(MatchMakingResponseEventArgs args)
    {
        switch (args.ErrInfo)
        {
            // 매칭 성공했을 때
            case ErrorCode.Success:
                Debug.LogAssertion("상대방 유저와 매칭 성공");
                ProcessMatchSuccess(args);
                break;
            // 매칭 신청 성공했을 때
            case ErrorCode.Match_InProgress:
                Debug.LogAssertion("매칭 시도 성공");
                break;
            // 매칭 신청이 취소되었을 때
            case ErrorCode.Match_MatchMakingCanceled:
                Debug.LogAssertion("매칭 취소 성공");
                break;
            // 매칭 타입을 잘못 전송했을 때
            case ErrorCode.Match_InvalidMatchType:
                Debug.LogAssertion("매칭 타입 오류");
                break;
            // 매칭 모드를 잘못 전송했을 때
            case ErrorCode.Match_InvalidModeType:
                Debug.LogAssertion("매칭 모드 오류");
                break;
            // 잘못된 요청을 전송했을 때
            case ErrorCode.InvalidOperation:
                Debug.LogAssertion("잘못된 요청");
                MatchMakingTry();
                break;
            // 매칭 되고, 서버에서 방 생성할 때 에러 발생 시 exception이 리턴됨 [다시 매칭을 신청하자.]
            case ErrorCode.Exception:
                Debug.LogAssertion("예외 발생 : " + args.Reason);
                MatchMakingTry();
                break;
        }
    }

    // 매칭 성공하여 룸으로 입장
    private void ProcessMatchSuccess(MatchMakingResponseEventArgs args)
    {
        ErrorInfo errorInfo;
        if (Backend.Match.JoinGameServer(args.RoomInfo.m_inGameServerEndPoint.m_address, args.RoomInfo.m_inGameServerEndPoint.m_port, false, out errorInfo) == false)
        {
            Debug.LogAssertion("JoinGameServer Error");
            // 에러 확인
            return;
        }
        else
        {
            roomToken = args.RoomInfo.m_inGameRoomToken;
        }

    }

    private void MatchHandler()
    {
        // 룸 입장 성공 관련 작업
        Backend.Match.OnSessionJoinInServer += (args) =>
        {
            Backend.Match.JoinGameRoom(roomToken);
        };

        Backend.Match.OnSessionListInServer += (args) =>
        {

        };

        Backend.Match.OnMatchInGameAccess += (args) =>
        {
             // 세션이 인게임 룸에 접속할 때마다 호출 (각 클라이언트가 인게임 룸에 접속할 때마다 호출됨)
             if (Backend.UserNickName == args.GameRecord.m_nickname)
            {
                MySessionId = args.GameRecord.m_sessionId;
                Managers.Player.IsSuper = args.GameRecord.m_isSuperGamer;
            }
        };

        // 게임 시작 응답 받았을 시
        Backend.Match.OnMatchInGameStart += () =>
        {
            Managers.UI.WindowInit();
            SceneManager.LoadScene("MultiGame");
        };

        // 다른 클라가 메시지 보냈을 때 받는 작업
        Backend.Match.OnMatchRelay += (args) =>
        {
            OnRecieve(args);
        };

        Backend.Match.OnMatchResult += (args) =>
        {
            if (args.ErrInfo == BackEnd.Tcp.ErrorCode.Success)
            {
                Debug.LogAssertion("결과 정상 반영 성공");

                var bro = Backend.Match.GetMatchRecord(Backend.UserInDate, BackEnd.Tcp.MatchType.MMR, BackEnd.Tcp.MatchModeType.OneOnOne, "2021-09-21T10:18:17.838Z", 1);
                var MyMMR = bro.GetReturnValuetoJSON()["rows"][0]["mmr"]["N"].ToString();

                Param param = new Param();
                param.Add("MMR", int.Parse(MyMMR));

                Backend.GameData.Update("DB_UserMMR", Managers.Player.MMRDBIndate, param);
                Debug.LogAssertion("MMR DB Update 완료");
            }
            else
            {
                Debug.LogAssertion("결과 정상 반영 실패");
            }

            Managers.UI.WindowInit();
            SceneManager.LoadScene("Main");
        };
    }

    public void SendData<T>(T msg)
    {
        var byteArray = Utility.DataToJsonData<T>(msg);
        Backend.Match.SendDataToInGameRoom(byteArray);
    }

    private void OnRecieve(MatchRelayEventArgs args)
    {
        if (args.BinaryUserData == null)
        {
            Debug.LogWarning(string.Format("빈 데이터가 브로드캐스팅 되었습니다.\n{0} - {1}", args.From, args.ErrInfo));
            return;
        }

        Message msg = Utility.ReadJsonData<Message>(args.BinaryUserData);
        if (msg == null)
            return;

        if (Backend.UserNickName == args.From.NickName)
            return;

        switch (msg.type)
        {
            case eType.CreateBoard:
                {
                    if (Managers.Player.IsSuper)
                        return;

                    ProtoCreateBoard protoCreateBoard = Utility.ReadJsonData<ProtoCreateBoard>(args.BinaryUserData);
                    var uiMultiGame = Managers.UI.GetWindow(WindowID.UIWindowMultiGame, false) as UIWindowMultiGame;
                    if (uiMultiGame != null)
                    {
                        uiMultiGame.CreateBoardNumber(protoCreateBoard.GetUnsolvedData(), protoCreateBoard.GetSolvedData());
                    }
                    break;
                }
            case eType.SessionID:
                {
                    ProtoSessionID protoSessionID = Utility.ReadJsonData<ProtoSessionID>(args.BinaryUserData);
                    Managers.BackEnd.EnemySessionId = protoSessionID.sessionID;
                    Managers.BackEnd.EnemyNickName = protoSessionID.nickName;
                    var uiMultiGame = Managers.UI.GetWindow(WindowID.UIWindowMultiGame, false) as UIWindowMultiGame;
                    if (uiMultiGame != null)
                    {
                        uiMultiGame.EnemyNickName();
                    }
                    break;
                }
            case eType.GameResult:
                {
                    ProtoGameResult protoGameResult = Utility.ReadJsonData<ProtoGameResult>(args.BinaryUserData);
                    if (protoGameResult.victory == true)
                    {
                        // 나는 진거임
                        Managers.BackEnd.GameResult(false);

                    }
                    else
                    {
                        // 내가 이긴거임
                        Managers.BackEnd.GameResult(true);
                    }
                    break;
                }
            case eType.Score:
                {
                    ProtoScore protoScore = Utility.ReadJsonData<ProtoScore>(args.BinaryUserData);
                    var uiMultiGame = Managers.UI.GetWindow(WindowID.UIWindowMultiGame, false) as UIWindowMultiGame;
                    if (uiMultiGame != null)
                    {
                        uiMultiGame.EnemeyScore(protoScore.solved);
                    }
                    break;
                }
            default:
                Debug.Log("Unknown protocol type");
                return;
        }
    }

    public void GameResult(bool victory)
    {
        MatchGameResult matchGameResult = new MatchGameResult();
        matchGameResult.m_winners = new List<SessionId>();
        matchGameResult.m_losers = new List<SessionId>();

        if (victory)
        {
            matchGameResult.m_winners.Add(MySessionId);
            matchGameResult.m_losers.Add(EnemySessionId);
        }
        else
        {
            matchGameResult.m_winners.Add(EnemySessionId);
            matchGameResult.m_losers.Add(MySessionId);
        }

        Backend.Match.MatchEnd(matchGameResult);
    }

    public void LeaveServer()
    {
        Backend.Match.LeaveGameServer();
    }

    public void MatchingInit(bool _PlayGame = false)
    {
        Backend.Match.CancelMatchMaking();
    }
}