using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.IO;

public class TakeThumbnail : MonoBehaviour
{

	private static TakeThumbnail instance;

	void Awake()
	{
		instance = this;
	}

	public static void Generate(string path, Canvas[] ignoreCanvas, float scale)
	{
		if (string.IsNullOrEmpty(path)) return;
		scale = Mathf.Clamp(scale, .1f, 1f);
		instance.GenerateThumbnail(path, ignoreCanvas, scale);
	}

	void GenerateThumbnail(string path, Canvas[] ignoreCanvas, float scale)
	{
		StartCoroutine(Thumbnail(path, ignoreCanvas, scale));
	}

	IEnumerator Thumbnail(string path, Canvas[] ignoreCanvas, float scale)
	{
		yield return null;

		if (ignoreCanvas != null)
			for (int i = 0; i < ignoreCanvas.Length; i++) ignoreCanvas[i].enabled = false;

		yield return new WaitForEndOfFrame();

		Texture2D result = null;
		Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		tex.Apply();

		if (ignoreCanvas != null)
			for (int i = 0; i < ignoreCanvas.Length; i++) ignoreCanvas[i].enabled = true;

		int width = (int)(Screen.width * scale);
		int height = (int)(Screen.height * scale);

		if (scale < 1)
		{
			result = new Texture2D(width, height, tex.format, true);
			Color[] pixels = result.GetPixels(0);

			float x = ((float)1 / tex.width) * ((float)tex.width / width);
			float y = ((float)1 / tex.height) * ((float)tex.height / height);

			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = tex.GetPixelBilinear(x * ((float)i % width), y * ((float)Mathf.Floor(i / width)));
			}

			result.SetPixels(pixels, 0);
			result.Apply();
		}
		else
		{
			result = tex;
		}



		byte[] bytes = result.EncodeToPNG();
		File.WriteAllBytes(path, bytes);

		Debug.Log(this + " --> Создано изображение (" + width + "x" + height + ") по адресу: " + path);

		if (tex != null) Destroy(tex);
		if (result != null) Destroy(result);
	}

	public static Texture2D LoadTexture2D(string path)
	{
		if (!File.Exists(path)) return null;
		byte[] data = File.ReadAllBytes(path);
		Texture2D result = new Texture2D(2, 2);
		result.LoadImage(data);
		return result;
	}

	public static Sprite LoadSprite(string path)
	{
		if (!File.Exists(path)) return null;
		byte[] data = File.ReadAllBytes(path);
		Texture2D tex = new Texture2D(2, 2);
		tex.LoadImage(data);
		return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
	}
}