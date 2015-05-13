using System.Collections.Generic;
using UnityEngine;
using System.Collections;


namespace LemonSpawn.Gamer {


public delegate void GetValue();
public delegate void SetValue();
	
public class Parameter {
	public string name;
	public float value;
	public float from;
	public float to;
	public float chisq;

	public virtual void Get() {}
	public virtual void Set() {} 
									
		
}

public class ParameterX: Parameter {
	public override void Get() {
		value = gamer.rast.RP.camera.camera.x;
	}
	public override void Set() {
		gamer.rast.RP.camera.camera.x = value;
		gamer.rast.RP.camera.target.x = value;
	}
}
public class ParameterY: Parameter {
	public override void Get() {
		value = gamer.rast.RP.camera.camera.y;
	}
	public override void Set() {
		gamer.rast.RP.camera.camera.y = value;
		gamer.rast.RP.camera.target.y = value;
	}
}
public class ParameterTheta: Parameter {
	public override void Get() {
		value = gamer.rast.RP.camera.camera.x;
	}
	public override void Set() {
		gamer.rast.RP.camera.camera.x = value;
		gamer.rast.RP.camera.target.x = value;
	}
}


public class Parameters {

	List<Parameter> parameters = new List<Parameter>();

	public void Initialize() {
	}

};


public class Likelihood {

	public Buffer2D Residual, Signal, Model, OrgModel; 


	


	public Likelihood(Buffer2D S) {
		Signal = S;
	}
	public void UpdateModel(Buffer2D m) {
		OrgModel = m;
		Initialize();
	}
	
	private void Initialize() {
		if (OrgModel == null)
			return;
		if (Model== null || (Model._width!=Signal._width || Model._height != Signal._height) ) {
			Model = new Buffer2D(Signal._width, Signal._height);
		}
		Model.StretchFrom(OrgModel);
		Model.NormalizeFluxTo(Signal);	
		if (Residual== null || (Residual._width!=Signal._width || Residual._height != Signal._height) ) {
			Residual = new Buffer2D(Signal._width, Signal._height);
		}
	
	}
	
	public float Chisq() {
		float chisq = 0;
		for (int i=0;i<Signal.buffer.Length;i++) {
			float d = Signal.buffer[i] - Model.buffer[i];
			Residual.buffer[i] = d;
			chisq += (d*d);///Signal.buffer[i];
		}
		//Residual.Normalize();
		Residual.Add (Residual.getMin()*-1);
		ColorBuffer2D buf = new ColorBuffer2D(Residual,0 ,Vector3.one);
		buf.Assemble();
		gamer.matResidual.mainTexture = buf.image;
			
		return chisq;// /Signal.buffer.Length;
	} 


}

}