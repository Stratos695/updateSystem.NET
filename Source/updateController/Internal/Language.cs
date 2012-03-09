﻿/*
 * updateSystem.NET
 * Copyright (c) 2008-2012 Maximilian Krauss <http://kraussz.com> eMail: max@kraussz.com
 *
 * This library is licened under The Code Project Open License (CPOL) 1.02
 * which can be found online at <http://www.codeproject.com/info/cpol10.aspx>
 * 
 * THIS WORK IS PROVIDED "AS IS", "WHERE IS" AND "AS AVAILABLE", WITHOUT
 * ANY EXPRESS OR IMPLIED WARRANTIES OR CONDITIONS OR GUARANTEES. YOU,
 * THE USER, ASSUME ALL RISK IN ITS USE, INCLUDING COPYRIGHT INFRINGEMENT,
 * PATENT INFRINGEMENT, SUITABILITY, ETC. AUTHOR EXPRESSLY DISCLAIMS ALL
 * EXPRESS, IMPLIED OR STATUTORY WARRANTIES OR CONDITIONS, INCLUDING
 * WITHOUT LIMITATION, WARRANTIES OR CONDITIONS OF MERCHANTABILITY,
 * MERCHANTABLE QUALITY OR FITNESS FOR A PARTICULAR PURPOSE, OR ANY
 * WARRANTY OF TITLE OR NON-INFRINGEMENT, OR THAT THE WORK (OR ANY
 * PORTION THEREOF) IS CORRECT, USEFUL, BUG-FREE OR FREE OF VIRUSES.
 * YOU MUST PASS THIS DISCLAIMER ON WHENEVER YOU DISTRIBUTE THE WORK OR
 * DERIVATIVE WORKS.
 */
using System.Reflection;
using System.Resources;
using System.Threading;

namespace updateSystemDotNet.Internal {
	/// <summary>
	/// Klasse zur Verwaltung von Sprachen
	/// </summary>
	internal class Language {
		private static ResourceManager res_man;

		private Language() {
		}

		/// <summary>
		/// Initialisiert den ResourceManager zum auslesen der Sprache
		/// </summary>
		/// <param name="language">Die Sprache die ausgelesen werden soll</param>
		public static void Set_Language(Languages language) {
			switch (language) {
				case Languages.Auto:
					if (Thread.CurrentThread.CurrentCulture.Name.StartsWith("de")) {
						res_man = new ResourceManager("updateSystemDotNet.Internal.Localization.language.deu",
						                              Assembly.GetExecutingAssembly());
						break;
					}
					else {
						res_man = new ResourceManager("updateSystemDotNet.Internal.Localization.language.eng",
						                              Assembly.GetExecutingAssembly());
						break;
					}
				case Languages.Deutsch:
					res_man = new ResourceManager("updateSystemDotNet.Internal.Localization.language.deu",
					                              Assembly.GetExecutingAssembly());
					break;
				case Languages.English:
					res_man = new ResourceManager("updateSystemDotNet.Internal.Localization.language.eng",
					                              Assembly.GetExecutingAssembly());
					break;
			}
			Log.Instance.writeKeyValue("Language has been set to", language.ToString());
		}

		/// <summary>
		/// Gibt die ID zu der ausgewählten Sprache zurück.
		/// </summary>
		/// <param name="language">Die Sprache von welcher die ID ermittelt werden soll</param>
		/// <returns>Die ID der Sprache.</returns>
		public static string getLanguageId(Languages language) {
			switch (language) {
				case Languages.Deutsch:
					return Languages.Deutsch.ToString();
				case Languages.English:
					return Languages.English.ToString();
				case Languages.Auto:
					if (Thread.CurrentThread.CurrentCulture.Name.StartsWith("de")) {
						return Languages.Deutsch.ToString();
					}
					else {
						return Languages.English.ToString();
					}
				default:
					return Languages.English.ToString();
			}
		}

		/// <summary>
		/// Gibt den entsprechenden String anhand der festgelegten Sprache wieder
		/// </summary>
		/// <param name="ID">Die ID zu welcher der passende Languagestring zurückgegeben werden soll</param>
		/// <returns></returns>
		public static string GetString(string ID) {
			try {
				return res_man.GetString(ID);
			}
			catch {
				return "Unkown translation!";
			}
		}
	}
}