using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace MagicCastle.Editor
{
    public static class PrototypeSceneBuilder
    {
        [MenuItem("Magic Castle/Create Local Co-op Prototype")]
        public static void Create()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Magic Castle", "Exit Play Mode before creating a scene.", "OK");
                return;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                EditorUtility.DisplayDialog("Magic Castle",
                    "Create a Universal 3D (URP) project before importing this prototype.", "OK");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            const string generated = "Assets/MagicCastle/Generated";
            if (!AssetDatabase.IsValidFolder(generated))
                AssetDatabase.CreateFolder("Assets/MagicCastle", "Generated");
            string folder = AssetDatabase.GenerateUniqueAssetPath(generated + "/Prototype");
            AssetDatabase.CreateFolder(generated, Path.GetFileName(folder));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            Material floor = CreateMaterial(folder, "Floor", new Color(0.22f, 0.28f, 0.36f), shader);
            Material stone = CreateMaterial(folder, "Stone", new Color(0.43f, 0.48f, 0.57f), shader);
            Material accent = CreateMaterial(folder, "Gold", new Color(0.93f, 0.72f, 0.30f), shader);
            Material ice = CreateMaterial(folder, "Ice", new Color(0.19f, 0.74f, 0.96f), shader);
            Material fire = CreateMaterial(folder, "Fire", new Color(0.98f, 0.36f, 0.19f), shader);
            Material targetMaterial = CreateMaterial(folder, "Target", new Color(0.36f, 0.72f, 0.33f), shader);
            Material defeatedMaterial = CreateMaterial(folder, "Defeated", new Color(0.25f, 0.27f, 0.30f), shader);

            GameObject environment = new GameObject("Castle Courtyard");
            CreateBlock("Floor", new Vector3(0f, -0.5f, 0f), new Vector3(26f, 1f, 20f),
                floor, environment.transform);
            CreateBlock("West Wall", new Vector3(-12.5f, 0.7f, 0f), new Vector3(1f, 1.4f, 20f),
                stone, environment.transform);
            CreateBlock("East Wall", new Vector3(12.5f, 0.7f, 0f), new Vector3(1f, 1.4f, 20f),
                stone, environment.transform);
            CreateBlock("North Wall", new Vector3(0f, 0.7f, 9.5f), new Vector3(24f, 1.4f, 1f),
                stone, environment.transform);
            CreateBlock("South Wall", new Vector3(0f, 0.7f, -9.5f), new Vector3(24f, 1.4f, 1f),
                stone, environment.transform);
            CreateBlock("Obstacle A", new Vector3(-4f, 0.65f, 3f), new Vector3(2.5f, 1.3f, 2.5f),
                stone, environment.transform);
            CreateBlock("Obstacle B", new Vector3(3f, 0.65f, -2f), new Vector3(2.5f, 1.3f, 2.5f),
                stone, environment.transform);
            CreateBlock("Obstacle C", new Vector3(6f, 0.65f, 4f), new Vector3(2f, 1.3f, 2f),
                stone, environment.transform);
            CreateBlock("Ice Starting Tile", new Vector3(-2f, 0.02f, -5f), new Vector3(2f, 0.04f, 2f),
                ice, environment.transform);
            CreateBlock("Fire Starting Tile", new Vector3(2f, 0.02f, -5f), new Vector3(2f, 0.04f, 2f),
                fire, environment.transform);
            foreach (float x in new[] { -11.5f, 11.5f })
            {
                foreach (float z in new[] { -8.5f, 8.5f })
                {
                    CreateBlock("Corner Tower", new Vector3(x, 1f, z), new Vector3(1.5f, 2f, 1.5f),
                        stone, environment.transform);
                    CreateBlock("Tower Cap", new Vector3(x, 2.15f, z), new Vector3(1.7f, 0.3f, 1.7f),
                        accent, environment.transform);
                }
            }

            LocalPlayerController[] players =
            {
                CreatePlayer("P1 Ice", new Vector3(-2f, 1.15f, -5f), ice, accent),
                CreatePlayer("P2 Fire", new Vector3(2f, 1.15f, -5f), fire, accent)
            };

            GameObject targetObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            targetObject.name = "Training Target - Ice then Fire";
            targetObject.transform.position = new Vector3(0f, 1.1f, 1f);
            Renderer targetRenderer = targetObject.GetComponent<Renderer>();
            targetRenderer.sharedMaterial = targetMaterial;
            TrainingEnemy target = targetObject.AddComponent<TrainingEnemy>();
            target.Configure(targetRenderer, targetMaterial, ice, defeatedMaterial);
            ConfigureCombat(players[0], AttackKind.Ice, target, ice);
            ConfigureCombat(players[1], AttackKind.Fire, target, fire);

            Camera camera = new GameObject("Main Camera").AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.orthographic = true;
            camera.orthographicSize = 12f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 120f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.10f, 0.13f, 0.20f);
            Quaternion rotation = Quaternion.Euler(55f, 0f, 0f);
            camera.transform.SetPositionAndRotation(Vector3.up - rotation * Vector3.forward * 40f, rotation);
            camera.gameObject.AddComponent<AudioListener>();
            camera.gameObject.AddComponent<SharedCameraRig>().Configure(players);

            Light light = new GameObject("Sun").AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.5f;
            light.shadows = LightShadows.Soft;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.50f, 0.55f, 0.65f);

            LocalCoopSession session = new GameObject("Local Co-op Session").AddComponent<LocalCoopSession>();
            session.Configure(players, target);
            string scenePath = folder + "/LocalCoopPrototype.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.SaveAssets();
            Selection.activeGameObject = session.gameObject;
            Debug.Log($"Created {scenePath}. Click Play, focus the Game view, then join with two devices.");
        }

        private static Material CreateMaterial(string folder, string name, Color color, Shader shader)
        {
            var material = new Material(shader) { name = name };
            material.SetColor("_BaseColor", color);
            material.SetFloat("_Smoothness", 0.25f);
            AssetDatabase.CreateAsset(material, folder + "/" + name + ".mat");
            return material;
        }

        private static void ConfigureCombat(LocalPlayerController player, AttackKind role,
            TrainingEnemy target, Material material)
        {
            GameObject effect = new GameObject("Attack Beam");
            effect.transform.SetParent(player.transform, false);
            LineRenderer beam = effect.AddComponent<LineRenderer>();
            beam.useWorldSpace = true;
            beam.positionCount = 2;
            beam.startWidth = 0.12f;
            beam.endWidth = 0.06f;
            beam.sharedMaterial = material;
            beam.shadowCastingMode = ShadowCastingMode.Off;
            beam.receiveShadows = false;
            beam.enabled = false;
            player.gameObject.AddComponent<PlayerCombat>().Configure(role, target, beam);
        }

        private static GameObject CreateBlock(string name, Vector3 position, Vector3 scale,
            Material material, Transform parent)
        {
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.transform.SetParent(parent, false);
            block.transform.position = position;
            block.transform.localScale = scale;
            block.GetComponent<Renderer>().sharedMaterial = material;
            block.isStatic = true;
            return block;
        }

        private static LocalPlayerController CreatePlayer(string name, Vector3 position,
            Material bodyMaterial, Material detailMaterial)
        {
            GameObject root = new GameObject(name);
            root.transform.position = position;
            CharacterController controller = root.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.45f;
            controller.center = Vector3.zero;
            controller.stepOffset = 0.25f;
            controller.skinWidth = 0.04f;
            controller.minMoveDistance = 0f;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(root.transform, false);
            Object.DestroyImmediate(body.GetComponent<Collider>());
            body.GetComponent<Renderer>().sharedMaterial = bodyMaterial;

            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = "Facing Marker";
            marker.transform.SetParent(root.transform, false);
            marker.transform.localPosition = new Vector3(0f, 0.25f, 0.53f);
            marker.transform.localScale = new Vector3(0.18f, 0.18f, 0.4f);
            Object.DestroyImmediate(marker.GetComponent<Collider>());
            marker.GetComponent<Renderer>().sharedMaterial = detailMaterial;

            LocalPlayerController player = root.AddComponent<LocalPlayerController>();
            root.SetActive(false);
            return player;
        }
    }
}
