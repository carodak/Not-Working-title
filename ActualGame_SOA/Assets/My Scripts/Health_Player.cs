using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health_Player : NetworkBehaviour
{
    public const int maxHealth = 100;
    public int curHealth = maxHealth;
    public RectTransform healthbar;

    public void TakeDamage(int amount){
     curHealth = curHealth - amount;
     if(curHealth <= 0){
      curHealth = 0;
      print("Robot Dead");
	 }
     healthbar.sizeDelta = new Vector2(curHealth*2, healthbar.sizeDelta.y);
	}
    public void AddHealth(){
     curHealth = 100;
     print("Robot revived");
	}
}
