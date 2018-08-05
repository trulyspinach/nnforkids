using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISLRCircleInspector : MonoBehaviour {

	private int division = 16;
	private float radius = 1;
	
	private Vector3[] m_cachedUnitPoints;
	private LineRenderer m_renderer;

	public static ISLRCircleInspector Create(float radius, Material material, int division = 16) {
		var obj = new GameObject("Circle Inspector");
		var renderer = obj.AddComponent<ISLRCircleInspector>();
		renderer.division = division;
		renderer.radius = radius;
		renderer.Init(material);
		return renderer;
	}

	public void SetWidth(float width) {
		m_renderer.widthMultiplier = width;
	}
	
	public void Init(Material lineMaterial) {
		m_renderer = gameObject.AddComponent<LineRenderer>();
		m_renderer.material = lineMaterial;
		GenerateUnitPoints();
	}

	public void Set(Vector3 center, float r) {
		var points = new Vector3[division + 1];
		for (var i = 0; i < points.Length; i++) {
			points[i] = center + m_cachedUnitPoints[i] * r;
		}
		m_renderer.SetPositions(points);
	}
	
	public void GenerateUnitPoints() {
		m_cachedUnitPoints = new Vector3[division + 1];
		var angleStep = 2 * Mathf.PI / division;
		for (var i = 0; i < division; i++) {
			m_cachedUnitPoints[i] = new Vector3(Mathf.Sin(angleStep * i), 0, Mathf.Cos(angleStep * i));
		}
		m_cachedUnitPoints[division] = new Vector3(0, 0, 1);
		m_renderer.positionCount = division + 1;
	}
}
