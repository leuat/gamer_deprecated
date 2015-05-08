using UnityEngine;
using System.Collections;

namespace LemonSpawn.Gamer {


[ System.Serializable ]
public class ComponentParams {
		public string className;
		public string spectrum = "White"; 
		public float strength;
		public float arm;
		public float z0;
		public float r0;
		public float active = 1;
		public float delta;
		public float winding;
		public float scale;
		public float noiseOffset;
		public float noiseTilt;
		
		
		//  float wavelength;
		// currently not used
		
		//CGraph* spectrum;
		
		public ComponentParams(float s, float a, float z, float r0, float winding) { //, CGraph* g) {
			strength = s;
			arm = a;
			z0 = z;
//			spectrum = g;
		}
		
		
		public ComponentParams() {
			//spectrum = 0;
			arm = 1.0f;
			strength = 1.0f;
			z0 = 0.02f;
			// default
			className = "Bulge";	
			
		}
}
	
	
	

public class GalaxyComponent {
		protected GalaxyParams param;
		public ComponentParams componentParams;
		public ComponentSpectrum spectrum = null;
		protected float keep;
		
		protected float strength;
		
		protected float average, count;
		
		protected float bulgeDust;
		
		
		protected float workTheta, workRad, workWinding, workLength;
		protected GalaxyInstance currentGI = null;
		
		
		protected float winding;
		protected float currentRadius;
		
		// Spline that arm!
	//	static Spline<float, float>* splineArm;
	//	void setupSplineArm(int N);

		public void Initialize(ComponentParams cp, GalaxyParams gp) {
			componentParams = cp;
			param = gp;
			count = 0;
			average = 0;
			spectrum = Spectra.FindSpectrum(cp.spectrum);
			//    quat = quaternion(0, param.orientation*-1);
		}

		public GalaxyComponent() {
			param = null;
			
			
		}
		
		// This method needs to be implemented by each component type
		public virtual void componentIntensity(RasterPixel rp, Vector3 p, float ival )  { 
			Debug.Log ("GALAXYCOMPONENT: COMPONENTINTENSITY SHOULD NEVER BE CALLED");
			rp.I = Vector3.zero;
		}
		// returns the intensity at a given position

		public void setGalaxyParam(GalaxyParams gp) {
			param = gp;
		}
		
		float cosh(float x) {
			return (Mathf.Exp (x) + Mathf.Exp (-x))/2f;
		}
		
		protected float getHeightModulation(float height) {
			float h = Mathf.Abs(height/componentParams.z0);
			float val = 0;
			// cos(h) is slow, so better to just test if it is really close to zero.
			if (Mathf.Abs(h)>2.0) {
				height = 0;
				return 0;
			}
			
			val = 1.0f/cosh(h);
			height = val*val;
			return height;
		}
		
		public virtual void calculateIntensity(RasterPixel rp, Vector3 p, GalaxyInstance gi) {
			Vector3 P;
			currentGI = gi;
			float z = 1;
			currentRadius = getRadius(p, out P, out z, gi);
			z = getHeightModulation(z);
			float armVal = 1;
			 if (z>0.01)
			 {
				
				float intensity = getRadialIntensity(currentRadius);
				if (intensity>0.1f) intensity = 0.1f;
				if (intensity >0.001f) {
					
					float scale = 1;
					if (componentParams.className == "Dust")
						scale = Mathf.SmoothStep(0, 1.0f*param.bulgeDust, currentRadius);
					if (componentParams.arm!=0) {
						armVal = calculateArmValue(currentRadius, P, componentParams.arm);
						winding = getWinding(currentRadius)*componentParams.winding;///(rad+1.0);
						
					}
					// equation 5 from the paper
					float val = (componentParams.strength)*scale*armVal*z*intensity*gi.intensityScale;
					componentIntensity(rp,p, val);
				}
			}
		}
		
		float getRadialIntensity(float rad) {
			float r = Mathf.Exp(-rad/(componentParams.r0*0.5f));
			return Mathf.Clamp(r - 0.01f,0,1f);
		}
		
		
		protected float getRadius(Vector3 p, out Vector3 P, out float dott, GalaxyInstance gi)  {
			dott = Vector3.Dot(p, gi.orientation);
			P = p - gi.orientation*dott;
//			if (Mathf.Abs(P.x*P.x)+ Mathf.Abs(P.y*P.y)+Mathf.Abs(P.z*P.z)<0.00001f)
//				return 0.00001f;
			return P.magnitude/ param.axis.x;
		}
		
		
		
/*		void loadSpectrum(CGraph sp) {
			componentParams.spectrum = new CGraph();
			componentParams.spectrum->Copy(*sp);
			componentParams.spectrum->scaleX(param.redshift);
		}
*/		
		
		
		
		
		Vector3 twirl( Vector3 p,  float twirl) {
			Quaternion q = Quaternion.AngleAxis(twirl*180f, currentGI.orientation);
			return q*p;
			
		}
		
		/*
 * Remember to always call calculate_statistics on the procedural noise before using this method
 *
*/
		public Vector3 dAdd = new Vector3();
		public float getPerlinCloudNoise(Vector3 p, float rad, float t, int NN, float ks, float pers, bool inv, float freq) {
			Vector3 r = twirl(p, t);
			float perlinnoise = 0;
			// add a random shift to avoid procedural artifacts
			Vector3 add = new Vector3(param.randShiftX, param.randShiftY, param.randShiftX*1.521f);
			// loop through N levels
			int N = (int)(NN*gamer.rast.RP.noiseDetail);			
			for (int ii=1;ii<N;ii++) {
				float k= (ii*ks);
				float pp = Util.Simplex.raw_noise_3d( (r.x + add.x)*k, (r.y+ add.y)*k, (r.z + add.z)*k);
				// add a random shift
				// OPTIMIZE THIS UNITY
				dAdd.Set (k*0.111f,k*k,-0.712f*k);
				add = add + dAdd;
				if (inv) {
					perlinnoise+=(1.0f/(pp/k+0.015f));
					//max+=1.0f/(perlin->standard_deviation/k + 0.015f);
				}
				else {
					perlinnoise+=pp/k;
					//max+=perlin->standard_deviation/k;
				}
			}
			
			return perlinnoise/1f;//max;
		}
		
		
		float findDifference( float t1,  float t2) {
			float v1 = Mathf.Abs(t1-t2);
			
			float M_PI = Mathf.PI;
			
			float v2 = Mathf.Abs(t1-t2-2*M_PI);
			float v3 = Mathf.Abs(t1-t2+2*M_PI);
			float v4 = Mathf.Abs(t1-t2-2*M_PI*2);
			float v5 = Mathf.Abs(t1-t2+2*M_PI*2);
			
			float v = Mathf.Min(v1,v2);
			v = Mathf.Min(v,v3);
			v = Mathf.Min(v,v4);
			v = Mathf.Min(v,v5);
			
			return v;
			
		}
		float calculateArmValue( float rad,  Vector3 P,  float armStrength) {
			
			workRad = Mathf.Sqrt(rad);
			workTheta = -getTheta(P);
			workWinding = getWinding(rad);
			
			
			float v1 = getArm(rad, P, param.arm1);
			if (param.noArms==1)
				return v1;
			float v = Mathf.Max(v1, getArm(rad, P, param.arm2));
			if (param.noArms==2)
				return v;
			
			v = Mathf.Max(v, getArm(rad, P, param.arm3));
			if (param.noArms==3)
				return v;
			
			v = Mathf.Max(v, getArm(rad, P, param.arm4));
			
			return v;
		}
		
		
		float getArm( float rad,  Vector3 p,  float disp) {
			float v = Mathf.Abs(findDifference(workWinding,workTheta + disp))/Mathf.PI;
			return Mathf.Pow(1.0f-v,componentParams.arm*15f);
		}
		
		
		
		float getTheta( Vector3 p ) {
			
			Vector3 quatRot = currentGI.rotmat*p;//rotmat.Mul(p, quatRot);
//			return Mathf.Atan2(rA[0], rB[0]) + componentParams.delta;
			return Mathf.Atan2(quatRot.x, quatRot.z) + componentParams.delta;
		}
		
		
		float getWinding( float rad) {
			float r = rad + 0.05f;
			float t = Mathf.Atan((float)Mathf.Exp(-0.25f/(0.5f*(r)))/param.windingB)*2*param.windingN;
			//float t = (*param.splineWinding)[r];
			float scale = 1.0f;
			float t2 = 0.0f;
			if (param.innerTwirl != 0) {
				t2 = -0.5f/(Mathf.Pow(r,1.0f)+0.1f);
				scale = Mathf.SmoothStep(0.00f, param.bulgeDust*1.5f, r);
			}
			t = t*scale + t2*(1-scale);
			return t;
			
		}
		
								

}

}