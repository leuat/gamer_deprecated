using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace LemonSpawn.Gamer {

public class ComponentSpectrum {
	public string Name;
	public Vector3 spectrum = Vector3.zero;
	public ComponentSpectrum(Vector3 s, string n) {
		Name = n;
		spectrum = s;
	}
}

public class Spectra  {
	
	public static List<ComponentSpectrum> spectra = new List<ComponentSpectrum>();
	
	public static void PopulateSpectra() {
		
		spectra.Add( new ComponentSpectrum(new Vector3( 1, 0.6f, 0.5f), "Red"));
		spectra.Add( new ComponentSpectrum(new Vector3( 1, 0.9f, 0.55f), "Yellow"));
		spectra.Add( new ComponentSpectrum(new Vector3( 0.3f, 0.6f, 1.0f), "Blue"));
		spectra.Add( new ComponentSpectrum(new Vector3( 1.0f, 1.0f, 1.0f), "White"));
		spectra.Add( new ComponentSpectrum(new Vector3( 0.5f, 1.0f, 1.0f), "Cyan"));
		spectra.Add( new ComponentSpectrum(new Vector3( 1.0f, 0.4f, 0.8f), "Purple"));
			
	}
	
	public static ComponentSpectrum FindSpectrum(string n) {
		foreach (ComponentSpectrum c in spectra) 
			if (c.Name == n) 
				return c;
		return null;
	}

}

}