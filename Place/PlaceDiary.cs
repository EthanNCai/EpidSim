using System.Collections.Generic;
// using System.Diagnostics;
using System.Text;
using UnityEngine;
public class PlaceDiary
{
    private readonly Queue<PlaceDiaryItem> diaryQueue = new Queue<PlaceDiaryItem>();
    private readonly Queue<string> diaryReprQueue = new Queue<string>(); // 可以理解为DiaryItem的数字孪生

    public void AppendDiaryItem(PlaceDiaryItem item)
    {
        if (diaryQueue.Count >= CommonMetas.diaryMaxEntries)
        {
            diaryQueue.Dequeue(); // 移除最旧的元素
            diaryReprQueue.Dequeue();
        }
        diaryQueue.Enqueue(item); // 添加新日志
        diaryReprQueue.Enqueue($"[d{item.timestamp.d:D2}, {item.timestamp.h:D2}:{item.timestamp.q:D2}] {item.palceBehaviorDetial}");
    }

    public Queue<string> GetDiaryReprQueue()
    {
        return diaryReprQueue;
    }
}

public struct PlaceDiaryItem{
    public (int d, int h, int q) timestamp;
    public string palceBehaviorDetial;

    // public StringBuilder stringBuilder;
    public PlaceDiaryItem( (int d, int h, int q) timestamp, string placeBehaviorDetial){
        this.timestamp = timestamp;
        this.palceBehaviorDetial = placeBehaviorDetial;
        // this.stringBuilder = new StringBuilder();
    }
}


public static class PlaceBehaviorsDetails
{
    public static StringBuilder stringBuilder = new StringBuilder();

    public static string ContributeTaxEvent<T>(T place) where T : Place, ITaxPayer
    {
        stringBuilder.Clear();
        stringBuilder.Append("Contributed Tax: ");
        stringBuilder.Append(place.GetAndResetTaxContributedLastD()); // 确保能调用 ITaxPayer 的方法
        return stringBuilder.ToString();
    }

}
