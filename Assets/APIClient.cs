using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Text;


[System.Serializable]
public class HighScoreDto {
    public string _id;
    public string screenname;
    public string firstname;
    public string lastname;
    public string date;
    public int score;

}

[System.Serializable]
public class HighScoreList {
    public HighScoreDto[] scores;
}


public class APIClient : MonoBehaviour
{
    [SerializeField] string baseURL = "http://localhost:3000";

    public string Token { get; private set; }

    private const string TokenKey = "API_JWT_Token";

    private void Awake()
    {
        if (PlayerPrefs.HasKey(TokenKey))
        {
            Token = PlayerPrefs.GetString(TokenKey);
        }
    }

    public void SetToken(string token)
    {
        Token = token;
        PlayerPrefs.SetString(TokenKey, token);
        PlayerPrefs.Save();
    }

    public void ClearToken()
    {
        Token = null;
        PlayerPrefs.DeleteKey(TokenKey);
    }

    public IEnumerator Get(string path, Action<string> onSuccess, Action<string> onError, bool auth = false)
    {
        Debug.Log($"{baseURL}{path}");
        using var req = UnityWebRequest.Get($"{baseURL}{path}");
        ApplyHeaders(req, auth);
        yield return req.SendWebRequest();
        Handle(req, onSuccess, onError);

    }

    public IEnumerator PostJson<T>(string path,T body ,Action<string> onSuccess, Action<string> onError, bool auth = false)
    {
        
        var json = JsonUtility.ToJson(body); 

        using var req = new UnityWebRequest($"{baseURL}{path}", "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        ApplyHeaders(req, auth);
        yield return req.SendWebRequest();
        Handle(req, onSuccess, onError);
    }

    public IEnumerator PutJson<T>(string path, T body, Action<string> onSuccess, Action<string> onError, bool auth = false)
    {
        var json = JsonUtility.ToJson(body);

       
        using var req = new UnityWebRequest($"{baseURL}{path}", "PUT");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        ApplyHeaders(req, auth);
        yield return req.SendWebRequest();
        Handle(req, onSuccess, onError);
    }

  
    public IEnumerator DeleteRequest(string path, Action<string> onSuccess, Action<string> onError, bool auth = false)
    {
        
        using var req = UnityWebRequest.Delete($"{baseURL}{path}");
        req.downloadHandler = new DownloadHandlerBuffer();

        ApplyHeaders(req, auth);
        yield return req.SendWebRequest();
        Handle(req, onSuccess, onError);
    }


    //Create headers for Authorization
    void ApplyHeaders(UnityWebRequest req, bool auth)
    {
        req.timeout = 15;

        if (auth)
        {
            if (string.IsNullOrEmpty(Token))
            {
                Debug.Log("Authorization Request Failed");
            }
            else
            {
                req.SetRequestHeader("Authorization", $"Bearer {Token}");
            }
        }
    }

    void Handle(UnityWebRequest req, Action<string> onSuccess, Action<string> onError)
    {
        var body = req.downloadHandler != null ? req.downloadHandler.text : "";

        if (req.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(body);
        }
        else
        {
            var msg = $"HTTP {(long)req.responseCode} : {req.error}\n{req.downloadHandler.text}";
            onError?.Invoke(msg);
        }
    }


    //Old base example of fetch request in unity
    
    //public void FetchScores() => StartCoroutine(GetScores());

    //IEnumerator GetScores()
    //{
    //    using var req = UnityWebRequest.Get($"{baseURL}/api/highscores");

    //    yield return req.SendWebRequest();

    //    if (req.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log(req.error);
    //        yield break;
    //    }

    //    Debug.Log(req.downloadHandler.text);

    //    string rawJson = req.downloadHandler.text;
    //    string wrappedJson = "{\"scores\":" + rawJson + " }";

    //    HighScoreList list = JsonUtility.FromJson<HighScoreList>(wrappedJson);

    //    foreach(var score in list.scores)
    //    {
    //        Debug.Log($"{score.playername} - {score.score} - {score.level}");
    //    }
    //}
}
