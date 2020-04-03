using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

// source: https://github.com/BlackthornProd/Hyperlink-project
public class LinkOpener : MonoBehaviour 
{
	protected virtual string getURL()
	{
		return "";
	}

	protected void openLink()
	{
#if UNITY_WEBGL && !UNITY_EDITOR
	openWindow(getURL());
#else
	Application.OpenURL(getURL());
#endif
	}

	[DllImport("__Internal")]
	private static extern void openWindow(string url);

}