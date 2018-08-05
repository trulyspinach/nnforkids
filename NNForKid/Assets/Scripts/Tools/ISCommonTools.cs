using UnityEngine;

public class ISCommonTools {

	public static Material CreateTempMaterial(string shaderName)
	{
		var shader = Shader.Find(shaderName);

		if (shader == null)
		{
			Debug.LogError("No shader names: " + shaderName); return null;
		}

		var material = new Material(shader) {hideFlags = HideFlags.HideAndDontSave};


		return material;
	}
}
