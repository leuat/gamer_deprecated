using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;


namespace LemonSpawn.Gamer {

[System.Serializable]
public class GalaxyParams {
		public Vector3 axis = new Vector3(1,1,1);
		public float bulgeDust = 0.025f;
		public Vector3 bulgeAxis = Vector3.one;
		
		
		public float windingB = 0.5f; 
		public float windingN = 4f;
		public float noArms = 2;
		public float arm1 = 0, arm2 = Mathf.PI, arm3 = 2*Mathf.PI,  arm4 = 3*Mathf.PI;
		
		
		public float innerTwirl = 0;
		
		public float warpAmplitude;
		public float warpScale;
		
		
/*		Spline<float, float>* splineWinding;
		
		void setupSplineWinding(int N) {
			std::vector<float> xx;
			std::vector<float> yy;
			for (int i=0;i<N;i++) {
				float rad = 1.0*((float)i/(float)N + 0.001);
				//          float y =  atan((float)exp(-0.5/(rad))/windingB)*2*windingN;
				float y =  atan((float)exp(-0.25/(0.5*rad))/windingB)*2*windingN;
				
				xx.push_back(rad);
				yy.push_back(y);
				//  cout << rad << " with " << y << endl;
			}
			splineWinding = new Spline<float, float>(xx,yy);
			
			
		}
		*/
		
		Vector3 camera;
		
		//  float dustScale;
		//float diskScale;
		
//		float* dataBuffer;
//		float bufferSize;
		
//		CNoise* perlin;
		
		public int randShiftX, randShiftY;
		
		public float diskStarModifier;
		
		
		public GalaxyParams() {
//			dataBuffer = (float*)this;
//			bufferSize = (sizeof(CGalaxyParams)-2)/8;
			
			noArms = 2;
			arm1 = 0;
			arm2 = Mathf.PI;
			arm3 = Mathf.PI/2.0f;
			arm4 = Mathf.PI + Mathf.PI/2.0f;
			
			
			randShiftX = (int)((Random.value*1001.1f));
			randShiftY = (int)((Random.value*1001.9f));
			
		}
		
		
		
		
	}
	

//[System.Serializable]
public class Galaxy  {
	
	public GalaxyParams param = new GalaxyParams();
	public string displayName = "";
		// Rotation matrix
	private Matrix4x4 rotation;
		// pointer to spectra
	//	CSpectra* spectra;
		// galaxy components
	public List<ComponentParams> componentParams = new List<ComponentParams>();	
	
	private List<GalaxyComponent> components = new List<GalaxyComponent>();
	
	public List<GalaxyComponent> getComponents() {
		return components;
	}
	
	public void SetupComponents() {
		components = new List<GalaxyComponent>();
		foreach (ComponentParams cp in componentParams) {
			GalaxyComponent ngc = (GalaxyComponent)System.Activator.CreateInstance (System.Type.GetType (Settings.classDictionary[cp.className]));
			ngc.Initialize(cp, param);
			components.Add (ngc);
		}
		SetupSpectra();	
	}
	public void SetupSpectra() {
		foreach (GalaxyComponent gc in components) {
			gc.spectrum = Spectra.FindSpectrum(gc.componentParams.spectrum);
			if (gc.spectrum == null) {
				Debug.Log ("ERROR Could not find spectrum : " + gc.componentParams.spectrum);
			}			
		}	
	}
	
	public int AddComponent() {
		ComponentParams cp = new ComponentParams();
		cp.className = "Bulge";
		cp.strength = 1;
		cp.r0 = 1;
		componentParams.Add (cp);
		SetupComponents();
		return componentParams.Count-1;
		
	}
	
							
	public Galaxy Clone() {
		Galaxy g = new Galaxy();
		g.param = param; 
		g.displayName = displayName;
		g.componentParams = componentParams;
		foreach (GalaxyComponent gc in components) {
			GalaxyComponent ngc = (GalaxyComponent)System.Activator.CreateInstance (gc.GetType());
			ngc.Initialize(gc.componentParams, param);
			g.components.Add (ngc);
		}
		return g;
	}
				
							
	public static void Save(string filename, Galaxy g) {
		XmlSerializer serializer = new XmlSerializer(typeof(Galaxy));
		TextWriter textWriter = new StreamWriter(filename);
		serializer.Serialize(textWriter, g);
		textWriter.Close();
			
	}
	public static Galaxy Load(string filename, string dn) {
	
		if (!File.Exists(filename))
			return null;
	
		XmlSerializer deserializer = new XmlSerializer(typeof(Galaxy));
		TextReader textReader = new StreamReader(filename);
		Galaxy g = (Galaxy)deserializer.Deserialize(textReader);
		textReader.Close();
		g.SetupComponents();
		g.displayName = dn;
		return g;
	}


}

}