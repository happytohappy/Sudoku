using UnityEngine;

public class Managers : MonoBehaviour
{
    #region 싱글톤
    private static Managers Instance = null;
    public static Managers GetInstance
    {
        get
        {
            if (Instance == null)
                Instance = FindObjectOfType(typeof(Managers)) as Managers;

            return Instance;
        }
    }
    #endregion

    [SerializeField]
    private Canvas m_UIRootCan = null;
    [SerializeField]
    private Camera m_MainCam = null;
    [SerializeField]
    private Camera m_UICam = null;
    [SerializeField]
    private UIWindowManager m_UI = null;
    [SerializeField]
    private SoundManager m_Sound = null;
    [SerializeField]
    private BackEndManager m_BackEnd = null;
    [SerializeField]
    private PlayerManager m_Player = null;

    public static Canvas UICanvas { get { return GetInstance.m_UIRootCan; } }
    public static Camera MainCam { get { return GetInstance.m_MainCam; } }
    public static Camera UICam { get { return GetInstance.m_UICam; } }
    public static UIWindowManager UI { get { return GetInstance.m_UI; } }
    public static SoundManager Sound { get { return GetInstance.m_Sound; } }
    public static BackEndManager BackEnd { get { return GetInstance.m_BackEnd; } }
    public static PlayerManager Player { get { return GetInstance.m_Player; } }

    private void Awake()
    {
        Init();
    }

    private void Init()
    { 
        BackEnd.Init();
        Sound.Init();
        UI.Init();
    }
}
