using UnityEngine;
using System.Collections;


namespace LemonSpawn.Gamer {

	public class Util {
		
		public static Noise4D Simplex = new Noise4D();
		public static System.Random rnd = new System.Random();
		
		static public int[] ShuffleArray(int[] array)
		{
			System.Random r = new System.Random();
			for (int i = array.Length; i > 0; i--)
			{
				int j = r.Next(i);
				int k = array[j];
				array[j] = array[i - 1];
				array[i - 1]  = k;
			}
			return array;
		}		
		public static bool IntersectSphere(Vector3 o, Vector3 d, Vector3 r, out Vector3 isp1, out Vector3 isp2, out float t0, out float t1) {
		
		r.x = 1f/(r.x*r.x);
		r.y = 1f/(r.y*r.y);
		r.z = 1f/(r.z*r.z);
		
		
		Vector3 rD = new Vector3(d.x*r.x, d.y*r.y, d.z*r.z);
		Vector3 rO = new Vector3(o.x*r.x, o.y*r.y, o.z*r.z);
		
		
		float A = Vector3.Dot (d,rD);
		float B = 2.0f*(Vector3.Dot (d, rO));
		float C = Vector3.Dot(o, rO) - 1.0f;
		
		float S = (B*B - 4.0f*A*C);
		
		if (S<=0) {
			isp1 = Vector3.zero;
			isp2 = Vector3.zero;
			t0 = 0;
			t1 = 0;
			return false;
		}
		
		t0 =  (-B - Mathf.Sqrt(S))/(2*A);
		t1 =  (-B + Mathf.Sqrt(S))/(2*A);
		
		isp1 = o+d*t0;
		isp2 = o+d*t1;
		
		return true;
	}
	
		public static string secondsToMinutes(float secs) {
			int i = (int)secs;
			int m = i / 60;
			int s = i % 60;
			return m + "m " + s + "s"; 
			
		}
		
	
		private static bool _hasDeviate;
		private static float _storedDeviate;
		public static float NextGaussian(float mu = 0, float sigma = 1)
		{
			
			if (_hasDeviate)
			{
				_hasDeviate = false;
				return _storedDeviate*sigma + mu;
			}
			
			float v1, v2, rSquared;
			do
			{
				// two random values between -1.0 and 1.0
				v1 = 2*(float)rnd.NextDouble() - 1;
				v2 = 2*(float)rnd.NextDouble() - 1;
				rSquared = v1*v1 + v2*v2;
				// ensure within the unit circle
			} while (rSquared >= 1 || rSquared == 0);
			
			// calculate polar tranformation for each deviate
			float polar = Mathf.Sqrt(-2*Mathf.Log(rSquared)/rSquared);
			// store first deviate
			_storedDeviate = v2*polar;
			_hasDeviate = true;
			// return second deviate
			return v1*polar*sigma + mu;
		}
	
	}

}