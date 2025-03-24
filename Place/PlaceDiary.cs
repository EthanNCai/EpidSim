using System.Collections.Generic;
using System.Text;
public class PlaceDiary
{
    private List<PlaceDiaryItem> diaryList = new List<PlaceDiaryItem>();

    public void AppendDiaryItem(PlaceDiaryItem item)
    {
        if (diaryList.Count >= 50) // check if the size is over 50, if so remove the oldest entries.
        {
            diaryList.RemoveAt(0); // remove the oldest entry
        }
        diaryList.Add(item); // add new entry
    }

    public void GetDiaryEntries(List<string> entries)
    {
        entries.Clear(); // Reuse the existing list, clear previous entries
        foreach (var item in diaryList)
        {
            string formattedTime = $"[day {item.timestamp.d:D2}, {item.timestamp.h:D2}:{item.timestamp.q:D2}]";
            entries.Add($"{formattedTime} {item.simBehaviorDetial}");
        }
    }
}

public struct PlaceDiaryItem{
    public (int d, int h, int q) timestamp;
    public string simBehaviorDetial;

    // public StringBuilder stringBuilder;
    public PlaceDiaryItem( (int d, int h, int q) timestamp, string simBehaviorDetial){
        this.timestamp = timestamp;
        this.simBehaviorDetial = simBehaviorDetial;
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
