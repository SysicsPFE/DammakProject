using UnityEngine;

namespace Cinemachine.Examples
{

[AddComponentMenu("")] // Don't display in add component menu
public class ExampleHelpWindow : MonoBehaviour
{
    public string mTitle;
    [Multiline]
    public string mDescription;

    private bool _mShowingHelpWindow = true;
    private const float KPadding = 40f;

    private void OnGUI()
    {
        if (_mShowingHelpWindow)
        {
            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(mDescription));
            Vector2 halfSize = size * 0.5f;

            float maxWidth = Mathf.Min(Screen.width - KPadding, size.x);
            float left = Screen.width * 0.5f - maxWidth * 0.5f;
            float top = Screen.height * 0.4f - halfSize.y;

            Rect windowRect = new Rect(left, top, maxWidth, size.y);
            GUILayout.Window(400, windowRect, (id) => DrawWindow(id, maxWidth), mTitle);
        }
    }

    private void DrawWindow(int id, float maxWidth)
    {
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(mDescription);
        GUILayout.EndVertical();
        if (GUILayout.Button("Got it!"))
        {
            _mShowingHelpWindow = false;
        }
    }
}

}