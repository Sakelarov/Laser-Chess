#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(FlexibleGridLayout))]
public class FlexibleGridLayoutEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var grid = (FlexibleGridLayout)target;

        base.OnInspectorGUI();
    }
}
#endif
