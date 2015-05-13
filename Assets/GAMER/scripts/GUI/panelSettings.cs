using UnityEngine;
using System.Collections;


namespace LemonSpawn.Gamer {

	public class PanelSettings : GamerPanel {

		public PanelSettings( GameObject p) : base(p) {
			
		}
		
		
		public override void PopulateGUI() {
			setComboBoxValue("ComboBoxNoProcs",""+gamer.rast.RP.no_procs);
			setComboBoxValue("ComboBoxRaytracingDetail",gamer.rast.RP.rayStepNormal);
			setComboBoxValue("ComboBoxNoiseDetail",gamer.rast.RP.noiseDetail);
			setSliderValue("SliderExposure", gamer.rast.RP.exposure);
			setSliderValue("SliderGamma", gamer.rast.RP.gamma);
			setInputValue("InputFieldNoStars", gamer.rast.RP.noStars);
			setInputValue("InputFieldStarSize", gamer.rast.RP.starSize);
			setInputValue("InputFieldStarSizeSpread", gamer.rast.RP.starSizeSpread);
			setInputValue("InputFieldStarStrength", gamer.rast.RP.starStrength);
			setSliderValue("SliderSaturation", gamer.rast.RP.saturation);
			setSliderValue("SliderR", gamer.rast.RP.color.x);
			setSliderValue("SliderG", gamer.rast.RP.color.y);
			setSliderValue("SliderB", gamer.rast.RP.color.z);
			
			
		}

		public override void UpdateData() {
			gamer.rast.RP.no_procs = (int)getComboBoxValue("ComboBoxNoProcs");
			gamer.rast.RP.noiseDetail = getComboBoxValueFromValue("ComboBoxNoiseDetail");
			gamer.rast.RP.rayStepNormal = getComboBoxValueFromValue("ComboBoxRaytracingDetail");
			gamer.rast.RP.noStars = (int)getInputValue("InputFieldNoStars");
			gamer.rast.RP.starSize = getInputValue("InputFieldStarSize");
			gamer.rast.RP.starSizeSpread = getInputValue("InputFieldStarSizeSpread");
			gamer.rast.RP.starStrength = getInputValue("InputFieldStarStrength");
			
			UpdatePostProcessing();
		}
		
		public override void Initialize() {
			SetActive(true);
			PopulateGUI();
			PopulateReferenceImage("ComboBoxReference");
		}
		
		public override void Render() {
			
		}	
		
		
		public void UpdatePostProcessing() {
			gamer.rast.RP.exposure = getSliderValue("SliderExposure");
			gamer.rast.RP.gamma = getSliderValue("SliderGamma");
			gamer.rast.RP.color.x = getSliderValue("SliderR");
			gamer.rast.RP.color.y = getSliderValue("SliderG");
			gamer.rast.RP.color.z = getSliderValue("SliderB");
			gamer.rast.RP.saturation = getSliderValue("SliderSaturation");
			
			gamer.rast.SetupStars();
			
			gamer.rast.AssembleImage();
		}
		public override void Update() {
			base.Update();
		}
		
	}

}
