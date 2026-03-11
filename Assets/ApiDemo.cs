using UnityEngine;

public class ApiDemo : MonoBehaviour
{
    [SerializeField]
    APIClient api;

    [Header("Testing Credentials")]
    [SerializeField]
    string username = "Jordan";
    [SerializeField]
    string password = "1234";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Basic test to see if our api is working
        //StartCoroutine(api.Get("/api/data", Debug.Log, Debug.LogError));

        //Test for login
        StartCoroutine(LoginThenHighScores());
    }

    System.Collections.IEnumerator LoginThenHighScores()
    {
        //Logs in new User
        yield return api.PostJson("/api/auth/login", new LoginRequest { username = username, password = password }, onSuccess: (json) =>
        {
            Debug.Log("Raw Login Info: " + json);
            var res = JsonUtility.FromJson<LoginResponse>(json);

            if (res != null && res.ok && !string.IsNullOrEmpty(json))
            {
                api.SetToken(res.token);
                Debug.Log("JWT added authorized");
            }
            else
            {
                Debug.LogError("Login Failed: " + (res?.error ?? "unknown"));
            }
        },
        onError: Debug.LogError, auth: false
        );

        //Gets High Scores on a protected route;
        yield return api.Get("/api/highscores", onSuccess: (json) =>
        {
            var scores = JsonArrayWrapper.FromJsonArray<HighScoreDto>(json);
            Debug.Log($"Scores Count:{scores.Length}");
            if (scores.Length > 0)
            {
               // Debug.Log($"Top Score: {scores[0].playername} {scores[0].score} Level: {scores[0].level}");
            }
        },
         onError: Debug.LogError, auth: true);
    }
}
