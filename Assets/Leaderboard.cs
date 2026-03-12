using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class LeaderboardManager : MonoBehaviour
{

    public TMP_Text leaderboardText; 
    public APIClient api;


    public void RefreshLeaderboard()
    {
 

        StartCoroutine(api.Get("/api/highscores",
            onSuccess: (json) => {
                var scores = JsonArrayWrapper.FromJsonArray<HighScoreDto>(json);
                DisplayTopTen(scores);
            },
            onError: (err) => {
          
                Debug.LogError(err);
            },
            auth: true
        ));
    }

    void DisplayTopTen(HighScoreDto[] data)
    {
        var topTen = data
            .OrderByDescending(x => x.wins)
            .Take(10)
            .ToList();

       
        string content = "Top Wins:\n";

        for (int i = 0; i < topTen.Count; i++)
        {
            content += $"{i + 1}. {topTen[i].screenname} - {topTen[i].wins} Wins\n";
        }

       
        leaderboardText.text = content;

      
    }

    public void Reset()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }



}