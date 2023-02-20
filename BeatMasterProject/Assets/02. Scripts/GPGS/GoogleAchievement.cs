using UnityEngine;

public class GoogleAchievement : MonoBehaviour
{
    private string _log;

    private void OnGUI()
    {
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 3);

        if (GUILayout.Button("ShowAchievementUI"))
            GPGSBinder.Instance.ShowAchievementUI();

        if (GUILayout.Button("Achievement"))
            GPGSBinder.Instance.UnlockAchievement(GPGSIds.achievement_first_purchase, success => _log = $"{success}");

        if (GUILayout.Button("Achievement2"))
            GPGSBinder.Instance.UnlockAchievement(GPGSIds.achievement_purchase_all_characters, success => _log = $"{success}");

        GUILayout.Label(_log);
    }
}
