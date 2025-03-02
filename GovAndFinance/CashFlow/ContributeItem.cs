using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEditor.UI;
using UnityEngine;

public class ContributeItem
{
    public static int QUARTS_PER_DAY =  4 * 24;
    public string CFEName;
    public Queue<int> contributeQueueOfDay = new Queue<int>();
    public ContributeSubTypes contributeSubType;
    public ContributeTypes contributeTypes;
    public int newQContribution;
    public ContributeItem(ContributeSubTypes contributeSubType){
        this.contributeSubType = contributeSubType;
        this.contributeTypes = ContributeItem.GetContributeType(contributeSubType);
    }
    public (int QuartContribute,int DayContribute) QGetContribution(){
        return (this.newQContribution,this.contributeQueueOfDay.Sum());
    }
    public void QUpdateContribution(int newQContribution){
        this.newQContribution = newQContribution;
        if( contributeQueueOfDay.Count >= QUARTS_PER_DAY){
            contributeQueueOfDay.Dequeue();
            contributeQueueOfDay.Enqueue(newQContribution);
        }else{
            contributeQueueOfDay.Enqueue(newQContribution);
        }
    }
    public static ContributeTypes GetContributeType(ContributeSubTypes type){
        switch (type)
        {
            case ContributeSubTypes.Taxes:
                return ContributeTypes.Common;
            case ContributeSubTypes.PublicServiceFees:
                return ContributeTypes.Fees;
            default:
                throw new Exception("Invalid contribute sub type");
        }
    }
}

public enum ContributeTypes {
    Common,
    Fees,
}

public enum ContributeSubTypes {
    Taxes,
    PublicServiceFees
}
