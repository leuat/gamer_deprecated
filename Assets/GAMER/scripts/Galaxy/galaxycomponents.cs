using UnityEngine;
using System.Collections;

namespace LemonSpawn.Gamer {

[System.Serializable]
public class GalaxyComponentBulge : GalaxyComponent {
		
		public override void componentIntensity(RasterPixel rp, Vector3 p, float ival) {
			//    return I;
			float rho_0 = componentParams.strength*ival;
			Vector3 pos = currentGI.rotmat*(p);
			
			float rad = (pos.magnitude+0.01f)*componentParams.r0;
			rad+=0.01f;
			
			float i = rho_0 * (Mathf.Pow(rad,-0.855f)*Mathf.Exp(- Mathf.Pow(rad,1/4.0f)) -0.05f) *currentGI.intensityScale;
			
//			float i = rho_0*componentParams.arm * (*spline)[rad] *params->intensityScale;
//			i = 1;
		 	//i = 1;
			
			if (i<0) i=0;
			rp.I += i*spectrum.spectrum*rp.scale;
			
		}
		

		public  override void calculateIntensity(RasterPixel rp, Vector3 p, GalaxyInstance gi) {
			Vector3 P;
			float z = 1;
			currentGI = gi;
			
			currentRadius = getRadius(p, out P, out z, gi);
			float val = 1;
			componentIntensity(rp,p, val);
			
		}
		
}

	public class GalaxyComponentDisk : GalaxyComponent {
	
		public override void componentIntensity(RasterPixel rp, Vector3 p, float ival) {
			float p2 = 0.5f;
			// Level = 14
			if (ival<0.0005)
				return;
//			if (componentParams.scale!=0.0)
				p2 = Mathf.Abs(getPerlinCloudNoise(p*componentParams.scale, 1.0f, winding, 9, 1, componentParams.noiseTilt, false, componentParams.scale));

/*			if (Util.rnd.NextDouble()>0.99)
				Debug.Log (p2);
*/			p2 = Mathf.Max (p2, 0.01f);
			if (p2!=0)
				p2=0.1f/p2;
			
			p2 = Mathf.Pow(p2,componentParams.noiseTilt);
			
			//params->diskStarModifier = p2;//CMath::Minmax(p2, 0.1, 1.0);
			
			p2+=componentParams.noiseOffset;
			
			if (p2<0)
				return;
			
	//		ival = 1;
			rp.I+= ival*p2*spectrum.spectrum*rp.scale;
		}
		
	
	}
		

	public class GalaxyComponentDust : GalaxyComponent {
		
		public override void componentIntensity(RasterPixel rp, Vector3 p, float ival) {
			if (ival<0.0005)
				return;
			
			float p2 = getPerlinCloudNoise(p*componentParams.scale, currentRadius, winding, 14, 1, 1.0f, false, componentParams.scale);
			//if (p2!=0)
			p2=0.1f/(Mathf.Abs(p2) + 0.1f);
			
			p2 = p2 + componentParams.noiseOffset;
			if (p2<0)
				return;
			p2 = Mathf.Pow(5*p2,componentParams.noiseTilt);
			//keep = p2;
			
			float s = gamer.rast.RP.rayStepNormal*100;
			
			rp.I.x*=Mathf.Exp(-p2*ival*spectrum.spectrum.x*s);
			rp.I.y*=Mathf.Exp(-p2*ival*spectrum.spectrum.y*s);
			rp.I.z*=Mathf.Exp(-p2*ival*spectrum.spectrum.z*s);
			
			
		}
		
		
	}

	public class GalaxyComponentStars : GalaxyComponent {
		
		public override void componentIntensity(RasterPixel rp, Vector3 r, float ival) {
			if (ival<0.0005)
				return;
			
			float shift = 2.0f;
			float perlinnoise = 0;
			//20
			int N = 14;
			Vector3 add = new Vector3(112.21f, 342.12f, 12.037f);
			for (int ii=1;ii<N;ii++) {
				float k= (ii*4 )*componentParams.scale;
				//double k= componentParams.scale*10.0;
				float pp = Mathf.Abs(Util.Simplex.raw_noise_3d( (r.x + 0.191f + param.randShiftX*51.13f)*k, (r.y+ 0.331f + shift*0.5113f+param.randShiftY*39.1f)*k, (r.z + 0.031f +shift + param.randShiftY*(41.7f))*k ));///pow(ii,0.25);
				r = r + add/k;
				perlinnoise+=pp;
			}
			perlinnoise/=N;
//			if (Util.rnd.NextDouble()>.99)
//				Debug.Log (perlinnoise);			
			perlinnoise-=componentParams.noiseOffset;
			float val = Mathf.Pow(perlinnoise+1,componentParams.noiseTilt);
//			val*=10;
			//	val*=1000;
		//	val=Mathf.Min (val, 10000);
			
			
			rp.I += ival*val*spectrum.spectrum*rp.scale;
			
			
		}
		
		
	}

	public class GalaxyComponentStars2 : GalaxyComponent {
		
		public override void componentIntensity(RasterPixel rp, Vector3 r, float ival) {
			if (ival<0.0005)
				return;
			
			float shift = 2.0f;
			float perlinnoise = 0;
			//20
			int N = 1;
			Vector3 add = new Vector3(112.21f, 342.12f, 12.037f);
			for (int ii=1;ii<N;ii++) {
				float k= (ii*4 )*componentParams.scale;
				//double k= componentParams.scale*10.0;
				float pp = Mathf.Abs(Util.Simplex.raw_noise_3d( (r.x + 0.191f + param.randShiftX*51.13f)*k, (r.y+ 0.331f + shift*0.5113f+param.randShiftY*39.1f)*k, (r.z + 0.031f +shift + param.randShiftY*(41.7f))*k ));///pow(ii,0.25);
				r = r + add/k;
				perlinnoise+=pp;
			}
			perlinnoise/=N;
			
			//			if (Util.rnd.NextDouble()>.99)
			//				Debug.Log (perlinnoise);			
			perlinnoise-=componentParams.noiseOffset;
			float val = Mathf.Pow(perlinnoise+1,componentParams.noiseTilt);
			//			val*=10;
			//	val*=1000;
			//	val=Mathf.Min (val, 10000);
			
			
			rp.I += ival*val*spectrum.spectrum*rp.scale;
			
			
		}
		
		
	}
	
	
}