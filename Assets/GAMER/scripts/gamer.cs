using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

namespace LemonSpawn.Gamer {


public class gamer : MonoBehaviour {

	// Use this for initialization
	
	
	public static Material material;
	public static Material altMaterial;
	public static Rasterizer rast = new Rasterizer();
	
	
	private PanelSettings pnlSettings;
	private PanelGalaxyRenderer pnlGalaxyRenderer;	
	private PanelScene pnlScene;
	private GamerPanel currentPanel;
	private GameObject p1, p2;
	public static Fits altImage;
	private bool toggle = false;

	public void AlternateImage() {
		toggle = !toggle;
		p1.SetActive(false);
		p2.SetActive(false);
		if (toggle)
			p1.SetActive(true);
		else
			p2.SetActive(true);
			
		}


	public static void LoadReferenceImage(string fname) {
		altImage = new Fits();
		altImage.Load(fname);
		altImage.colorBuffer.Assemble();
		altMaterial.mainTexture = altImage.colorBuffer.image;
			
	}
	
	void SetColorMode(Vector3 v) {
		rast.buffer.colorFilter = v;
		rast.AssembleImage();
	}
												
	void Start () {
		// Test galaxy
		material = (Material)Resources.Load ("matGalaxy");
		altMaterial = (Material)Resources.Load ("matAltGalaxy");
		p1 = GameObject.Find ("PlaneGalaxy");
		p2 = GameObject.Find ("PlaneAltGalaxy");
		Settings.SetupGamer();

		pnlGalaxyRenderer = new PanelGalaxyRenderer( GameObject.Find ("pnlGalaxyRender"));
		pnlSettings = new PanelSettings( GameObject.Find ("pnlSettings"));
		pnlScene = new PanelScene( GameObject.Find ("pnlScene"));
		
		HideAllPanels();
		clickPnlGalaxyRenderer();
			
		//rast.RP = RenderingParams.Load (Settings.ParamFile);

			
		pnlGalaxyRenderer.Initialize();
		pnlSettings.Initialize();
		pnlScene.Initialize();
			
		AlternateImage();

	}
	
	public void SaveGalaxyImage() {
		string f = Settings.GetNextOutputFile();
		File.WriteAllBytes( f , gamer.rast.buffer.image.EncodeToPNG());
		pnlGalaxyRenderer.SetSaveStatus("Last image saved to: " + f);
		Fits fit = new Fits();
		fit.colorBuffer = gamer.rast.buffer;
		fit.SaveFloat ("testR.fits", 0);
		fit.SaveFloat ("testG.fits", 1);
		fit.SaveFloat ("testB.fits", 2);
}
		
		public void UpdatePostProcessingParams() {
		pnlSettings.UpdatePostProcessing();
	}
	
	public void NewComponent() {
		pnlGalaxyRenderer.NewComponent();
	}
	public void NewGalaxy() {
		pnlGalaxyRenderer.NewGalaxy();
	}
	public void NewScene() {
		pnlScene.NewScene();
	}
	
		
	public void clickPnlSettings() {
		HideAllPanels();
		pnlSettings.SetActive(true);	
		currentPanel = pnlSettings;
	}
	public void clickPnlScene() {
		HideAllPanels();
		pnlScene.SetActive(true);	
		currentPanel = pnlScene;
	}
	public void clickPnlFiles() {
		HideAllPanels();
//		pnlFiles.SetActive(true);	
//		currentPanel = pnlFiles;
	}
	public void clickPnlGalaxyRenderer() {
		HideAllPanels();
		pnlGalaxyRenderer.SetActive(true);	
		currentPanel = pnlGalaxyRenderer;
		}
	
	public void Render() {
		currentPanel.Render();
	}
	
	
	void HideAllPanels() {
		pnlSettings.SetActive(false);
		pnlGalaxyRenderer.SetActive(false);
		pnlScene.SetActive(false);
//		pnlFiles.SetActive(false);
	}
	void ShowAllPanels() {
		pnlSettings.SetActive(true);
		pnlGalaxyRenderer.SetActive(true);
//		pnlFiles.SetActive(true);
	}
	
	public void PopulateGUI() {
		if (currentPanel!=null)
			currentPanel.PopulateGUI();
	}
	
	public void PopulateData() {
		if (currentPanel!=null)
			currentPanel.UpdateData();
	}			
		
	public void SaveImage() {
		
	}	
		
	public void ClickSceneGalaxyButton() {
		pnlScene.ClickGalaxyButton();
	}	
		
	public void RenderSkybox() {
		rast.RenderSkybox();
		
	}
		
				
	public void DeleteGalaxyFromList() {
		pnlScene.DeleteGalaxy();
	}	
	
	public void ClearList() {
		pnlScene.ClearList();
	}
	
	public void GenerateList() {
		pnlScene.GenerateList();
	}
		
	public void SaveCurrentGalaxy() {	
		if (pnlGalaxyRenderer.currentGalaxy != null)
			Galaxy.Save ( Settings.GalaxyDirectory +gamer.rast.RP.currentGalaxy,pnlGalaxyRenderer.currentGalaxy.GetGalaxy());
	}
		
	// Update is called once per frame
	void Update () {
		Rasterizer.MaintainThreadQueue();
		rast.UpdateRendering();
		
		if (currentPanel!=null)
			currentPanel.Update();
		
		if (Input.GetKeyUp(KeyCode.Space))
			AlternateImage();
			
		if (Input.GetKeyUp (KeyCode.Alpha1))
			SetColorMode(new Vector3(0,1,2));
		if (Input.GetKeyUp (KeyCode.Alpha2))
			SetColorMode(new Vector3(0,0,0));
		if (Input.GetKeyUp (KeyCode.Alpha3))
			SetColorMode(new Vector3(1,1,1));
		if (Input.GetKeyUp (KeyCode.Alpha4))
			SetColorMode(new Vector3(2,2,2));
			
		if (Input.GetKey (KeyCode.Escape)) {
			//RenderingParams.Save (Settings.ParamFile, rast.RP);
			pnlGalaxyRenderer.SaveParams();
			pnlScene.SaveParams();
				
			Application.Quit();
		}
			
	}
}

}