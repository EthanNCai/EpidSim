using UnityEngine;


public class DelayExecutorDebugger : MonoBehaviour
{
    public DelayExecutor delayExecutor;
    public void Test()
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
