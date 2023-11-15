using UnityEngine;
using System.Collections;
using System;

public class CoroutineWorker : MonoBehaviour
{
	~CoroutineWorker()
	{
	}

	//public void Update()
	//{
	//	if(isStop)
	//	{
	//		StopAllCoroutines();
	//	}
	//}

	//bool isStop = false;
	//public void StopCoroutines()
	//{
	//	isStop = true;
	//	StopAllCoroutines();
	//}
}

public class CoroutineExecutor : IExecutor
{
	public static CoroutineExecutor instance;

	private CoroutineWorker worker;
	~CoroutineExecutor()
	{
		instance = null;
		worker = null;
	}
	public CoroutineExecutor()
	{
		var go = new GameObject("CoroutineWorker");
		worker = go.AddComponent<CoroutineWorker>();
		instance = this;
		GameObject.DontDestroyOnLoad(go);
	}

	public Coroutine Execute(IEnumerator coroutine)
	{
		return worker.StartCoroutine(coroutine);
	}

	public void StopExecution(IEnumerator coroutine)
	{
		worker.StopCoroutine(coroutine);
	}

    public void StopAllExecution()
    {
        worker.StopAllCoroutines();
    }
}
