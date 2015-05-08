using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;


namespace LemonSpawn.Gamer {
	[ System.Serializable ]
	public class RenderingParams {

		public float idx;
		public float idxCount;
		public float divide;
		public GamerCamera camera = new GamerCamera();
		public Vector3 direction;
		
		public float detailLevel;
		public float noiseDetail = 1;
		public float seed;
		public int size; // pixels
		public float exposure = 1;
		public float gamma = 1;
		public float saturation = 1;
		public int no_procs;
		
		public bool continuousPostprocessing=true;
		
		public int noStars=0;
		public float starSize=1, starSizeSpread=1;
		public float starStrength = 1;
		
		public string currentScene = "";
		
		public float rayStep;
		public float rayStepPreview;
		public float rayStepNormal;
		public Vector3 color = new Vector3(1,1,1);
		public float wavelength;
		
		public float lensing;
		public string currentGalaxy = "";
		public float bufferSize;
		//CNoise* perlin;
		
		public float randomize_spectra;
		
		
		
		public static void Save(string filename, RenderingParams p) {
			XmlSerializer serializer = new XmlSerializer(typeof(RenderingParams));
			TextWriter textWriter = new StreamWriter(filename);
			serializer.Serialize(textWriter, p);
			textWriter.Close();
			
		}
		public static RenderingParams Load(string filename) {
			XmlSerializer deserializer = new XmlSerializer(typeof(RenderingParams));
			TextReader textReader = new StreamReader(filename);
			RenderingParams p = (RenderingParams)deserializer.Deserialize(textReader);
			textReader.Close();
			return p;
		}
		
		
		/*float lensingSize;
		float lensingStrength;
		float healpix;
		*/
		

	}

}
