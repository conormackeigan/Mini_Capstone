//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System;

//public class TrooperCondition : SpecialCondition
//{
//	public TrooperCondition(Unit u) : base(u)
//    {

//    }

//    // evaluates number of charisma buffs this unit gets
//    public override bool eval()
//    {
//        rval = false;        

//        // iterate through all ally units, checking distance from charisma units
//        List<Unit> units = new List<Unit>();
//        if (unit.playerID == 1)
//        {
//            foreach(GameObject u in ObjectManager.Instance.PlayerOneUnits)
//            {
//                units.Add(u.GetComponent<Unit>());
//            }
//        }
//        else if (unit.playerID == 2)
//        {
//            foreach (GameObject u in ObjectManager.Instance.PlayerTwoUnits)
//            {
//                units.Add(u.GetComponent<Unit>());
//            }
//        }

//        foreach(Unit u in units)
//        {
//            if (unit.Pos.Distance(u.Pos) <= 3 && u.tags.Contains("charisma"))
//            {
//                rval = true;
//            }
//        }

//        return rval;
//    }
//}
