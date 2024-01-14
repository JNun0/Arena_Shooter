using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;
using Photon.Pun.Demo.Asteroids;

public class Weapon : MonoBehaviour
{
    public int damage;
    public Camera camera;
    public float fireRate;

    [Header("VFX")]
    public GameObject hitVFX;

    private float nextFire;

    [Header("Ammo")]
    public int mag = 5;
    public int ammo = 30;
    public int magAmmo = 30;

    [Header("UI")]
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;

    [Header("Sound")]
    public AudioSource audioGunshot;
    public AudioSource audioReload;

    [Header("Animation")]
    public Animation animation;
    public AnimationClip reload;

    [Header("Recoil Settings")]
    [Range(0, 1)]
    public float recoilPercent = 0.3f;
    [Range(0, 2)]
    public float recoverPercent = 0.7f;

    [Space]
    public float recoilUp = 1f;
    public float recoilBack = 0f;


    private Vector3 originalPosition;
    private Vector3 recoilVelocity = Vector3.zero;

    private float recoilLength;
    private float recoverLength;

    private bool recoiling;
    private bool recovering;

    void Start()
    {
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;

        originalPosition = transform.localPosition;

        recoilLength = 0;
        recoverLength = 1 / fireRate * recoverPercent;
    }

    void Update()
    {
        //Atualiza o tempo até o próximo disparo
        if (nextFire > 0)
        {
            nextFire -= Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && nextFire <= 0 && ammo > 0 && animation.isPlaying == false)
        {
            //Configura o tempo até o próximo disparo
            nextFire = 1 / fireRate;
            
            ammo--;

            magText.text = mag.ToString();
            ammoText.text = ammo + "/" + magAmmo;

            Fire();
            audioGunshot.time = 0.7f;
            audioGunshot.Play();
        }

        if (Input.GetKeyDown(KeyCode.R) && animation.isPlaying == false && mag > 0 )
        {
            Reload();
            audioReload.Play();
        }

        if (recoiling)
        {
            Recoil();
        }

        if (recovering)
        {
            Recovering();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void Reload()
    {

        animation.Play(reload.name);

        if (mag > 0)
        {
            mag--;

            ammo = magAmmo;
        }

        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
    }

    void Fire()
    {
        recoiling = true;
        recovering = false;

        //Cria um raio na direção da câmera
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        RaycastHit hit;

        //Verifica se o raio atinge algum objeto
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f))
        {
            //Mostra o efeito visual no ponto de impacto
            PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);

            //Verifica se o objeto atingido possui vida
            if (hit.transform.gameObject.GetComponent<Health>())
            {
                //Verifica se o dano é suficiente para matar
                if (damage >= hit.transform.gameObject.GetComponent<Health>().health)
                {
                    //kill
                    RoomManager.instance.kills++;
                    RoomManager.instance.SetHashes();

                    PhotonNetwork.LocalPlayer.AddScore(25);
                }
                //Aplica dano a todos os jogadores em rede
                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            }
        }
    }

    void Recoil()
    {
        //Calcula a posição final de recuo
        Vector3 finalPosition = new Vector3(originalPosition.x, originalPosition.y + recoilUp, originalPosition.z - recoilBack);

        //Aplica suavização na transição entre posições
        transform.localPosition = 
            Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoilLength);

        //Se atingir a posição final, desativa o recuo e ativa a recuperação
        if (transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = true;
        }
    }

    void Recovering()
    {
        //Calcula a posição final de recuperação
        Vector3 finalPosition = originalPosition;

        //Aplica suavização na transição entre posições
        transform.localPosition = 
            Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoverLength);

        //Se atingir a posição final, desativa a recuperação
        if (transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = false;
        }
    }
}
