#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace bHapticsOSC.VRChat
{
    public class bReorderableListContainer<T>
    {
        private string Name;
        private T DefaultElement;
        private GUIStyle LabelStyle;
        private SerializedObject SerializedObject;
        private SerializedProperty SerializedProperty;
        private ReorderableList List;
        private bool Draggable;
        private bool DisplayAddButton;
        private bool DisplayRemoveButton;
        private Type ElementType;

        public bReorderableListContainer(
            string name,
            T defaultElement,
            GUIStyle labelStyle,
            SerializedProperty serializedProperty,
            bool draggable = true,
            bool displayAddButton = true,
            bool displayRemoveButton = true)
        {
            Name = name;
            DefaultElement = defaultElement;
            LabelStyle = labelStyle;
            SerializedObject = serializedProperty.serializedObject;
            SerializedProperty = serializedProperty;
            Draggable = draggable;
            DisplayAddButton = displayAddButton;
            DisplayRemoveButton = displayRemoveButton;
            ElementType = typeof(T);

            List = new ReorderableList(SerializedObject, SerializedProperty, Draggable, true, DisplayAddButton, DisplayRemoveButton);
            List.drawHeaderCallback = DrawHeader;
            List.drawElementCallback = DrawElement;
            List.drawFooterCallback = DrawFooter;
            List.onAddCallback = (ReorderableList x) => OnAdd();
        }

        public void Draw()
        {
            SerializedObject.Update();

            List.DoLayoutList();

            SerializedObject.ApplyModifiedProperties();
        }

        private void OnAdd()
        {
            ReorderableList.defaultBehaviours.DoAddButton(List);

            int index = List.count - 1;
            SerializedProperty element = SerializedProperty.GetArrayElementAtIndex(index);

            if (ElementType == typeof(string))
                element.stringValue = $"{DefaultElement as string}_{index + 1}";

            element.serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(Rect rect)
        {
            rect.x -= 7;
            rect.y -= 1;
            rect.width += 13;
            rect.height += 2;

            GUI.Box(rect, Name, GUI.skin.window);
            GUI.Label(rect, Name, LabelStyle);
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (List.count <= 0)
                return;

            ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, isFocused, Draggable);

            rect.x += 1;
            rect.width += 5;

            SerializedProperty element = SerializedProperty.GetArrayElementAtIndex(index);

            if (ElementType == typeof(string))
                element.stringValue = GUI.TextField(rect, element.stringValue);

            element.serializedObject.ApplyModifiedProperties();

            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Text);

            rect.x -= 20;
            rect.width = 40;

            EditorGUIUtility.AddCursorRect(rect, MouseCursor.MoveArrow);
        }

        private void DrawFooter(Rect rect)
        {
            if (!DisplayAddButton && !DisplayRemoveButton)
                return;

            ReorderableList.defaultBehaviours.DrawFooter(rect, List);

            int offset = 50;

            rect.x = rect.width - offset + 8;
            rect.width = offset + 7;

            bool isRemoveUseable = (List.index != -1) && (List.count > 0);
            bool onlyOneButton = (!DisplayAddButton && DisplayRemoveButton) || (DisplayAddButton && !DisplayRemoveButton);
            if (onlyOneButton)
            {
                if (DisplayRemoveButton)
                {
                    if (!isRemoveUseable)
                        return;
                    rect.x += offset / 2;
                }
                rect.width /= 2;
            }
            else if (!isRemoveUseable)
                rect.width /= 2;

            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
        }
    }
}
#endif