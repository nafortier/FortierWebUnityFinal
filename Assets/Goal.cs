using Unity.Netcode;
using UnityEngine;
using System.Collections;
using TMPro;

public class Goal : NetworkBehaviour
{
    public APIClient api;
    private bool gameFinished = false;

    public GameObject Leaderboard;
    public TMP_Text txt;
    private void OnTriggerEnter(Collider other)
    {
       
        if (!IsServer || gameFinished) return;

        if (other.TryGetComponent<PlayerController>(out var player))
        {
            gameFinished = true;
            string winnerId = player.myCharacterId;

            StartCoroutine(UpdateWinCountSequence(winnerId));
        }
    }

    private IEnumerator UpdateWinCountSequence(string winnerId)
    {
      

        HighScoreDto currentData = null;
        bool fetchSuccess = false;

   
        yield return api.Get($"/api/highscores/{winnerId}",
            onSuccess: (json) => {
                currentData = JsonUtility.FromJson<HighScoreDto>(json);
                fetchSuccess = true;
            },
            onError: (err) => {
        
                gameFinished = false; 
            },
            auth: true
        );

        if (fetchSuccess && currentData != null)
        {
          
            currentData.wins += 1;

  
            yield return api.PutJson($"/api/highscores/{winnerId}", currentData,
                onSuccess: (json) => {
                    Debug.Log("Database update successful");
                    NotifyWinClientRpc(currentData.screenname);
                },
                onError: (err) => {
                    Debug.LogError("Failed to update winner data: " + err);
                    gameFinished = false;
                },
                auth: true
            );
        }
    }

    [ClientRpc]
    void NotifyWinClientRpc(string winnerName)
    {
      
        txt.text = $"The winner is {winnerName}";
        Leaderboard.SetActive(true);
        FindFirstObjectByType<LeaderboardManager>().RefreshLeaderboard();

    }
}