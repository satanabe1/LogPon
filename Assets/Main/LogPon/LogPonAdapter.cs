using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LogPon
{
    /// <summary>
    /// ゲーム側とEditorScript側の中間
    /// LogPonWindowはこいつを見て表示を変える
    /// </summary>
    [ExecuteInEditMode]
    public static class LogPonAdapter
    {

        #region selectedTag
        private static bool tagIsChanged = false;
        private static string selectedTag;

        /// <summary>
        /// 表示されるログのタグ設定
        /// この値を書き変えるとtagIsChangedがtrueになる
        /// </summary>
        /// <value>The select tag.</value>
        public static string SelectedTag {
            get {
                return selectedTag;
            }
            set {
                if (selectedTag != value) {
                    tagIsChanged = true;
                }
                selectedTag = value;
                RequireRepaintView ();
            }
        }
        #endregion selectedTag

        #region logList
        private static List<LogEntry> logList;

        /// <summary>
        /// すべてのログ
        /// </summary>
        /// <value>The log list.</value>
        private static List<LogEntry> LogList {
            get {
                if (logList == null) {
                    logList = new List<LogEntry> ();
                }
                return logList;
            }
        }

        /// <summary>
        /// すべてのログ
        /// </summary>
        /// <value>The read only logs.</value>
        public static System.Collections.ObjectModel.ReadOnlyCollection<LogEntry> LogCollection {
            get {
                return LogList.AsReadOnly ();
            }
        }
        #endregion logList

        #region selectedLogList
        private static List<LogEntry> selectedLogList;

        /// <summary>
        /// 選択中のタグに該当するログ
        /// </summary>
        /// <value>The selected log list.</value>
        private static List<LogEntry> SelectedLogList {
            get {
                if (tagIsChanged) {
                    tagIsChanged = false;
                    if (string.IsNullOrEmpty (SelectedTag)) {
                        selectedLogList = LogList;
                    } else {
                        selectedLogList = LogList.FindAll ((log) => log != null && log.Tag == SelectedTag);
                    }
                }
                if (selectedLogList == null) {
                    selectedLogList = new List<LogEntry> ();
                }
                return selectedLogList;
            }
        }

        /// <summary>
        /// 選択中のタグに該当するログ
        /// </summary>
        /// <value>The selected logs.</value>
        public static System.Collections.ObjectModel.ReadOnlyCollection<LogEntry> SelectedLogCollection {
            get {
                return SelectedLogList.AsReadOnly ();
            }
        }
        #endregion selectedLogList


        /// <summary>
        /// ログを登録する LogEventDistributor#AddLogCallback に突っ込む事を想定？
        /// </summary>
        /// <param name="logEntry">Log entry.</param>
        public static void AddLog (LogEntry logEntry)
        {
            if (logEntry == null) {
                return;
            }
            LogList.Add (logEntry);
            if (logEntry.Tag == SelectedTag) {
                SelectedLogList.Add (logEntry);
            }
            RequireRepaintView ();
        }

        /// <summary>
        /// ログのクリア
        /// </summary>
        public static void ClearLogList ()
        {
            LogList.Clear ();
            SelectedLogList.Clear ();
            RequireRepaintView ();
        }

        /// <summary>
        /// EditorWindowへログの変更を通知する
        /// クラス名を知らなくても伝えられる方法を模索中
        /// </summary>
        private static void RequireRepaintView ()
        {
#if UNITY_EDITOR
            LogPonWindow.RequireRepaint ();
#endif
        }
    }
}