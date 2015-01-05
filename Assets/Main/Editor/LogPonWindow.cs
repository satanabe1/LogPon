#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace LogPon
{
    public class LogPonWindow : EditorWindow
    {
        private static class Command
        {
            public const string PREFIX = "LogPon/";
            public const string OPEN = PREFIX + "Open Window";
            public const string CLOSE = PREFIX + "Close Window";

            [MenuItem (Command.OPEN)]
            public static void OpenWindow ()
            {
                GetWindow<LogPonWindow> ();
            }

            [MenuItem (Command.CLOSE)]
            public static void CloseWindow ()
            {
                if (instance != null) {
                    instance.Close ();
                }
            }
        }

        public static LogPonWindow instance;
        private Vector2 listScrollPosition;

        public static void RequireRepaint ()
        {
            if (instance != null) {
                instance.Repaint ();
            }
        }

        protected void OnEnable ()
        {
            instance = this;
        }

        protected void OnDisable ()
        {
            instance = null;
        }

        protected void OnGUI ()
        {
            listScrollPosition = EditorGUILayout.BeginScrollView (listScrollPosition);
            foreach (var log in LogPonAdapter.LogCollection) {
                // てきとーに、ボタン
                GUILayout.Button (log.Condition);
            }
            EditorGUILayout.EndScrollView ();
        }
    }
}
#endif
