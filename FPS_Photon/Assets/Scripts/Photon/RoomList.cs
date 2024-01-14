using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomList : MonoBehaviourPunCallbacks
{
    public static RoomList Instance;

    public GameObject roomManagerGameObject;
    public RoomManager roomManager;


    [Header("UI")]
    public Transform roomListParent;
    public GameObject roomListItemPrefab;

    private List<RoomInfo> cacheRoomList = new List<RoomInfo>();


    //Alterar a sala a ser criada com um nome específico
    public void ChangeRoomToCreateName(string _roomName)
    {
        roomManager.roomNameToJoin = _roomName;

    }

    private void Awake()
    {
        Instance = this;
    }

    //Iniciar a conexão com o Photon
    IEnumerator Start()
    {
        // Precautions
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        //Espera até que não esteja conectado antes de se reconectar
        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

        //Conecta-se usando as configurações
        PhotonNetwork.ConnectUsingSettings();
    }

    //Chamado quando conectado ao servidor 
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        //Liga-se ao lobby depois de se conectar ao servidor
        PhotonNetwork.JoinLobby();
    }

    //Chamado quando a lista de salas é atualizada
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //Vê se há salas na lista de cache
        if (cacheRoomList.Count <= 0)
        {
            cacheRoomList = roomList;
        }
        else
        {
            //Atualiza a lista com as novas salas
            foreach (var room in roomList)
            {
                for (int i = 0; i < cacheRoomList.Count; i++)
                {
                    if (cacheRoomList[i].Name == room.Name)
                    {
                        List<RoomInfo> newList = cacheRoomList;

                        if (room.RemovedFromList)
                        {
                            //Remove a sala da lista
                            roomList.Remove(newList[i]);
                        }
                        else
                        {
                            //Atualiza a sala se ela foi modificada
                            newList[i] = room;
                        }

                        cacheRoomList = newList;
                    }
                }
            }
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        foreach (Transform roomItem in roomListParent)
        {
            Destroy(roomItem.gameObject);
        }

        foreach (var room in cacheRoomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);

            roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/16";

            roomItem.GetComponent<RoomItemButton>().RoomName = room.Name;
        }
    }

    //Entrar numa sala pelo nome
    public void JoinRoomByName (string _name)
    {
        roomManager.roomNameToJoin = _name;
        roomManagerGameObject.SetActive(true);
        gameObject.SetActive(false);
    }

}
