using UnityEngine;
using System.Collections;

public class aiBase
{ 
	public virtual void StartTurn() { }

    public virtual void SelectAction() { }

    public virtual void EndTurn() { }
}
