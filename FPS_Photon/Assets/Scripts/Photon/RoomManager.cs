using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;

    public GameObject player;

    [Space]
    public Transform[] spawnPoints;

    [Space]
    public GameObject roomCam;

    [Space]
    public GameObject nameUI;
    public GameObject connectingUI;

    private string nickname = "Unnamed";

    [HideInInspector]
    public int kills = 0;
    [HideInInspector]
    public int deaths = 0;

    public string roomNameToJoin = "test";

    void Awake()
    {
        instance = this;
    }

    public void ChangeNickname(string _name)
    {
        nickname = _name;
    }

    public void JoinRoomButtonPress()
    {
        Debug.Log("Connecting...");

        //Junta-se ou cria uma sala com o nome escolhido
        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, null, null);

        nameUI.SetActive(false);
        connectingUI.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("Connected to Room");

        //Desativa a câmera que mostra o lobby
        roomCam.SetActive(false);

        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        //Ponto se spawn aleatório
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0,spawnPoints.Length)];
        
        //Invoca o jogador na posição de spawn
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);

        //Configura o jogador
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        _player.GetComponent<Health>().isLocalPlayer = true;

        //RPC para definir o apelido do jogador em rede
        _player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);

        //Define o apelido do jogador
        PhotonNetwork.LocalPlayer.NickName = nickname;
    }

    //Atualiza as estatísticas do jogador local
    public void SetHashes()
    {
        try
        {
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

            hash["kills"] = kills;
            hash["deaths"] = deaths;

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch
        {

        }
    }
}
