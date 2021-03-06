﻿using UnityEngine;
using System.Collections;
using System;

public class HawkeyeSpecial : Special
{
    public HawkeyeSpecial(Unit u) : base(u)
    {
        condition = new CombatCondition(u);
    }

    //the effect this special grants
    public override void effect()
    {
        Debug.Log("hawkeye effect");
        // increases accuracy by 10% and final damage by 1
        if (CombatSequence.Instance.attacker == unit)
        {
            CombatSequence.Instance.attackerHitrate = Mathf.Min(CombatSequence.Instance.attackerHitrate + 10, 99.9f);
            CombatSequence.Instance.attackerDamage += 1;
        }
        else if (CombatSequence.Instance.defender == unit && CombatSequence.Instance.retaliation)
        {
            CombatSequence.Instance.defenderHitrate = Mathf.Min(CombatSequence.Instance.defenderHitrate + 10, 99.9f);
            CombatSequence.Instance.defenderDamage += 1;
        }
    }
}
