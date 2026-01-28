using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace EditorShortcuts
{
    public static class EditorShortcuts
    {
        // Alt + C Clear Console
        [Shortcut("Clear Console", KeyCode.C, ShortcutModifiers.Alt)]
        public static void ClearConsole()
        {
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }

        //Ctrl + F1 - Create New Folder
        [Shortcut("CreateNewFolder", KeyCode.F1, ShortcutModifiers.Control)]
        public static void CreateNewFolder()
        {
            ProjectWindowUtil.CreateFolder();
        }


        //Ctrl + Shift + F3 Create new C# Class
        [Shortcut("CreateNewCSScript", KeyCode.F3, ShortcutModifiers.Control | ShortcutModifiers.Shift)]
        public static void CreateNewCSScript()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/Editor/ScriptTemplate/C#Class.cs.txt", "NewScript.cs");
        }

        //Ctrl + Shift + F1 Create new MonoBehaviour Class
        [Shortcut("CreateNewMonoBehaviourScript", KeyCode.F1, ShortcutModifiers.Control | ShortcutModifiers.Shift)]
        public static void CreateNewMonoBehaviourScript()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/Editor/ScriptTemplate/C#MonoBehaviourClass.cs.txt", "NewMonoBehaviour.cs");
        }

        //Ctrl + Shift + F2 Create new Scriptable Object Class
        [Shortcut("CreateNewScriptableObjectScript", KeyCode.F2, ShortcutModifiers.Control | ShortcutModifiers.Shift)]
        public static void CreateNewScriptableObjectScript()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/Editor/ScriptTemplate/C#ScriptableObjectClass.cs.txt", "NewScriptableObject.cs");
        }
        //Ctrl + Shift + I Create new Interface
        [Shortcut("CreateNewInterfaceScript", KeyCode.F9, ShortcutModifiers.Control | ShortcutModifiers.Shift)]
        public static void CreateNewInterface()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/Editor/ScriptTemplate/C#Interface.cs.txt", "INewInterface.cs");
        }


    }
}
