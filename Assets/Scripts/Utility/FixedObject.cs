using UnityEngine;

public class FixedObject : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;

        DontDestroyOnLoad(gameObject);
    }
}
