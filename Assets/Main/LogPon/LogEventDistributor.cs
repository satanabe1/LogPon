using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LogPon
{
    /// <summary>
    /// Debug#Logの出力を表す
    /// あとで、フィルターとか、タグとかがつくかも知れない
    /// </summary>
    [System.Serializable]
    public class LogEntry
    {
        [SerializeField]
        private string
            condition;

        public string Condition { get { return condition; } }

        [SerializeField]
        private string
            stackTrace;

        public string StackTrace { get { return stackTrace; } }

        [SerializeField]
        private LogType
            logType;

        public LogType LogType { get { return logType; } }

        public LogEntry (string condition, string stackTrace, LogType logType)
        {
            this.condition = condition;
            this.stackTrace = stackTrace;
            this.logType = logType;
        }
    }

    /// <summary>
    /// Debug#Log** を Application#RegisterLogCallback で引っ掛けるクラス
    /// 複数のコールバックを設定することができる分配器的なもの
    /// LogEventDistributor#Init をすると Application#RegisterLogCallback が呼び出される
    /// </summary>
    public static class LogEventDistributor
    {
        private static List<Application.LogCallback> logCallbackList = new List<Application.LogCallback> ();
        private static List<System.Action<LogEntry>> logActionList = new List<System.Action<LogEntry>> ();

        /// <summary>
        /// Application#RegisterLogCallbackを実行する
        /// </summary>
        /// <param name="withClearCallback">これがtrueなら、過去にAddLogCallback等で登録されていたCallbackを削除する</param>
        public static void Init (bool withClearCallback)
        {
            if (withClearCallback) {
                ClearLogCallbackList ();
            }
            Application.RegisterLogCallback (OnLogEvent);
        }

        /// <summary>
        /// コールバック一覧の取得
        /// </summary>
        /// <returns>The log handler list.</returns>
        public static System.Collections.ObjectModel.ReadOnlyCollection<Application.LogCallback> GetLogHandlerList ()
        {
            return logCallbackList.AsReadOnly ();
        }

        /// <summary>
        /// コールバックの追加
        /// </summary>
        /// <param name="logCallback">Log callback.</param>
        public static bool AddLogCallback (Application.LogCallback logCallback)
        {
            var isValid = IsValidLogCallback (logCallback);
            if (IsValidLogCallback (logCallback) && (logCallbackList.Contains (logCallback) == false)) {
                logCallbackList.Add (logCallback);
            }
            return isValid;
        }

        /// <summary>
        /// コールバックの追加
        /// </summary>
        /// <param name="logCallback">Log callback.</param>
        public static bool AddLogCallback (System.Action<LogEntry> logCallback)
        {
            var isValid = IsValidLogCallback (logCallback);
            if (IsValidLogCallback (logCallback) && (logActionList.Contains (logCallback) == false)) {
                logActionList.Add (logCallback);
            }
            return isValid;
        }

        /// <summary>
        /// コールバックの追加
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="logCallback">Log callback.</param>
        public static bool InsertLogCallback (int index, Application.LogCallback logCallback)
        {
            var isValid = IsValidLogCallback (logCallback);
            if (IsValidLogCallback (logCallback) && (logCallbackList.Contains (logCallback) == false)) {
                logCallbackList.Insert (index, logCallback);
            }
            return isValid;
        }

        /// <summary>
        /// コールバックの追加
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="logCallback">Log callback.</param>
        public static bool InsertLogCallback (int index, System.Action<LogEntry> logCallback)
        {
            var isValid = IsValidLogCallback (logCallback);
            if (IsValidLogCallback (logCallback) && (logActionList.Contains (logCallback) == false)) {
                logActionList.Insert (index, logCallback);
            }
            return isValid;
        }

        /// <summary>
        /// コールバックの削除
        /// </summary>
        /// <returns><c>true</c>, if log callback was removed, <c>false</c> otherwise.</returns>
        /// <param name="logCallback">Log callback.</param>
        public static bool RemoveLogCallback (Application.LogCallback logCallback)
        {
            return logCallbackList.Remove (logCallback);
        }

        /// <summary>
        /// コールバックの削除
        /// </summary>
        /// <returns><c>true</c>, if log callback was removed, <c>false</c> otherwise.</returns>
        /// <param name="logCallback">Log callback.</param>
        public static bool RemoveLogCallback (System.Action<LogEntry> logCallback)
        {
            return logActionList.Remove (logCallback);
        }

        /// <summary>
        /// コールバックを全て削除する
        /// </summary>
        public static void ClearLogCallbackList ()
        {
            logCallbackList.Clear ();
            logActionList.Clear ();
        }

        /// <summary>
        /// Unity的に破棄済みのオブジェクトへのコールバックを削除する
        /// </summary>
        public static void CleanNullObjects ()
        {
            logCallbackList = (from callback in logCallbackList
                                        where IsValidLogCallback (callback)
                                        select callback).ToList ();
            logActionList = (from callback in logActionList
                                      where IsValidLogCallback (callback)
                                      select callback).ToList ();
        }

        /// <summary>
        /// コールバックがnullだったり、破棄済みのUnityオブジェクトだったりしないかを検証する
        /// </summary>
        /// <returns><c>true</c> if is valid log callback the specified logCallback; otherwise, <c>false</c>.</returns>
        /// <param name="logCallback">Log callback.</param>
        private static bool IsValidLogCallback (Application.LogCallback logCallback)
        {
            if (logCallback == null) {
                return false;
            }
            if (logCallback.Method == null) {
                return false;
            }
            if ((logCallback.Target is object) && (logCallback.Target == null)) {
                // 破棄済みのUnityオブジェクトだった場合
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns><c>true</c> if is valid log callback the specified logCallback; otherwise, <c>false</c>.</returns>
        /// <param name="logCallback">Log callback.</param>
        private static bool IsValidLogCallback (System.Action<LogEntry> logCallback)
        {
            if (logCallback == null) {
                return false;
            }
            if (logCallback.Method == null) {
                return false;
            }
            if ((logCallback.Target is object) && (logCallback.Target == null)) {
                // 破棄済みのUnityオブジェクトだった場合
                return false;
            }
            return true;
        }

        /// <summary>
        /// Application#RegisterLogCallbackに渡される、コールバックの実体
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="stackTrace">Stack trace.</param>
        /// <param name="logType">Log type.</param>
        private static void OnLogEvent (string condition, string stackTrace, LogType logType)
        {
            logCallbackList.ForEach ((callback) => {
                try {
                    callback.Invoke (condition, stackTrace, logType);
                } catch (System.Exception ex) {
                    if (IsValidLogCallback (callback) == false) {
                        RemoveLogCallback (callback);
                        Debug.LogError (ex);
                    }
                }
            });

            var entry = CreateEntry (condition, stackTrace, logType);
            logActionList.ForEach ((callback) => {
                try {
                    callback.Invoke (entry);
                } catch (System.Exception ex) {
                    if (IsValidLogCallback (callback) == false) {
                        RemoveLogCallback (callback);
                        Debug.LogError (ex);
                    }
                }
            });
        }

        /// <summary>
        /// とりあえず、今は、てきとーに
        /// </summary>
        /// <returns>The entry.</returns>
        /// <param name="condition">Condition.</param>
        /// <param name="stackTrace">Stack trace.</param>
        /// <param name="logType">Log type.</param>
        private static LogEntry CreateEntry (string condition, string stackTrace, LogType logType)
        {
            return new LogEntry (condition, stackTrace, logType);
        }
    }
    //  public static void Dump ()
    //  {
    //    logCallbackList.ForEach ((callback) => {
    //      System.Text.StringBuilder sb = new System.Text.StringBuilder ();
    //      sb.Append (callback).AppendLine ();
    //      sb.Append ("Method:").Append (callback.Method).AppendLine ();
    //      sb.Append ("Target:").Append (callback.Target).AppendLine ();
    //      Debug.Log (sb);
    //    });
    //  }
}