using UnityEngine;
using UnityEditor;
using System.Collections;

//Much of this was taken or at least derived from iTweenPathEditor.cs

[CustomEditor(typeof(CAudioHelper))]
public class CAudioHelperEditor : Editor
{
	CAudioHelper m_audioHelperTarget;
	bool m_fShowSFX = true;
	bool m_fShowMusic = true;

	Rect m_rectDropArea;

	void OnEnable()
	{
		m_audioHelperTarget = target as CAudioHelper;

		 m_rectDropArea = new Rect(0, 0, 1, 1);			
	}

	public override void OnInspectorGUI()
	{
		m_audioHelperTarget.vEnsureListSizes();

		EditorGUILayout.PrefixLabel("SFX Player Count");
		m_audioHelperTarget.m_cSFXPlayers = EditorGUILayout.IntField(m_audioHelperTarget.m_cSFXPlayers);
		EditorGUILayout.PrefixLabel("SFX Player Cycle Count");
		m_audioHelperTarget.m_cSFXCycle = EditorGUILayout.IntField(m_audioHelperTarget.m_cSFXCycle);

		EditorGUI.indentLevel = 0;
		m_fShowSFX = EditorGUILayout.Foldout(m_fShowSFX, "Sound Effects");
		if (m_fShowSFX)
		{
			EditorGUI.indentLevel = 1;

			Rect t_rectStart = EditorGUILayout.BeginHorizontal();
			EditorGUILayout.EndHorizontal();

			for (int t_i = 0; t_i < m_audioHelperTarget.m_listaudioclipSFXs.Count; ++t_i)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel(((CAudioHelper.ESound)t_i).ToString());
				m_audioHelperTarget.m_listaudioclipSFXs[t_i] = EditorGUILayout.ObjectField(m_audioHelperTarget.m_listaudioclipSFXs[t_i], typeof(AudioClip), false) as AudioClip;
				m_audioHelperTarget.m_listSFXVolume[t_i] = EditorGUILayout.Slider(m_audioHelperTarget.m_listSFXVolume[t_i], 0.0f, 1.0f);
				EditorGUILayout.EndHorizontal();
			}

			Rect t_rectEnd = EditorGUILayout.BeginHorizontal();
			EditorGUILayout.EndHorizontal();
			//m_rectDropArea.x = t_rectStart.x;
			m_rectDropArea.y = t_rectStart.y;
			m_rectDropArea.width = 16;
			m_rectDropArea.height = t_rectEnd.y - t_rectStart.y;
			EditorGUI.DrawRect(m_rectDropArea, Color.green);
		}

		EditorGUI.indentLevel = 0;
		m_fShowMusic = EditorGUILayout.Foldout(m_fShowMusic, "Music");
		if (m_fShowMusic)
		{
			EditorGUI.indentLevel = 1;
			for (int t_i = 0; t_i < m_audioHelperTarget.m_listaudioclipMusic.Count; ++t_i)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel(((CAudioHelper.EMusic)t_i).ToString());
				m_audioHelperTarget.m_listaudioclipMusic[t_i] = EditorGUILayout.ObjectField(m_audioHelperTarget.m_listaudioclipMusic[t_i], typeof(AudioClip), false) as AudioClip;
				EditorGUILayout.EndHorizontal();
			}
		}
		
		//update and redraw:
		if(GUI.changed)
		{
			EditorUtility.SetDirty(m_audioHelperTarget);			
		}

		vDragDropGUI(m_rectDropArea);
	}

	protected void vDragDropGUI(Rect dropArea)//Rect dropArea, SerializedProperty property)
	{
		// Cache References:
		Event currentEvent = Event.current;
		EventType currentEventType = currentEvent.type;
		
		// The DragExited event does not have the same mouse position data as the other events,
		// so it must be checked now:
		if ( currentEventType == EventType.DragExited ) 
		{	
			// Clear generic data when user pressed escape. (Unfortunately, DragExited is also called when the mouse leaves the drag area)
			DragAndDrop.PrepareStartDrag();
		}
		
		if (!dropArea.Contains(currentEvent.mousePosition))
		{
			return;
		}
		
		switch (currentEventType)
		{


			case EventType.DragUpdated:
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Link;
				currentEvent.Use();
			}
				break;     

			case EventType.Repaint:
			{
				if (DragAndDrop.visualMode == DragAndDropVisualMode.None||
					DragAndDrop.visualMode == DragAndDropVisualMode.Rejected)
				{
					break;
				}
				
				EditorGUI.DrawRect(dropArea, Color.grey);      
			}
				break;

			case EventType.DragPerform:
			{
				DragAndDrop.AcceptDrag();
				
				vAddDraggedObjectsToList();
				
				currentEvent.Use();
			}
				break;

			case EventType.MouseUp:
			{
				// Clean up, in case MouseDrag never occurred:
				DragAndDrop.PrepareStartDrag();
			}
				break;
		}
		
	}

	protected void vAddDraggedObjectsToList()
	{
		foreach (Object t_obj in DragAndDrop.objectReferences)
		{
			string t_strName = t_obj.name;
			if (t_strName.EndsWith(".wav"))
			{
				t_strName = t_strName.Remove(t_strName.IndexOf(".wav"));
			}

			vEnsureClip(t_strName, t_obj);
		}
	}

	protected void vEnsureClip(string p_strName, Object p_obj)
	{
		int t_iEnum = m_audioHelperTarget.iFromName(p_strName);
		if (t_iEnum != -1)
		{
			m_audioHelperTarget.m_listaudioclipSFXs[t_iEnum] = p_obj as AudioClip;
		}
	}


}


