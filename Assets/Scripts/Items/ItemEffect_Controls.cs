using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Effect Controls", fileName = "Item_Effect_Controls")]
public class ItemEffect_Controls : BaseItemEffect
{
    public override float ModifyInput(float currentInput)
    {
        return -currentInput;
    }    
}
