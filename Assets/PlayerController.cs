using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed;
    public float jumpHeight = 2.0f;
    public float gravityValue = -9.81f;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    public NetworkVariable<Color> playerColor = new NetworkVariable<Color>( default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    CharacterController characterController;
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction jumpAction;
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
            jumpAction = playerInput.actions["Jump"];
            jumpAction.Enable();
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

        groundedPlayer = characterController.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; 
        }

        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 move = new Vector3(input.x, 0, input.y);


        if (jumpAction.triggered && groundedPlayer)
        {
           
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }


        playerVelocity.y += gravityValue * Time.deltaTime;
        Vector3 finalVelocity = (move * moveSpeed) + playerVelocity;
        characterController.Move(finalVelocity * Time.deltaTime);

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
