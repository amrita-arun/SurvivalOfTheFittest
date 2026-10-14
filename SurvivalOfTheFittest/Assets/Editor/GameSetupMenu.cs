using SurvivalOfTheFittest;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SurvivalOfTheFittest.EditorTools
{
    public static class GameSetupMenu
    {
        [MenuItem("Survival of the Fittest/Setup Game Scene")]
        public static void SetupGameScene()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                Debug.LogError("No active scene. Open Assets/Scenes/SampleScene.unity first.");
                return;
            }

            var existing = Object.FindFirstObjectByType<GameBootstrap>();
            if (existing == null)
            {
                var go = new GameObject("GameBootstrap");
                go.AddComponent<GameBootstrap>();
                Undo.RegisterCreatedObjectUndo(go, "Create GameBootstrap");
            }

            var cam = Camera.main;
            if (cam != null)
            {
                Undo.RecordObject(cam, "Configure Camera");
                cam.orthographic = true;
                cam.backgroundColor = Color.black;
                cam.clearFlags = CameraClearFlags.SolidColor;
                EditorUtility.SetDirty(cam);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("[Survival of the Fittest] Scene ready. Press Play — the bootstrap builds the maze, UI, and systems automatically.");
        }

        [MenuItem("Survival of the Fittest/Open Playtest Checklist")]
        public static void OpenChecklist()
        {
            var path = "Assets/Docs/PLAYTEST_CHECKLIST.md";
            var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (asset != null)
                Selection.activeObject = asset;
            else
                Debug.LogWarning("Checklist not found at " + path);
        }
    }
}
