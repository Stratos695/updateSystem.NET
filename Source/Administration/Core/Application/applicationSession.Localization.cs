﻿/**
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
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace updateSystemDotNet.Administration.Core.Application {
	internal sealed partial class applicationSession {

		private const string LOCALIZATION_FILE_PATH = "updateSystemDotNet.Administration.Administration";
		private const string LOCALIZATION_NOT_FOUND = "TRANSLATION FOR {0} NOT FOUND!";
		private const string SECTION_NAME_DIALOGS = "Dialogs";
		private const string SECTION_NAME_PAGES = "Pages";
		
		private string _currentCulture;
		private string _dialogTitleAppendix;
		private List<Type> _localizableControls;

		/// <summary>Gets direct access to the Localizationfile. For Translation use getLocalizedString(key) instead.</summary>
		public Localization.localizationFile localizationFile { get; private set; }

		private void initializeLocalization() {
			localizationFile.Load(LOCALIZATION_FILE_PATH);
			_currentCulture = Thread.CurrentThread.CurrentCulture.Name.Substring(0, 2);
			_dialogTitleAppendix = applicationName;
			_localizableControls = new List<Type>(new[] {
			                                            	typeof (Button),
			                                            	typeof (CheckBox),
			                                            	typeof (RadioButton),
			                                            	typeof (Label),
			                                            	typeof (GroupBox)
			                                            });
		}

		public string getLocalizedString(string key) {
			return getLocalizedString(key, _currentCulture);
		}
		public string getLocalizedString(string key, string language) {
			string localizedString = localizationFile[key, language];
			return string.IsNullOrEmpty(localizedString) ? string.Format(LOCALIZATION_NOT_FOUND, key) : localizedString;
		}
		public void localizeForm(Form form) {
			//TODO: Remove this check if the localization is final
			if (!isDevEnvironment)
				return;

			form.Text = string.Format("{0} - {1}",
			                          getLocalizedString(string.Format("Dialogs.{0}.Title", form.Name), _currentCulture),
			                          _dialogTitleAppendix);
			localizeControls(form.Controls);
		}
		public void localizeControls(Control.ControlCollection controls) {
			foreach(Control control in controls)
				localizeControl(control);
		}
		public void localizeControl(Control control) {
			//Check if the Control is localizable
			if (isLocalizable(control) && isDevEnvironment) {

				//Determine ParentControl (Form/Usercontrol/WhatEver)
				Control parentContainer = null;
				for (Control parent = control.Parent; parent != null; parent = parent.Parent)
					parentContainer = parent;

				if (parentContainer == null)
					throw new InvalidOperationException("Could not determine Parent which is required for Localization.");

				string localizedText = localizationFile[string.Format("{0}.{1}.{2}.Text",
				                                                      parentContainer.GetType() == typeof (Form)
				                                                      	? SECTION_NAME_DIALOGS
				                                                      	: SECTION_NAME_PAGES,
				                                                      parentContainer.Name,
				                                                      control.Name)];

				//If there is no direct Localization available, try to find a global localization
				if (string.IsNullOrEmpty(localizedText))
					localizedText = localizationFile[string.Format("Dialogs.Global.{0}.Text", control.Name)];

				control.Text = string.IsNullOrEmpty(localizedText)
				               	? string.Format(LOCALIZATION_NOT_FOUND, control.Name)
				               	: localizedText;

			}
			//Localize Childcontrols
			if (control.Controls.Count > 0)
				localizeControls(control.Controls);
		}
		private bool isLocalizable(Control control) {
			for (Type ctrlType = control.GetType(); ctrlType != null; ctrlType = ctrlType.BaseType)
				if (_localizableControls.Contains(ctrlType))
					return true;

			return false;
		}
	}
}