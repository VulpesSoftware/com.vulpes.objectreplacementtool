using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// TODO Add option for relative offsets.

namespace Vulpes.Development
{
    public sealed class ObjectReplacementTool : EditorWindow
    {
        private GameObject[] currentSelection = new GameObject[0];

        public GameObject replacementObject;

        public int numberToAdd = 1;

        public string objectName = "";

        public bool maintainPosition = true;
        public bool maintainRotation = true;
        public bool maintainScale = true;

        public bool offset = false;

        public Vector3 offsetPosition = Vector3.zero;
        public Vector3 offsetRotation = Vector3.zero;
        public Vector3 offsetScale = Vector3.zero;

        public ManageObject manageObject;
        public enum ManageObject { Replace, Add }

        public HierarchyParameters hierarchyParameters;
        public enum HierarchyParameters { None, MaintainHierarchy, MaintainChildren, MaintainParent }

        private bool isSelectionPersistent = false;
        private bool isReplacementPersistent = true;

        private Vector2 scrollView = Vector2.zero;

        [MenuItem("GameObject/Replace Object...")]

        static void Init()
        {
            ObjectReplacementTool window = (ObjectReplacementTool)EditorWindow.GetWindow(typeof(ObjectReplacementTool), false, "Replace Object");
            window.minSize = new Vector2(420, 320);
            window.maxSize = new Vector2(480, 384);
        }

        private void OnGUI()
        {
            if (Selection.gameObjects.Length > 0)
            {
                currentSelection = Selection.gameObjects;
            } else
            {
                currentSelection = new GameObject[0];
            }
            
            if (Selection.gameObjects.Length > 0)
            {
                isSelectionPersistent = EditorUtility.IsPersistent(Selection.activeGameObject);
            } else
            {
                isSelectionPersistent = false;
            }
            
            if (replacementObject != null)
            {
                isReplacementPersistent = EditorUtility.IsPersistent(replacementObject);
            } else
            {
                isReplacementPersistent = true;
            }
            
            scrollView = EditorGUILayout.BeginScrollView(scrollView);

            GUILayout.Label("Select a GameObject from the Scene or Hierarchy... ");
            EditorGUILayout.Separator();
            GUILayout.BeginHorizontal();
            objectName = EditorGUILayout.TextField(new GUIContent("Search", "Search for, and select all objects by name."), objectName);
            if (objectName == string.Empty)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Find and Select"))
            {
                List<GameObject> objectsToSelect = new List<GameObject>();
                foreach (GameObject go in FindObjectsOfType<GameObject>())
                {
                    if (go.name == objectName)
                    {
                        objectsToSelect.Add(go);
                    }
                }
                if (objectsToSelect.Count > 0)
                {
                    Selection.objects = objectsToSelect.ToArray();
                }
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            manageObject = (ManageObject)EditorGUILayout.EnumPopup(new GUIContent("Function", "The selection can either be replaced by the Replacement Object, or the Replacement Object be added without removing the selection from the scene."), manageObject);
            hierarchyParameters = (HierarchyParameters)EditorGUILayout.EnumPopup(new GUIContent("Hierarchy Parameters", "Optional Parameters for retention of Hierarchy status."), hierarchyParameters);
            replacementObject = EditorGUILayout.ObjectField(new GUIContent("Replacement Object", "The object that will replace the current selection."), replacementObject, typeof(GameObject), true) as GameObject;

            if (manageObject == ManageObject.Add)
            {
                GUI.enabled = true;
            } else
            {
                GUI.enabled = false;
                numberToAdd = 1;
            }

            numberToAdd = EditorGUILayout.IntField(new GUIContent("Number to Add", "The number of times the object will be instantiated at each selection."), (int)Mathf.Clamp(numberToAdd, 1, Mathf.Infinity));
            GUI.enabled = true;
            GUILayout.Label("Maintain Transform");
            EditorGUILayout.BeginHorizontal();
            maintainPosition = GUILayout.Toggle(maintainPosition, new GUIContent("Position", "Will the Replacement Object inherit the currently selected object's position?"));
            maintainRotation = GUILayout.Toggle(maintainRotation, new GUIContent("Rotation", "Will the Replacement Object inherit the currently selected object's rotation?"));
            maintainScale = GUILayout.Toggle(maintainScale, new GUIContent("Scale", "Will the Replacement Object inherit the currently selected object's scale?"));
            EditorGUILayout.EndHorizontal();
            offset = EditorGUILayout.Foldout(offset, new GUIContent("Offset Transform", "Manually define the offset values of the replacement transform."));

            if (offset)
            {
                offsetPosition = EditorGUILayout.Vector3Field("Position", offsetPosition);
                offsetRotation = EditorGUILayout.Vector3Field("Rotation", offsetRotation);
                offsetScale = EditorGUILayout.Vector3Field("Scale", offsetScale);
            }
            
            if (Selection.gameObjects.Length > 0 && !isSelectionPersistent && replacementObject != null && isReplacementPersistent)
            {
                GUI.enabled = true;
            } else
            {
                GUI.enabled = false;
            }

            if (manageObject == ManageObject.Add && hierarchyParameters != HierarchyParameters.None && numberToAdd > 1)
            {
                GUI.enabled = false;
            }

            EditorGUILayout.Separator();
            if (GUILayout.Button(manageObject + " Selection"))
            {
                Replace();
            }

            GUI.enabled = true;
            
            if (Selection.gameObjects.Length == 0)
            {
                EditorGUILayout.HelpBox("No Object Selected.", MessageType.Error, true);
            } else if (replacementObject == null)
            {
                EditorGUILayout.HelpBox("No Replacement Object was Assigned.", MessageType.Error, true);
            } else if (!isReplacementPersistent)
            {
                EditorGUILayout.HelpBox("Replacement Object must be added from the Project Window.", MessageType.Error, true);
            } else if (isSelectionPersistent)
            {
                EditorGUILayout.HelpBox("Selection must be made from the Scene or Hierarchy Window.", MessageType.Error, true);
            } else if (manageObject == ManageObject.Add && hierarchyParameters != HierarchyParameters.None && numberToAdd > 1)
            {
                EditorGUILayout.HelpBox("Hierarchy Parameters are only available if 'Number to Add' is equal to one.", MessageType.Error, true);
            }

            EditorGUILayout.EndScrollView();
        }

        private void Replace()
        {
            for (int i = 1; i <= numberToAdd; i++)
            {
                foreach (GameObject selection in currentSelection)
                {
                    GameObject replacement = PrefabUtility.InstantiatePrefab(replacementObject) as GameObject;
                    Transform[] children = selection.GetComponentsInChildren<Transform>();
                    
                    if (selection.transform.parent != null && hierarchyParameters == HierarchyParameters.MaintainHierarchy || selection.transform.parent != null && hierarchyParameters == HierarchyParameters.MaintainParent)
                    {
                        replacement.transform.parent = selection.transform.parent;
                    }
                    
                    if (maintainPosition)
                    {
                        if (hierarchyParameters == HierarchyParameters.None)
                        {
                            replacement.transform.position = selection.transform.position + (offsetPosition * i);
                        } else
                        {
                            replacement.transform.localPosition = selection.transform.localPosition + (offsetPosition * i);
                        }
                    } else
                    {
                        replacement.transform.localPosition = Vector3.zero;
                    }
                    
                    if (maintainRotation)
                    {
                        replacement.transform.eulerAngles = selection.transform.eulerAngles + (offsetRotation * i);
                    } else
                    {
                        replacement.transform.eulerAngles = Vector3.zero;
                    }
                    
                    if (maintainScale)
                    {
                        replacement.transform.localScale = selection.transform.localScale + (offsetScale * i);
                    } else
                    {
                        replacement.transform.localScale = Vector3.one;
                    }

                    if (manageObject != ManageObject.Add)
                    {
                        Undo.RegisterFullObjectHierarchyUndo(selection, manageObject.ToString());
                    }
                    
                    if (selection.transform.childCount > 0 && hierarchyParameters == HierarchyParameters.MaintainHierarchy || selection.transform.childCount > 0 && hierarchyParameters == HierarchyParameters.MaintainChildren)
                    {
                        foreach (Transform child in children)
                        {
                            if (child.parent == selection.transform)
                            {
                                if (manageObject == ManageObject.Add)
                                {
                                    Undo.RegisterFullObjectHierarchyUndo(child.gameObject, manageObject.ToString());
                                }
                                
                                Vector3 storedPosition = child.localPosition;
                                Vector3 storedRotation = child.localEulerAngles;
                                Vector3 storedScale = child.localScale;
                                child.parent = replacement.transform;
                                child.localPosition = storedPosition;
                                child.localEulerAngles = storedRotation;
                                child.localScale = storedScale;
                            }
                        }
                    }
                    
                    if (manageObject == ManageObject.Replace)
                    {
                        DestroyImmediate(selection);
                    }
                    
                    Undo.RegisterCreatedObjectUndo(replacement, manageObject.ToString() + " Selection");
                }
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}
