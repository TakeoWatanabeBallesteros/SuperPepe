using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(GameManager))]
public class ItemCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Set Items Not Spawned"))
        {
            Item[] items = FindObjectsOfType<Item>();
            foreach (var item in items)
            {
                item.SetSpawned(false);
                EditorUtility.SetDirty(item);
            }
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}
