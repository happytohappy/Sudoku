using UnityEngine;
using System.Text;
using System.Collections.Generic;

public static class Utility
{
    public static GameObject LoadPrefab(string path, GameObject parent = null)
    {
        GameObject prefabs = Resources.Load<GameObject>(string.Format("UI/{0}", path));
        if (prefabs == null)
            Debug.LogAssertion(string.Format("{0} 경로를 찾을 수 없습니다.", path));

        GameObject go = GameObject.Instantiate(prefabs) as GameObject;

        SetParent(go, Managers.UICanvas.gameObject);

        var item = go.GetComponent<RectTransform>();
        item.offsetMin = Vector2.zero;
        item.offsetMax = Vector2.zero;

        return go;
    }

    public static void SetParent(GameObject obj, GameObject parent)
    {
        obj.transform.SetParent(parent.transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
    }

    public static void SetParent(GameObject obj, GameObject parent, Vector3 scale, Quaternion rotation)
    {
        obj.transform.SetParent(parent.transform);
        obj.transform.localScale = scale;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.rotation = rotation;
    }

    public static List<string> CSVSplitData(string t)
    {
        t = t.Replace("\r\n", "\n");
        string[] lineTemp = t.Split("\n"[0]);

        return new List<string>(lineTemp);
    }

    public static T ReadJsonData<T>(byte[] buf)
    {
        var strByte = Encoding.Default.GetString(buf);
        return JsonUtility.FromJson<T>(strByte);
    }

    public static byte[] DataToJsonData<T>(T obj)
    {
        var jsonData = JsonUtility.ToJson(obj);
        return Encoding.UTF8.GetBytes(jsonData);
    }
}
