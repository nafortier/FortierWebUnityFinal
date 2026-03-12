using UnityEngine;
using Unity.Netcode;
using TMPro;

public class TestStatus : MonoBehaviour
{
    public TMP_Text statusText;

    // Update is called once per frame
    void Update()
    {
        if (!NetworkManager.Singleton.IsListening)
        {
            statusText.text = "Not Connected";
        }
        else if (NetworkManager.Singleton.IsHost)
        {
            statusText.text = "Host Running";
        }
        else if (NetworkManager.Singleton.IsClient) {
            statusText.text = "Client Connected";
        }
    }
}
