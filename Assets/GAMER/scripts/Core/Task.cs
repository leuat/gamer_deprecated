using UnityEngine;
using System.Collections;
using System.IO;

namespace LemonSpawn.Gamer {

public class Task {
	public Vector3 camera;
	public Vector3 target;
	public Vector3 up;
	public float FOV;
	protected Rasterizer rast;
		
	public Task(Rasterizer r) {
		rast = r;
	}
	
	public virtual void Perform() {
		rast.RP.camera.camera = camera;
		rast.RP.camera.target = target;
		rast.RP.camera.up = up;
		rast.RP.camera.perspective = FOV;
	}
	
	public virtual void PostTask() {
	
	}
	
	
}

public class RenderImageTask : Task {

	
	public RenderImageTask(Rasterizer r) : base(r) {
	}
	
	public override void Perform() {
		base.Perform();
		rast.Prepare();
		rast.InitializeRendering(gamer.material);
	}
	
	
	public override void PostTask() {
		
	}
	

}



public class RenderImageSaveTask : RenderImageTask {

	private string imageName; 
	
	public RenderImageSaveTask(string im, Rasterizer r) : base(r) {
		imageName = im;
	}
	
	public override void Perform() {
		base.Perform();
	}
	
	
	public override void PostTask() {
		base.Perform();
		File.WriteAllBytes( imageName , rast.buffer.image.EncodeToPNG());
			
	}
	

}


}
