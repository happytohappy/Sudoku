using BackEnd.Tcp;
using System;

public abstract class WindowParam
{

}

public class LoadingParam : WindowParam
{
    public int SceneIndex = 0;
    public WindowID NextWindow = WindowID.None;
    public WindowParam Param = null;
}

public class ToastParam : WindowParam
{
    public string Message = null;
}


public class Message
{
    public eType type;

    public Message(eType type)
    {
        this.type = type;
    }
}

[Serializable]
public class ProtoCreateBoard : Message
{
    public int[] unsolved_data;
    public int[] solved_data;

    public ProtoCreateBoard(int[] _unsolved_data, int[] _solved_data) : base(eType.CreateBoard)
    {
        unsolved_data = _unsolved_data;
        solved_data = _solved_data;
    }

    public int[] GetUnsolvedData()
    {
        return unsolved_data;
    }

    public int[] GetSolvedData()
    {
        return solved_data;
    }
}

[Serializable]
public class ProtoScore : Message
{
    public int solved;

    public ProtoScore(int _solved) : base(eType.Score)
    {
        solved = _solved;
    }
}

[Serializable]
public class ProtoSessionID : Message
{
    public SessionId sessionID;
    public string nickName;

    public ProtoSessionID(SessionId _sessionID, string _nickName) : base(eType.SessionID)
    {
        sessionID = _sessionID;
        nickName = _nickName;
    }
}

[Serializable]
public class ProtoGameResult : Message
{
    public bool victory;

    public ProtoGameResult(bool _victory) : base(eType.GameResult)
    {
        victory = _victory;
    }
}