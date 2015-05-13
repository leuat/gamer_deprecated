using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

namespace LemonSpawn.Gamer {


	public class FitsParam {
		public string value;
		public string name;
		public FitsParam(FitsHeader fh, int pos) {
			string d = "";
			for (int i=pos;i<pos+80;i++) {
				if (fh.data[i]=='/')
					break;
				if (fh.data[i]!=' ') 
					d+=fh.data[i];
			}
			d = d.Trim();
			if (!d.Contains("=")) {
				name ="";
				value="";
				return;
			}
//			Debug.Log (d);
			string[] v = d.Split('=');
			
			name = v[0];
			value = v[1];		
//			Debug.Log (value);
		}
	}

	public class FitsHeader {
		public const int Length = 2880;
		int curLine = 0;
		public char[] data = new char[Length];
		public List<FitsParam> param = new List<FitsParam>();
		public bool isEnd = false;
		public string getParam(string name) {
			foreach (FitsParam fp in param) 
				if (fp.name.ToLower() == name.ToLower())
					return fp.value;
			return "0";		
		}
		public float getParamFloat(string name) {
			return float.Parse(getParam(name));
		}
		public void ExtractParams() {
			param.Clear();
			for (int i=0;i<2880/80;i++) {
				FitsParam fp = new FitsParam(this, i*80);
				if (fp.name != "")
					param.Add(fp);
				if (fp.name.ToLower().Contains("end"))
					isEnd = true;
				
			}
		}
		
		public FitsHeader() {
			for (int i=0;i<Length;i++)
				data[i] = ' ';
		}
		
		public void Add(string name, string value) {
			int cur = curLine*80;			
			for (int i=0;i<name.Length;i++) 
				data[i + cur] = name[i];
			cur += name.Length;
//			data[cur] ='=';
//			cur+=2;
//			value = "                    " + value;
			for (int i=0;i<value.Length;i++) 
				data[i + cur] = value[i];
			
			curLine++;
			
		}
		
	
	}

	public class Fits  {
		FitsHeader header = new FitsHeader();
		public ColorBuffer2D colorBuffer;
		public void SaveFloat(string fname, int no) {
			header = new FitsHeader();
			Buffer2D buffer = colorBuffer.buffers[no];
			
			header.Add("SIMPLE  = ","T");
			header.Add("BITPIX  = ","-" + sizeof(float)*8);
			header.Add("NAXIS   = ","2");
			header.Add("NAXIS1  = ","" + buffer._width);
			header.Add("NAXIS2  = ","" + buffer._height);
			header.Add("END","");
			
			byte[] data = new byte[buffer.buffer.Length * sizeof(float)];
			for (int i=0;i<buffer.buffer.Length;i++) {
				byte[] b = BitConverter.GetBytes(buffer.buffer[i]);
				
				
				data[i*4 + 0 ] = b[3];
				data[i*4 + 1 ] = b[2];
				data[i*4 + 2 ] = b[1];
				data[i*4 + 3 ] = b[0];
				
			}
			using (BinaryWriter writer = new BinaryWriter(File.Open(fname, FileMode.Create))) {
				writer.Write(header.data);
				writer.Write(data);
//				writer.Write(rest);
			}
		}
			public void ReadFloat(Buffer2D buffer, BinaryReader reader) {
				byte[] data = new byte[buffer.buffer.Length * sizeof(float)];
				reader.Read (data, 0, data.Length);
				byte[] b = new byte[4];
				for (int i=0;i<buffer.buffer.Length;i++) {
				// Endian
				b[ 0 ] = data[4* i +3];
				b[ 1 ] = data[4* i +2];
				b[ 2 ] = data[4* i +1];
				b[ 3 ] = data[4* i +0];
				buffer.buffer[i] = BitConverter.ToSingle(b,0);
			}
			}
		public void ReadDouble(Buffer2D buffer, BinaryReader reader) {
			byte[] data = new byte[buffer.buffer.Length * sizeof(double)];
			reader.Read (data, 0, data.Length);
			byte[] b = new byte[8];
			for (int i=0;i<buffer.buffer.Length;i++) {
				// Endian
				for (int j=0;j<8;j++)
					b[j] = data[8*i + 7-j];			
							
				buffer.buffer[i] = (float)BitConverter.ToDouble(b,0);
			}
		}
		
			public Buffer2D Load(string fname) {
				
				Buffer2D buffer;
				using (BinaryReader reader = new BinaryReader(File.Open(fname, FileMode.Open))) {
					reader.Read(header.data, 0, header.data.Length);
					header.ExtractParams();
					buffer = new Buffer2D((int)header.getParamFloat("naxis1"), (int)header.getParamFloat("naxis2"));
					int bpx = (int)header.getParamFloat("bitpix");
					if (bpx==-32) 
						ReadFloat(buffer, reader);
					else
					if (bpx==-64) 
						ReadDouble(buffer, reader);
					else 
						Debug.Log ("ERROR unsupported fits bitpix: " + bpx);
			
				}	
			buffer.Normalize();
			colorBuffer = new ColorBuffer2D(buffer,0 ,Vector3.one);
			return buffer;
				
		}
/*		public void SaveInt(string fname) {
			FitsHeader header = new FitsHeader();
			header.Add("SIMPLE  = ","T");
			header.Add("BITPIX  = ","" + 16);
			header.Add("NAXIS   = ","2");
			header.Add("NAXIS1  = ","" + buffer._width);
			header.Add("NAXIS2  = ","" + buffer._height);
			header.Add("END","");
			
			Buffer2D copy = new Buffer2D(buffer._width, buffer._height);
			copy.Copy(buffer);
			copy.NormalizeUpper();
			short[] d = new short[buffer.buffer.Length];
			for (int i=0;i<copy.buffer.Length;i++) {
			//	Debug.Log (copy.buffer[i]);
				d[i] = (short)(copy.buffer[i]*2048f);
			}
			
			byte[] data = new byte[d.Length * 2+1];
			System.Buffer.BlockCopy(d, 0, data, 1, data.Length-1);
			using (BinaryWriter writer = new BinaryWriter(File.Open(fname, FileMode.Create))) {
				writer.Write(header.data);
				writer.Write(data);
				//				writer.Write(rest);
			}	
			
		}
*/		
		
		
	}
}
