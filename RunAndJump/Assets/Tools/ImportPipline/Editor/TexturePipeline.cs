using UnityEditor;
using UnityEngine;

namespace Assets.Tools.ImportPipline.Editor
{
    public class TexturePipeline : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            Debug.LogFormat("OnPreprocessTexture, The path is {0}",
                assetPath);

            if (assetPath.StartsWith("Assets/Art/Bg"))
            {
                PreprocessBg();
            }
            else if (assetPath.StartsWith("Assets/Art/Platformer"))
            {
                PreprocessLevelPieces();
            }
        }
        private void OnPostprocessTexture(Texture2D texture)
        {
            Debug.LogFormat("OnPostprocessTexture, The path is {0}",
                assetPath);
        }

        private void PreprocessBg()
        {
            TextureImporter importer = assetImporter as TextureImporter;
            importer.textureType = TextureImporterType.Sprite;
            TextureImporterSettings texSettings = new TextureImporterSettings();
            importer.ReadTextureSettings(texSettings);
            texSettings.spriteAlignment = (int)SpriteAlignment.BottomLeft;
            texSettings.mipmapEnabled = false;
            importer.SetTextureSettings(texSettings);
        }

        private void PreprocessLevelPieces()
        {
            TextureImporter importer = assetImporter as TextureImporter;
            importer.textureType = TextureImporterType.Sprite;
            TextureImporterSettings texSettings = new TextureImporterSettings();
            importer.ReadTextureSettings(texSettings);
            texSettings.spriteAlignment = (int)SpriteAlignment.Center;
            texSettings.mipmapEnabled = false;
            importer.SetTextureSettings(texSettings);
        }
    }
}