public enum WindowID
{
    None,
    UIWindowLoading,
    UIWindowIntro,
    UIWindowMain,
    UIWindowMultiGame,
    UIPopupToastMessage,
}

[System.Flags]
public enum WindowMode
{
    None = 0,                       // 기본값
    WindowClose = 1 << 0,           // 윈도우가 열릴때 WindowClose면 이전 윈도우를 비활성화시킨다.
    WindowOverlay = 1 << 1,         // 윈도우가 열릴때 OverLay면 이전 윈도우를 비활성화시키지 않고 덮어 씌운다.
    WindowJustClose = 1 << 2,       // 닫힐때 무조건 닫히고 이전 노드에 간섭하지 않는다.
}

public enum eBGMSound
{

}

public enum eEffectSound
{
    Click,
    Erase,
    Pen,
    Undo
}

public enum eType
{
    CreateBoard,
    Score,
    SessionID,
    GameResult,
}