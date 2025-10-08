using UnityEngine;

public class OpenURL : MonoBehaviour
{
    public void Open(string url)
    {
        if (string.IsNullOrEmpty(url)) return;
        Application.OpenURL(url);
    }
}
