using UnityEngine;
using System.Collections;


namespace LemonSpawn.Gamer {


	public class ColorBuffer2D {
		public Buffer2D[] buffers;
		public Buffer2D reference;
		public Color[] colBuffer;
		public int _width, _height;
		public Texture2D image;
		public Vector3 colorFilter = new Vector3(0,1,2);
		
		
		public ColorBuffer2D(int w, int h) {
			_width = w;
			_height = h;
			buffers = new Buffer2D[3];
			for (int i=0;i<buffers.Length;i++)
				buffers[i] = new Buffer2D(w,h);
			
			reference = new Buffer2D(w,h);
			colBuffer = new Color[w*h];
			for (int i=0;i<colBuffer.Length;i++) {
				colBuffer[i] = new Color(0,0,0,1);
			}
			image = new Texture2D(w,h);
			image.filterMode = FilterMode.Point;
			
		}
		
		public void Clear() {
			if (reference!=null) reference.Set (0);
			for (int i=0;i<buffers.Length;i++)
				buffers[i].Set (0);
			
		}
		
		public void Add(ColorBuffer2D o) {
			for (int i=0;i<buffers.Length;i++) 
				buffers[i].Add (o.buffers[i]);
		}
		
		
		public void RenderStars(int noStars, float size, float spread, float s) {
			for (int i=0;i<buffers.Length;i++)
				buffers[i].Set (0);
			
			Vector3 c = Vector3.one;
			
			int _size = (int)(size*_width);
			int _spread = (int)(spread*_width);
			
			for (int i=0;i<noStars;i++) {
				int x = Util.rnd.Next()%_width;
				int y = Util.rnd.Next()%_height;
				
				c.x = Mathf.Min ((float)Util.rnd.NextDouble() + 0.8f ,1);
				c.y = Mathf.Min ((float)Util.rnd.NextDouble() + 0.8f ,1);
				c.z = Mathf.Min ((float)Util.rnd.NextDouble() + 0.8f ,1);
				float sz = Mathf.Max (Util.NextGaussian(size, spread), size/3);
				int asz = (int)(sz*_width);
				float ss = Mathf.Max (Mathf.Abs (Util.NextGaussian(s*0.25f, 15*sz)),0.1f);
				
				buffers[0].RenderGaussian(x,y, asz, ss*c.x);
				buffers[1].RenderGaussian(x,y, asz, ss*c.y);
				buffers[2].RenderGaussian(x,y, asz, ss*c.z);
			
				
			}			
		}
		
		public void Assemble() {
			image.SetPixels(colBuffer);
			image.Apply();
		}
		
		
		public void Set(int i, RasterPixel rp) {
			buffers[0].Set (i, rp.I.x);
			buffers[1].Set (i, rp.I.y);
			buffers[2].Set (i, rp.I.z);
		}
		public void SetFill(int i, RasterPixel rp) {
			// block size 8
			int N = Mathf.Max (_width/64,1);
			for (int x=0;x<N;x++)
				for (int y=0;y<N;y++) {
					int idx = i + x + y*_width;
					if (idx<_width*_height && reference.buffer[idx]==0) {
						buffers[0].Set (idx, rp.I.x);
						buffers[1].Set (idx, rp.I.y);
						buffers[2].Set (idx, rp.I.z);
					}
				}
			reference.Set (i,1);
			
		}
		private Buffer2D[] nBuf;
		
		
		public void CreateColorBuffer(float exposure, float gamma, Vector3 color, float saturation, ColorBuffer2D stars) {
			Color col = new Color();
//			float m = 0;
/*			m = Mathf.Max (buffers[0].getMax(), m);
			m = Mathf.Max (buffers[1].getMax(), m);
			m = Mathf.Max (buffers[2].getMax(), m);*/
			// clone
			if (nBuf==null) {
				nBuf = new Buffer2D[3];
				nBuf[0] = new Buffer2D(buffers[0]);
				nBuf[1] = new Buffer2D(buffers[1]);
				nBuf[2] = new Buffer2D(buffers[2]);
			}
			else {
				for (int i=0;i<buffers.Length;i++)
					nBuf[i].Copy(buffers[i]);
			}
			
			
			if (gamma!=-100) 
			for (int i=0;i<buffers.Length;i++) {
				nBuf[i].Scale((1f/exposure)*color[i]);
				
				if (gamma!=1)
					nBuf[i].Gamma(gamma);
				
			}
			Vector3 tmp = new Vector3();
			for (int i=0;i<buffers[0].buffer.Length;i++) {
			
			
				col.r = nBuf[(int)colorFilter.x].buffer[i];
				col.g = nBuf[(int)colorFilter.y].buffer[i];
				col.b = nBuf[(int)colorFilter.z].buffer[i];
				if (stars!=null && stars._width == _width) {
					col.r+=stars.buffers[0].buffer[i];
					col.g+=stars.buffers[1].buffer[i];
					col.b+=stars.buffers[2].buffer[i];
				}
				col.a = 1;//Mathf.Max(colBuffer[i].r,colBuffer[i].g);
				
				float c = (col.r + col.g + col.b)/3f;
				tmp.Set (c-col.r, c-col.g, c-col.b);
				col.r = c - saturation*tmp.x;
				col.g = c - saturation*tmp.y;
				col.b = c - saturation*tmp.z;
				
				colBuffer[i] = col;
			}
			
		}
		
		private float prevScale;
		int count=0, maxcount = 20;
		
		public void CreateColorBufferOld() {
			Color col = new Color();
			if (nBuf==null) {
				nBuf = new Buffer2D[3];
				nBuf[0] = new Buffer2D(buffers[0]);
				nBuf[1] = new Buffer2D(buffers[1]);
				nBuf[2] = new Buffer2D(buffers[2]);
			}
			else {
				for (int i=0;i<buffers.Length;i++)
					nBuf[i].Copy(buffers[i]);
			}
			
			if (count--<0) {
				count = maxcount;
			
				float m0 = buffers[0].getMean();
				float s0 = buffers[0].getSigma(m0);
				float m1 = buffers[1].getMean();
				float s1 = buffers[1].getSigma(m1);
				float m2 = buffers[2].getMean();
				float s2 = buffers[2].getSigma(m1);

				float m = (m0+m1+m2)/3f;
				float s = (s0+s1+s2)/3f;
//			Debug.Log (m + " , " + s);						
			// clone
				prevScale = 1f/(m + 8*s);
			}
			for (int i=0;i<buffers.Length;i++) {
				nBuf[i].Scale(prevScale);
				
			}
			
			for (int i=0;i<buffers[0].buffer.Length;i++) {
				col.r = nBuf[0].buffer[i];
				col.g = nBuf[1].buffer[i];
				col.b = nBuf[2].buffer[i];
				col.a = 1;//Mathf.Max(colBuffer[i].r,colBuffer[i].g);
				colBuffer[i] = col;
			}
			
		}
		
		
	
	}


	public class Buffer2D {
	
		public float[] buffer;
		public Complex[] fftBuffer;
		public int _width, _height;
		
		public Buffer2D(int w, int h) {
			_width = w;
			_height = h;
			buffer = new float[w*h];
			for (int i=0;i<buffer.Length;i++) {
				buffer[i] = 0;
			}
		}
		
		public void Copy(Buffer2D o) {
			for (int i=0;i<buffer.Length;i++)
				buffer[i] = o.buffer[i];
		}
		public void Add(Buffer2D o) {
			for (int i=0;i<buffer.Length;i++)
				buffer[i] += o.buffer[i];
		}
		public void Sub(Buffer2D o) {
			for (int i=0;i<buffer.Length;i++)
				buffer[i] -= o.buffer[i];
		}
		public void Mul(Buffer2D o) {
			for (int i=0;i<buffer.Length;i++)
				buffer[i] *= o.buffer[i];
		}
		
		public Buffer2D( Buffer2D o ) {
			_width = o._width;
			_height = o._height;
			buffer = new float[_width*_height];
			for (int i=0;i<buffer.Length;i++) {
				buffer[i] = o.buffer[i];
			}
		}
		
		public void Gamma(float g) {
			for (int i=0;i<buffer.Length;i++)
				buffer[i] = Mathf.Pow (buffer[i], g);
		}
		
		public float Get(int i) {
			return buffer[i];
		}
		public float Get(int i,int j) {
			return buffer[i + _width*j];
		}
		
		public void Set(int i, float v) {
			buffer[i] = v;
		}
		public void Set(int i, int j, float v) {
			buffer[i + j*_width] = v;
		}
		
		public void Set(float v) {
			for (int i=0;i<buffer.Length;i++)
				buffer[i] = v;
		}
		
		public float getMax() {
			float m = -1E10f;
			for (int i=0;i<buffer.Length;i++)
				m = Mathf.Max (m, buffer[i]);
			return m;
		}
		public float getMin() {
			float m = 1E10f;
			for (int i=0;i<buffer.Length;i++)
				m = Mathf.Min (m, buffer[i]);
			return m;
		}
		public void Scale(float s) {
			for (int i=0;i<buffer.Length;i++)
				buffer[i]*=s;
		}
		
		public void Add(float s) {
			for (int i=0;i<buffer.Length;i++)
				buffer[i]+=s;
		}
		public void NormalizeUpper() {
			float m = getMax();
			Scale(1f/m);
		}
		public void Normalize() {
			float max = getMax();
			float min = getMin();
			Add(-min);
			Scale(1f/max);
		}
		
		public void RenderGaussian(int i, int j, int w, float s) {
			for (int x=-w/2; x<w/2; x++) 
				for (int y = -w/2; y<w/2;y++) {
					int xx = i + x;
					int yy = j + y;
					float dx =x/(float)w;
					float dy = y/(float)w;
					if (xx>=0 && xx<_width && yy>=0 && yy<_height) {
						float d = (dx*dx + dy*dy);
						float ss = 0.01f;
						float v = Mathf.Exp (-d/ss);
						buffer[ xx + yy*_width] += v*s;
				//	buffer[ xx + yy*_width] = Mathf.Max (buffer[xx+yy*_width], v*s);
				}
					
				}
		
		}


		public float getMean() {
			float mu = 0;
			for (int i=0;i<buffer.Length;i++)
				mu+=buffer[i];
			return mu/(_width*_height);
		}

		public float getSigma(float mu) {
			float ss = 0;
			for (int i=0;i<buffer.Length;i++)
				ss+=Mathf.Pow((buffer[i]-mu),2);
			ss/=(_width*_height);
			return Mathf.Sqrt (ss);
		}
		
		public void NormalizeSigma() {
			float mu = getMean();
			float sigma = getSigma(mu);
			Scale(1f/(mu+sigma));
		}
		
		
		
	}
}