using System;

using UnityEngine;
using UnityEditor;

namespace ClottlyCode
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Transform))]
	public class CCTransformInspector : Editor
	{

		static public CCTransformInspector self;

		SerializedProperty position;
		SerializedProperty rotation;
		SerializedProperty scale;

		DateTime startDate = new DateTime(2018, 8, 4);
		DateTime endDate = new DateTime(2018, 8, 5, 12, 0, 0);

		GUIStyle countdownLabelStyle;
		GUIStyle titleLabelStyle;
		GUIStyle detailLabelStyle;

		Texture2D cloud;

		Color skyColorPro = new Color(0.1568628f, 0.2078431f, 0.5764706f);
		Color skyColor = new Color(0.3921569f, 0.7098039f, 0.9647059f);

		bool matchStarted = false;
		bool matchEnded = false;

		void Awake()
		{
			countdownLabelStyle = new GUIStyle();
			var fontGUID = AssetDatabase.FindAssets("PingFang Medium")[0];
			countdownLabelStyle.font = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(fontGUID));
			countdownLabelStyle.alignment = TextAnchor.MiddleCenter;
			countdownLabelStyle.fontSize = 18;
			countdownLabelStyle.normal.textColor = Color.white;

			titleLabelStyle = new GUIStyle();
			titleLabelStyle.font = countdownLabelStyle.font;
			titleLabelStyle.alignment = TextAnchor.UpperLeft;
			titleLabelStyle.fontSize = 15;
			titleLabelStyle.normal.textColor = Color.white;

			detailLabelStyle = new GUIStyle();
			detailLabelStyle.font = countdownLabelStyle.font;
			detailLabelStyle.alignment = TextAnchor.LowerRight;
			detailLabelStyle.fontSize = 13;
			detailLabelStyle.normal.textColor = Color.white;

			var cloudGUID = AssetDatabase.FindAssets(EditorGUIUtility.isProSkin ? "background_cloud" : "background_cloud light")[0];
			cloud = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(cloudGUID));

			matchStarted = DateTime.Now > startDate;
			matchEnded = DateTime.Now > endDate;


		}

		void OnEnable()
		{
			self = this;

			position = serializedObject.FindProperty("m_LocalPosition");
			rotation = serializedObject.FindProperty("m_LocalRotation");
			scale = serializedObject.FindProperty("m_LocalScale");



			EditorApplication.update += Update;
		}

		void OnDestroy()
		{
			self = null;
			EditorApplication.update -= Update;
		}

		/// <summary>
		/// Draw the inspector widget.
		/// </summary>

		public override void OnInspectorGUI()
		{
			EditorGUIUtility.LookLikeControls(15f);

			serializedObject.Update();

			if (!matchEnded) DrawCountDown();
			EditorGUILayout.Space();
			DrawPosition();
			DrawRotation();
			DrawScale();

			serializedObject.ApplyModifiedProperties();
		}

		double lastUpdate = 0;
		void Update()
		{
			if (EditorApplication.timeSinceStartup - lastUpdate < 0.06f) return;
			lastUpdate = EditorApplication.timeSinceStartup;
			Repaint();
		}

		void DrawCountDown()
		{
			TimeSpan span = endDate - DateTime.Now;

			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(120));
			rect.y += 10;
			rect.height -= 20;
			//EditorGUI.DrawRect(rect, new Color(0.6901961f, 0.7450981f, 0.7725490f));

			int space = 1;
			Rect inner = new Rect(rect.x + space, rect.y + space, rect.width - space * 2, rect.height - space * 2);
			EditorGUI.DrawRect(inner, EditorGUIUtility.isProSkin ? skyColorPro : skyColor);

			GUI.BeginGroup(inner);

			Rect title = new Rect(12, 9, inner.width, inner.height);
			Rect detail = new Rect(0, 0, inner.width, inner.height);

			float imageWidth = 530f;
			float cloudStartX = -(float)EditorApplication.timeSinceStartup * 10f;
			float curPos = cloudStartX;
			while (true)
			{
				if (curPos + imageWidth < 0)
				{
					curPos += imageWidth;
					continue;
				}

				if (curPos > inner.width) break;

				Rect cloudRect = new Rect(curPos, 10, imageWidth, inner.height);
				GUI.DrawTexture(cloudRect, cloud);
				curPos += imageWidth;
			}

			if (matchStarted)
			{
				EditorGUI.LabelField(title, "Upcoming Deadline :", titleLabelStyle);
				EditorGUI.LabelField(detail, $"{LineCounter.Count()} Lines ", detailLabelStyle);
			}
			else EditorGUI.LabelField(detail, "比赛将于2017年8月1日开始", detailLabelStyle);

			GUI.EndGroup();

			EditorGUI.LabelField(inner, matchStarted ? string.Format("{0} days, {1}h, {2}min, {3}sec", span.Days, span.Hours.ToString("D2"), span.Minutes.ToString("D2"), span.Seconds.ToString("D2")) : "比赛即将开始, 尽情期待...", countdownLabelStyle);


		}

		void DrawPosition()
		{
			GUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Position");
				EditorGUILayout.PropertyField(position.FindPropertyRelative("x"));
				EditorGUILayout.PropertyField(position.FindPropertyRelative("y"));
				EditorGUILayout.PropertyField(position.FindPropertyRelative("z"));

				bool reset = GUILayout.Button("X", GUILayout.Width(20f));

				if (reset) position.vector3Value = Vector3.zero;
			}
			GUILayout.EndHorizontal();
		}

		void DrawScale()
		{
			GUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Scale");
				EditorGUILayout.PropertyField(scale.FindPropertyRelative("x"));
				EditorGUILayout.PropertyField(scale.FindPropertyRelative("y"));
				EditorGUILayout.PropertyField(scale.FindPropertyRelative("z"));

				bool reset = GUILayout.Button("X", GUILayout.Width(20f));

				if (reset) scale.vector3Value = Vector3.one;
			}
			GUILayout.EndHorizontal();
		}

		enum Axes : int
		{
			None = 0,
			X = 1,
			Y = 2,
			Z = 4,
			All = 7,
		}

		Axes CheckDifference(Transform t, Vector3 original)
		{
			Vector3 next = t.localEulerAngles;

			Axes axes = Axes.None;

			if (Differs(next.x, original.x)) axes |= Axes.X;
			if (Differs(next.y, original.y)) axes |= Axes.Y;
			if (Differs(next.z, original.z)) axes |= Axes.Z;

			return axes;
		}

		Axes CheckDifference(SerializedProperty property)
		{
			Axes axes = Axes.None;

			if (property.hasMultipleDifferentValues)
			{
				Vector3 original = property.quaternionValue.eulerAngles;

				foreach (UnityEngine.Object obj in serializedObject.targetObjects)
				{
					axes |= CheckDifference(obj as Transform, original);
					if (axes == Axes.All) break;
				}
			}
			return axes;
		}

		static bool FloatField(string name, ref float value, bool greyedOut, GUILayoutOption opt)
		{
			float newValue = value;
			GUI.changed = false;
			newValue = EditorGUILayout.FloatField(name, newValue, opt);

			if (GUI.changed && Differs(newValue, value))
			{
				value = newValue;
				return true;
			}
			return false;
		}

		static bool Differs(float a, float b) { return Mathf.Abs(a - b) > 0.0001f; }

		void DrawRotation()
		{
			GUILayout.BeginHorizontal();
			{
				Vector3 visible = (serializedObject.targetObject as Transform).localEulerAngles;
				Axes changed = CheckDifference(rotation);
				Axes altered = Axes.None;

				GUILayoutOption opt = GUILayout.MinWidth(30f);

				EditorGUILayout.LabelField("Rotation");
				if (FloatField("X", ref visible.x, (changed & Axes.X) != 0, opt)) altered |= Axes.X;
				if (FloatField("Y", ref visible.y, (changed & Axes.Y) != 0, opt)) altered |= Axes.Y;
				if (FloatField("Z", ref visible.z, (changed & Axes.Z) != 0, opt)) altered |= Axes.Z;

				bool reset = GUILayout.Button("X", GUILayout.Width(20f));

				if (reset)
				{
					rotation.quaternionValue = Quaternion.identity;
				}
				else if (altered != Axes.None)
				{
					Undo.RecordObjects(serializedObject.targetObjects, "Undo Inspector");

					foreach (UnityEngine.Object obj in serializedObject.targetObjects)
					{
						Transform t = obj as Transform;
						Vector3 v = t.localEulerAngles;

						if ((altered & Axes.X) != 0) v.x = visible.x;
						if ((altered & Axes.Y) != 0) v.y = visible.y;
						if ((altered & Axes.Z) != 0) v.z = visible.z;

						t.localEulerAngles = v;
					}
				}
			}
			GUILayout.EndHorizontal();
		}
	}
}