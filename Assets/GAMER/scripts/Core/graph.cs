using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace LemonSpawn.Gamer {

public class graph {

	public List<float> X = new List<float>();
	public List<float> Y = new List<float>();
	public List<float> E = new List<float>();
	
	public int max = -1;

	public bool renderValues = true;
			
	int curX = 0;
	
	private int border = 25;
	
	public void InsertCount(float y, float e) {
		X.Add(curX);
		Y.Add (y);
		E.Add (e);
		
		if (max!=-1) {
			if (X.Count >max ) {
				X.RemoveAt(0);
				Y.RemoveAt(0);
				E.RemoveAt(0);
				for (int i=0;i<X.Count;i++)
					X[i]--;
			curX--;
			}
		}
		
	}
	
	private Texture2D background = null;
	
	public void setBackgroundColor(Color c) {
			background = new Texture2D (1, 1, TextureFormat.RGBA32, false);
			background.SetPixel (0, 0, c);
			background.Apply ();
		
	
	}
	
	public Color graphColor = new Color(0,1,0);
	
	public void Render(Rect r) {
	
		float N = X.Count;
		if (N<1)
			return;
			
		int fsize = 25;
		int dx = (int)((r.width-2*border) / N);
		float max = -1E10f;
		float min = 1E10f;
		foreach (float f in Y) {
			max = Mathf.Max (f, max);
			min = Mathf.Min (f, min);
		}
			
		float hScale = (r.height-2*border) / max;

		Vector2 p1 = new Vector2(0,0);
		Vector2 p2 = new Vector2(0,0);
		Color ec = new Color(1,0,0);	
		
		if (background != null)
			GUI.DrawTexture (r, background);
		
		float labelY = r.height + r.y - fsize;
		float labelX = r.x + border;
		int NY = (int)((r.height- border)/fsize);
		int dy = (int)((r.height-border) / NY);
		for (int i=0;i<NY;i++) {
			float v =  (i)/(max-min) + min;
			string s = v.ToString("0.000");
			//GUI.Label (new Rect(labelX, labelY - dy*i, 60,fsize), s);
		}
		
						
		for (int i=0;i<X.Count;i++) {
			p1.x = i*dx + r.x + border;
			p2.x = (i+1)*dx + r.x + border;
			p1.y = r.height - Y[i]*hScale + r.y - border;
			
			if (i!=X.Count-1) {
				
				p2.y = r.height - Y[i+1]*hScale+ r.y - border;
			
				Drawing.DrawLine(p1,p2, graphColor, 1);
			}
			if (renderValues)
				GUI.Label (new Rect(p1.x, p1.y, fsize, fsize), Y[i].ToString("0.00"));
				
		}

		for (int i=0;i<X.Count;i++) {
			if (E[i] == 0)
				continue;
			p1.x = i*dx + r.x + border;
			p2.x = (i)*dx + r.x + border;
			p1.y = r.height - Y[i]*hScale + r.y + E[i]*hScale - border;
			p2.y = r.height - Y[i]*hScale + r.y - E[i]*hScale - border;
			
			Drawing.DrawLine(p1,p2, ec, 1);
			
		}
		
	}
	
}

}
