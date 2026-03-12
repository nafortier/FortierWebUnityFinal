using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed;
    public NetworkVariable<Color> playerColor = new NetworkVariable<Color>( default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    CharacterController characterController;
    PlayerInput playerInput;
    InputAction moveAction;
    Renderer rendMat;





    public string myCharacterId;

    [ServerRpc]
    public void RegisterCharacterIdServerRpc(string characterId)
    {
        myCharacterId = characterId;
    }

    

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        rendMat = GetComponent<Renderer>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            string selectedId = FindFirstObjectByType<ApiUIController>().playingId;
            RegisterCharacterIdServerRpc(selectedId);
            moveAction = playerInput.actions["Move"];
            moveAction.Enable();
        }
        else
        {
            playerInput.enabled = false;
        }

        
        playerColor.OnValueChanged += OnColorChange;
        OnColorChange(Color.white, playerColor.Value);

        if (IsServer)
        {
            var colorValue = new Color(Random.value, Random.value, Random.value, 1f);
            playerColor.Value = colorValue;
        }
    }


    void Update()
    {
        if (!IsOwner) { return; }
        
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);

        characterController.Move(move * moveSpeed * Time.deltaTime);

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            RequestNewColorServerRpc();
        }
    }

    void OnColorChange(Color oldColor, Color newColor)
    {
        rendMat.material.color = newColor;
    }

    [ServerRpc]
    void RequestNewColorServerRpc()
    {
        playerColor.Value = new Color(Random.value, Random.value, Random.value, 1f);
    }
}
