using UnityEngine;
using System.Collections;

public enum  AccountType
{
	Student,
	Teacher,
	RandomPlay
}

public class LocalStorage
{
	public static AccountType accountType { get; set; }

	public static string SchoolID { get; set; }

	public static int StudentID { get; set; }

	public static int TeacherID { get; set; }

	public static string SceneID { get; set; }

	public static string Language { get; set; }

	public static bool IsRandomPlay { get; set; }

	public static string Email { get; set; }

	public static int Score { get; set; }

	public static bool IsSwitchBG = true;

	public static bool IsTest { get; set; }

	public static int SelectClassID { get; set; }

	public static string SelectClassName { get; set; }

	public static int PaperID { get; set; }
}
