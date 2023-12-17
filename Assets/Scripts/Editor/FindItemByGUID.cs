using UnityEditor;
using UnityEngine;

public class FindAssetByGUID : EditorWindow
{
    private string guidToFind = "";

    [MenuItem("Tools/Find Asset By GUID")]
    public static void ShowWindow()
    {
        GetWindow<FindAssetByGUID>("Find Asset By GUID");
    }

    void OnGUI()
    {
        GUILayout.Label("Enter the GUID to find the asset:", EditorStyles.boldLabel);
        guidToFind = EditorGUILayout.TextField("GUID", guidToFind);

        if (GUILayout.Button("Find Asset"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guidToFind);
            if (!string.IsNullOrEmpty(path))
            {
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                EditorGUIUtility.PingObject(obj);
                Debug.Log("Asset found: " + path, obj);
            }
            else
            {
                Debug.LogWarning("No asset found for GUID: " + guidToFind);
            }
        }
    }
}
