using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace ysk
{
    public class SymbolicLinkToolWindow : EditorWindow
    {
        [MenuItem("Tools/Asset/シンボリックリンク作成")]
        private static void Open()
        {
            GetWindow<SymbolicLinkToolWindow>().titleContent = new GUIContent("シンボリックリンク作成");
        }

        private string from, to;

        private void OnGUI()
        {
            bool fromExists = false;
            Color defaultColor = GUI.backgroundColor;
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    from = EditorGUILayout.TextField(from);
                    if (GUILayout.Button(new GUIContent("...", "Open Select Folder Dialog"), EditorStyles.miniButton, GUILayout.Width(20f)))
                    {
                        from = GetFolder(from);
                        Repaint();
                    }

                    fromExists = Directory.Exists(from) || File.Exists(from);
                    GUI.enabled = fromExists;
                    if (GUILayout.Button("フォルダ削除", EditorStyles.miniButton, GUILayout.Width(80f)))
                    {
                        try
                        {
                            Directory.Delete(from);
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogException(e);
                            fromExists = false;
                        }
                    }
                    GUI.enabled = true;
                }

                if (fromExists || !Path.IsPathRooted(from)) GUI.backgroundColor = Color.red;
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    if (SymbolicLinkTool.IsSymbolicLinkFolder(from))
                    {
                        EditorGUILayout.LabelField("既に使用されているシンボリックリンクです");
                    }
                    else if (fromExists)
                    {
                        EditorGUILayout.LabelField("既に存在するパスです。");
                    }
                    else if (!Path.IsPathRooted(from))
                    {
                        EditorGUILayout.LabelField("フルパスを指定してください。");
                    }
                    else
                    {
                        EditorGUILayout.LabelField("作成するショートカットのフルパスを指定してください。\n既に存在するパスは指定できません。");
                    }
                }
                GUI.backgroundColor = defaultColor;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("↓ Link To ↓", GUILayout.Width(75f));
                GUILayout.FlexibleSpace();
            }

            var toExists = false;
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    to = EditorGUILayout.TextField(to);
                    if (GUILayout.Button(new GUIContent("...", "Open Select Folder Dialog"), EditorStyles.miniButton, GUILayout.Width(20f)))
                    {
                        to = GetFolder(to);
                        Repaint();
                    }

                    toExists = Directory.Exists(to) || File.Exists(to);
                    GUI.enabled = !toExists;
                    if (GUILayout.Button("フォルダ作成", EditorStyles.miniButton, GUILayout.Width(80f)))
                    {
                        Directory.CreateDirectory(to);
                    }
                    GUI.enabled = true;
                }

                if (!toExists || !Path.IsPathRooted(to)) GUI.backgroundColor = Color.red;
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    if (!toExists)
                    {
                        EditorGUILayout.LabelField("存在しないパスです。");
                    }
                    else if (!Path.IsPathRooted(to))
                    {
                        EditorGUILayout.LabelField("フルパスを指定してください。");
                    }
                    else
                    {
                        EditorGUILayout.LabelField("参照先のフルパスを指定してください。\n既に存在するパスのみ指定できます。");
                    }
                }
                GUI.backgroundColor = defaultColor;
            }

            GUI.backgroundColor = Color.green;
            GUI.enabled = !string.IsNullOrEmpty(from)
                && !string.IsNullOrEmpty(to)
                && from != to
                && !fromExists
                && toExists;
            if (GUILayout.Button("Create Symbolic Link"))
            {
                try
                {
                    if (!SymbolicLinkTool.CreateSymbolicLinkBetweenFolders(from, to))
                    {
                        Debug.LogError("failed");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            GUI.enabled = true;

            GUI.backgroundColor = defaultColor;
        }

        private string GetFolder(string path)
        {
            return EditorUtility.OpenFolderPanel("select folder", path, "");
        }
    }
}