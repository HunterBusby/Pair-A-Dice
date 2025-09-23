using UnityEngine;

public class ScaleModifier : MonoBehaviour
{
    // ✅ Set scale from FloatData (ScriptableObject)

    public void SetScaleX(FloatData data)
    {
        if (data == null) return;
        Vector3 s = transform.localScale;
        s.x = data.value;
        transform.localScale = s;
    }

    public void SetScaleY(FloatData data)
    {
        if (data == null) return;
        Vector3 s = transform.localScale;
        s.y = data.value;
        transform.localScale = s;
    }

    public void SetScaleZ(FloatData data)
    {
        if (data == null) return;
        Vector3 s = transform.localScale;
        s.z = data.value;
        transform.localScale = s;
    }

    public void SetScale(FloatData data)
    {
        if (data == null) return;
        transform.localScale = Vector3.one * data.value;
    }
}
