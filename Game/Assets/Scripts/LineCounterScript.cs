using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class LineCounterScript : MonoBehaviour 
{




	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			CountLines();	
		}
	}

	private void CountLines()
	{
		int nLineCount = 0;
		int nCharCount = 0;
		ProcessFiles("./", ref nLineCount, ref nCharCount);
		Debug.Log("Lines : " + nLineCount + " , Characters: " + nCharCount);
	}

	private void ProcessFiles(string _directory,ref int _nNumOfLines, ref int _numOfChars)
	{
		var vFiles = Directory.GetFiles(_directory);
		foreach(var file in vFiles)
		{
			var ext = Path.GetExtension(file);
			if(ext == ".cs")
			{
				var lines = File.ReadAllLines(file);
				foreach(string line in lines)
					_numOfChars += line.Trim().Length;
				_nNumOfLines += lines.Length;
				Debug.Log("Scanned " + Path.GetFileNameWithoutExtension(file));
			}
		}

		var dirs = Directory.GetDirectories(_directory);
		foreach(var d in dirs)
			ProcessFiles(d, ref _nNumOfLines, ref _numOfChars);
	}
}
