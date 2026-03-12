using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class Goal : NetworkBehaviour
{
    public APIClient api;
    private bool gameFinished = false;

  
    private void OnTriggerEnter(Collider other)
    {
        // Only the Server/Host manages the API sequence
        if (!IsServer || gameFinished) return;

        if (other.TryGetComponent<PlayerController>(out var player))
        {
            gameFinished = true;
            string winnerId = player.myCharacterId;

            // Start the Get -> Modify -> Put sequence
            StartCoroutine(UpdateWinCountSequence(winnerId));
        }
    }

    private IEnumerator UpdateWinCountSequence(string winnerId)
    {
        Debug.Log($"Fetching current data for: {winnerId}");

        HighScoreDto currentData = null;
        bool fetchSuccess = false;

        // 1. Fetch current data for this specific ID
        yield return api.Get($"/api/highscores/{winnerId}",
            onSuccess: (json) => {
                currentData = JsonUtility.FromJson<HighScoreDto>(json);
                fetchSuccess = true;
            },
            onError: (err) => {
                Debug.LogError("Failed to fetch winner data: " + err);
                gameFinished = false; // Allow another attempt
            },
            auth: true
        );

        if (fetchSuccess && currentData != null)
        {
            // 2. Increment the win counter locally
            currentData.wins += 1;

            Debug.Log($"Updating wins for {currentData.screenname} to {currentData.wins}");

            // 3. PUT the updated data back to the server
            yield return api.PutJson($"/api/highscores/{winnerId}", currentData,
                onSuccess: (json) => {
                    Debug.Log("Database update successful!");
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
        Debug.Log($"THE WINNER IS: {winnerName}");

        // Refresh leaderboards on all clients
        //var ui = FindFirstObjectByType<APIClient>(); // Or your specific UI Manager
        // ui.RefreshLeaderboard(); 
    }
}