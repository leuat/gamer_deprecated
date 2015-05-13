using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.UI;
using System.Xml.Serialization;
using System.IO;

namespace LemonSpawn.Gamer {

	public class ThreadRenderState {
		public int from, to;
		public int current;
		public bool done = false;
		public List<GalaxyInstance> galaxies;
	}



	public class Rasterizer : ThreadQueue {

		public List<GalaxyInstance> galaxies = new List<GalaxyInstance>();
			// All spectra
	//		CSpectra spectra;
			// Rendering parameters. Duh.
		public RenderingParams RP = new RenderingParams();
		public RenderingParams RPold = new RenderingParams();
		public int[] renderList;
		public enum RenderState {Idle, Performing, Postprocessing, RequestCancel};
		public RenderState currentState = RenderState.Idle;
		
		public static List<Task> taskList = new List<Task>();
		public static Task currentTask = null;
		
		Material material;
		public ColorBuffer2D buffer;
		ColorBuffer2D stars;
		const int maxTimer = 1;
		int timer = maxTimer;
		ThreadRenderState[]  threadRenderStates;
		public float time;
		
		public void ClearBuffers() {
			buffer.Clear();
		}
		
/*		private bool galaxySort (CGalaxy* a,CGalaxy* b) { 
			return ((a->params.position-a->params.camera).Length()> (b->params.position-b->params.camera).Length());
		}
*/		

		private void prepareRenderList() {
			renderList = new int[RP.size*RP.size];
			for (int i=0;i<RP.size*RP.size;i++) 
				renderList[i] = i;
			renderList = Util.ShuffleArray(renderList);
				
		}
		
		public void SetupStars() {
		
		
			
			if (stars!=null && RPold.noStars == RP.noStars && RPold.starStrength == RP.starStrength && RPold.starSize == RP.starSize && RP.starSizeSpread == RPold.starSizeSpread)
				if (RP.size == stars._width)
					return;
		
			stars = new ColorBuffer2D(RP.size, RP.size);
			stars.RenderStars(RP.noStars, RP.starSize/100f, RP.starSizeSpread/100f,RP.starStrength);
			RPold.starSizeSpread = RP.starSizeSpread;
			RPold.starSize = RP.starSize;
			RPold.noStars = RP.noStars;
			RPold.starStrength = RP.starStrength;
			
		}
						
										
		public void Prepare() {
			//sort (galaxies.begin(), galaxies.end(), galaxySort);
			
			galaxies.Sort(
				delegate(GalaxyInstance i1, GalaxyInstance i2) 
				{ 
				float d = (i1.position-RP.camera.camera).magnitude - (i2.position-RP.camera.camera).magnitude;
				if (d<0) return 1;
				else return -1;
//				return (i1.param.position-RP.camera.camera).magnitude> (i2.param.position-RP.camera.camera).magnitude;
				}
			);
			if (buffer==null || buffer._width!=RP.size ) {
				buffer = new ColorBuffer2D(RP.size, RP.size);
				SetupStars();
			}
			buffer.reference.Set(0);
			
			foreach (GalaxyInstance g in galaxies)
				g.GetGalaxy().SetupSpectra();
			
			prepareRenderList();
			RP.camera.setupViewmatrix();
			
		}
		

		public Galaxy AddGalaxy(string file, Vector3 position, Vector3 orientation, float iscale, float redshift, string name) {
			Galaxy g = Galaxy.Load(file, name);//  new CGalaxy();
			if (g!=null)
				galaxies.Add ( new GalaxyInstance(g, name, position, orientation, iscale, redshift)  );
			return g;
		}
		
		public Galaxy AddGalaxy(GalaxyInstance gi) {
			galaxies.Add (gi);
			return gi.GetGalaxy();
		}
		
		public static void SaveGalaxyList(string filename, List<GalaxyInstance> gi) {
			XmlSerializer serializer = new XmlSerializer(typeof(List<GalaxyInstance>));
			TextWriter textWriter = new StreamWriter(filename);
			serializer.Serialize(textWriter, gi);
			textWriter.Close();
			
		}
		public static List<GalaxyInstance> LoadGalaxyList(string filename) {
			XmlSerializer deserializer = new XmlSerializer(typeof(List<GalaxyInstance>));
			TextReader textReader = new StreamReader(filename);
			List<GalaxyInstance> g = (List<GalaxyInstance>)deserializer.Deserialize(textReader);
			textReader.Close();
			foreach (GalaxyInstance gi in g) {
				gi.SetGalaxy(Galaxy.Load (Settings.GalaxyDirectory +  gi.galaxyName + ".xml", gi.galaxyName));
				gi.GetGalaxy().displayName = gi.galaxyName;
			}
			return g;
		}
		
		
		
		private void ThreadRenderPixels(ThreadRenderState trs) {
			threadDone = false;
			
			// Clone galaxies
			trs.current = 0;
			for (int k=trs.from;k<trs.to;k++) {
				int idx = renderList[ k ];
				Vector3 dir = setupCamera(idx);
				RasterPixel rp = renderPixel(dir, trs.galaxies);
				//rp.I.Set (1,1,0);
				buffer.SetFill (idx, rp);
				if (currentState == RenderState.RequestCancel) {
					threadDone = true;
					return;
				}
				trs.current++;
			}		
			threadDone = true;
			trs.done = true;
		}
		
		public float GetPercentage() {
			int cur = 0;
			if (threadRenderStates == null)
				return 100;
			for (int i=0; i<threadRenderStates.Length;i++) 
				cur+=threadRenderStates[i].current;
			return 100f * (float)cur / (RP.size*RP.size);
		}
		
		
		public void GenerateGalaxyList(int N, float size, string[] galaxies) {
			for (int i=0;i<N;i++) {
				Vector3 p = new Vector3();
				p.x = (float)((Util.rnd.NextDouble()-0.5)*size);
				p.y = (float)((Util.rnd.NextDouble()-0.5)*size);
				p.z = (float)((Util.rnd.NextDouble()-0.5)*size);
				Vector4 orient = new Vector3();
				orient.x = (float)((Util.rnd.NextDouble()-0.5));
				orient.y = (float)((Util.rnd.NextDouble()-0.5));
				orient.z = (float)((Util.rnd.NextDouble()-0.5));
				orient = orient.normalized;
				string n = galaxies[Util.rnd.Next()%galaxies.Length];
				AddGalaxy(Settings.GalaxyDirectory + n  + ".xml", p, orient, 0.5f + (float)Util.rnd.NextDouble(), 1, n);
					
			}			
		
		}

		public void Statistics() {
			if (gamer.likelihood != null) {
				Buffer2D curBuf = buffer.buffers[(int)buffer.colorFilter.x];
				gamer.likelihood.UpdateModel(curBuf);
				float chisq = gamer.likelihood.Chisq();
				gamer.ChiSQ.InsertCount(chisq,0);
				//Debug.Log ("normalizing: " + curBuf.getMean() + " " + gamer.likelihood.Signal.getMean());
				//curBuf.Scale (2);
				//curBuf.NormalizeFluxTo(gamer.likelihood.Signal);
				
				AssembleImage();
			}
			
		}		
		
		public void AssembleImage() {
			if (buffer==null)
				return;
				
			float gamma = RP.gamma;
			
			if (!RP.continuousPostprocessing)
				gamma = -100;
						
			buffer.CreateColorBuffer(RP.exposure, gamma, RP.color, RP.saturation, stars);
/*			if (stars!=null)
				buffer.Add (stars);
*/				
			
			buffer.Assemble();			
			material.mainTexture = buffer.image;
			
		}
		
		
		public override void PostThread() {
		}
		
		public void Render() {
			if (currentTask!=null)
				return;
			Prepare();
			InitializeRendering(gamer.material);
		}
		
		
		public void RenderSkybox() {
		
			Vector3[] planes = new Vector3[6];
			Vector3[] ups = new Vector3[6];
			planes[0] = new Vector3(0,0,-1);
			planes[1] = new Vector3(0,0,1);
			planes[2] = new Vector3(0,1,0);
			planes[3] = new Vector3(0,-1,0);
			planes[4] = new Vector3(1,0,0);
			planes[5] = new Vector3(-1,0,0);
			
			ups[0] = new Vector3(0,-1f,0);
			ups[1] = new Vector3(0,-1f,0);
			ups[2] = new Vector3(0,0,1);
			ups[3] = new Vector3(0,0,-1);
			ups[4] = new Vector3(0,-1f,0);
			ups[5] = new Vector3(0,-1f,0);
			
			string[] names = new string[6];
			names[0] ="Z-";
			names[1] ="Z+";
			names[2] ="Y+";
			names[3] ="Y-";
			names[4] ="X-";
			names[5] ="X+";
			
			GamerCamera gc = new GamerCamera();
			gc.camera = RP.camera.camera;
			
			GamerCamera oldCam = RP.camera;			
			RP.camera = gc;
			
			time = 0;
			currentState = RenderState.RequestCancel;
			//			currentTask = null;
			AbortAll(); // threads
			
			
			for (int i=0;i<6;i++) {
				RenderImageSaveTask r = new RenderImageSaveTask("skybox" + names[i] + ".png", this);
				r.camera = RP.camera.camera;
				r.target = r.camera + planes[i];
				r.up = ups[i];
				r.FOV = 90;
				taskList.Add (r);
			}
			RP.camera = oldCam;
			currentState = RenderState.Idle;
		}
		
		
		public void InitializeRendering(Material mat) {
			material = mat;
			time = 0;
			currentState = RenderState.RequestCancel;
//			currentTask = null;
			AbortAll(); // threads
			
			int N = RP.no_procs;
			int T = RP.size*RP.size/N;
			currentState = RenderState.Performing;
			if (threadRenderStates == null || threadRenderStates.Length != N)		
				threadRenderStates = new ThreadRenderState[N];
			
			
			for (int k=0;k<N;k++) {
				ThreadRenderState trs = new ThreadRenderState();
				threadRenderStates[k] = trs;
				trs.from = k*T;
				trs.to = (k+1)*T;
				
				// setup galaxy list (clone)
				List<GalaxyInstance> gals = new List<GalaxyInstance>();
				foreach (GalaxyInstance g in galaxies) 
					gals.Add (g.Clone());
				trs.galaxies = gals;		
				
				TQueue tq = new TQueue();
				tq.thread = new Thread(() => ThreadRenderPixels(trs));
				tq.gt = this;
				threadQueue.Add (tq);	
			}

		}
		
		public void UpdateRendering() {
			if (currentState == RenderState.Idle) {
				if (taskList.Count>0) {
					currentTask = taskList[0];
					currentTask.Perform();
				}
			}
			if (currentState != RenderState.Performing)
				return;
						
		
			timer--;
			if (timer<=0) {
				timer = 2*maxTimer*Mathf.Max (RP.size/512,1);
				AssembleImage();
			}
			bool done = true;
			for (int i=0;i<threadRenderStates.Length;i++)
				if (threadRenderStates[i].done==false)
					done = false;
					
			if (done) {
				AssembleImage();
				Statistics();
				if (currentTask!=null) {
					currentTask.PostTask();
					taskList.Remove(currentTask);
				}
				currentTask = null;
					
				currentState = RenderState.Idle;
			}						
			
			if (currentState == RenderState.Performing)
				time+=Time.deltaTime;
			
			
		}
		
		Vector3 setupCamera(int idx) {
			// Converts from index to 2D (i,j) coordinates
			int i = idx%(int)RP.size;
			int j = (idx-i)/(int)RP.size;
			
			Vector3 p = new Vector3(i,j,0);
			// Projects 2D screen coordinates through camera to 3D ray
			return RP.camera.coord2ray(p.x, p.y, RP.size, RP.size)*-1;
//			Debug.Log (RP.direction);
		}
		
		
		
		RasterPixel renderPixel(Vector3 dir, List<GalaxyInstance> gals) {
			Vector3 isp1, isp2;
			RasterPixel rp = new RasterPixel();
			for (int i=0;i<gals.Count;i++) {

				GalaxyInstance gi = gals[i];
												
				Galaxy g = gi.GetGalaxy();
				float t1, t2;
				bool intersects = Util.IntersectSphere(RP.camera.camera-gi.position, dir, g.param.axis, out isp1, out isp2, out t1, out t2);
				//if (intersects) 
				//	Debug.Log (intersects);				
				if (t1<0) {
					isp2 = RP.camera.camera-gi.position;// + RP.direction*
				}
				if (t1>0 && t2>0)
					intersects = false;
				if (intersects) {
					getIntensity(gi, rp, isp1, isp2);
				}
			}
			return rp;
		}
		
		/*
 * Loops through galaxy components and integrates the intensity
 *
*/
		
		/*
 * Calculates number of steps N to integrate a ray through a galaxy. Calls getIntensityAtPoint to combine intensities.
 *
 *
*/
		void getIntensity(GalaxyInstance gi, RasterPixel rp, Vector3 isp1, Vector3 isp2) {
			Vector3 origin = isp1;
			float length = (isp1-isp2).magnitude;
			Vector3 dir = (isp1-isp2).normalized;
			Galaxy g = gi.GetGalaxy();
			int N = (int)(length/RP.rayStep);
			Vector3 p = origin;
			//Debug.Log (N);
	//		RasterPixel rrp = new RasterPixel(rp);
			rp.scale = RP.rayStep;
			for (int i=0;i<N;i++) {
				//Debug.Log (p.magnitude);
				for (int j=0;j<g.getComponents().Count;j++) {
					GalaxyComponent gc = (g.getComponents())[j];
					//if (p.magnitude<0.1)
					//	rp.I = 1;//1f/Mathf.Pow (p.magnitude,2);
					if (gc.componentParams.active==1)
						gc.calculateIntensity( rp, p, gi);
					//rp.I.Set (1,1,1);
				}
				// Propagate ray
				p=p-dir*RP.rayStep;
//				Debug.Log ( "  : " + p);
				// Negative intensities should never happen. But just to be sure.
				rp.Floor(0);
			}
		}
		
		
		
		
	}

}