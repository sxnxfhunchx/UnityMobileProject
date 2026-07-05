using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class WeaponIconGenerator
    {
        private const string PrefabsFolder = "Assets/ThirdParty/Blink/Art/Weapons/LowPoly/FreeSwords/Prefabs";
        private const string IconsFolder = "Assets/Assets/Art/WeaponIcons";

        private const int IconSize = 512;

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

            Debug.Log($"Generated {prefabGuids.Length} weapon icons.");
        }

        private static void GenerateIcon(GameObject prefab)
        {
            GameObject instance = Object.Instantiate(prefab);
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.identity;

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
    }
}