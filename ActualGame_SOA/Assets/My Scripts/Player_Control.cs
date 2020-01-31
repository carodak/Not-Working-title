using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
//https://www.youtube.com/watch?v=WFkbqdo2OI4
//https://answers.unity.com/questions/810806/spawn-random-enemy.html : Spawn objects
public class Player_Control : NetworkBehaviour
{
    public FirstPersonController fpsController;
    public Animator playerAnimator;
    public float spawnTime = 6f;
    public float spawnTime_coin = 2f;
    private Vector3 spawnPosition;
    public float speed = 1.0f;
    //public GameObject[] objects_tocreate;
    public GameObject enemyPrefab;
    private List<GameObject> objects_created = new List<GameObject>();
    private int objectsCount = 0;

    public GameObject coinPrefab;
    void Start(){
    
     if(isLocalPlayer && isServer){
      gameObject.tag = "Robot";
      InvokeRepeating ("CmdSpawn", spawnTime, spawnTime);
      InvokeRepeating("CmdSpawnCoin", spawnTime_coin, spawnTime_coin);
	 }
     else{
      gameObject.tag = "Human";
	 }

	}
    [Command]
    void CmdSpawnCoin(){
     //Create prefab
     spawnPosition.x = Random.Range (-17, 17);
     spawnPosition.y = 0.5f;
     spawnPosition.z = Random.Range (-17, 17);
     GameObject coin = (GameObject)(Instantiate(coinPrefab, spawnPosition, Quaternion.identity));
     Vector3 updatePos = new Vector3(0, 0.5f, 0);
     coin.transform.position = coin.transform.position + updatePos;
     coin.tag = "Coin";
     //Spawn the bullet 
     NetworkServer.Spawn(coin);
     Destroy(coin, 10);
	}
    [Command]
     void CmdSpawn ()
     {
         GameObject tempObject;
         spawnPosition.x = Random.Range (-17, 17);
         spawnPosition.y = 0.5f;
         spawnPosition.z = Random.Range (-17, 17);
         //tempObject = (GameObject)Instantiate(objects_tocreate[UnityEngine.Random.Range(0, objects_tocreate.Length - 1)], spawnPosition, Quaternion.identity);
         tempObject = (GameObject)Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
         tempObject.tag = "Enemy";
         Vector3 updatePos = new Vector3(0, -0.5f, 0);
         tempObject.transform.position = tempObject.transform.position + updatePos;
         objects_created.Add(tempObject);
        //Spawn the bullet 
         NetworkServer.Spawn(tempObject);
         Destroy(tempObject, 40);
         objectsCount = objectsCount + 1;
     }
    void Update(){
        speed = 1.0f;
        for(int i =0; i < objects_created.Count; i++){
          if(objects_created[i] != null)
          {
            float step =  speed * Time.deltaTime;
            GameObject Player = GameObject.FindWithTag("Robot");
            objects_created[i].transform.position = Vector3.MoveTowards(objects_created[i].transform.position, Player.transform.position, step);
            objects_created[i].transform.LookAt(Player.transform);
          }
          else
          {
            objects_created.Remove(objects_created[i]);
            objectsCount = objectsCount - 1;
		  }
		}
	}
    void OnTriggerEnter(Collider collision) {
        bool server = isLocalPlayer && isServer;
        if (server && ((tag == "Human" && collision.gameObject.tag == "Robot") || (tag == "Robot" && collision.gameObject.tag == "Human")))
        {
              playerAnimator.SetBool("IsNear", true);
              GameObject hit;
              if(tag == "Human"){
               hit = collision.gameObject;
               }
               else{
                hit = gameObject;     
			   }
              Health_Player health = hit.GetComponent<Health_Player>();
              if(health != null){
                health.AddHealth();     
			  }
        }
        if(!server && ((tag == "Human" && collision.gameObject.tag == "Robot") || (tag == "Robot" && collision.gameObject.tag == "Human")))
        {
            playerAnimator.SetBool("GivePower", true);
            GameObject hit;
            if(tag == "Human"){
                hit = collision.gameObject;
            }
            else{
                hit = gameObject;     
			}
            Health_Player health = hit.GetComponent<Health_Player>();
            if(health != null){
                health.AddHealth();     
			}
		}
        if(server && ((tag == "Enemy" && collision.gameObject.tag == "Robot") || (tag == "Robot" && collision.gameObject.tag == "Enemy")))
        {
              GameObject hit;
              if(tag == "Enemy"){
               hit = collision.gameObject;
               }
               else{
                hit = gameObject;     
			   }
              Health_Player health = hit.GetComponent<Health_Player>();
              if(health != null){
                print("Health component exist");
                health.TakeDamage(10);     
			  }
		}
        if(!server && ((tag == "Human" && collision.gameObject.tag == "Coin") || (tag == "Coin" && collision.gameObject.tag == "Human")))
        {
              print("Fuel");
              if(tag == "Haman"){
              Destroy(collision.gameObject);
              }
              else{
                Destroy(gameObject);     
			  }
		}
    }
    void OnTriggerExit(Collider collision) {
        bool server = isLocalPlayer && isServer;
        if (server && ((tag == "Human" && collision.gameObject.tag == "Robot") || (tag == "Robot" && collision.gameObject.tag == "Human")))
        {
                playerAnimator.SetBool("IsNear", false);
        }
        if(!server && ((tag == "Human" && collision.gameObject.tag == "Robot") || (tag == "Robot" && collision.gameObject.tag == "Human")))
        {
            playerAnimator.SetBool("GivePower", false);
		}
    }
}
