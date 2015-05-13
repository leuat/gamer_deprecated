using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;


namespace LemonSpawn.Gamer {

public class GamerPanel {
	
	public GameObject panel;
	protected RenderingParams currentParams = null;
	protected string currentParamsFile = "";
	protected List<GalaxyInstance> galaxies = null;
	protected List<string> allGalaxies = new List<string>();	
	public GamerPanel( GameObject p ) {
		panel = p;
	}
	
	public virtual void Initialize() {
		
	}
	
	public virtual void Update() {
		int percent = ((int)gamer.rast.GetPercentage());
			
		GameObject.Find("TextRenderPercent").GetComponent<Text>().text = "" + (int)gamer.rast.GetPercentage() + " %";
		int s2 = (int)((gamer.rast.time/(percent/100f))/5)*5;
		GameObject.Find ("TextRenderTime").GetComponent<Text>().text = Util.secondsToMinutes(gamer.rast.time) + " / " + Util.secondsToMinutes(s2); 
			
	}

		private float lastPressed = 0;
		

	public void SaveParams() {
		if (currentParams!=null)
			RenderingParams.Save (currentParamsFile, currentParams);
			
	}
		void Pressed() {
//			gamer.rast.RP.rayStep = gamer.rast.RP.rayStepPreview;
			Render();
			lastPressed=0;
			
		}
		
	public void InputKeys() {
			float angle = 0.45f;
			
			if (Input.GetKey(KeyCode.LeftControl)) {
			
			if (Input.GetKey(KeyCode.DownArrow)) {
				if (Input.GetKey(KeyCode.LeftShift))
					gamer.rast.RP.camera.Zoom(angle*0.1f);
				else			
					gamer.rast.RP.camera.RotateVertical(angle);
				Pressed();
				PopulateGUI();
			}
			if (Input.GetKey(KeyCode.UpArrow)) {
				if (Input.GetKey(KeyCode.LeftShift))
					gamer.rast.RP.camera.Zoom(-angle*0.1f);
				else			
					gamer.rast.RP.camera.RotateVertical(-angle);
				Pressed();
				PopulateGUI();
			}
			if (Input.GetKey(KeyCode.LeftArrow)) {
				if (Input.GetKey(KeyCode.LeftShift))
					gamer.rast.RP.camera.Roll(angle);
				else
					gamer.rast.RP.camera.RotateHorisontal(angle);
				Pressed();
				PopulateGUI();
			}
			if (Input.GetKey(KeyCode.RightArrow)) {
				if (Input.GetKey(KeyCode.LeftShift))
					gamer.rast.RP.camera.Roll(-angle);
				else
					gamer.rast.RP.camera.RotateHorisontal(-angle);
				PopulateGUI();
				Pressed();
			}
			}
			lastPressed+=Time.deltaTime;
			if (lastPressed>0.40f) {// && gamer.rast.RP.rayStep != gamer.rast.RP.rayStepNormal) {
				gamer.rast.RP.rayStep = gamer.rast.RP.rayStepNormal;
				//	gamer.rast.ClearBuffers();
				//Render();
			}
			
		}
	
	
	public void LoadParams() {
		if (currentParamsFile!="") {
			currentParams = RenderingParams.Load(currentParamsFile);
			currentParams.rayStep = currentParams.rayStepNormal;
			gamer.rast.RP = currentParams;
		}			
	}
	
			
	public virtual void SetActive(bool b) {
		if (panel!=null)
			panel.SetActive(b);
		
		if (b==true && currentParams!=null) 
			gamer.rast.RP = currentParams;
			
		if (b==true && galaxies != null)
			gamer.rast.galaxies = galaxies;
			
		if (b==true)
			PopulateGUI();			
			
					
/*		if (b == false && currentParams != null) 
			RenderingParams.Save (currentParamsFile, currentParams);
		else {
			if (currentParamsFile!="") {
				currentParams = RenderingParams.Load(currentParamsFile);
				gamer.rast.RP = currentParams;
					
			}
		}
		*/
	}
	
	protected float getComboBoxValue(string combobox) {
		float f = 0;
		float.TryParse(getComboBoxValueString(combobox),out f);
		return f;
	}

	public string getComboBoxValueString(string combobox) {
		ComboBox combo = GameObject.Find (combobox).GetComponent<ComboBox>();
		return combo.Items[combo.SelectedIndex].Caption;
	}
	public float getComboBoxValueFromValue(string combobox) {
		ComboBox combo = GameObject.Find (combobox).GetComponent<ComboBox>();
		return combo.Items[combo.SelectedIndex].value;
	}
	
	protected void setSliderValue(string slider, float val) {
		Slider s = GameObject.Find (slider).GetComponent<Slider>();
		s.value = val;	
	}
	protected void setTextValue(string c, string val) {
		GameObject.Find (c).GetComponent<Text>().text = val;
	}

	public float getSliderValue(string slider) {
		return GameObject.Find (slider).GetComponent<Slider>().value;
	}
	
	public bool getToggle(string toggle) {
		return GameObject.Find (toggle).GetComponent<Toggle>().isOn;	
	}
	protected void setToggle(string toggle, bool v) {
		GameObject.Find (toggle).GetComponent<Toggle>().isOn = v;	
	}
		
	
	protected void setComboBoxValue(string combobox, float val) {
		ComboBox combo = GameObject.Find (combobox).GetComponent<ComboBox>();
		int idx = 0;
		for (int i=0;i<combo.Items.Length;i++) {
			if (combo.Items[i].value == val) {
				idx = i;
				break;
			}
		}
		combo.SelectedIndex = idx;
	}
	
	protected void setComboBoxValue(string combobox, string val) {
//		Debug.Log (combobox);
		ComboBox combo = GameObject.Find (combobox).GetComponent<ComboBox>();
		int idx = 0;
		for (int i=0;i<combo.Items.Length;i++) {
			if (combo.Items[i].Caption == val) {
				idx = i;
				break;
			}
		}
		combo.SelectedIndex = idx;
	}
	
	protected void setInputValue(string name, float val) {
		GameObject.Find (name).GetComponent<InputField>().text = "" + val;
	}
	protected void setInputValue(string name, string val) {
		GameObject.Find (name).GetComponent<InputField>().text = val;
	}
	public float getInputValue(string name) {
		float f = 0;
		float.TryParse (getInputValueString(name),out f);
		return f;
	}
	protected string getInputValueString(string name) {
		return GameObject.Find (name).GetComponent<InputField>().text;
	}
	
	public virtual void PopulateGUI() {
			
	}
	
	public virtual void UpdateData() {
	}
	
		public void UpdateRenderingParamsGUI() {
			setInputValue("InputFieldFov", gamer.rast.RP.camera.perspective);	
			setComboBoxValue("ComboBoxImageSize", ""+gamer.rast.RP.size);
			setInputValue("InputFieldCameraX", gamer.rast.RP.camera.camera.x);
			setInputValue("InputFieldCameraY", gamer.rast.RP.camera.camera.y);
			setInputValue("InputFieldCameraZ", gamer.rast.RP.camera.camera.z);
			setInputValue("InputFieldTargetX", gamer.rast.RP.camera.target.x);
			setInputValue("InputFieldTargetY", gamer.rast.RP.camera.target.y);
			setInputValue("InputFieldTargetZ", gamer.rast.RP.camera.target.z);
		}

		public void UpdateRenderingParamsData() {
			gamer.rast.RP.size = (int)getComboBoxValue("ComboBoxImageSize");
			gamer.rast.RP.camera.perspective = getInputValue("InputFieldFov");
			gamer.rast.RP.camera.camera.x = getInputValue("InputFieldCameraX");
			gamer.rast.RP.camera.camera.y = getInputValue("InputFieldCameraY");
			gamer.rast.RP.camera.camera.z = getInputValue("InputFieldCameraZ");
			gamer.rast.RP.camera.target.x = getInputValue("InputFieldTargetX");
			gamer.rast.RP.camera.target.y = getInputValue("InputFieldTargetY");
			gamer.rast.RP.camera.target.z = getInputValue("InputFieldTargetZ");
			
		}

	
	public virtual void LoadGalaxy(string n) {}
	
	public virtual void Render() {
		RenderingParams.Save (Settings.ParamFileSingle, gamer.rast.RP);
		gamer.rast.RP.continuousPostprocessing = getToggle("TogglePostProcess");
		gamer.rast.Render ();		
	}
		
	
	protected void PopulateLoadList(string box) {
			ComboBox cbx = GameObject.Find (box).GetComponent<ComboBox>();
			cbx.ClearItems();
			DirectoryInfo info = new DirectoryInfo(Settings.GalaxyDirectory);
			FileInfo[] fileInfo = info.GetFiles();
			List<ComboBoxItem> l = new List<ComboBoxItem>();
			allGalaxies.Clear();
			foreach (FileInfo f in fileInfo)  {
				string name = f.Name.Remove(f.Name.Length-4, 4);
				ComboBoxItem ci = new ComboBoxItem();
				ci.Caption = name;
				string n = f.Name;
				allGalaxies.Add (name);
				ci.OnSelect = delegate {
					LoadGalaxy(n);
				};
				if (n.Contains("xml")) {
					l.Add (ci);
					}
			}
	//		foreach (ComboBoxItem i in l)
	//			Debug.Log (i.Caption);
				
			cbx.AddItems(l.ToArray());
			
		}

		protected void PopulateReferenceImage(string box) {
			ComboBox cbx = GameObject.Find (box).GetComponent<ComboBox>();
			cbx.ClearItems();
			DirectoryInfo info = new DirectoryInfo(Settings.ReferenceDirectory);
			FileInfo[] fileInfo = info.GetFiles();
			List<ComboBoxItem> l = new List<ComboBoxItem>();
			allGalaxies.Clear();
			foreach (FileInfo f in fileInfo)  {
				string name = f.Name.Remove(f.Name.Length-4, 4);
				ComboBoxItem ci = new ComboBoxItem();
				ci.Caption = name;
				string n = f.Name;
				ci.OnSelect = delegate {
					gamer.LoadReferenceImage(Settings.ReferenceDirectory + n);
				};
				if (n.Contains("fits")) {
					l.Add (ci);
				}
			}
			//		foreach (ComboBoxItem i in l)
			//			Debug.Log (i.Caption);
			
			cbx.AddItems(l.ToArray());
			
		}
		
}
}

