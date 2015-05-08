using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


namespace LemonSpawn {

	public class TQueue {
		public Thread thread;
		public ThreadQueue gt;
	}	
	
	
	public class ThreadQueue {

		public static List<TQueue> threadQueue = new List<TQueue>(); 
		public static List<TQueue> currentThreads =new List<TQueue>();
		public bool threadDone = true;
		private static int maxThreads = 64;	
		public Vector3 localPosition;
		
/*		public static void SortQueue(Vector3 cam) {
			threadQueue.Sort(
				delegate(TQueue p1, TQueue p2)
				{
					float d1 = (cam - p1.gt.localPosition).magnitude;
					float d2 = (cam - p2.gt.localPosition).magnitude;
					if (d1>d2) return 1;
					if (d1<d2) return -1;
					return 0;
				}
			);
		}
*/		
		
		public static void MaintainThreadQueue() {
			
			List<TQueue> removes = new List<TQueue>();
			foreach (TQueue tq in currentThreads) {
				if (tq.gt.threadDone) {	
					tq.gt.PostThread();
					removes.Add (tq);
				}
				
			}	
			foreach (TQueue tq in removes)
				currentThreads.Remove(tq);
			
			if (threadQueue.Count==0)
				return;
			
			while (currentThreads.Count<maxThreads && threadQueue.Count>0) {
				TQueue currentThread = threadQueue[0];
				currentThreads.Add(currentThread);
			
				threadQueue.RemoveAt(0);
				currentThread.gt.threadDone = false;
				currentThread.thread.Start();
			}	
		}
		
		public static void AbortAll() {
			threadQueue.Clear();
			// Wait for all threads to end.. 
			foreach (TQueue tq in currentThreads)  {
				while (tq.thread.IsAlive)
					Thread.Sleep(1);
			}
			
			currentThreads.Clear();
				
		}
		
		
		public virtual void PostThread() {
		
		}

}

}