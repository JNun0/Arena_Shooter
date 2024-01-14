using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MpChat : MonoBehaviourPun
{
    // Variáveis para controle do chat
    bool isChatting = false;
    string chatInput = "";
    bool shouldFocus = false;

    // Estrutura de dados para armazenar mensagens do chat
    [System.Serializable]
    public class ChatMessage
    {
        public string sender = "";
        public string message = "";
        public float timer = 0;
        public float timestamp; // Adiciona um carimbo de tempo a cada mensagem
    }

    // Lista de mensagens do chat
    List<ChatMessage> chatMessages = new List<ChatMessage>();

    void Start()
    {
        // Obtem o PhotonView associado a este GameObject
        PhotonView photonView = gameObject.GetComponent<PhotonView>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T) && !isChatting)
        {
            isChatting = true;
            chatInput = "";
            shouldFocus = true;

            if (photonView.IsMine)
            {
                GUI.FocusControl("ChatField");
            }
        }
    }

    void OnGUI()
    {
        if (!isChatting)
        {
            GUI.Label(new Rect(5, Screen.height - 75, 200, 25), "Press 'T' to chat");
        }
        else
        {
            //Interface
            GUI.SetNextControlName("ChatField");
            GUI.Label(new Rect(5, Screen.height - 75, 200, 25), "Say:");
            GUIStyle inputStyle = GUI.skin.GetStyle("box");
            inputStyle.alignment = TextAnchor.MiddleLeft;

            chatInput = GUI.TextField(new Rect(10 + 25, Screen.height - 75, 400, 22), chatInput, 60, inputStyle);

            //Envia a mensagem para o chat ao pressionar Enter
            if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
            {
                isChatting = false;
                if (chatInput.Replace(" ", "") != "")
                {
                    photonView.RPC("SendChat", RpcTarget.All, PhotonNetwork.LocalPlayer, chatInput);
                }
                chatInput = "";
            }

            //Mete o foco no chat
            if (shouldFocus)
            {
                GUI.FocusControl("ChatField");
                shouldFocus = false;
            }
        }

        //Mensagens do chat ordenadas pelo timestamp
        chatMessages.Sort((a, b) => a.timestamp.CompareTo(b.timestamp));

        float messageHeight = 25f;
        float messageSpacing = 5f;

        //Mostra a mensagem 
        for (int i = 0; i < chatMessages.Count; i++)
        {
            float yPos = Screen.height - 100 - (messageHeight + messageSpacing) * (chatMessages.Count - 1 - i);

            GUI.Label(new Rect(5, yPos, 500, messageHeight), chatMessages[i].sender + ": " + chatMessages[i].message);
        }
    }

    //Enviar mensagens do chat para todos os jogadores
    [PunRPC]
    void SendChat(Player sender, string message)
    {
        ChatMessage m = new ChatMessage();
        m.sender = sender.NickName;
        m.message = message;
        m.timestamp = Time.time;


        //Nova mensagem no início da lista e remove a mais antiga
        chatMessages.Add(m);
        if (chatMessages.Count > 8)
        {
            chatMessages.RemoveAt(0);
        }
    }
}
