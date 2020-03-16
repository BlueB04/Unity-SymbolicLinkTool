using UnityEditor;
using UnityEngine;
using UnityEngine.AssetGraph;
using System.IO;

namespace ysk
{
    public class SymbolicSAABTool
    {
        static readonly string LOCAL_STREAMING_AB_ROOT = Path.Combine(Application.streamingAssetsPath, "LocalResources");
        static readonly string OUTPUT_BUILT_AB_ROOT = Path.Combine(Application.dataPath.Replace("Assets", ""), "AssetBundle/Streaming");
        const string MENU_RELINK_SYM_LINK = "Tools/Asset/ローカルABへのリンク再生成";
        const string MENU_REMOVE_SYM_LINK = "Tools/Asset/ローカルABへのリンクを削除";


        [MenuItem(MENU_RELINK_SYM_LINK)]
        public static void ReLinkBuiltStreamingAssetsFolderWithCurrentTarget()
        {
            ReLinkBuiltStreamingAssetsFolder(EditorUserBuildSettings.activeBuildTarget);
        }

        public static void ReLinkBuiltStreamingAssetsFolder(BuildTarget target)
        {
            string path = "";
            if (target.ToString().ToLowerInvariant().StartsWith("standalone"))
            {
                path = Path.Combine(OUTPUT_BUILT_AB_ROOT, "PC");
            }
            else
            {
                path = Path.Combine(OUTPUT_BUILT_AB_ROOT, BuildTargetUtility.TargetToHumaneString(target));
            }

            RemoveSymbolicLinkToBuiltStreamingAssetsFolder();

            if (Directory.Exists(path))
            {
                try
                {
                    if (SymbolicLinkTool.CreateSymbolicLinkBetweenFolders(LOCAL_STREAMING_AB_ROOT, path))
                    {
                        AssetDatabase.Refresh();
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            else
            {
                Debug.LogWarning("cannot found SAAB folder for platform(" + target + ").");
            }
        }

        [MenuItem(MENU_REMOVE_SYM_LINK)]
        public static void RemoveSymbolicLinkToBuiltStreamingAssetsFolder()
        {
            if (Directory.Exists(LOCAL_STREAMING_AB_ROOT))
            {
                try
                {
                    if (Directory.Exists(LOCAL_STREAMING_AB_ROOT))
                    {
                        SymbolicLinkTool.RemoveSymbolicLinkFolder(LOCAL_STREAMING_AB_ROOT);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}