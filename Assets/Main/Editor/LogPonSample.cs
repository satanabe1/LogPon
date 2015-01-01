using UnityEngine;
using UnityEditor;
using System.Collections;

public class LogPonSample : EditorWindow
{
	private static LogPonSample instance;
	
	[MenuItem("Tools/LogPon")]
	public static void OpenWindow ()
	{
		instance = GetWindow<LogPonSample> ();
	}
	
	[MenuItem("Tools/LogPon SendEvent")]
	public static void DoRepaint ()
	{
		Debug.Log ("visit1");
		instance = GetWindow<LogPonSample>(true, null, false);
		if (instance == null) {
			return;
		}
		Debug.Log ("visit2");
		//Event guiEvent =  new Event();
		//guiEvent.type = EventType.Repaint;
		//instance.SendEvent(guiEvent);
		instance.count++;
		var repaintEvent = EditorGUIUtility.CommandEvent ("DoRepaint");
		repaintEvent.type = EventType.used;
		instance.SendEvent (repaintEvent);
	}
	
	private int count = 0;
	
	public void OnGUI ()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.AppendLine(Event.current.type.ToString());
		sb.Append("command:").AppendLine(Event.current.commandName);
		Debug.Log (sb.ToString());
		EditorGUILayout.LabelField("count:" + count);
	}
	
	public void Update()
	{
		//DoRepaint();
	}
	
	protected virtual void OnEnable ()
	{
		instance = this;
	}
}
