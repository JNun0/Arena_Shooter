using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;

public class LeaderBoard : MonoBehaviour
{
    public GameObject playersHolder;

    [Header("Options")]
    public float refreshRate = 1f;

    [Header("UI")]
    public GameObject[] slots;

    [Space]
    public TextMeshProUGUI[] scoreTexts;
    public TextMeshProUGUI[] nameTexts;
    public TextMeshProUGUI[] kdTexts;

    private void Start()
    {
        //Repete o Refresh com um intervalo definido
        InvokeRepeating(nameof(Refresh), 1f, refreshRate);
    }

    //Atualiza o quadro de líderes
    public void Refresh()
    {
        //Desativa todos os slots para poder atualizá-los depois
        foreach (var slot in slots)
        {
            slot.SetActive(false);
        }

        //Lista ordenada de jogadores pelo score decrescente
        var sortedPlayerList = (from player in PhotonNetwork.PlayerList orderby player.GetScore() descending select player).ToList();

        int i = 0;

        //Preenche os slots com informações dos jogadores
        foreach (var player in sortedPlayerList)
        {
            slots[i].SetActive(true);

            //Se não tiver nome
            if (player.NickName == "")
            {
                player.NickName = "Unnamed";
            }

            nameTexts[i].text = player.NickName;
            scoreTexts[i].text = player.GetScore().ToString();

            if (player.CustomProperties["kills"] != null)
            {
                kdTexts[i].text = player.CustomProperties["kills"] + "/" + player.CustomProperties["deaths"];
            }
            else
            {
                kdTexts[i].text = "0/0";
            }

            i++;
        }
    }

    private void Update()
    {
        playersHolder.SetActive(Input.GetKey(KeyCode.Tab));
    }
}
