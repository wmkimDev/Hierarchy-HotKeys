# Hierarchy HotKeys for Unity Editor

This Unity Editor script enhances productivity by providing a series of hotkeys for common operations in the Unity Hierarchy window. It allows developers and artists to quickly manipulate GameObjects without having to navigate through menus, speeding up the workflow within the Unity Editor.


### How to Use

```
/Assets
    /Editor
        /HierarchyHotKeys.cs
```

- **Installation** : Copy the `HierarchyHotKeys.cs` script into an Editor folder within your Unity project's Assets directory.
- **Accessing Hotkeys** : Once the script is compiled by Unity, the hotkeys will be available whenever the Unity Editor is focused, particularly in the Hierarchy window.
- **Customization** : You can customize the assigned hotkeys by navigating to `Edit > Shortcuts...` in Unity and searching for the corresponding command names.

### Hotkeys


| Command Name | Hotkey Combination | Action |
| ---- | ---- | ---- |
| Toggle GameObject Active State | `A` | Toggle the active state of selected game objects. |
| Activate/Deactivate Selected Objects | `Shift + A` | Set all selected game objects to active or inactive, depending on the state of any inactive object. |
| Delete Selected Objects | `Shift + X` | Delete selected game objects. |
| Group Selected Objects | `G` | Group selected top-level game objects. |
| Unpack Group | `Shift + G` | Ungroup selected game objects. |
| Unpack Group Recursively | `Shift + Alt + G` | Ungroup selected game objects recursively. |
| Rename Selected Object | `Shift + R` | Rename selected game object. |
| Toggle Expand Hierarchy | `Ctrl + E` | Toggle expansion of the currently selected hierarchy item. |
| Expand Hierarchy Recursively | `Shift + E` | Expand the currently selected hierarchy item recursively. |
| Collapse Hierarchy Recursively | `Alt + E` | Collapse the currently selected hierarchy item recursively. |
| Select Parent Object | `Backquote` | Select the parent object of the current selection |

### Contributing

We welcome contributions from the community! Whether you're fixing bugs, adding new features, or improving documentation, your help is appreciated.


