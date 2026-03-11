using UnityEngine;
using Unity.Netcode;
using TMPro;
public class TestStatus : MonoBehaviour
{
    public TMP_Text statustxt;
 
    // Update is called once per frame
    void Update()
    {
        if (!NetworkManager.Singleton.IsListening)
        {
            statustxt.text = "Not Connected";
        } else if(NetworkManager.Singleton.IsHost)
        {
            statustxt.text = "Host Running";
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            statustxt.text = "Client Connected";
        }
    }
}
