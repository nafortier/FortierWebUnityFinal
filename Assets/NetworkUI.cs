using System;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;

using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using Unity.Networking.Transport.Relay;


public class NetworkUI : MonoBehaviour
{

    //Original Connection for NetCode for GameObjects
    //public void StartHost()
    //{
    //    NetworkManager.Singleton.StartHost();
    //    this.gameObject.SetActive(false);
    //}

    //public void StartClient()
    //{
    //    NetworkManager.Singleton.StartClient();
    //    this.gameObject.SetActive(false);
    //}

    [Header("Network References")]
    [SerializeField]
    NetworkManager networkManager;
    [SerializeField]
    UnityTransport unityTranport;

    [Header("User Interface Items")]
    [SerializeField]
    TMP_InputField joinCodeInput;
    [SerializeField]
    TMP_Text statusText;
    [SerializeField]
    TMP_Text hostJoinCodeText;

    [Header("Unity Relay Settings")]
    [SerializeField]
    int maxClients = 8;
    [SerializeField]
    string connectionType = "dtls";

    private void Awake()
    {
        if (networkManager == null) networkManager = NetworkManager.Singleton;
        if(unityTranport == null && networkManager != null)
        {
            unityTranport = networkManager.GetComponent<UnityTransport>();
        } 
    }

    public async void StartHost()
    {
        try
        {
            SetStatus("Initializing Services...");
            await UGSBootStrap.EnsureReady();

            SetStatus("Creating Relay...");
            Allocation alloc = await RelayService.Instance.CreateAllocationAsync(maxClients);

            SetStatus("Creating Join Code...");
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

            //Configure the transport for the relay service
            RelayServerData rsd = AllocationUtils.ToRelayServerData(alloc, connectionType);
            unityTranport.SetRelayServerData(rsd);

            SetStatus("Starting Host");
            networkManager.StartHost();

            if (hostJoinCodeText != null)
            {
                hostJoinCodeText.text = $"Join Code: {joinCode}";
            }

            gameObject.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.LogError($"Starthost Failed: {e}");
            SetStatus("Host Failed");
        }
    }

    public async void StartClient()
    {
        try 
        {
            string code = joinCodeInput != null ? joinCodeInput.text.Trim() : "";
            if (string.IsNullOrEmpty(code))
            {
                SetStatus("Enter a Join Code");
                return;
            }

            SetStatus("Initializing Services...");
            await UGSBootStrap.EnsureReady();

            SetStatus("Joining Relay Allocation...");
            JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(code);

            //Set Transport
            RelayServerData rsd = AllocationUtils.ToRelayServerData(joinAlloc, connectionType);
            unityTranport.SetRelayServerData(rsd);

            SetStatus("Starting Client...");
            networkManager.StartClient();

            gameObject.SetActive(false);
        } 
        catch (Exception e)
        {
            Debug.LogError($"Startclient Failed: {e}");
            SetStatus("Client Failed");
        }
    }

    public void Leave()
    {
        if(NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

        SetStatus("Disconnected");
        gameObject.SetActive(true);
        if (hostJoinCodeText != null)
        {
            hostJoinCodeText.text = "Join Code: ";
        }
    }

    void SetStatus(string msg)
    {
        if(statusText != null)
        {
            statusText.text = msg;
            Debug.Log(msg);
        }
    }

}
