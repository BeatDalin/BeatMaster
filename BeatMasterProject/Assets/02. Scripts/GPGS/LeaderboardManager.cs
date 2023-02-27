using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class LeaderboardManager : MonoBehaviour
{

    public void ReportScore(int stageIdx, int levelIdx, int score)
    {
        GPGSBinder.Instance.ReportLeaderboard(GPGSBinder.Instance.CheckStageIdx(stageIdx), score, success =>
        {
            if (success)
            {
                Debug.Log("Successfully reported score " + score);
            }
            else
            {
                Debug.LogWarning("Failed to report score.");
            }
        });
    }

    public int CalculateScore(int starCount, int deathCount)
    {
        int score = starCount * 100000 - deathCount * 100;
        return score;
    }

    private int GetAlphabeticalIndex()
    {
        char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        char[] userName = Social.localUser.userName.ToUpper().ToCharArray();
        int sum = 0;

        for (int i = 0; i < userName.Length; i++)
        {
            sum += System.Array.IndexOf(alphabet, userName[i]) * (int)Mathf.Pow(26, userName.Length - i - 1);
        }

        return sum;
    }
}
