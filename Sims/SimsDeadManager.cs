using System;
using System.Collections;
using UnityEngine;

public class SimsDeadManager : MonoBehaviour
{
    public GameObject deadReprPrefab;
    public InfoManager infoManager;
    public SimsManager simsManager;

    public bool isAnySimDeadBefore = false;

    // ✨ 添加一个静态事件喵，参数是死掉的 Sim 本体
    public static event Action<Sims> OnSimsDied;

    public void HandleSimsDie(Sims targetSim)
    {
        // Debug.Log("Someone just died (｡•́︿•̀｡)");
        // Debug.Assert(targetSim.infectionStatus == InfectionStatus.Dead, "Bug here! This person not dead yet!");
        if(isAnySimDeadBefore == false){
            infoManager.notificationManager.SendFirstDeadCaseNotification(targetSim);
            isAnySimDeadBefore = true;
        }

        // 1. 从Building里面注销
        if (targetSim.home != null) targetSim.home.registeredSims.Remove(targetSim);
        if (targetSim.office != null) targetSim.office.registeredSims.Remove(targetSim);
        simsManager.activeSimsList.Remove(targetSim); // 从Simsmanager里面彻底移除掉， 如果想要死去的模拟市民的信息的话，那就只能从dead里面去找了


        // 2. 注销事件监听
        TimeManager.OnQuarterChanged -= targetSim.HandleTimeChange;
        TimeManager.OnDayChanged -= targetSim.HandleDayChange;

        // 3. 发出死亡事件喵～～让别人知道发生了啥！
        OnSimsDied?.Invoke(targetSim);

        // 4. 开始淡出+生成纪念物+摧毁 GameObject
        StartCoroutine(FadeAndDestroy(targetSim));
    }

    private IEnumerator FadeAndDestroy(Sims targetSim)
    {
        float duration = 1.5f;
        float elapsed = 0f;

        SpriteRenderer simSR = targetSim.GetComponent<SpriteRenderer>();
        if (simSR == null)
        {
            Debug.LogWarning("Sim没有SpriteRenderer喵，无法淡出，直接摧毁！");

            GameObject deadRepr = Instantiate(deadReprPrefab, targetSim.transform.parent);
            deadRepr.transform.localPosition = targetSim.transform.localPosition;

            Destroy(targetSim.gameObject);
            yield break;
        }

        Transform originalParent = targetSim.transform.parent;
        Vector3 originalLocalPosition = targetSim.transform.localPosition;

        GameObject memorial = Instantiate(deadReprPrefab, originalParent);
        memorial.transform.localPosition = originalLocalPosition;

        SpriteRenderer memorialSR = memorial.GetComponent<SpriteRenderer>();
        if (memorialSR != null)
        {
            Color color = memorialSR.color;
            color.a = 0f;
            memorialSR.color = color;
        }

        Color simColor = simSR.color;
        Color memorialColor = memorialSR != null ? memorialSR.color : Color.clear;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            simSR.color = new Color(simColor.r, simColor.g, simColor.b, 1 - t);
            if (memorialSR != null)
            {
                memorialSR.color = new Color(memorialColor.r, memorialColor.g, memorialColor.b, t);
            }

            yield return null;
        }

        Destroy(targetSim.gameObject);
    }
}
