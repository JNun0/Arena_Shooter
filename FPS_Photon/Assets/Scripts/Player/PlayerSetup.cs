using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public Movement movement;

    public GameObject camera;

    public string nickname;

    public TextMeshPro nicknameText;

    public Transform TPweaponHolder;

    //Configuração do jogador local
    public void IsLocalPlayer()
    {
        TPweaponHolder.gameObject.SetActive(false);

        movement.enabled = true;
        camera.SetActive(true);
    }

    //Configuração a arma em terceira pessoa
    [PunRPC]
    public void SetTPweapon(int weaponIndex)
    {
        foreach (Transform _weapon in TPweaponHolder)
        {
            _weapon.gameObject.SetActive(false);
        }

        TPweaponHolder.GetChild(weaponIndex).gameObject.SetActive(true);
    }

    //Configuração do nome do jogador
    [PunRPC]
    public void SetNickname(string _name)
    {
        nickname = _name;
        nicknameText.text = nickname;
    }
}
