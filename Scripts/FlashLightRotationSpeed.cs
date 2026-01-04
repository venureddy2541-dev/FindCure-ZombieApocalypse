using UnityEngine;

public class FlashLightRotationSpeed : MonoBehaviour
{
    [SerializeField] float falshLightRotationSpeed = 5;
    GameObject player;
    PlayerFiring playerFiring;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerFiring = player.GetComponent<PlayerFiring>();
        playerFiring.flashLight = this.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if(player == null) return;
        
        transform.position = playerFiring.playerWeaponPos.transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation,playerFiring.playerWeaponPos.transform.rotation,Time.deltaTime*falshLightRotationSpeed);
    }
}
