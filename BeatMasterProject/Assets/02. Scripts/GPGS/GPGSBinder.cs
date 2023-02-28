using UnityEngine;
using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGSBinder
{
    private static GPGSBinder instance = new GPGSBinder();
    public static GPGSBinder Instance => instance;

    public PlayGamesClientConfiguration config;
    private string _authCode;

    private void Init()
    {
        config = new PlayGamesClientConfiguration.Builder()
            .RequestEmail() // Get Email address to save
            .AddOauthScope("email") // You need to request the Email before asking for it
            .RequestServerAuthCode(false /* Don't force refresh */)
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }
    
    public void Login(Action<bool, UnityEngine.SocialPlatforms.ILocalUser> onLoginSuccess = null)
    {
        Init();
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (success) =>
        {
            // bool isAuthenticated = PlayGamesPlatform.Instance.localUser.authenticated;
            // // _authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
            // onLoginSuccess?.Invoke(isAuthenticated, Social.localUser);
            Social.localUser.Authenticate((bool isAuthenticated) => {
                if (isAuthenticated)
                {
                    _authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                    FirebaseDataManager.Instance.GetCredential(_authCode);
                }
                onLoginSuccess?.Invoke(isAuthenticated, Social.localUser);
            });
        });
    }

    public void Logout()
    {
        PlayGamesPlatform.Instance.SignOut();
    }

    public bool IsLoggedIn()
    {
        return PlayGamesPlatform.Instance.localUser.authenticated;
    }

    public void ShowAchievementUI() =>
        Social.ShowAchievementsUI();

    public void UnlockAchievement(string gpgsId, Action<bool> onUnlocked = null) =>
        Social.ReportProgress(gpgsId, 100, success => onUnlocked?.Invoke(success));

    public void ShowAllLeaderboardUI() =>
        Social.ShowLeaderboardUI();

    public void ShowTargetLeaderboardUI(string gpgsId) =>
        ((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(gpgsId);

    public void ReportLeaderboard(string gpgsId, long score, Action<bool> onReported = null) =>
        Social.ReportScore(score, gpgsId, success => onReported?.Invoke(success));

    public void LoadAllLeaderboardArray(string gpgsId, Action<UnityEngine.SocialPlatforms.IScore[]> onloaded = null) =>
        Social.LoadScores(gpgsId, onloaded);

    public void LoadCustomLeaderboardArray(string gpgsId, int rowCount, LeaderboardStart leaderboardStart,
        LeaderboardTimeSpan leaderboardTimeSpan, Action<bool, LeaderboardScoreData> onloaded = null)
    {
        PlayGamesPlatform.Instance.LoadScores(gpgsId, leaderboardStart, rowCount, LeaderboardCollection.Public, leaderboardTimeSpan, data =>
        {
            onloaded?.Invoke(data.Status == ResponseStatus.Success, data);
        });
    }

    public string CheckStageIdx(int stageIdx)
    {
        string leaderboardId = "";

        switch (stageIdx)
        {
            case 0:
                leaderboardId = GPGSIds.leaderboard_stage1_beatmasters;
                break;
            case 1:
                leaderboardId = GPGSIds.leaderboard_stage2_beatmasters;
                break;
            case 2:
                leaderboardId = GPGSIds.leaderboard_stage3_beatmasters;
                break;
            case 3:
                leaderboardId = GPGSIds.leaderboard_stage4_beatmasters;
                break;
            default:
                Debug.LogError("Invalid stage index.");
                break;
        }
        return leaderboardId;
    }
}
