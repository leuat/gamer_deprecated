using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace LemonSpawn.Gamer {
	
	public class PanelScene : GamerPanel {
		
		
		protected GalaxyInstance GI = null;//new GalaxyInstance();
		
		public PanelScene( GameObject p) : base(p) {
			currentParamsFile = Settings.ParamFileScene;
			LoadParams();
			galaxies = new List<GalaxyInstance>();
			if (gamer.rast.RP.currentScene != "" ) { 
				LoadScene(gamer.rast.RP.currentScene);
			}
			PopulateSceneList("ComboBoxLoadScene");
			
		}
		
		
		public void PopulateListbox() {
			ListBox lb = GameObject.Find ("lstGalaxies").GetComponent<ListBox>();
			List<string> galaxyList = new List<string>();
			List<int> galaxyCnt = new List<int>();
			for (int i=0;i<galaxies.Count;i++) {
				galaxyCnt.Add (i);
				galaxyList.Add ( galaxies[i].GetGalaxy().displayName.PadRight(15) + "    |     " + galaxies[i].position +  "     |     " + galaxies[i].orientation); 
//				galaxyList.Add ( galaxies[i].galaxy.displayName.PadRight(15) ); 
			}
			lb.alwaysOpen = false;
			lb.SetOptions(galaxyCnt.ToArray(), galaxyList.ToArray());
			lb.alwaysOpen = true;
			lb.onValueChanged = delegate {
				int i = lb.GetSelectedIndex();
				if (i>=0 && i<galaxies.Count) {
					GI = galaxies[i];
					PopulateGUI();
				}
			};
			
			
//			lb.OpenListBox();
		}
		
		public void NewScene() {
			gamer.rast.RP.currentScene = getInputValueString("InputFieldNewScene") + ".xml";
			SaveScene();
			PopulateSceneList("ComboBoxLoadScene");
			
			
		}
		
		public void ClickGalaxyButton() {
			bool isNew = false;
			if (GI == null) {
				isNew = true;
				GI = new GalaxyInstance();	
			}
			GI.position.x = getInputValue("InputFieldPosX");
			GI.position.y = getInputValue("InputFieldPosY");
			GI.position.z = getInputValue("InputFieldPosZ");
			GI.orientation.x = getInputValue("InputFieldOrientX");
			GI.orientation.y = getInputValue("InputFieldOrientY");
			GI.orientation.z = getInputValue("InputFieldOrientZ");
			Debug.Log (GI.orientation);
			string n = getComboBoxValueString("ComboBoxLoadGalaxyScene");
			GI.SetGalaxy(Galaxy.Load( Settings.GalaxyDirectory + n + ".xml", n));
			
			 if (isNew) {
			 	gamer.rast.AddGalaxy(GI);
			  } 
			 GI = null;
			 PopulateGUI();                       
		}
		
		public void GenerateList() {
			gamer.rast.GenerateGalaxyList((int)getInputValue("InputFieldNoGalaxies"), getInputValue("InputFieldSpread"), allGalaxies.ToArray());
			PopulateListbox();
			                        
		}
		
		
		public void SaveScene() {
			if (gamer.rast.RP.currentScene!="")
				Rasterizer.SaveGalaxyList (Settings.SceneDirectory + gamer.rast.RP.currentScene, gamer.rast.galaxies);
			SaveParams();
			
		}
		
		public override void PopulateGUI() {
			
			PopulateListbox();
			if (GI!=null) {
				setComboBoxValue("ComboBoxLoadGalaxyScene", GI.GetGalaxy().displayName);
				setInputValue("InputFieldPosX", GI.position.x);
				setInputValue("InputFieldPosY", GI.position.y);
				setInputValue("InputFieldPosZ", GI.position.z);
				setInputValue("InputFieldOrientX", GI.orientation.x);
				setInputValue("InputFieldOrientY", GI.orientation.y);
				setInputValue("InputFieldOrientZ", GI.orientation.z);
			}
			UpdateRenderingParamsGUI();
			
			SaveScene();
		
/*			setComboBoxValue("ComboBoxNoProcs",""+gamer.rast.RP.no_procs);
			setComboBoxValue("ComboBoxRaytracingDetail",gamer.rast.RP.rayStepNormal);
			setComboBoxValue("ComboBoxNoiseDetail",gamer.rast.RP.noiseDetail);
			setSliderValue("SliderExposure", gamer.rast.RP.exposure);
			setSliderValue("SliderGamma", gamer.rast.RP.gamma);
			setInputValue("InputFieldNoStars", gamer.rast.RP.noStars);
			setInputValue("InputFieldStarSize", gamer.rast.RP.starSize);
			setInputValue("InputFieldStarSizeSpread", gamer.rast.RP.starSizeSpread);
			setInputValue("InputFieldStarStrength", gamer.rast.RP.starStrength);
			
			setSliderValue("SliderR", gamer.rast.RP.color.x);
			setSliderValue("SliderG", gamer.rast.RP.color.y);
			setSliderValue("SliderB", gamer.rast.RP.color.z);
			
*/			
		}
		
		public override void UpdateData() {
			UpdateRenderingParamsData();
			
/*			gamer.rast.RP.no_procs = (int)getComboBoxValue("ComboBoxNoProcs");
			gamer.rast.RP.noiseDetail = getComboBoxValueFromValue("ComboBoxNoiseDetail");
			gamer.rast.RP.rayStepNormal = getComboBoxValueFromValue("ComboBoxRaytracingDetail");
			gamer.rast.RP.noStars = (int)getInputValue("InputFieldNoStars");
			gamer.rast.RP.starSize = getInputValue("InputFieldStarSize");
			gamer.rast.RP.starSizeSpread = getInputValue("InputFieldStarSizeSpread");
			gamer.rast.RP.starStrength = getInputValue("InputFieldStarStrength");
*/			
		}
		
		public override void Initialize() {
			SetActive(true);
			PopulateLoadList("ComboBoxLoadGalaxyScene");
			PopulateGUI();
		}
		
/*		public void UpdatePostProcessing() {
			gamer.rast.RP.exposure = getSliderValue("SliderExposure");
			gamer.rast.RP.gamma = getSliderValue("SliderGamma");
			gamer.rast.RP.color.x = getSliderValue("SliderR");
			gamer.rast.RP.color.y = getSliderValue("SliderG");
			gamer.rast.RP.color.z = getSliderValue("SliderB");
			gamer.rast.SetupStars();
			
			gamer.rast.AssembleImage();
		}
*/		
		public void LoadScene(string f) {
			gamer.rast.RP.currentScene = f;
			gamer.rast.galaxies = Rasterizer.LoadGalaxyList(Settings.SceneDirectory + f);
			galaxies = gamer.rast.galaxies;
			PopulateListbox();
		}

		public override void Update() {
			base.Update();
			InputKeys();
		}

		public void DeleteGalaxy() {
			if (GI==null)
				return;
			galaxies.Remove(GI);
			PopulateListbox();
		}
	
		public void ClearList() {
			galaxies.Clear();
			PopulateListbox ();
		}

	
	
		protected void PopulateSceneList(string box) {
			ComboBox cbx = GameObject.Find (box).GetComponent<ComboBox>();
			cbx.ClearItems();
			DirectoryInfo info = new DirectoryInfo(Settings.SceneDirectory);
			FileInfo[] fileInfo = info.GetFiles();
			List<ComboBoxItem> l = new List<ComboBoxItem>();
			foreach (FileInfo f in fileInfo)  {
				string name = f.Name.Remove(f.Name.Length-4, 4);
				ComboBoxItem ci = new ComboBoxItem();
				ci.Caption = name;
				string n = f.Name;
				ci.OnSelect = delegate {
					SaveScene();
					LoadScene(n);
				};
				l.Add (ci);
			}
			cbx.AddItems(l.ToArray());
		}
		
	}
	
}
