using System.Collections.Generic;
using Demos.MetaVerse;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string Identifier;
    public Vector3 Position;
    public Quaternion Rotation;
    public string Username;
}

public class PlayerHandler : MonoBehaviour
{
    public UDPService UDP;

    public CinemachineCamera cinemachineCamera;
    public GameObject MainAvatarPrefab;
    public GameObject AvatarPrefab;


    public float NextTimeout = -1;
    public static List<GameObject> Players = new List<GameObject>();

    public GameObject AddPlayerAvatar(PlayerData data, bool isMainCharacter)
    {
        GameObject avatar = InstantiateAvatar(isMainCharacter, data);
        ConfigureAvatar(avatar, data);
        ConfigureUsernameContainer(avatar, data);

        Players.Add(avatar);
        return avatar;
    }

    private GameObject InstantiateAvatar(bool isMainCharacter, PlayerData data)
    {
        GameObject avatar = isMainCharacter
            ? Instantiate(MainAvatarPrefab)
            : Instantiate(AvatarPrefab);

        if (!isMainCharacter)
        {
            avatar.AddComponent<UDPCharacterController>();
            return avatar;
        }

        avatar.AddComponent<CharacterController>();
        cinemachineCamera.Follow = avatar.transform;
        cinemachineCamera.LookAt = avatar.transform;

        return avatar;
    }

    private void ConfigureAvatar(GameObject avatar, PlayerData data)
    {
        avatar.transform.position = new Vector3(250, 0, 250);

        if (data.Position != Vector3.zero)
        {
            avatar.transform.position = data.Position;
        }

        avatar.name = data.Username;

        Player playerComponent = avatar.AddComponent<Player>();
        playerComponent.Identifier = data.Identifier;
        playerComponent.Avatar = avatar;
        playerComponent.Username = data.Username;
    }

    private void ConfigureUsernameContainer(GameObject avatar, PlayerData data)
    {
        GameObject container = ConfigureAvatarTextContainer(
            avatar,
            data.Username,
            new Vector3(0, 3, 0)
        );

        container.AddComponent<FaceCamera>();
    }

    // potentiel mauvaise pratique
    // on peux setup un objet directement sur l'avatar et update son etat
    private GameObject ConfigureAvatarTextContainer(GameObject avatar, string text, Vector3 position)
    {
        GameObject container = new GameObject(text);
        container.transform.SetParent(avatar.transform);
        container.transform.localPosition = position;

        GameObject textObject = new GameObject(text + "Text");
        textObject.transform.SetParent(container.transform);
        textObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        TMP_Text textMesh = textObject.AddComponent<TextMeshPro>();
        textMesh.text = text;
        textMesh.fontSize = 36;
        textMesh.color = Color.white;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.transform.localPosition = Vector3.zero;

        return container;
    }

    public void SyncPlayerPosition(PlayerData data)
    {
        var username = data.Username;
        var position = data.Position;
        var rotation = data.Rotation;
        var existingPlayer = Players.Find(player => player.GetComponent<Player>().Username == username);

        if (!existingPlayer)
        {
            bool isMainCharacter = username == State.Username;

            var newPlayer = AddPlayerAvatar(data, isMainCharacter);
            Players.Add(newPlayer);
        }
        else if (username != State.Username)
        {

            if (rotation != existingPlayer.transform.rotation)
            {
                existingPlayer.GetComponent<UDPCharacterController>().SetRotation(rotation);
            }

            if (position != existingPlayer.transform.position)
            {
                existingPlayer.GetComponent<UDPCharacterController>().SetMovement(position);
            }           
        }
    }

    public string GeneratePlayerUDPData()
    {
        var avatar = Players.Find(player => player.GetComponent<Player>().Username == State.Username);

        var username = State.Username;
        var position = new Vector3(250, 0, 250);
        var rotation = new Quaternion();

        if (avatar)
        {
            var player = avatar.GetComponent<Player>();

            if (player)
            {
                username = player.Username;
                position = avatar.transform.position;
                rotation = avatar.transform.rotation;
            }
        }

        PlayerData playerData = new PlayerData
        {
            Username = username,
            Position = position,
            Rotation = rotation,
        };

        return JsonUtility.ToJson(playerData);
    }
}