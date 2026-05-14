#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEditor.Toolbars;
using System.IO;
using Unity.VisualScripting;
public class EditorTools 
{
    [MainToolbarElement("Open Scene")]
    public static MainToolbarElement CreateSceneDropdown()
    {
        var content = new MainToolbarContent("Open Scene");
        return new MainToolbarDropdown(content, rect =>
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Lobby"), false, () => OpenSceneByPath("Assets/NK/Scenes/LobbyScene.unity"));
            menu.AddItem(new GUIContent("Map"), false, () => OpenSceneByPath("Assets/NK/Scenes/MapScene.unity"));
            menu.AddItem(new GUIContent("Stage"), false, () => OpenSceneByPath("Assets/NK/Scenes/StageScene.unity"));
            menu.DropDown(rect);
        });
    }
    private static void OpenSceneByPath(string path)
    {
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(path);
        }
    }
    [MainToolbarElement("Open Folder")]
    private static MainToolbarElement CreateFolderDropdown()
    {
        var content = new MainToolbarContent("Open Folder");
        return new MainToolbarDropdown(content, rect =>
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Resources"), false, () => OpenFolderByPath("Assets/NK/Resources"));
            menu.AddItem(new GUIContent("Managers"), false, () => OpenFolderByPath("Assets/NK/Scripts/Managers"));
            
            menu.DropDown(rect);
        });
    }
    private static void OpenFolderByPath(string path)
    {
        Object folder = AssetDatabase.LoadAssetAtPath<Object>(path);
        if (folder != null)
        {
            Selection.activeObject = folder;
            EditorApplication.delayCall += () =>
            {
                EditorGUIUtility.PingObject(folder);
            };
        }
        else
        {
            Debug.LogWarning($"Folder not found : {path}");
        }
    }
}
#endif