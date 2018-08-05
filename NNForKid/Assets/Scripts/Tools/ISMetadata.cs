using System;
using UnityEngine;

[Serializable]
public struct ISMetadata {
	public string name;
	public string label;

	[TextArea(3, 3)] public string description;

	[TextArea(2, 3)] public string shortDescription;

	public Sprite icon;
	public Sprite spriteSet;
	public Color primaryColor;

	[Range(0f,1f)] public float dropRate;
}