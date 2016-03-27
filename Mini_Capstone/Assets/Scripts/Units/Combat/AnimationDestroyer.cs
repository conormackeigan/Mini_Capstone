using UnityEngine;
using System.Collections;

public class AnimationDestroyer : MonoBehaviour
{
    public float animTime;
    private float timer;
    
	// Use this for initialization
	void Start ()
    {
        timer = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (timer < animTime)
        {
            timer += Time.deltaTime;
        }

        else // animation expired, destroy and tell combatsequence to deal damage
        {
            GameObject.Find("CombatSequence").GetComponent<CombatSequence>().AoEDamage();
            Destroy(gameObject);
        }
	}
}
