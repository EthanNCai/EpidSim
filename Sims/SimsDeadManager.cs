using System.Collections;
using UnityEngine;


public class SimsDeadManager:MonoBehaviour{

    public GameObject deadReprPrefab;

    public void HandleSimsDie(Sims targetSim)
    {
        Debug.Log("Some one just Dead");

        Debug.Assert(targetSim.infectionStatus == InfectionStatus.Dead, "bug here, this person not die yet");

        // 1. 从Building里面注销
        if (targetSim.home != null) targetSim.home.registeredSims.Remove(targetSim);
        if (targetSim.office != null) targetSim.office.registeredSims.Remove(targetSim);

        // 2. 注销事件监听
        TimeManager.OnQuarterChanged -= targetSim.HandleTimeChange;
        TimeManager.OnDayChanged -= targetSim.HandleDayChange;

        // 3. 开始淡出+生成纪念物+摧毁 GameObject
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

            // 用原来的 parent 和 localPosition 放纪念物
            GameObject deadRepr = Instantiate(deadReprPrefab, targetSim.transform.parent);
            deadRepr.transform.localPosition = targetSim.transform.localPosition;

            Destroy(targetSim.gameObject);
            yield break;
        }

        // 🎯 先记录原始位置和父对象
        Transform originalParent = targetSim.transform.parent;
        Vector3 originalLocalPosition = targetSim.transform.localPosition;

        // 🧸 生成纪念物，但一开始设为透明！
        GameObject memorial = Instantiate(deadReprPrefab, originalParent);
        memorial.transform.localPosition = originalLocalPosition;

        SpriteRenderer memorialSR = memorial.GetComponent<SpriteRenderer>();
        if (memorialSR != null)
        {
            Color color = memorialSR.color;
            color.a = 0f;
            memorialSR.color = color;
        }

        // ✨ 开始淡出 Sim，同步淡入纪念物
        Color simColor = simSR.color;
        Color memorialColor = memorialSR != null ? memorialSR.color : Color.clear;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Sim 渐隐
            simSR.color = new Color(simColor.r, simColor.g, simColor.b, 1 - t);

            // 纪念物 渐显
            if (memorialSR != null)
            {
                memorialSR.color = new Color(memorialColor.r, memorialColor.g, memorialColor.b, t);
            }

            yield return null;
        }

        // 🧼 最后，销毁 Sim
        Destroy(targetSim.gameObject);
    }



}