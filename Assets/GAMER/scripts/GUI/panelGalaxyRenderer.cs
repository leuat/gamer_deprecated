using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;


namespace LemonSpawn.Gamer {

	public class PanelGalaxyRenderer : GamerPanel {

			private ComponentParams currentComponent = null;
			public GalaxyInstance currentGalaxy;
			
			private int currentComponentIndex;
		
			public PanelGalaxyRenderer( GameObject p) : base(p) {
				currentParamsFile = Settings.ParamFileSingle;
				LoadParams();
				galaxies = new List<GalaxyInstance>();			
			}				
				
			private void PopulateLoadList(string box) {
				ComboBox cbx = GameObject.Find (box).GetComponent<ComboBox>();
				cbx.ClearItems();
				DirectoryInfo info = new DirectoryInfo(Settings.GalaxyDirectory);
				FileInfo[] fileInfo = info.GetFiles();
				List<ComboBoxItem> l = new List<ComboBoxItem>();
				foreach (FileInfo f in fileInfo)  {
					string name = f.Name.Remove(f.Name.Length-4, 4);
					ComboBoxItem ci = new ComboBoxItem();
					ci.Caption = name;
					string n = f.Name;
					ci.OnSelect = delegate {
						LoadGalaxy(n);
					};
					l.Add (ci);
				}
				cbx.AddItems(l.ToArray());
				
			}
			
			
			
			
			private void PopulateSpectra() {
				ComboBox cbx = GameObject.Find ("ComboBoxSpectrum").GetComponent<ComboBox>();
				cbx.ClearItems();
				List<ComboBoxItem> l = new List<ComboBoxItem>();
				
				foreach (ComponentSpectrum s in Spectra.spectra)  {
					ComboBoxItem ci = new ComboBoxItem();
					ci.Caption = s.Name;
					ci.OnSelect = delegate { UpdateComponentParams(); };
					l.Add (ci);
				}
				cbx.AddItems(l.ToArray());
				
			}
			
			
			public void PopulateComboBox(ComboBox cbx) {
				cbx.ClearItems();
				if (currentGalaxy == null)
					return;
				//		cbx.AddItems(componentParams.ToArray
				List<ComboBoxItem> items = new List<ComboBoxItem>();
				int i=0;
				foreach (ComponentParams cp in currentGalaxy.GetGalaxy().componentParams) {
					ComboBoxItem ci = new ComboBoxItem();
					ci.Caption = cp.className + " (" + (i) + ")";
					int j=i;
					ci.OnSelect = delegate {
						currentComponentIndex = j;
						populateComponentParams();
					};
					items.Add (ci);
					i++;
					
				}
				cbx.AddItems(items.ToArray());
			}
			
			
			
			private void SetupSingleGalaxy() {
				if (gamer.rast.RP.currentGalaxy =="")
					return;
				gamer.rast.galaxies.Clear();
				gamer.rast.AddGalaxy(Settings.GalaxyDirectory + gamer.rast.RP.currentGalaxy , Vector3.zero,  new Vector3(0,1,0), 1,1, gamer.rast.RP.currentGalaxy);
				
				currentGalaxy = gamer.rast.galaxies[0];
				PopulateComboBox(GameObject.Find ("ComboBoxComponents").GetComponent<ComboBox>());
				currentComponentIndex = 0;
				populateComponentParams();
				populateGalaxyParams();
			}
			
			public override void Initialize()  {
				SetActive(true);
			
				PopulateLoadList("ComboBoxLoadGalaxy");
				PopulateSpectra();
			
				SetupSingleGalaxy();
				PopulateGUI();
				Render();
				
			}
			
			
			public override void PopulateGUI() {
				string[] split = gamer.rast.RP.currentGalaxy.Split(Settings.FileSeparator);
				GameObject.Find("CurrentGalaxyText").GetComponent<Text>().text = split[split.Length-1];
				populateGalaxyParams();
				populateComponentParams();
				UpdateRenderingParamsGUI();
			
				
			}
			
			public override void UpdateData() {
				UpdateComponentParams();
				UpdateGalaxyParams();
				UpdateRenderingParamsData();
			
		}
			
			
			
		
		
		
		public override void LoadGalaxy(string path) {
			gamer.rast.RP.currentGalaxy = path;
			SetupSingleGalaxy();
			Render();
			
			
		}
		
		public void NewComponent() {
			currentComponentIndex = currentGalaxy.GetGalaxy().AddComponent();
			populateComponentParams();
			PopulateComboBox(GameObject.Find ("ComboBoxComponents").GetComponent<ComboBox>());
			Render();
		}
		
		
		
		
		public void NewGalaxy() {
			string path = getInputValueString("InputFieldNewGalaxy") + ".xml";
			if (path.Length != 0) {
				gamer.rast.RP.currentGalaxy = path;
				// Create new galaxy
				
				 Galaxy g = new Galaxy();;
			 	 Galaxy.Save (Settings.GalaxyDirectory + gamer.rast.RP.currentGalaxy,g);
				SetupSingleGalaxy();
				Render();
				setInputValue("InputFieldNewGalaxy", "");
				PopulateLoadList("ComboBoxLoadGalaxy");
			}
			
		}
		
		public void SetSaveStatus(string s) {
			setTextValue("TextSaveFile", s);
		}
		
		
		private void populateGalaxyParams() {
			if (currentGalaxy == null)
				return;
			setComboBoxValue("ComboBoxNoArms", ""+currentGalaxy.GetGalaxy().param.noArms);
			setInputValue("InputFieldBulgeDust", currentGalaxy.GetGalaxy().param.bulgeDust);		
			setInputValue("InputFieldWindingB", currentGalaxy.GetGalaxy().param.windingB);		
			setInputValue("InputFieldWindingN", currentGalaxy.GetGalaxy().param.windingN);	
		}
		
		private void populateComponentParams() {
			if (currentGalaxy == null)
				return;
			if (currentGalaxy.GetGalaxy().componentParams.Count == 0) {
				return;
			}
			currentComponent = currentGalaxy.GetGalaxy().componentParams[currentComponentIndex];
			setComboBoxValue("ComboBoxComponentType", currentComponent.className);
			setComboBoxValue("ComboBoxIsActive", currentComponent.active);
			
			setInputValue("InputFieldStrength", currentComponent.strength);		
			setInputValue("InputFieldArm", currentComponent.arm);		
			setInputValue("InputFieldZ0", currentComponent.z0);		
			setInputValue("InputFieldR0", currentComponent.r0);		
			setInputValue("InputFieldDelta", currentComponent.delta);		
			setInputValue("InputFieldWinding", currentComponent.winding);		
			setInputValue("InputFieldScale", currentComponent.scale);		
			setInputValue("InputFieldNoiseOffset", currentComponent.noiseOffset);		
			setInputValue("InputFieldNoiseTilt", currentComponent.noiseTilt);
			setComboBoxValue("ComboBoxSpectrum", currentComponent.spectrum);
			
		}
		
		public void UpdateGalaxyParams() {
			if (currentGalaxy == null)
				return;
			
						
			currentGalaxy.GetGalaxy().param.noArms = getComboBoxValue("ComboBoxNoArms");
			currentGalaxy.GetGalaxy().param.bulgeDust = getInputValue("InputFieldBulgeDust");		
			currentGalaxy.GetGalaxy().param.windingB = getInputValue("InputFieldWindingB");		
			currentGalaxy.GetGalaxy().param.windingN = getInputValue("InputFieldWindingN");		
			
			currentGalaxy.GetGalaxy().SetupComponents();
			Render();
		}	
		
		public void UpdateComponentParams() {
			if (currentComponent == null)
				return;
			currentComponent.className = getComboBoxValueString("ComboBoxComponentType");	
			currentComponent.spectrum = getComboBoxValueString("ComboBoxSpectrum");	
			currentComponent.active = getComboBoxValueFromValue("ComboBoxIsActive");	
			currentComponent.strength = getInputValue("InputFieldStrength");
			currentComponent.arm = getInputValue("InputFieldArm");
			currentComponent.z0 = getInputValue("InputFieldZ0");
			currentComponent.r0 = getInputValue("InputFieldR0");
			currentComponent.delta = getInputValue("InputFieldDelta");
			currentComponent.winding = getInputValue("InputFieldWinding");
			currentComponent.scale = getInputValue("InputFieldScale");
			currentComponent.noiseOffset = getInputValue("InputFieldNoiseOffset");
			currentComponent.noiseTilt = getInputValue("InputFieldNoiseTilt");
			currentGalaxy.GetGalaxy().SetupComponents();
			Render();
		}
		
		
		
		public override void Update() {
			base.Update();
			InputKeys();
				
				
				
			
		}
		
	}
		
}
