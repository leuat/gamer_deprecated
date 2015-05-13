using UnityEngine;
using System.Collections;

namespace LemonSpawn.Gamer {
	
	public class PanelLikelihood : GamerPanel {
		
		
		public PanelLikelihood( GameObject p) : base(p) {
		}
		
		
		
		public override void PopulateGUI() {
			
			UpdateRenderingParamsGUI();
			
		}
		
		public override void UpdateData() {
			UpdateRenderingParamsData();
		}
		
		public override void Initialize() {
			SetActive(true);
			PopulateGUI();
		}
		
		public override void Update() {
			base.Update();
			InputKeys();
		}
		
}
}
