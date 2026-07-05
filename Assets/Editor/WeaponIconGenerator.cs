using System.IO;
using SO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class WeaponIconGenerator
    {
        private const string PrefabsFolder = "Assets/ThirdParty/Blink/Art/Weapons/LowPoly/FreeSwords/Prefabs";
        private const string IconsFolder = "Assets/Assets/Art/WeaponIcons";
        private const string DataFolder = "Assets/SO/Rewards/Swords";
        private const string DatabasePath = "Assets/SO/Rewards/Swords/WeaponDatabase.asset";
        
        private const int IconSize = 512;
        
        private static readonly string[] PowerNames =
        {
            "Broken",      // 1
            "Rusty",       // 2
            "Simple",      // 3
            "Sturdy",      // 4
            "Sharp",       // 5
            "Fine",        // 6
            "Powerful",    // 7
            "Masterwork",  // 8
            "Epic",        // 9
            "Legendary",   // 10
            "Mythic",      // 11
            "Ancient",     // 12
            "Runic",       // 13
            "Divine",      // 14
            "Celestial"    // 15
        };

        [MenuItem("Tools/Generate Weapon Icons")]
        public static void GenerateIcons()
        {
            if (!Directory.Exists(IconsFolder))
                Directory.CreateDirectory(IconsFolder);

            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabsFolder });

            foreach (string guid in prefabGuids)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if (prefab == null)
                    continue;

                GenerateIcon(prefab);
            }

            AssetDatabase.Refresh();
            ImportIconsAsSprites();
            //CreateWeaponDataAssets();

            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();

            Debug.Log($"Generated {prefabGuids.Length} weapon icons.");
        }

        private static void GenerateIcon(GameObject prefab)
        {
            GameObject instance = Object.Instantiate(prefab);
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

            Camera camera = CreateCamera();
            Light light = CreateLight();

            Bounds bounds = CalculateBounds(instance);
            SetupCamera(camera, bounds);

            RenderTexture renderTexture = new RenderTexture(IconSize, IconSize, 24);
            camera.targetTexture = renderTexture;

            Texture2D texture = new Texture2D(IconSize, IconSize, TextureFormat.RGBA32, false);

            camera.Render();

            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, IconSize, IconSize), 0, 0);
            texture.Apply();

            texture = CropTransparent(texture, 12);
            
            string filePath = $"{IconsFolder}/{prefab.name}.png";
            File.WriteAllBytes(filePath, texture.EncodeToPNG());

            RenderTexture.active = null;
            camera.targetTexture = null;

            Object.DestroyImmediate(renderTexture);
            Object.DestroyImmediate(texture);
            Object.DestroyImmediate(camera.gameObject);
            Object.DestroyImmediate(light.gameObject);
            Object.DestroyImmediate(instance);
        }

        private static Camera CreateCamera()
        {
            GameObject cameraObj = new GameObject("Icon Camera");
            Camera camera = cameraObj.AddComponent<Camera>();

            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0, 0, 0, 0);
            camera.orthographic = true;
            camera.nearClipPlane = 0.01f;
            camera.farClipPlane = 100f;

            return camera;
        }

        private static Light CreateLight()
        {
            GameObject lightObj = new GameObject("Icon Light");
            Light light = lightObj.AddComponent<Light>();

            light.type = LightType.Directional;
            light.intensity = 1.5f;
            light.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

            return light;
        }

        private static Bounds CalculateBounds(GameObject obj)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
                return new Bounds(obj.transform.position, Vector3.one);

            Bounds bounds = renderers[0].bounds;

            foreach (Renderer renderer in renderers)
                bounds.Encapsulate(renderer.bounds);

            return bounds;
        }

        private static void SetupCamera(Camera camera, Bounds bounds)
        {
            Vector3 center = bounds.center;

            camera.transform.position = center + new Vector3(0, 0, -10f);
            camera.transform.rotation = Quaternion.identity;

            float size = Mathf.Max(bounds.size.x, bounds.size.y);
            camera.orthographicSize = size * 0.65f;
        }

        private static void ImportIconsAsSprites()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { IconsFolder }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                TextureImporter importer =
                    AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer == null)
                    continue;

                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;

                importer.SaveAndReimport();
            }
        }
        
        private static void CreateWeaponDataAssets()
        {
            if (!Directory.Exists(DataFolder))
                Directory.CreateDirectory(DataFolder);

            WeaponDatabase database = AssetDatabase.LoadAssetAtPath<WeaponDatabase>(DatabasePath);

            if (database == null)
            {
                database = ScriptableObject.CreateInstance<WeaponDatabase>();
                AssetDatabase.CreateAsset(database, DatabasePath);
            }

            database.Weapons.Clear();

            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabsFolder });

            foreach (string guid in prefabGuids)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if (prefab == null)
                    continue;

                string prefabName = prefab.name;

                string displayName = MakeDisplayName(prefabName);
                string id = MakeId(prefabName);

                string assetPath = $"{DataFolder}/{id}_Data.asset";
                string iconPath = $"{IconsFolder}/{prefabName}.png";

                WeaponData data = AssetDatabase.LoadAssetAtPath<WeaponData>(assetPath);

                if (data == null)
                {
                    data = ScriptableObject.CreateInstance<WeaponData>();
                    AssetDatabase.CreateAsset(data, assetPath);
                }

                Sprite icon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);

                data.WeaponId = id;
                data.WeaponName = displayName;
                data.VisualPrefab = prefab;
                data.Icon = icon;

                EditorUtility.SetDirty(data);

                if (!database.Weapons.Contains(data))
                    database.Weapons.Add(data);
            }

            EditorUtility.SetDirty(database);
        }
        
        private static void ParseWeaponName(string prefabName, out int number, out string modifier)
        {
            // Sword5_Yellow
            string cleaned = prefabName.Replace("_", " ");

            string[] parts = cleaned.Split(' ');

            number = 1;
            modifier = "Unknown";

            foreach (string part in parts)
            {
                if (part.StartsWith("Sword"))
                {
                    string numberText = part.Replace("Sword", "");

                    if (int.TryParse(numberText, out int parsedNumber))
                        number = parsedNumber;
                }
                else if (!string.IsNullOrWhiteSpace(part))
                {
                    modifier = part;
                }
            }
        }

        private static string GetPowerName(int number)
        {
            int index = Mathf.Clamp(number - 1, 0, PowerNames.Length - 1);
            return PowerNames[index];
        }

        private static string MakeDisplayName(string prefabName)
        {
            ParseWeaponName(prefabName, out int number, out string modifier);

            string powerName = GetPowerName(number);
            string readableModifier = ObjectNames.NicifyVariableName(modifier);

            return $"{powerName} {readableModifier} Sword";
        }

        private static string MakeId(string prefabName)
        {
            string displayName = MakeDisplayName(prefabName);

            return displayName
                .ToLower()
                .Replace(" ", "_")
                .Replace("-", "_");
        }
        
        private static Texture2D CropTransparent(Texture2D source, int padding)
        {
            Color32[] pixels = source.GetPixels32();

            int width = source.width;
            int height = source.height;

            int minX = width;
            int minY = height;
            int maxX = 0;
            int maxY = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color32 pixel = pixels[y * width + x];

                    if (pixel.a > 5)
                    {
                        minX = Mathf.Min(minX, x);
                        minY = Mathf.Min(minY, y);
                        maxX = Mathf.Max(maxX, x);
                        maxY = Mathf.Max(maxY, y);
                    }
                }
            }

            if (minX > maxX || minY > maxY)
                return source;

            minX = Mathf.Max(0, minX - padding);
            minY = Mathf.Max(0, minY - padding);
            maxX = Mathf.Min(width - 1, maxX + padding);
            maxY = Mathf.Min(height - 1, maxY + padding);

            int croppedWidth = maxX - minX + 1;
            int croppedHeight = maxY - minY + 1;

            Texture2D cropped = new Texture2D(croppedWidth, croppedHeight, TextureFormat.RGBA32, false);
            cropped.SetPixels(source.GetPixels(minX, minY, croppedWidth, croppedHeight));
            cropped.Apply();

            return cropped;
        }
    }
}