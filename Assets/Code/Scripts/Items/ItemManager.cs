using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
static class ItemManager
{
    [MenuItem("Items/Set Scene Items")]
    static void SetNotSpawnedItems()
    {
        Item[] items = MonoBehaviour.FindObjectsOfType<Item>();
        Star[] starItems = MonoBehaviour.FindObjectsOfType<Star>();
        foreach (var item in items)
        {
            item.SetSpawned(false);
            EditorUtility.SetDirty(item);
        }
        foreach (var item in starItems)
        {
            item.SetSpawned(false);
            EditorUtility.SetDirty(item);
        }
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
}
