using System;
using System.Collections;
using UnityEngine;

public class DelayExecutor : MonoBehaviour
{
    /// <summary>
    /// 延迟执行一个无参 Action
    /// </summary>
    public void DelayExecute(Action action, float delaySeconds)
    {
        StartCoroutine(DelayCoroutine(delaySeconds, action));
    }

    /// <summary>
    /// 延迟执行一个带1个参数的 Action
    /// </summary>
    public void DelayExecute<T>(Action<T> action, T arg1, float delaySeconds)
    {
        StartCoroutine(DelayCoroutine(delaySeconds, () => action(arg1)));
    }

    /// <summary>
    /// 延迟执行一个带2个参数的 Action
    /// </summary>
    public void DelayExecute<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, float delaySeconds)
    {
        StartCoroutine(DelayCoroutine(delaySeconds, () => action(arg1, arg2)));
    }

    /// <summary>
    /// 延迟执行一个带3个参数的 Action（可以按需继续扩展）
    /// </summary>
    public void DelayExecute<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, float delaySeconds)
    {
        StartCoroutine(DelayCoroutine(delaySeconds, () => action(arg1, arg2, arg3)));
    }

    /// <summary>
    /// 通用延迟执行逻辑
    /// </summary>
    private IEnumerator DelayCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
/*
public class TestMeow : MonoBehaviour
{
    public DelayExecutor delayExecutor;

    void Start()
    {
        delayExecutor.DelayExecute(() => Debug.Log("喵！2秒后爆炸💥"), 2f);

        delayExecutor.DelayExecute<string>((name) => {
            Debug.Log($"喵呜～你好呀，{name}！");
        }, "JunZhi", 3f);

        delayExecutor.DelayExecute<int, string>((id, msg) => {
            Debug.Log($"第 {id} 个任务：{msg}");
        }, 5, "延迟执行成功喵～", 4f);
    }
}

*/