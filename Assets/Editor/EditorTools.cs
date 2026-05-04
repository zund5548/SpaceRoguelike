#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEditor.Toolbars;
using System.IO;
public class EditorTools 
{
    [MainToolbarElement("Open Scene")]
    public static MainToolbarElement CreateDropdown(){
    var content = new MainToolbarContent("Open Scene");
    return new MainToolbarDropdown(content, rect => {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Lobby"), false, () => OpenSceneByPath("Assets/NK/Scenes/LobbyScene.unity"));
        menu.AddItem(new GUIContent("Map"), false, () => OpenSceneByPath("Assets/NK/Scenes/MapScene.unity"));
        menu.AddItem(new GUIContent("Stage"), false, () => OpenSceneByPath("Assets/NK/Scenes/StageScene.unity"));
        menu.DropDown(rect);
    });
    }
    public static void OpenSceneByPath(string path)
    {
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(path);
        }
    }
}
#endif