using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


namespace LemonSpawn.Gamer {


	public class RasterPixel {
		public Vector3 I = Vector3.zero;
		public void Add(RasterPixel o) {
			I += o.I;
		}
		public float scale = 1;
		public RasterPixel(RasterPixel o) {
			I = o.I;
		}
		public RasterPixel() {
			I.Set(0,0,0);
		}
		public void Floor(float f) {
			if (I.x<f) I.x=f;
			if (I.y<f) I.y=f;
			if (I.z<f) I.z=f;
		}
	}


	public class Settings {
	
		public static Dictionary<string, string> classDictionary = new Dictionary<string,string>(); 
		
		public static string ParamFileSingle = "RenderingParamsGalaxy.xml";
		public static string ParamFileScene = "RenderingParamsScene.xml";
		public static char FileSeparator = '/';
		public static string GalaxyDirectory = "Galaxies" + FileSeparator;
		public static string ReferenceDirectory = "References" + FileSeparator;
		public static string SceneDirectory = "Scenes"+ FileSeparator;
		public static string OutputDir = "OutputImages"+ FileSeparator;
		public static string GalaxyImageName = "galaxy";
		
		
		public static string GetNextOutputFile() {
			DirectoryInfo info = new DirectoryInfo(OutputDir);
			FileInfo[] fileInfo = info.GetFiles();
			int current = 0;
			foreach (FileInfo f in fileInfo)  {
				string name = f.Name.Remove(f.Name.Length-4, 4);
				Regex rgx = new Regex("[^0-9 -]");
				name = rgx.Replace(name, "");
				int next;			
				if (int.TryParse(name, out next))
					current=next;
			}
			current++;
			return OutputDir +GalaxyImageName + current.ToString("0000") + ".png";
			
		}	
		
		
		public static void SetupGamer() {
			if (Application.platform == RuntimePlatform.WindowsPlayer)
				FileSeparator = '\\';
			
			SetupDictionary();
			Spectra.PopulateSpectra();	
		}
			
		
		public static void SetupDictionary() {
			classDictionary.Clear();
			classDictionary.Add("Bulge","LemonSpawn.Gamer.GalaxyComponentBulge,Assembly-CSharp,Version=0.0.0.0,Culture=neutral,PublicKeyToken=null");
			classDictionary.Add("Disk","LemonSpawn.Gamer.GalaxyComponentDisk,Assembly-CSharp,Version=0.0.0.0,Culture=neutral,PublicKeyToken=null");
			classDictionary.Add("Dust","LemonSpawn.Gamer.GalaxyComponentDust,Assembly-CSharp,Version=0.0.0.0,Culture=neutral,PublicKeyToken=null");
			classDictionary.Add("Stars","LemonSpawn.Gamer.GalaxyComponentStars,Assembly-CSharp,Version=0.0.0.0,Culture=neutral,PublicKeyToken=null");
			classDictionary.Add("Stars2","LemonSpawn.Gamer.GalaxyComponentStars2,Assembly-CSharp,Version=0.0.0.0,Culture=neutral,PublicKeyToken=null");
		}
		

	}
}

