using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AlephVault.Unity.Boilerplates.Utils;
using AlephVault.Unity.MenuActions.Utils;
using UnityEditor;
using UnityEngine;

namespace GameMeanMachine.Unity.NetRose
{
    namespace MenuActions
    {
        namespace Boilerplates
        {
            /// <summary>
            ///   This boilerplate function creates:
            ///   - Scripts/Models/:
            ///     - CharacterSpawnData, CharacterRefreshData
            /// </summary>
            public static class ProjectStartup
            {
                /// <summary>
                ///   Utility window used to create the files for new
                ///   spawn/refresh REFMAP data file(s).
                /// </summary>
                public class CreateREFMAPModels : EditorWindow
                {
                    private Regex nameCriterion = new Regex("^[A-Z][A-Za-z0-9_]*$");
                    
                    // The base name to use.
                    private string baseName = "MyModel";

                    // Whether to use a simple model instead of standard one.
                    private bool useSimpleModel = true;
                    
                    // Whether to create the spawn model or not.
                    private bool makeSpawnModel = true;

                    // Whether to create the refresh model or not.
                    private bool makeRefreshModel = true;
                    
                    private void OnGUI()
                    {
                        GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();

                        EditorGUILayout.BeginVertical();
                        
                        EditorGUILayout.LabelField(@"
This utility generates the two REFMAP model files, with boilerplate code and instructions on how to understand that code.

The base name has to be chosen (carefully and according to the game design):
- It must start with an uppercase letter.
- It must continue with letters, numbers, and/or underscores.

The Spawn and Refresh data types will be generated, both implementing ISerializable, depending on:
- Whether to generate the spawn data class.
- Whether to generate the refresh data class.

The two files will be generated:
- {base name}SpawnData for the spawn data class.
- {base name}RefreshData for the refresh data class.

WARNING: THIS MIGHT OVERRIDE EXISTING CODE. Always use proper source code management & versioning.
".Trim(), longLabelStyle);

                        // The base name
                        EditorGUILayout.BeginHorizontal();
                        baseName = EditorGUILayout.TextField("Base name", baseName).Trim();
                        bool validBaseName = nameCriterion.IsMatch(baseName);
                        if (!validBaseName)
                        {
                            EditorGUILayout.LabelField("The base name is invalid!");
                        }
                        EditorGUILayout.EndHorizontal();
                        
                        EditorGUILayout.BeginHorizontal();
                        useSimpleModel = EditorGUILayout.ToggleLeft("Use Simple (instead of Standard) model",
                            useSimpleModel);
                        EditorGUILayout.EndHorizontal();
                        
                        EditorGUILayout.BeginHorizontal();
                        makeSpawnModel = EditorGUILayout.ToggleLeft("Make Spawn Data class",
                            makeSpawnModel);
                        EditorGUILayout.EndHorizontal();
                        
                        EditorGUILayout.BeginHorizontal();
                        makeRefreshModel = EditorGUILayout.ToggleLeft("Make Refresh Data class",
                            makeRefreshModel);
                        EditorGUILayout.EndHorizontal();

                        bool execute = validBaseName && (makeSpawnModel || makeRefreshModel) &&
                                       GUILayout.Button("Generate");
                        EditorGUILayout.EndVertical();
                        
                        if (execute) Execute();
                    }

                    private void Execute()
                    {
                        DumpModelClasses(
                            baseName, useSimpleModel, makeSpawnModel, makeRefreshModel
                        );
                        Close();
                    }
                }

                
                // Performs the full dump of the code.
                private static void DumpModelClasses(
                    string basename, bool useSimple, bool dumpSpawnModel, bool dumpRefreshModel
                ) {
                    string directory = "Packages/com.gamemeanmchine.unity.netrose.refmapchars/" +
                                       "Editor/MenuActions/Boilerplates/Templates";
                    
                    TextAsset refresh = AssetDatabase.LoadAssetAtPath<TextAsset>(
                        directory + (useSimple ? "/RefMapSimpleRefreshData.cs.txt" : "/RefMapStandardRefreshData.cs.txt")
                    );
                    TextAsset spawn = AssetDatabase.LoadAssetAtPath<TextAsset>(
                        directory + (useSimple ? "/RefMapSimpleSpawnData.cs.txt" : "/RefMapStandardSpawnData.cs.txt")
                    );
                    string refreshFile = basename + "RefreshData";
                    string spawnFile = basename + "SpawnData";
                    Dictionary<string, string> replacements = new Dictionary<string, string>();

                    new Boilerplate()
                        .IntoDirectory("Scripts", false)
                            .IntoDirectory("Models", false)
                                .Do(Boilerplate.InstantiateScriptCodeTemplate(spawn, spawnFile, replacements))
                                .Do(Boilerplate.InstantiateScriptCodeTemplate(refresh, refreshFile, replacements))
                                .Do()
                            .End()
                        .End();
                }
                
                [MenuItem("Assets/Create/Net Rose/RefMap Chars/Boilerplates/Create RefMap Chars models", false, 204)]
                public static void ExecuteBoilerplate()
                {
                    CreateREFMAPModels window = ScriptableObject.CreateInstance<CreateREFMAPModels>();
                    Vector2 size = new Vector2(750, 332);
                    window.position = new Rect(new Vector2(110, 250), size);
                    window.minSize = size;
                    window.maxSize = size;
                    window.titleContent = new GUIContent("Networked Object Behaviours generation");
                    window.ShowUtility();
                }
            }
        }
    }
}
