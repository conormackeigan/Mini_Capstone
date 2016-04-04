using UnityEngine;
using System.Collections;

public class EnergyChainShot : MonoBehaviour
{
    public Vector2i direction;
    public int max;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void createNext()
    {
        Unit unit = PlayerManager.Instance.getCurrentPlayer().selectedObject.GetComponent<Unit>();
        Vector2i next = GLOBAL.worldToGrid(transform.position) + direction;
        if (next.x >= 0 && next.x < MapScript.Instance.mapWidth && next.y >= 0 && next.y < MapScript.Instance.mapHeight)
        {
            GameObject go = GameObject.Instantiate(Resources.Load("WeaponEffects/EnergyGreen") as GameObject, GLOBAL.gridToWorld(GLOBAL.worldToGrid(transform.position) + direction), Quaternion.identity) as GameObject;
            go.GetComponent<EnergyChainShot>().max = max;
            go.GetComponent<EnergyChainShot>().direction = direction;
        }
    }

    public void destroy()
    {
        Vector2i pos = GLOBAL.worldToGrid(transform.position);
        Unit unit = PlayerManager.Instance.getCurrentPlayer().selectedObject.GetComponent<Unit>();

        if (pos.Distance(unit.pos) == max)
        {
            // last shot, administer damage (TODO: only administer once regardless of how many max distance tiles there are)
            GameObject.Find("CombatSequence").GetComponent<CombatSequence>().AoEDamage();           
        }

        Destroy(gameObject);
    }
}
