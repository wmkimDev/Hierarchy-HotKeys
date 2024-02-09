using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace WMK.Hierarchy_HotKeys.Editor
{
    public static class HierarchyHotKeys
    {
        [Shortcut("Toggle GameObject Active State", KeyCode.A)]
        private static void ToggleActive()
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                Undo.RecordObject(obj, "Toggle GameObject Active State");
                obj.SetActive(!obj.activeSelf);
            }
        }
        
        [Shortcut("Activate/Deactivate Selected Objects", KeyCode.A, ShortcutModifiers.Shift)]
        private static void ToggleActiveConsistently()
        {
            bool anyInactive = false;
            foreach (GameObject obj in Selection.gameObjects)
            {
                if (!obj.activeSelf)
                {
                    anyInactive = true;
                    break;
                }
            }

            foreach (GameObject obj in Selection.gameObjects)
            {
                Undo.RecordObject(obj, anyInactive ? "Activate Selected Objects" : "Deactivate Selected Objects");
                obj.SetActive(anyInactive);
            }
        }
        
        [Shortcut("Delete Selected Object", KeyCode.X, ShortcutModifiers.Shift)]
        private static void DeleteSelectedObject()
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                Undo.DestroyObjectImmediate(obj);
            }
        }

        [Shortcut("Group Selected Object", KeyCode.G)]
        private static void GroupSelectedObject()
        {
            var topObjects = GetTopSelectedObjects();
            if (Selection.gameObjects.Length > 1 && !AreSiblings(topObjects))
            {
                Debug.LogWarning("Cannot group game objects in different hierarchies");
                return;
            }
            
            GameObject group = new GameObject("New Group");
            group.transform.SetParent(Selection.activeTransform.parent, false);
            group.transform.SetSiblingIndex(Selection.gameObjects[0].transform.GetSiblingIndex());
            Undo.RegisterCreatedObjectUndo(group, "Group Selected Object");
            foreach (GameObject obj in topObjects)
            {
                Undo.SetTransformParent(obj.transform, group.transform, "Group Selected Object");
            }
            
            Selection.activeGameObject = group;
        }
        
        private static GameObject[] GetTopSelectedObjects()
        {
            return Selection.gameObjects
                .Where(obj =>
                {
                    Transform parent;
                    return (parent = obj.transform.parent) == null || !Selection.gameObjects.Contains(parent.gameObject);
                })
                .ToArray();
        }
        
        private static bool AreSiblings(GameObject[] gameObjects)
        {
            if (gameObjects.Length < 2) return false;
            Transform parent = gameObjects[0].transform.parent;
            return gameObjects.All(obj => obj.transform.parent == parent);
        }
        
        [Shortcut("Unpack Group", KeyCode.G, ShortcutModifiers.Shift)]
        private static void UnpackGroup()
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                Unpack(obj, false);
            }
        }
        
        [Shortcut("Unpack Group Recursively", KeyCode.G, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void UnpackGroupRecursively()
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                Unpack(obj, true);
            }
        }
        
        private static void Unpack(GameObject obj, bool recursively)
        {
            if (obj.transform.childCount == 0) return;
            
            int parentSiblingIndex = obj.transform.GetSiblingIndex();
            while (obj.transform.childCount > 0)
            {
                var child = obj.transform.GetChild(0);
                Undo.SetTransformParent(child, obj.transform.parent, "Unpack Group");
                Undo.RecordObject(child, "Set Sibling Index");
                child.SetSiblingIndex(++parentSiblingIndex);
                if (recursively) Unpack(child.gameObject, true);
            }
        }
        
        [Shortcut("Rename Selected Object", KeyCode.R, ShortcutModifiers.Shift)]
        private static void RenameSelectedObject()
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                Selection.activeObject = obj;
                EditorApplication.ExecuteMenuItem("Edit/Rename");
            }
        }
        
        // used reflection because unity doesn't expose required methods
        [Shortcut("Expand Hierarchy", KeyCode.E, ShortcutModifiers.Action)]
        private static void ToggleExpandHierarchy()
        {
            var expandedIDs = GetExpandedHierarchyIDs();
            var isExpanded  = expandedIDs.Contains(Selection.activeInstanceID);
            SendExpandEvent(!isExpanded);
        }
        
        [Shortcut("Expand Hierarchy Recursively", KeyCode.E, ShortcutModifiers.Shift)]
        private static void ExpandHierarchyRecursively() => SendExpandEvent(true, true);
        
        [Shortcut("Collapse Hierarchy Recursively", KeyCode.E, ShortcutModifiers.Alt)]
        private static void CollapseHierarchyRecursively() => SendExpandEvent(false, true);
        
        
        private static int[] GetExpandedHierarchyIDs()
        {
            var hierarchyWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            var getCurrentlyExpandedIDs = hierarchyWindowType.GetMethod("GetExpandedIDs", BindingFlags.NonPublic | BindingFlags.Instance);
            var focusedHierarchyWindow = hierarchyWindowType.GetProperty("lastInteractedHierarchyWindow", BindingFlags.Static | BindingFlags.Public);

            if (focusedHierarchyWindow == null) 
                throw new InvalidOperationException("Cannot find focused hierarchy window.");
            if (getCurrentlyExpandedIDs == null) 
                throw new InvalidOperationException("Cannot retrieve GetExpandedIDs method.");

            return getCurrentlyExpandedIDs.Invoke(focusedHierarchyWindow.GetValue(null), null) as int[];
        }
        
        private static void SendExpandEvent(bool expand, bool recursive = false)
        {
            var e = new Event { keyCode = expand ? KeyCode.RightArrow : KeyCode.LeftArrow, type = EventType.KeyDown , alt = recursive };
            EditorWindow.focusedWindow.SendEvent(e);
        }
        
        [Shortcut("Select Parent Object", KeyCode.BackQuote)]
        private static void SelectParentObject()
        {
            Selection.activeGameObject = Selection.activeTransform.parent
                ? Selection.activeTransform.parent.gameObject
                : Selection.activeGameObject;
        }
    }
}
