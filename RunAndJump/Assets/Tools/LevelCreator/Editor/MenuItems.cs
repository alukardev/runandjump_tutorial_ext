using Assets.Scripts.Level;
using UnityEditor;

namespace Assets.Tools.LevelCreator.Editor
{
    public class MenuItems
    {
        [MenuItem("Tools/Level Creator/New Level Scene")]
        private static void NewLevel()
        {
            EditorUtils.NewLevel();
        }

        [MenuItem("Tools/Level Creator/Show Palette")]
        private static void ShowPalette()
        {
            PaletteWindow.ShowPalette();
        }

        [MenuItem("Tools/Level Creator/New Level Settings")]
        private static void NewLevelSettings()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "New Level Settings",
                "LevelSettings",
                "asset",
                "Define the name for the LevelSettings asset");
            if (path != "")
            {
                EditorUtils.CreateAsset<LevelSettings>(path);
            }
        }
    }
}