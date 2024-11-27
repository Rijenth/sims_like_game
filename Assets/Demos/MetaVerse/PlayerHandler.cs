using System.Collections.Generic;
using Demos.MetaVerse;
using Unity.Cinemachine;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string Identifier;
    public Vector3 Position;
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
        GameObject avatar = (isMainCharacter)
            ? Instantiate(MainAvatarPrefab)
            : Instantiate(AvatarPrefab);
        
        if (isMainCharacter)
        {
            avatar.AddComponent<CharacterController>();

            cinemachineCamera.Follow = avatar.transform;
            cinemachineCamera.LookAt = avatar.transform;
        }

        avatar.transform.position = new Vector3(250, 0, 250);
        avatar.name = data.Username;

        Player playerComponent = avatar.AddComponent<Player>();
        playerComponent.Identifier = data.Identifier;
        playerComponent.Avatar = avatar;
        playerComponent.Username = data.Username;

        Players.Add(avatar);

        return avatar;
    }
}
