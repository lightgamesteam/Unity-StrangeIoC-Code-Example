using System.Collections;
using UnityEngine;

public interface IExecutor
{
    Coroutine Execute(IEnumerator coroutine);

	void StopExecution(IEnumerator coroutine);

    void StopAllExecution();
}
