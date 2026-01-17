using UnityEngine;

public class FlashLightRotationSpeed : MonoBehaviour
{
    [SerializeField] float falshLightRotationSpeed = 5;
    GameObject player;
    PlayerManager playerManager;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerManager = player.GetComponent<PlayerManager>();
        playerManager.flashLight = this.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if(player == null) return;
        
        transform.position = playerManager.playerWeaponPos.transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation,playerManager.playerWeaponPos.transform.rotation,Time.deltaTime*falshLightRotationSpeed);
    }
}
