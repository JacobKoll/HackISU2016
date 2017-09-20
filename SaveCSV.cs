using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class SaveCSV : MonoBehaviour{

	public string filename = "_DATA.csv";
	public string[] headers = {"Object", "Score", "Time"};
	List<string[]> rowData = new List<string[]>();

	static SaveCSV instance;

	void Awake(){
		instance = this;
		clearAndSetHeaders(headers);
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.D))
			Save();
	}

	public static void ClearAndSetHeaders(string[] newHeaders){
		instance.clearAndSetHeaders(newHeaders);
	}
	public void clearAndSetHeaders(string[] newHeaders){
		if(newHeaders.Length != headers.Length){
			headers = new string[newHeaders.Length];
		}
		for(int i = 0; i < headers.Length; i++){
			headers[i] = newHeaders[i];
		}
		rowData = new List<string[]>();
		addRow(headers);
	}

	public static void AddEntry(string value, string header){
		instance.addEntry(value, header);
	}
	public void addEntry(string value, string header){
		int column = -1;
		for(int i = 0; i < headers.Length; i++){
			if(header.Equals(headers[i])){
				column = i;
				break;
			}
		}
		if(column == -1){
			UnityEngine.Debug.LogError("No header " + header);
			return;
		}
		for(int r = 0; r < rowData.Count; r++){
			if(String.IsNullOrEmpty(rowData[r][column])){
				rowData[r][column] = value;
				return;
			}
		}
		// else
		string[] row = new string[headers.Length];
		row[column] = value;
		rowData.Add(row);
	}

	public static void AddRow(string[] row){
		instance.addRow(row);
	}
	public void addRow(string[] row){
		if(row.Length == headers.Length)
			rowData.Add(row);
		else
			UnityEngine.Debug.LogError("Row column mismatch " + row.ToString() + " -> " + headers.ToString());
	}

	public static void Save(){
		instance.save();
	}
	public void save(){

		string[][] output = new string[rowData.Count][];

		for(int i = 0; i < output.Length; i++){
			output[i] = rowData[i];
		}

		int length = output.GetLength(0);
		string delimiter = ",";

		StringBuilder sb = new StringBuilder();

		for(int index = 0; index < length; index++)
			sb.AppendLine(string.Join(delimiter, output[index]));


		string filePath = getPath();

		StreamWriter outStream = System.IO.File.CreateText(filePath);
		outStream.WriteLine(sb);
		outStream.Close();
	}

	// Following method is used to retrive the relative path as device platform
	private string getPath(){
		#if UNITY_EDITOR
		return Application.dataPath + "/" + filename;
		#elif UNITY_ANDROID
		return Application.persistentDataPath+"Saved_data.csv";
		#elif UNITY_IPHONE
		return Application.persistentDataPath+"/"+"Saved_data.csv";
		#else
		return Application.dataPath +"/"+"Saved_data.csv";
		#endif
	}
}