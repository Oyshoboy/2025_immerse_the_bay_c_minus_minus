using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GhostPrefabGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Generate Ghost Prefabs")]
    public static void GenerateGhostPrefabs()
    {
        string prefabPath = "Assets/Prefabs/";
        
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        CreateHeadGhostPrefab(prefabPath);
        CreateHandGhostPrefab(prefabPath, "LeftHandGhost", "Assets/Materials/HandLeft.mat");
        CreateHandGhostPrefab(prefabPath, "RightHandGhost", "Assets/Materials/HandRight.mat");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("Ghost prefabs generated successfully!");
    }

    private static void CreateHeadGhostPrefab(string path)
    {
        GameObject headGhost = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        headGhost.name = "HeadGhost";
        headGhost.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        Material headMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Head.mat");
        if (headMat != null)
        {
            headGhost.GetComponent<Renderer>().sharedMaterial = headMat;
        }

        Collider collider = headGhost.GetComponent<Collider>();
        if (collider != null) DestroyImmediate(collider);

        PrefabUtility.SaveAsPrefabAsset(headGhost, path + "HeadGhost.prefab");
        DestroyImmediate(headGhost);
    }

    private static void CreateHandGhostPrefab(string path, string name, string materialPath)
    {
        GameObject handGhost = GameObject.CreatePrimitive(PrimitiveType.Cube);
        handGhost.name = name;
        handGhost.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        Material handMat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (handMat != null)
        {
            handGhost.GetComponent<Renderer>().sharedMaterial = handMat;
        }

        Collider collider = handGhost.GetComponent<Collider>();
        if (collider != null) DestroyImmediate(collider);

        PrefabUtility.SaveAsPrefabAsset(handGhost, path + name + ".prefab");
        DestroyImmediate(handGhost);
    }
#endif
}

